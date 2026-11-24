using Microsoft.Extensions.Logging;
using Migracion.Models;
using Migracion.Models.Bonificados;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Migracion.Service.Bonificados
{
    public interface IBonificadosPedidoService
    {
        /// <summary>Valida sin escribir nada (la app la usa cuando tiene señal).</summary>
        Task<BonResultadoDto> ValidarAsync(BonPedidoDto pedido);

        /// <summary>
        /// Registra los bonificados de un pedido ya guardado: revierte lo que
        /// el mismo pedido había consumido, revalida con datos frescos, guarda
        /// mob_rordbonif/bonline/opcsel y descuenta stock y consumo de lo
        /// aceptado. Lo rechazado queda con orbl_estado = 'R'.
        /// </summary>
        Task<BonResultadoDto?> RegistrarAsync(mob_rordheadE encabezado, BonPedidoDto bonificacion);

        /// <summary>Tablas espejo filtradas para la app de un vendedor.</summary>
        Task<BonSyncDto> SyncAsync(string cmpy, string codusu, string reg, string teri);
    }

    public class BonificadosPedidoService : IBonificadosPedidoService
    {
        private readonly ApplicationDbContext _contexto;
        private readonly ILogger<BonificadosPedidoService> _logger;

        public BonificadosPedidoService(ApplicationDbContext contexto, ILogger<BonificadosPedidoService> logger)
        {
            _contexto = contexto;
            _logger = logger;
        }

        // =====================================================================
        // Validación en línea
        // =====================================================================

        public async Task<BonResultadoDto> ValidarAsync(BonPedidoDto pedido)
        {
            var contexto = Contexto(pedido, null);
            var sql = await BonificadosSql.AbrirAsync(_contexto);
            return await new BonificadosMotor(sql).EvaluarAsync(contexto, pedido.Bonificados ?? new List<BonAplicadoDto>());
        }

        // =====================================================================
        // Registro con el pedido
        // =====================================================================

        public async Task<BonResultadoDto?> RegistrarAsync(mob_rordheadE encabezado, BonPedidoDto bonificacion)
        {
            var p = Contexto(bonificacion, encabezado);
            if (p.Num == null) throw new BonificadosException("El pedido no tiene un número válido para registrar bonificados.");
            var sql = await BonificadosSql.AbrirAsync(_contexto);

            // Un pedido ya procesado (orh_marca = 'S') no cambia: InsertFull tampoco lo toca.
            var guardado = (await sql.ConsultarAsync(
                    "SELECT orh_marca, orh_entdt FROM mob_rordhead WHERE orh_cmpy = ? AND orh_num = ? AND orh_reg = ? AND orh_teri = ?",
                    p.Cmpy, p.Num, p.Reg, p.Teri))
                .FirstOrDefault();
            if (guardado == null) throw new BonificadosException($"El pedido {p.Num} no está guardado; no se registran sus bonificados.");
            if (guardado.Texto("orh_marca") == "S") return null;
            // La llave foránea de mob_rordbonif incluye orh_entdt: se usa la fecha tal como quedó en el pedido.
            if (guardado.Fecha("orh_entdt") is { } fechaGuardada) p.Fecha = fechaGuardada;

            var bonificados = bonificacion.Bonificados ?? new List<BonAplicadoDto>();
            var motor = new BonificadosMotor(sql);
            BonResultadoDto resultado;
            var nombres = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var auditorias = new List<BonAuditoria>();

            sql.Iniciar();
            try
            {
                await RevertirAsync(sql, p);
                resultado = await motor.EvaluarAsync(p, bonificados);
                if (resultado.Reglas.Count > 0)
                {
                    foreach (var f in await sql.ConsultarAsync(
                                 $"SELECT rbr_regla, rbr_descr FROM mob_rbonreglas WHERE rbr_cmpy = ? AND rbr_regla IN ({BonificadosSql.Marcadores(resultado.Reglas.Count)})",
                                 new object?[] { p.Cmpy }.Concat(resultado.Reglas.Select(r => r.Regla)).ToArray()))
                        nombres[f.TextoO("rbr_regla")] = f.Texto("rbr_descr") ?? f.TextoO("rbr_regla");
                    await GuardarAsync(sql, p, bonificados, resultado, motor.Selecciones, nombres, auditorias);
                }
                sql.Confirmar();
            }
            catch (Exception)
            {
                sql.Deshacer();
                throw;
            }

            foreach (var a in auditorias) await BonificadosAuditoria.RegistrarAsync(sql, _logger, a);
            return resultado;
        }

        /// <summary>
        /// Si el pedido se reenvía (InsertFull es upsert por número), lo que
        /// consumió en el envío anterior se devuelve antes de volver a validar.
        /// </summary>
        private static async Task RevertirAsync(BonificadosSql sql, BonPedidoContexto p)
        {
            var clave = new object?[] { p.Cmpy, p.Reg, p.Teri, p.Num };
            var cabeceras = await sql.ConsultarAsync(
                "SELECT b.orb_regla, b.orb_entdt, b.orb_cust, r.rbr_frecuencia FROM mob_rordbonif b, mob_rbonreglas r " +
                "WHERE r.rbr_cmpy = b.orb_cmpy AND r.rbr_regla = b.orb_regla " +
                "AND b.orb_cmpy = ? AND b.orb_reg = ? AND b.orb_teri = ? AND b.orb_num = ?", clave);
            var lineas = await sql.ConsultarAsync(
                "SELECT orbl_regla, orbl_part, orbl_cant, orbl_estado FROM mob_rordbonline " +
                "WHERE orbl_cmpy = ? AND orbl_reg = ? AND orbl_teri = ? AND orbl_num = ?", clave);

            foreach (var l in lineas.Where(l => l.Texto("orbl_estado") != "R"))
            {
                await sql.EjecutarAsync(
                    "UPDATE mob_rbonstock SET rbs_consumido = rbs_consumido - ?, estado_envio = 0 WHERE rbs_cmpy = ? AND rbs_regla = ? AND rbs_id = ?",
                    l.Decimal("orbl_cant") ?? 0, p.Cmpy, l.TextoO("orbl_regla"), l.TextoO("orbl_part"));
                await sql.EjecutarAsync(
                    "UPDATE mob_rbonstock SET rbs_consumido = 0 WHERE rbs_cmpy = ? AND rbs_regla = ? AND rbs_id = ? AND rbs_consumido < 0",
                    p.Cmpy, l.TextoO("orbl_regla"), l.TextoO("orbl_part"));
            }

            var aceptadas = lineas.Where(l => l.Texto("orbl_estado") != "R").Select(l => l.TextoO("orbl_regla"))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var c in cabeceras.Where(c => aceptadas.Contains(c.TextoO("orb_regla"))))
            {
                var periodo = BonPedidoContexto.Periodo_(c.Texto("rbr_frecuencia") ?? "N", c.Fecha("orb_entdt") ?? p.Fecha);
                var parametros = new object?[] { p.Cmpy, c.TextoO("orb_regla"), c.TextoO("orb_cust"), periodo };
                await sql.EjecutarAsync(
                    "UPDATE mob_rbonconsumo SET cns_veces = cns_veces - 1, estado_envio = 0 " +
                    "WHERE cns_cmpy = ? AND cns_regla = ? AND cns_cust = ? AND cns_periodo = ?", parametros);
                await sql.EjecutarAsync(
                    "UPDATE mob_rbonconsumo SET cns_veces = 0 WHERE cns_cmpy = ? AND cns_regla = ? AND cns_cust = ? AND cns_periodo = ? AND cns_veces < 0",
                    parametros);
            }

            await sql.EjecutarAsync("DELETE FROM mob_rbonopcsel WHERE obs_cmpy = ? AND obs_reg = ? AND obs_teri = ? AND obs_num = ?", clave);
            await sql.EjecutarAsync("DELETE FROM mob_rordbonline WHERE orbl_cmpy = ? AND orbl_reg = ? AND orbl_teri = ? AND orbl_num = ?", clave);
            await sql.EjecutarAsync("DELETE FROM mob_rordbonif WHERE orb_cmpy = ? AND orb_reg = ? AND orb_teri = ? AND orb_num = ?", clave);
        }

        private static async Task GuardarAsync(BonificadosSql sql, BonPedidoContexto p, List<BonAplicadoDto> bonificados,
            BonResultadoDto resultado, List<BonSeleccion> selecciones, Dictionary<string, string> nombres, List<BonAuditoria> auditorias)
        {
            var ahora = DateTime.Now;
            foreach (var evaluada in resultado.Reglas)
            {
                var regla = evaluada.Regla;
                // Las reglas que no existen no se pueden guardar: la llave foránea las rechaza.
                if (!nombres.ContainsKey(regla))
                {
                    auditorias.Add(new BonAuditoria
                    {
                        Cmpy = p.Cmpy, Regla = regla, Num = p.Num, Cust = p.Cust, Accion = "RECHAZADA", Validacion = "V-01",
                        Usuario = p.Codusu, Observacion = $"Pedido {p.Num}: la regla {regla} no existe.",
                    });
                    continue;
                }

                var violaciones = resultado.Violaciones.Where(v => string.Equals(v.Regla, regla, StringComparison.OrdinalIgnoreCase)).ToList();
                var aceptada = evaluada.Aceptada && violaciones.Count == 0;
                var items = bonificados.Where(b => string.Equals(b.Regla?.Trim(), regla, StringComparison.OrdinalIgnoreCase))
                    .SelectMany(b => b.Items ?? new List<BonItemDto>())
                    .Where(i => !string.IsNullOrWhiteSpace(i.Part) && i.Cantidad > 0)
                    .GroupBy(i => i.Part.Trim(), StringComparer.OrdinalIgnoreCase)
                    .Select(g => new
                    {
                        Part = g.Key,
                        Cantidad = g.Sum(i => i.Cantidad),
                        Origen = g.Select(i => i.PartOrigen).FirstOrDefault(o => !string.IsNullOrWhiteSpace(o)),
                        Precio = g.Select(i => i.PrecioLista).FirstOrDefault(v => v != null),
                        Valor = g.All(i => i.ValorBonificado == null) ? (decimal?)null : g.Sum(i => i.ValorBonificado ?? 0),
                    })
                    .ToList();

                var descripcion = aceptada
                    ? nombres[regla]
                    : $"RECHAZADA {violaciones.First().Codigo}: {violaciones.First().Mensaje}";

                await sql.EjecutarAsync(
                    "INSERT INTO mob_rordbonif (orb_cmpy, orb_reg, orb_teri, orb_num, orb_entdt, orb_cust, orb_ship, orb_regla, orb_campana, " +
                    "orb_codusu, orb_nivel, orb_descr, orb_totitems, orb_totvalor, orb_fecha, estado_envio) " +
                    "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 0)",
                    p.Cmpy, p.Reg, p.Teri, p.Num, p.Fecha.Date, p.Cust, p.Ship, regla, evaluada.Campana,
                    Recortar(p.Codusu, 10), evaluada.Nivel, Recortar(descripcion, 200),
                    items.Sum(i => i.Cantidad), items.Sum(i => i.Valor ?? 0), ahora);

                foreach (var i in items)
                {
                    await sql.EjecutarAsync(
                        "INSERT INTO mob_rordbonline (orbl_cmpy, orbl_reg, orbl_teri, orbl_num, orbl_entdt, orbl_cust, orbl_ship, orbl_regla, " +
                        "orbl_part, orbl_partorigen, orbl_cant, orbl_preciolist, orbl_valorbonif, orbl_estado, estado_envio) " +
                        "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 0)",
                        p.Cmpy, p.Reg, p.Teri, p.Num, p.Fecha.Date, p.Cust, p.Ship, regla,
                        i.Part, i.Origen, i.Cantidad, i.Precio, i.Valor, aceptada ? "C" : "R");
                }

                if (!aceptada)
                {
                    foreach (var v in violaciones)
                    {
                        auditorias.Add(new BonAuditoria
                        {
                            Cmpy = p.Cmpy, Regla = regla, Campana = evaluada.Campana, Num = p.Num, Cust = p.Cust,
                            Accion = "RECHAZADA", Validacion = v.Codigo, Usuario = p.Codusu,
                            Observacion = $"Pedido {p.Num}: {v.Mensaje}",
                        });
                    }
                    continue;
                }

                // ---- aceptada: opción elegida, stock y consumo ----
                foreach (var s in selecciones.Where(s => string.Equals(s.Regla, regla, StringComparison.OrdinalIgnoreCase)))
                {
                    await sql.EjecutarAsync(
                        "INSERT INTO mob_rbonopcsel (obs_cmpy, obs_reg, obs_teri, obs_num, obs_entdt, obs_cust, obs_ship, obs_regla, " +
                        "obs_seqbenef, obs_orden, obs_codusu, obs_fecha, estado_envio) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 0)",
                        p.Cmpy, p.Reg, p.Teri, p.Num, p.Fecha.Date, p.Cust, p.Ship, regla, s.SeqBenef, s.Orden, Recortar(p.Codusu, 10), ahora);
                }

                var limitada = (await sql.ConsultarAsync(
                        "SELECT rbr_stocklimit, rbr_frecuencia FROM mob_rbonreglas WHERE rbr_cmpy = ? AND rbr_regla = ?", p.Cmpy, regla))
                    .First();
                if (limitada.SiNo("rbr_stocklimit"))
                {
                    foreach (var i in items)
                    {
                        await sql.EjecutarAsync(
                            "UPDATE mob_rbonstock SET rbs_consumido = rbs_consumido + ?, rbs_fecha_mod = ?, estado_envio = 0 " +
                            "WHERE rbs_cmpy = ? AND rbs_regla = ? AND rbs_id = ?",
                            i.Cantidad, ahora, p.Cmpy, regla, i.Part);
                    }
                    // C-11: agotado el cupo de todos los productos, la regla se apaga sola.
                    if (await sql.ContarAsync(
                            "SELECT COUNT(*) FROM mob_rbonstock WHERE rbs_cmpy = ? AND rbs_regla = ? AND rbs_consumido < rbs_asignado",
                            p.Cmpy, regla) == 0)
                    {
                        await sql.EjecutarAsync(
                            "UPDATE mob_rbonreglas SET rbr_activo = 'N', rbr_fecha_mod = ?, estado_envio = 0 WHERE rbr_cmpy = ? AND rbr_regla = ?",
                            ahora, p.Cmpy, regla);
                        auditorias.Add(new BonAuditoria
                        {
                            Cmpy = p.Cmpy, Regla = regla, Campana = evaluada.Campana, Num = p.Num, Accion = "AGOTADA",
                            Usuario = p.Codusu, Observacion = $"Stock agotado con el pedido {p.Num}; la regla quedó inactiva.",
                        });
                    }
                }

                var periodo = BonPedidoContexto.Periodo_(limitada.Texto("rbr_frecuencia") ?? "N", p.Fecha);
                var actualizadas = await sql.EjecutarAsync(
                    "UPDATE mob_rbonconsumo SET cns_veces = cns_veces + 1, cns_ultfecha = ?, cns_ultnum = ?, estado_envio = 0 " +
                    "WHERE cns_cmpy = ? AND cns_regla = ? AND cns_cust = ? AND cns_periodo = ?",
                    ahora, p.Num, p.Cmpy, regla, p.Cust, periodo);
                if (actualizadas == 0)
                {
                    await sql.EjecutarAsync(
                        "INSERT INTO mob_rbonconsumo (cns_cmpy, cns_regla, cns_cust, cns_periodo, cns_veces, cns_ultfecha, cns_ultnum, estado_envio) " +
                        "VALUES (?, ?, ?, ?, 1, ?, ?, 0)",
                        p.Cmpy, regla, p.Cust, periodo, ahora, p.Num);
                }

                auditorias.Add(new BonAuditoria
                {
                    Cmpy = p.Cmpy, Regla = regla, Campana = evaluada.Campana, Num = p.Num, Cust = p.Cust, Accion = "APLICADA",
                    Usuario = p.Codusu, Observacion = $"Pedido {p.Num}: {items.Sum(i => i.Cantidad):0.##} unidades bonificadas.",
                });
            }
        }

        // =====================================================================
        // Descarga para la app
        // =====================================================================

        /// <summary>Tablas que descarga la app, en orden de dependencia.</summary>
        private static readonly (string Tabla, string Cmpy, string? Regla)[] TablasDeReglas =
        {
            ("mob_rbonalcance", "rbe_cmpy", "rbe_regla"),
            ("mob_rbonregcond", "rbc_cmpy", "rbc_regla"),
            ("mob_rbonnivel", "rbn_cmpy", "rbn_regla"),
            ("mob_rbonnivelbenef", "rnb_cmpy", "rnb_regla"),
            ("mob_rbonregbenef", "rbb_cmpy", "rbb_regla"),
            ("mob_rbonbenefopc", "rbo_cmpy", "rbo_regla"),
            ("mob_rbonstock", "rbs_cmpy", "rbs_regla"),
        };

        public async Task<BonSyncDto> SyncAsync(string cmpy, string codusu, string reg, string teri)
        {
            cmpy = (cmpy ?? "").Trim();
            codusu = (codusu ?? "").Trim();
            reg = (reg ?? "").Trim();
            teri = (teri ?? "").Trim();
            if (cmpy.Length == 0 || codusu.Length == 0) throw new BonificadosException("Faltan la compañía o el vendedor.");

            var sql = await BonificadosSql.AbrirAsync(_contexto);
            var ahora = DateTime.Now;
            var resultado = new BonSyncDto
            {
                Fecha = ahora.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                Codigos = BonificadosCodigos.Todos.ToList(),
            };

            void Poner(string tabla, IEnumerable<Fila> filas) => resultado.Tablas[tabla] = filas.Select(Exportar).ToList();

            Poner("mob_rbontipos", await sql.ConsultarAsync("SELECT * FROM mob_rbontipos WHERE rbt_cmpy = ?", cmpy));

            // Campañas activas que no han terminado (incluye las programadas: la app revisa la vigencia).
            var campanas = await sql.ConsultarAsync(
                "SELECT * FROM mob_rboncampana WHERE cmp_cmpy = ? AND cmp_estado = 'A' AND cmp_fecfin >= ?", cmpy, ahora.Date);
            Poner("mob_rboncampana", campanas);

            var reglas = campanas.Count == 0
                ? new List<Fila>()
                : await sql.ConsultarAsync(
                    "SELECT * FROM mob_rbonreglas WHERE rbr_cmpy = ? AND rbr_activo <> 'N' AND rbr_campana IN " +
                    "(SELECT cmp_campana FROM mob_rboncampana WHERE cmp_cmpy = ? AND cmp_estado = 'A' AND cmp_fecfin >= ?)",
                    cmpy, cmpy, ahora.Date);

            // Solo las reglas cuya región y territorio incluyen al vendedor (o que no los restringen).
            var codigos = reglas.Select(r => r.TextoO("rbr_regla")).ToList();
            var alcance = await PorReglasAsync(sql, "mob_rbonalcance", "rbe_cmpy", "rbe_regla", cmpy, codigos);
            bool Alcanza(string regla, string tipo, string valor)
            {
                var valores = alcance.Where(a => a.TextoO("rbe_regla") == regla && a.Texto("rbe_tipoalcance")?.ToUpperInvariant() == tipo)
                    .Select(a => a.TextoO("rbe_valor")).ToList();
                return valores.Count == 0 || valores.Contains(valor, StringComparer.OrdinalIgnoreCase);
            }
            reglas = reglas.Where(r =>
            {
                var codigo = r.TextoO("rbr_regla");
                return Alcanza(codigo, "REGION", reg) && Alcanza(codigo, "TERRITORIO", teri);
            }).ToList();
            codigos = reglas.Select(r => r.TextoO("rbr_regla")).ToList();
            var vigentes = codigos.ToHashSet(StringComparer.OrdinalIgnoreCase);
            Poner("mob_rbonreglas", reglas);

            foreach (var (tabla, columnaCmpy, columnaRegla) in TablasDeReglas)
            {
                var filas = tabla == "mob_rbonalcance"
                    ? alcance.Where(a => vigentes.Contains(a.TextoO("rbe_regla"))).ToList()
                    : await PorReglasAsync(sql, tabla, columnaCmpy, columnaRegla!, cmpy, codigos);
                Poner(tabla, filas);
            }

            Poner("mob_rbongrupo", await sql.ConsultarAsync("SELECT * FROM mob_rbongrupo WHERE grp_cmpy = ?", cmpy));
            Poner("mob_rbongrupodet", await sql.ConsultarAsync("SELECT * FROM mob_rbongrupodet WHERE grd_cmpy = ?", cmpy));

            // Consumo del mes y de "una vez por cliente", solo de los clientes del vendedor.
            var consumos = codigos.Count == 0 ? new List<Fila>() : new List<Fila>();
            foreach (var tanda in codigos.Chunk(100))
            {
                consumos.AddRange(await sql.ConsultarAsync(
                    "SELECT * FROM mob_rbonconsumo WHERE cns_cmpy = ? AND cns_periodo IN (?, '000000') " +
                    $"AND cns_regla IN ({BonificadosSql.Marcadores(tanda.Length)}) " +
                    "AND cns_cust IN (SELECT rctt_cust FROM mob_rcustmain WHERE rctt_cmpy = ? AND rctt_codusu = ?)",
                    new object?[] { cmpy, ahora.ToString("yyyyMM", CultureInfo.InvariantCulture) }.Concat(tanda).Concat(new object?[] { cmpy, codusu }).ToArray()));
            }
            Poner("mob_rbonconsumo", consumos);

            // Eventos QR vigentes de esas reglas y sus invitados (fase II).
            var eventos = new List<Fila>();
            foreach (var tanda in codigos.Chunk(100))
            {
                eventos.AddRange(await sql.ConsultarAsync(
                    "SELECT * FROM mob_rbonevento WHERE evt_cmpy = ? AND evt_estado = 'A' AND evt_fecfin >= ? " +
                    $"AND evt_regla IN ({BonificadosSql.Marcadores(tanda.Length)})",
                    new object?[] { cmpy, ahora }.Concat(tanda).ToArray()));
            }
            Poner("mob_rbonevento", eventos);
            var codigosEvento = eventos.Select(e => e.TextoO("evt_evento")).ToList();
            var invitados = new List<Fila>();
            foreach (var tanda in codigosEvento.Chunk(100))
            {
                invitados.AddRange(await sql.ConsultarAsync(
                    $"SELECT * FROM mob_rboneventoinv WHERE evi_cmpy = ? AND evi_evento IN ({BonificadosSql.Marcadores(tanda.Length)})",
                    new object?[] { cmpy }.Concat(tanda).ToArray()));
            }
            Poner("mob_rboneventoinv", invitados);

            // Rechazos de los últimos 30 días en el territorio del vendedor.
            resultado.Rechazos = (await sql.ConsultarAsync(
                    "SELECT orb_num, orb_cust, orb_regla, orb_entdt, orb_descr FROM mob_rordbonif " +
                    "WHERE orb_cmpy = ? AND orb_reg = ? AND orb_teri = ? AND orb_fecha >= ? AND orb_descr LIKE 'RECHAZADA%'",
                    cmpy, reg, teri, ahora.AddDays(-30)))
                .Select(f => new BonRechazoDto
                {
                    Num = f.TextoO("orb_num"),
                    Cust = f.Texto("orb_cust"),
                    Regla = f.TextoO("orb_regla"),
                    Fecha = f.Fecha("orb_entdt")?.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    Motivo = f.Texto("orb_descr"),
                }).ToList();

            return resultado;
        }

        private static async Task<List<Fila>> PorReglasAsync(BonificadosSql sql, string tabla, string columnaCmpy, string columnaRegla,
            string cmpy, List<string> reglas)
        {
            var filas = new List<Fila>();
            foreach (var tanda in reglas.Chunk(100))
            {
                filas.AddRange(await sql.ConsultarAsync(
                    $"SELECT * FROM {tabla} WHERE {columnaCmpy} = ? AND {columnaRegla} IN ({BonificadosSql.Marcadores(tanda.Length)})",
                    new object?[] { cmpy }.Concat(tanda).ToArray()));
            }
            return filas;
        }

        /// <summary>Fila lista para sqflite: nombres de columna en minúscula y fechas en MM/dd/yyyy.</summary>
        private static Dictionary<string, object?> Exportar(Fila fila) =>
            fila.Valores.ToDictionary(
                c => c.Key.ToLowerInvariant(),
                c => c.Value switch
                {
                    DateTime f when f.TimeOfDay == TimeSpan.Zero => f.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    DateTime f => f.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    TimeSpan t => t.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture),
                    _ => c.Value,
                });

        // =====================================================================
        // Utilidades
        // =====================================================================

        /// <summary>Arma el contexto del motor; con encabezado, la llave sale del pedido guardado.</summary>
        private static BonPedidoContexto Contexto(BonPedidoDto b, mob_rordheadE? encabezado)
        {
            string Valor(string? principal, string? alterno) => (principal ?? alterno ?? "").Trim();

            var cmpy = Valor(encabezado?.orh_cmpy, b.Cmpy);
            var cust = Valor(encabezado?.orh_cust, b.Cust);
            if (cmpy.Length == 0 || cust.Length == 0) throw new BonificadosException("Faltan la compañía o el cliente del pedido.");

            var numero = Valor(encabezado?.orh_num, b.Num);
            var lineas = new Dictionary<string, (decimal Cantidad, decimal? Precio)>(StringComparer.OrdinalIgnoreCase);
            foreach (var l in (b.Lineas ?? new List<BonLineaPedidoDto>()).Where(l => !string.IsNullOrWhiteSpace(l.Part)))
            {
                var part = l.Part.Trim();
                var previa = lineas.GetValueOrDefault(part);
                lineas[part] = (previa.Cantidad + l.Cantidad, l.Precio ?? previa.Precio);
            }

            return new BonPedidoContexto
            {
                Cmpy = cmpy,
                Reg = Valor(encabezado?.orh_reg, b.Reg),
                Teri = Valor(encabezado?.orh_teri, b.Teri),
                Num = decimal.TryParse(numero, NumberStyles.Number, CultureInfo.InvariantCulture, out var n) ? n : null,
                Fecha = LeerFecha(Valor(encabezado?.orh_entdt, b.Entdt)) ?? DateTime.Today,
                Cust = cust,
                Ship = Valor(encabezado?.orh_ship, b.Ship),
                Codusu = string.IsNullOrWhiteSpace(b.Codusu) ? null : b.Codusu.Trim(),
                Lineas = lineas,
            };
        }

        /// <summary>La app manda orh_entdt en yyyy-MM-dd; los datos fijos usan MM/dd/yyyy.</summary>
        private static DateTime? LeerFecha(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return null;
            string[] formatos = { "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss.FFFFFFF", "MM/dd/yyyy", "M/d/yyyy", "MM/dd/yyyy HH:mm:ss" };
            return DateTime.TryParseExact(texto.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out var f)
                ? f
                : DateTime.TryParse(texto, CultureInfo.InvariantCulture, DateTimeStyles.None, out f) ? f : null;
        }

        private static string? Recortar(string? texto, int largo)
        {
            if (string.IsNullOrWhiteSpace(texto)) return null;
            var limpio = texto.Trim();
            return limpio.Length <= largo ? limpio : limpio.Substring(0, largo);
        }
    }
}
