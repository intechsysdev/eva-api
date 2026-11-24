using Microsoft.Extensions.Logging;
using Migracion.Models.Bonificados;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Migracion.Service.Bonificados
{
    /// <summary>Error que el controlador devuelve tal cual, con su código HTTP.</summary>
    public class BonificadosException : Exception
    {
        public int Codigo { get; }

        public BonificadosException(string mensaje, int codigo = 400) : base(mensaje) => Codigo = codigo;
    }

    public interface IBonificadosService
    {
        Task<BonCatalogosDto> CatalogosAsync(string cmpy);
        Task<List<BonCampanaDto>> CampanasAsync(string cmpy);
        Task<BonCampanaDto> CampanaAsync(string cmpy, string codigo);
        Task<BonCampanaDto> CrearCampanaAsync(BonCampanaDto datos);
        Task<BonCampanaDto> ActualizarCampanaAsync(BonCampanaDto datos);
        Task<BonCampanaDto> DuplicarCampanaAsync(string cmpy, string codigo, string? usuario);
        Task EliminarCampanaAsync(string cmpy, string codigo, string? usuario);
        Task<List<BonReglaDto>> ReglasAsync(string cmpy, string campana);
        Task<BonReglaDto> ReglaAsync(string cmpy, string codigo);
        Task<BonReglaDto> CrearReglaAsync(BonReglaDto datos);
        Task<BonReglaDto> ActualizarReglaAsync(BonReglaDto datos);
        Task EliminarReglaAsync(string cmpy, string codigo, string? usuario);
        Task<List<BonGrupoEditarDto>> GruposAsync(string cmpy);
        Task<BonGrupoEditarDto> GrupoAsync(string cmpy, string codigo);
        Task<BonGrupoEditarDto> CrearGrupoAsync(BonGrupoEditarDto datos);
        Task<BonGrupoEditarDto> ActualizarGrupoAsync(BonGrupoEditarDto datos);
        Task EliminarGrupoAsync(string cmpy, string codigo, string? usuario);
    }

    /// <summary>
    /// Administración de campañas y reglas de bonificados (módulo web).
    /// Modelo: sql_bonificados_recomendado.sql en la raíz del repo.
    /// </summary>
    /// <remarks>
    /// Una regla se guarda entera en una transacción: se actualiza la fila de
    /// mob_rbonreglas y se reescriben sus hijas. Lo que el portal no maneja se
    /// conserva: condiciones de producto foco (rbc_foco = 'S'), alcances que no
    /// son canal/región/territorio, las columnas de fase II de la regla y el
    /// consumo del stock.
    /// </remarks>
    public class BonificadosService : IBonificadosService
    {
        private const int LargoDescripcion = 150;
        private const int LargoUsuario = 10;
        private const int LargoBeneficio = 200;

        private const string Unidades = "UNIDADES";
        private const string Monto = "MONTO";

        private static readonly Regex PatronCmpy = new("^[A-Za-z0-9]{1,2}$");
        private static readonly Regex PatronCodigo = new(@"^[A-Za-z0-9_.\-|]{1,32}$");

        private static readonly string[] Agrupaciones = { "PRODUCTO", "LINEA", "SUBLINEA", "GRUPO_MIXTO" };
        private static readonly string[] TiposBeneficio = { "SPECIFIC_PRODUCT", "SAME_PRODUCT", "LOWEST_VALUE", "MULTIPLE_OPTION" };

        private readonly ApplicationDbContext _contexto;
        private readonly ILogger<BonificadosService> _logger;

        public BonificadosService(ApplicationDbContext contexto, ILogger<BonificadosService> logger)
        {
            _contexto = contexto;
            _logger = logger;
        }

        // =====================================================================
        // Catálogos
        // =====================================================================

        public async Task<BonCatalogosDto> CatalogosAsync(string cmpy)
        {
            cmpy = Compania(cmpy);
            var sql = await BonificadosSql.AbrirAsync(_contexto);

            static BonOpcionDto Opcion(Fila f, string codigo, string nombre) =>
                new() { Codigo = f.TextoO(codigo), Nombre = f.Texto(nombre) ?? f.TextoO(codigo) };

            var catalogos = new BonCatalogosDto
            {
                Canales = (await sql.ConsultarAsync(
                        "SELECT cu_type, cu_desc FROM mob_rcusttype WHERE cu_cmpy = ? ORDER BY cu_desc", cmpy))
                    .Select(f => Opcion(f, "cu_type", "cu_desc")).ToList(),

                // Igual que Regiones/GetList y Territorios/GetList: solo los que
                // tienen vendedores, que son los que la app puede encontrar.
                Regiones = (await sql.ConsultarAsync(
                        "SELECT reg_regi, reg_desc FROM region WHERE reg_cmpy = ? " +
                        "AND reg_regi IN (SELECT rusu_reg FROM mob_rusuarios WHERE rusu_cmpy = ?) ORDER BY reg_desc",
                        cmpy, cmpy))
                    .Select(f => Opcion(f, "reg_regi", "reg_desc")).ToList(),

                Territorios = (await sql.ConsultarAsync(
                        "SELECT ter_teri, ter_desc FROM territor WHERE ter_cmpy = ? " +
                        "AND ter_teri IN (SELECT rusu_teri FROM mob_rusuarios WHERE rusu_cmpy = ?) ORDER BY ter_desc",
                        cmpy, cmpy))
                    .Select(f => Opcion(f, "ter_teri", "ter_desc")).ToList(),

                Lineas = (await sql.ConsultarAsync(
                        "SELECT rln_linea, rln_descr FROM mob_rlinea WHERE rln_cmpy = ? ORDER BY rln_descr", cmpy))
                    .Select(f => Opcion(f, "rln_linea", "rln_descr")).ToList(),

                Sublineas = (await sql.ConsultarAsync(
                        "SELECT rsb_linea, rsb_subl, rsb_descr FROM mob_rsublineas WHERE rsb_cmpy = ? ORDER BY rsb_descr", cmpy))
                    .Select(f => new BonSublineaDto
                    {
                        Linea = f.TextoO("rsb_linea"),
                        Codigo = f.TextoO("rsb_subl"),
                        Nombre = f.Texto("rsb_descr") ?? f.TextoO("rsb_subl"),
                    }).ToList(),

                Productos = (await sql.ConsultarAsync(
                        "SELECT rpr_id, rpr_desc, rpr_linea, rpr_subl FROM mob_rproductos WHERE rpr_cmpy = ? ORDER BY rpr_desc", cmpy))
                    .Select(f => new BonProductoDto
                    {
                        Codigo = f.TextoO("rpr_id"),
                        Nombre = f.Texto("rpr_desc") ?? f.TextoO("rpr_id"),
                        Linea = f.Texto("rpr_linea"),
                        Sublinea = f.Texto("rpr_subl"),
                    }).ToList(),

                Grupos = (await sql.ConsultarAsync(
                        "SELECT g.grp_grupo, g.grp_descr, g.grp_activo, " +
                        "(SELECT COUNT(*) FROM mob_rbongrupodet d WHERE d.grd_cmpy = g.grp_cmpy AND d.grd_grupo = g.grp_grupo) AS elementos " +
                        "FROM mob_rbongrupo g WHERE g.grp_cmpy = ? ORDER BY g.grp_descr", cmpy))
                    .Select(f => new BonGrupoDto
                    {
                        Codigo = f.TextoO("grp_grupo"),
                        Nombre = f.Texto("grp_descr") ?? f.TextoO("grp_grupo"),
                        Activo = f.Texto("grp_activo") != "N",
                        Elementos = f.Entero("elementos") ?? 0,
                    }).ToList(),
            };

            // Un mismo territorio aparece una vez aunque lo compartan varias regiones.
            catalogos.Territorios = catalogos.Territorios.GroupBy(t => t.Codigo).Select(g => g.First()).ToList();
            return catalogos;
        }

        // =====================================================================
        // Campañas
        // =====================================================================

        private const string SelectCampana =
            "SELECT c.cmp_campana, c.cmp_descr, c.cmp_fecini, c.cmp_fecfin, c.cmp_estado, c.cmp_origen, " +
            "c.cmp_codusu_crea, c.cmp_fecha_crea, c.cmp_codusu_mod, c.cmp_fecha_mod, " +
            "(SELECT COUNT(*) FROM mob_rbonreglas r WHERE r.rbr_cmpy = c.cmp_cmpy AND r.rbr_campana = c.cmp_campana) AS total_reglas " +
            "FROM mob_rboncampana c WHERE c.cmp_cmpy = ?";

        public async Task<List<BonCampanaDto>> CampanasAsync(string cmpy)
        {
            cmpy = Compania(cmpy);
            var sql = await BonificadosSql.AbrirAsync(_contexto);
            var filas = await sql.ConsultarAsync(SelectCampana + " ORDER BY c.cmp_fecini DESC, c.cmp_campana DESC", cmpy);
            return filas.Select(f => LeerCampana(cmpy, f)).ToList();
        }

        public async Task<BonCampanaDto> CampanaAsync(string cmpy, string codigo)
        {
            cmpy = Compania(cmpy);
            codigo = Codigo(codigo, "campaña");
            var sql = await BonificadosSql.AbrirAsync(_contexto);
            return await CargarCampanaAsync(sql, cmpy, codigo);
        }

        public async Task<BonCampanaDto> CrearCampanaAsync(BonCampanaDto datos)
        {
            var cmpy = Compania(datos.Cmpy);
            ValidarCampana(datos);
            var sql = await BonificadosSql.AbrirAsync(_contexto);

            string codigo;
            sql.Iniciar();
            try
            {
                codigo = await SiguienteCodigoAsync(sql, "mob_rboncampana", "cmp_cmpy", "cmp_campana", "CAM", cmpy);
                await InsertarCampanaAsync(sql, cmpy, codigo, datos, origen: null);
                sql.Confirmar();
            }
            catch (Exception ex)
            {
                sql.Deshacer();
                throw Traducir(ex);
            }

            var creada = await CargarCampanaAsync(sql, cmpy, codigo);
            await AuditarAsync(sql, cmpy, null, codigo, "CREADA", null, creada, datos.Usuario);
            return creada;
        }

        public async Task<BonCampanaDto> ActualizarCampanaAsync(BonCampanaDto datos)
        {
            var cmpy = Compania(datos.Cmpy);
            var codigo = Codigo(datos.Codigo, "campaña");
            ValidarCampana(datos);
            var sql = await BonificadosSql.AbrirAsync(_contexto);
            var anterior = await CargarCampanaAsync(sql, cmpy, codigo);

            await sql.EjecutarAsync(
                "UPDATE mob_rboncampana SET cmp_descr = ?, cmp_fecini = ?, cmp_fecfin = ?, cmp_estado = ?, " +
                "cmp_codusu_mod = ?, cmp_fecha_mod = ?, estado_envio = 0 WHERE cmp_cmpy = ? AND cmp_campana = ?",
                datos.Nombre.Trim(), datos.FechaInicio.Date, FinDelDia(datos.FechaFin), datos.Estado,
                Usuario(datos.Usuario), DateTime.Now, cmpy, codigo);

            var actual = await CargarCampanaAsync(sql, cmpy, codigo);
            await AuditarAsync(sql, cmpy, null, codigo,
                anterior.Estado != "I" && actual.Estado == "I" ? "DESACTIVADA" : "MODIFICADA",
                anterior, actual, datos.Usuario);
            return actual;
        }

        public async Task<BonCampanaDto> DuplicarCampanaAsync(string cmpy, string codigo, string? usuario)
        {
            cmpy = Compania(cmpy);
            codigo = Codigo(codigo, "campaña");
            var sql = await BonificadosSql.AbrirAsync(_contexto);
            var origen = await CargarCampanaAsync(sql, cmpy, codigo);
            var reglas = await sql.ConsultarAsync(
                "SELECT rbr_regla FROM mob_rbonreglas WHERE rbr_cmpy = ? AND rbr_campana = ? ORDER BY rbr_regla", cmpy, codigo);

            var copia = new BonCampanaDto
            {
                Cmpy = cmpy,
                Nombre = Recortar("Copia de " + origen.Nombre, LargoDescripcion)!,
                FechaInicio = origen.FechaInicio,
                FechaFin = origen.FechaFin,
                // La copia nace en borrador: la app no la ve hasta que trade la active.
                Estado = "B",
                Usuario = usuario,
            };

            string nuevo;
            sql.Iniciar();
            try
            {
                nuevo = await SiguienteCodigoAsync(sql, "mob_rboncampana", "cmp_cmpy", "cmp_campana", "CAM", cmpy);
                await InsertarCampanaAsync(sql, cmpy, nuevo, copia, origen: codigo);

                foreach (var regla in reglas.Select(r => r.TextoO("rbr_regla")))
                {
                    var reglaNueva = await SiguienteCodigoAsync(sql, "mob_rbonreglas", "rbr_cmpy", "rbr_regla", "REG", cmpy);
                    await CopiarReglaAsync(sql, cmpy, regla, reglaNueva, nuevo, usuario);
                }

                sql.Confirmar();
            }
            catch (Exception ex)
            {
                sql.Deshacer();
                throw Traducir(ex);
            }

            var creada = await CargarCampanaAsync(sql, cmpy, nuevo);
            await AuditarAsync(sql, cmpy, null, nuevo, "DUPLICADA", origen, creada, usuario);
            return creada;
        }

        public async Task EliminarCampanaAsync(string cmpy, string codigo, string? usuario)
        {
            cmpy = Compania(cmpy);
            codigo = Codigo(codigo, "campaña");
            var sql = await BonificadosSql.AbrirAsync(_contexto);
            var anterior = await CargarCampanaAsync(sql, cmpy, codigo);
            var reglas = (await sql.ConsultarAsync(
                    "SELECT rbr_regla FROM mob_rbonreglas WHERE rbr_cmpy = ? AND rbr_campana = ?", cmpy, codigo))
                .Select(r => r.TextoO("rbr_regla")).ToList();

            foreach (var regla in reglas) await ExigirSinUsoAsync(sql, cmpy, regla, "la campaña");

            sql.Iniciar();
            try
            {
                foreach (var regla in reglas)
                {
                    await BorrarHijasAsync(sql, cmpy, regla, completo: true);
                    await sql.EjecutarAsync("DELETE FROM mob_rbonreglas WHERE rbr_cmpy = ? AND rbr_regla = ?", cmpy, regla);
                }
                await sql.EjecutarAsync("DELETE FROM mob_rboncampana WHERE cmp_cmpy = ? AND cmp_campana = ?", cmpy, codigo);
                sql.Confirmar();
            }
            catch (Exception ex)
            {
                sql.Deshacer();
                throw Traducir(ex);
            }

            await AuditarAsync(sql, cmpy, null, codigo, "ELIMINADA", anterior, null, usuario);
        }

        private async Task<BonCampanaDto> CargarCampanaAsync(BonificadosSql sql, string cmpy, string codigo)
        {
            var fila = (await sql.ConsultarAsync(SelectCampana + " AND c.cmp_campana = ?", cmpy, codigo)).FirstOrDefault();
            if (fila == null) throw new BonificadosException($"No existe la campaña {codigo}.", 404);
            return LeerCampana(cmpy, fila);
        }

        private static BonCampanaDto LeerCampana(string cmpy, Fila f) => new()
        {
            Cmpy = cmpy,
            Codigo = f.Texto("cmp_campana"),
            Nombre = f.TextoO("cmp_descr"),
            FechaInicio = f.Fecha("cmp_fecini")?.Date ?? DateTime.MinValue,
            FechaFin = f.Fecha("cmp_fecfin")?.Date ?? DateTime.MinValue,
            Estado = f.Texto("cmp_estado") ?? "B",
            Origen = f.Texto("cmp_origen"),
            TotalReglas = f.Entero("total_reglas") ?? 0,
            UsuarioCreacion = f.Texto("cmp_codusu_crea"),
            FechaCreacion = f.Fecha("cmp_fecha_crea"),
            UsuarioModificacion = f.Texto("cmp_codusu_mod"),
            FechaModificacion = f.Fecha("cmp_fecha_mod"),
        };

        private static Task<int> InsertarCampanaAsync(BonificadosSql sql, string cmpy, string codigo, BonCampanaDto datos, string? origen) =>
            sql.EjecutarAsync(
                "INSERT INTO mob_rboncampana (cmp_cmpy, cmp_campana, cmp_descr, cmp_fecini, cmp_fecfin, cmp_estado, " +
                "cmp_origen, cmp_codusu_crea, cmp_fecha_crea, estado_envio) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, 0)",
                cmpy, codigo, datos.Nombre.Trim(), datos.FechaInicio.Date, FinDelDia(datos.FechaFin), datos.Estado,
                origen, Usuario(datos.Usuario), DateTime.Now);

        private static void ValidarCampana(BonCampanaDto datos)
        {
            if (string.IsNullOrWhiteSpace(datos.Nombre))
                throw new BonificadosException("El nombre de la campaña es obligatorio.");
            if (datos.Nombre.Trim().Length > LargoDescripcion)
                throw new BonificadosException($"El nombre de la campaña admite {LargoDescripcion} caracteres.");
            if (datos.FechaInicio == default || datos.FechaFin == default)
                throw new BonificadosException("Las fechas de inicio y fin son obligatorias.");
            if (datos.FechaFin.Date < datos.FechaInicio.Date)
                throw new BonificadosException("La fecha de fin debe ser igual o posterior a la de inicio.");
            datos.Estado = (datos.Estado ?? "").Trim().ToUpperInvariant();
            if (datos.Estado is not ("A" or "I" or "B"))
                throw new BonificadosException("El estado de la campaña debe ser A (activa), I (inactiva) o B (borrador).");
        }

        // =====================================================================
        // Reglas: lectura
        // =====================================================================

        public async Task<List<BonReglaDto>> ReglasAsync(string cmpy, string campana)
        {
            cmpy = Compania(cmpy);
            campana = Codigo(campana, "campaña");
            var sql = await BonificadosSql.AbrirAsync(_contexto);
            await CargarCampanaAsync(sql, cmpy, campana);
            return await CargarReglasAsync(sql, cmpy, "rbr_campana", campana);
        }

        public async Task<BonReglaDto> ReglaAsync(string cmpy, string codigo)
        {
            cmpy = Compania(cmpy);
            codigo = Codigo(codigo, "regla");
            var sql = await BonificadosSql.AbrirAsync(_contexto);
            return await CargarReglaAsync(sql, cmpy, codigo);
        }

        private async Task<BonReglaDto> CargarReglaAsync(BonificadosSql sql, string cmpy, string codigo) =>
            (await CargarReglasAsync(sql, cmpy, "rbr_regla", codigo)).FirstOrDefault()
            ?? throw new BonificadosException($"No existe la regla {codigo}.", 404);

        /// <param name="columna">rbr_regla o rbr_campana: constante interna, nunca entrada del usuario.</param>
        private async Task<List<BonReglaDto>> CargarReglasAsync(BonificadosSql sql, string cmpy, string columna, string valor)
        {
            var reglas = await sql.ConsultarAsync(
                "SELECT rbr_regla, rbr_campana, rbr_descr, rbr_tipo, rbr_prioridad, rbr_acumulable, rbr_activo, " +
                "rbr_fecini, rbr_fecfin, rbr_frecuencia, rbr_maxclimes, rbr_preciominbon, rbr_stocklimit, " +
                "rbr_codusu_crea, rbr_fecha_crea, rbr_codusu_mod, rbr_fecha_mod " +
                $"FROM mob_rbonreglas WHERE rbr_cmpy = ? AND {columna} = ? ORDER BY rbr_prioridad, rbr_regla",
                cmpy, valor);
            if (reglas.Count == 0) return new List<BonReglaDto>();

            var de = $"IN (SELECT rbr_regla FROM mob_rbonreglas WHERE rbr_cmpy = ? AND {columna} = ?)";

            async Task<ILookup<string, Fila>> Hijas(string select, string columnaRegla, string cmpyColumna, string orden) =>
                (await sql.ConsultarAsync($"{select} WHERE {cmpyColumna} = ? AND {columnaRegla} {de} ORDER BY {orden}",
                    cmpy, cmpy, valor))
                .ToLookup(f => f.TextoO(columnaRegla));

            var alcance = await Hijas("SELECT rbe_regla, rbe_tipoalcance, rbe_valor FROM mob_rbonalcance",
                "rbe_regla", "rbe_cmpy", "rbe_regla, rbe_tipoalcance, rbe_valor");
            var condiciones = await Hijas(
                "SELECT rbc_regla, rbc_seq, rbc_grupo, rbc_linea, rbc_subl, rbc_id, rbc_cantmin, rbc_multiplo, rbc_valmin, rbc_foco FROM mob_rbonregcond",
                "rbc_regla", "rbc_cmpy", "rbc_regla, rbc_seq");
            var niveles = await Hijas("SELECT rbn_regla, rbn_nivel, rbn_desde, rbn_hasta FROM mob_rbonnivel",
                "rbn_regla", "rbn_cmpy", "rbn_regla, rbn_nivel");
            var beneficiosNivel = await Hijas(
                "SELECT rnb_regla, rnb_nivel, rnb_seq, rnb_id, rnb_cant, rnb_formula FROM mob_rbonnivelbenef",
                "rnb_regla", "rnb_cmpy", "rnb_regla, rnb_nivel, rnb_seq");
            var beneficios = await Hijas(
                "SELECT rbb_regla, rbb_seq, rbb_tipobenef, rbb_cant, rbb_id, rbb_grupo, rbb_multiopc FROM mob_rbonregbenef",
                "rbb_regla", "rbb_cmpy", "rbb_regla, rbb_seq");
            var opciones = await Hijas(
                "SELECT rbo_regla, rbo_seqbenef, rbo_orden, rbo_id, rbo_cant FROM mob_rbonbenefopc",
                "rbo_regla", "rbo_cmpy", "rbo_regla, rbo_seqbenef, rbo_orden");
            var stock = await Hijas("SELECT rbs_regla, rbs_id, rbs_asignado, rbs_consumido FROM mob_rbonstock",
                "rbs_regla", "rbs_cmpy", "rbs_regla, rbs_id");

            var resultado = new List<BonReglaDto>();
            foreach (var r in reglas)
            {
                var codigo = r.TextoO("rbr_regla");
                var dto = new BonReglaDto
                {
                    Cmpy = cmpy,
                    Codigo = codigo,
                    Campana = r.TextoO("rbr_campana"),
                    Nombre = r.TextoO("rbr_descr"),
                    Tipo = r.Texto("rbr_tipo"),
                    Prioridad = r.Entero("rbr_prioridad") ?? 1,
                    Acumulable = r.SiNo("rbr_acumulable"),
                    Activa = r.Texto("rbr_activo") != "N",
                    FechaInicio = r.Fecha("rbr_fecini")?.Date,
                    FechaFin = r.Fecha("rbr_fecfin")?.Date,
                    Frecuencia = r.Texto("rbr_frecuencia") ?? "N",
                    MaximoPorCliente = r.Entero("rbr_maxclimes"),
                    PrecioMinimoBonificado = r.Decimal("rbr_preciominbon"),
                    StockLimitado = r.SiNo("rbr_stocklimit"),
                    UsuarioCreacion = r.Texto("rbr_codusu_crea"),
                    FechaCreacion = r.Fecha("rbr_fecha_crea"),
                    UsuarioModificacion = r.Texto("rbr_codusu_mod"),
                    FechaModificacion = r.Fecha("rbr_fecha_mod"),
                };

                foreach (var a in alcance[codigo])
                {
                    var valorAlcance = a.TextoO("rbe_valor");
                    switch (a.Texto("rbe_tipoalcance")?.ToUpperInvariant())
                    {
                        case "CANAL": dto.Canales.Add(valorAlcance); break;
                        case "REGION": dto.Regiones.Add(valorAlcance); break;
                        case "TERRITORIO": dto.Territorios.Add(valorAlcance); break;
                    }
                }

                // Las condiciones de producto foco (fase II) no son agrupación.
                var agrupacion = condiciones[codigo].Where(c => !c.SiNo("rbc_foco")).ToList();
                if (agrupacion.Any(c => c.Texto("rbc_grupo") != null))
                {
                    dto.Agrupacion = "GRUPO_MIXTO";
                    dto.AgrupacionValores = agrupacion.Select(c => c.Texto("rbc_grupo")).OfType<string>().ToList();
                }
                else if (agrupacion.Any(c => c.Texto("rbc_id") != null))
                {
                    dto.Agrupacion = "PRODUCTO";
                    dto.AgrupacionValores = agrupacion.Select(c => c.Texto("rbc_id")).OfType<string>().ToList();
                }
                else if (agrupacion.Any(c => c.Texto("rbc_subl") != null))
                {
                    dto.Agrupacion = "SUBLINEA";
                    dto.AgrupacionValores = agrupacion.Where(c => c.Texto("rbc_subl") != null)
                        .Select(c => $"{c.TextoO("rbc_linea")}|{c.TextoO("rbc_subl")}").ToList();
                }
                else
                {
                    dto.Agrupacion = "LINEA";
                    dto.AgrupacionValores = agrupacion.Select(c => c.Texto("rbc_linea")).OfType<string>().ToList();
                }
                dto.AgrupacionValores = dto.AgrupacionValores.Distinct().ToList();

                var primera = agrupacion.FirstOrDefault();
                var nivelesRegla = niveles[codigo].ToList();
                dto.UsaEscalonamiento = dto.Tipo == "C-03" || nivelesRegla.Count > 0;
                if (dto.UsaEscalonamiento)
                {
                    dto.Medida = primera?.Decimal("rbc_valmin") != null && primera.Decimal("rbc_cantmin") == null ? Monto : Unidades;
                }
                else
                {
                    dto.CantidadMinima = primera?.Decimal("rbc_cantmin");
                    dto.MontoMinimo = primera?.Decimal("rbc_valmin");
                    dto.Multiplo = primera?.Decimal("rbc_multiplo");
                }

                foreach (var n in nivelesRegla)
                {
                    var numero = n.Entero("rbn_nivel") ?? 0;
                    dto.Niveles.Add(new BonNivelDto
                    {
                        Nivel = numero,
                        Desde = n.Decimal("rbn_desde") ?? 0,
                        Hasta = n.Decimal("rbn_hasta"),
                        Beneficios = beneficiosNivel[codigo]
                            .Where(b => b.Entero("rnb_nivel") == numero && b.Texto("rnb_id") != null)
                            .Select(b => new BonProductoCantidadDto
                            {
                                ProductoId = b.TextoO("rnb_id"),
                                Cantidad = b.Decimal("rnb_cant") ?? 0,
                                IgualAlTotal = string.Equals(b.Texto("rnb_formula"), "IGUAL_TOTAL", StringComparison.OrdinalIgnoreCase),
                            }).ToList(),
                    });
                }

                foreach (var b in beneficios[codigo])
                {
                    var seq = b.Entero("rbb_seq") ?? 0;
                    var tipo = b.Texto("rbb_tipobenef")?.ToUpperInvariant();
                    if (b.SiNo("rbb_multiopc")) tipo = "MULTIPLE_OPTION";
                    dto.Beneficios.Add(new BonBeneficioDto
                    {
                        Tipo = TiposBeneficio.Contains(tipo) ? tipo! : "SPECIFIC_PRODUCT",
                        Cantidad = b.Decimal("rbb_cant") ?? 1,
                        ProductoId = b.Texto("rbb_id"),
                        Grupo = b.Texto("rbb_grupo"),
                        Opciones = opciones[codigo].Where(o => o.Entero("rbo_seqbenef") == seq)
                            .Select(o => new BonProductoCantidadDto
                            {
                                ProductoId = o.TextoO("rbo_id"),
                                Cantidad = o.Decimal("rbo_cant") ?? 1,
                            }).ToList(),
                    });
                }

                dto.Stock = stock[codigo].Select(s => new BonStockDto
                {
                    ProductoId = s.TextoO("rbs_id"),
                    Asignado = s.Entero("rbs_asignado") ?? 0,
                    Consumido = s.Entero("rbs_consumido") ?? 0,
                }).ToList();

                resultado.Add(dto);
            }

            await PonerNombresAsync(sql, cmpy, resultado);
            return resultado;
        }

        /// <summary>Completa la descripción de cada producto mencionado en las reglas.</summary>
        private static async Task PonerNombresAsync(BonificadosSql sql, string cmpy, List<BonReglaDto> reglas)
        {
            var ids = reglas.SelectMany(IdsDeProductos).Distinct().ToList();
            var nombres = await NombresDeProductosAsync(sql, cmpy, ids);
            string? Nombre(string? id) => id != null && nombres.TryGetValue(id, out var n) ? n : null;

            foreach (var r in reglas)
            {
                foreach (var b in r.Beneficios)
                {
                    b.ProductoNombre = Nombre(b.ProductoId);
                    b.Opciones.ForEach(o => o.ProductoNombre = Nombre(o.ProductoId));
                }
                r.Niveles.ForEach(n => n.Beneficios.ForEach(b => b.ProductoNombre = Nombre(b.ProductoId)));
                r.Stock.ForEach(s => s.ProductoNombre = Nombre(s.ProductoId));
            }
        }

        private static IEnumerable<string> IdsDeProductos(BonReglaDto r) =>
            r.Beneficios.Select(b => b.ProductoId)
                .Concat(r.Beneficios.SelectMany(b => b.Opciones).Select(o => o.ProductoId))
                .Concat(r.Niveles.SelectMany(n => n.Beneficios).Select(b => b.ProductoId))
                .Concat(r.Stock.Select(s => s.ProductoId))
                .Concat(r.Agrupacion == "PRODUCTO" ? r.AgrupacionValores : Enumerable.Empty<string>())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id!.Trim());

        private static async Task<Dictionary<string, string>> NombresDeProductosAsync(BonificadosSql sql, string cmpy, List<string> ids)
        {
            var nombres = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            // Por tandas: un IN con cientos de marcadores no es buena idea en ningún motor.
            foreach (var tanda in ids.Chunk(100))
            {
                var parametros = new List<object?> { cmpy };
                parametros.AddRange(tanda);
                var filas = await sql.ConsultarAsync(
                    $"SELECT rpr_id, rpr_desc FROM mob_rproductos WHERE rpr_cmpy = ? AND rpr_id IN ({BonificadosSql.Marcadores(tanda.Length)})",
                    parametros.ToArray());
                foreach (var f in filas) nombres[f.TextoO("rpr_id")] = f.Texto("rpr_desc") ?? f.TextoO("rpr_id");
            }
            return nombres;
        }

        // =====================================================================
        // Reglas: escritura
        // =====================================================================

        public Task<BonReglaDto> CrearReglaAsync(BonReglaDto datos) => GuardarReglaAsync(datos, esNueva: true);

        public Task<BonReglaDto> ActualizarReglaAsync(BonReglaDto datos) => GuardarReglaAsync(datos, esNueva: false);

        private async Task<BonReglaDto> GuardarReglaAsync(BonReglaDto datos, bool esNueva)
        {
            var cmpy = Compania(datos.Cmpy);
            datos.Cmpy = cmpy;
            datos.Campana = Codigo(datos.Campana, "campaña");
            NormalizarRegla(datos);

            var sql = await BonificadosSql.AbrirAsync(_contexto);
            await CargarCampanaAsync(sql, cmpy, datos.Campana);
            var tipo = TipoDeRegla(datos);
            await ValidarReferenciasAsync(sql, cmpy, datos, tipo);

            BonReglaDto? anterior = null;
            string codigo;
            if (esNueva)
            {
                codigo = "";
            }
            else
            {
                codigo = Codigo(datos.Codigo, "regla");
                anterior = await CargarReglaAsync(sql, cmpy, codigo);
                if (!string.Equals(anterior.Campana, datos.Campana, StringComparison.OrdinalIgnoreCase))
                    throw new BonificadosException($"La regla {codigo} pertenece a la campaña {anterior.Campana}.");
            }

            var nombres = await NombresDeProductosAsync(sql, cmpy, IdsDeProductos(datos).Distinct().ToList());

            sql.Iniciar();
            try
            {
                if (esNueva)
                {
                    codigo = await SiguienteCodigoAsync(sql, "mob_rbonreglas", "rbr_cmpy", "rbr_regla", "REG", cmpy);
                    await sql.EjecutarAsync(
                        "INSERT INTO mob_rbonreglas (rbr_cmpy, rbr_regla, rbr_campana, rbr_descr, rbr_tipo, rbr_prioridad, " +
                        "rbr_acumulable, rbr_activo, rbr_fecini, rbr_fecfin, rbr_frecuencia, rbr_maxclimes, rbr_preciominbon, " +
                        "rbr_stocklimit, rbr_codusu_crea, rbr_fecha_crea, estado_envio) " +
                        "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 0)",
                        cmpy, codigo, datos.Campana, datos.Nombre, tipo, datos.Prioridad, SN(datos.Acumulable), SN(datos.Activa),
                        datos.FechaInicio?.Date, datos.FechaFin == null ? null : FinDelDia(datos.FechaFin.Value),
                        datos.Frecuencia, datos.MaximoPorCliente, datos.PrecioMinimoBonificado, SN(datos.StockLimitado),
                        Usuario(datos.Usuario), DateTime.Now);
                }
                else
                {
                    // Solo las columnas que maneja el portal: las de fase II
                    // (días sin compra, categoría, QR, primer pedido) se conservan.
                    await sql.EjecutarAsync(
                        "UPDATE mob_rbonreglas SET rbr_descr = ?, rbr_tipo = ?, rbr_prioridad = ?, rbr_acumulable = ?, " +
                        "rbr_activo = ?, rbr_fecini = ?, rbr_fecfin = ?, rbr_frecuencia = ?, rbr_maxclimes = ?, " +
                        "rbr_preciominbon = ?, rbr_stocklimit = ?, rbr_codusu_mod = ?, rbr_fecha_mod = ?, estado_envio = 0 " +
                        "WHERE rbr_cmpy = ? AND rbr_regla = ?",
                        datos.Nombre, tipo, datos.Prioridad, SN(datos.Acumulable), SN(datos.Activa),
                        datos.FechaInicio?.Date, datos.FechaFin == null ? null : FinDelDia(datos.FechaFin.Value),
                        datos.Frecuencia, datos.MaximoPorCliente, datos.PrecioMinimoBonificado, SN(datos.StockLimitado),
                        Usuario(datos.Usuario), DateTime.Now, cmpy, codigo);
                }

                await BorrarHijasAsync(sql, cmpy, codigo, completo: false);
                await InsertarHijasAsync(sql, cmpy, codigo, datos, nombres);
                await GuardarStockAsync(sql, cmpy, codigo, datos.StockLimitado ? datos.Stock : new List<BonStockDto>());
                sql.Confirmar();
            }
            catch (Exception ex)
            {
                sql.Deshacer();
                throw Traducir(ex);
            }

            var guardada = await CargarReglaAsync(sql, cmpy, codigo);
            await AuditarAsync(sql, cmpy, codigo, datos.Campana,
                esNueva ? "CREADA" : anterior!.Activa && !guardada.Activa ? "DESACTIVADA" : "MODIFICADA",
                anterior, guardada, datos.Usuario);
            return guardada;
        }

        public async Task EliminarReglaAsync(string cmpy, string codigo, string? usuario)
        {
            cmpy = Compania(cmpy);
            codigo = Codigo(codigo, "regla");
            var sql = await BonificadosSql.AbrirAsync(_contexto);
            var anterior = await CargarReglaAsync(sql, cmpy, codigo);
            await ExigirSinUsoAsync(sql, cmpy, codigo, "la regla");

            sql.Iniciar();
            try
            {
                await BorrarHijasAsync(sql, cmpy, codigo, completo: true);
                await sql.EjecutarAsync("DELETE FROM mob_rbonreglas WHERE rbr_cmpy = ? AND rbr_regla = ?", cmpy, codigo);
                sql.Confirmar();
            }
            catch (Exception ex)
            {
                sql.Deshacer();
                throw Traducir(ex);
            }

            await AuditarAsync(sql, cmpy, codigo, anterior.Campana, "ELIMINADA", anterior, null, usuario);
        }

        /// <summary>C-03 con escalonamiento; C-02 si solo pide monto; C-01 en el resto.</summary>
        private static string TipoDeRegla(BonReglaDto r)
        {
            if (r.UsaEscalonamiento) return "C-03";
            if (r.MontoMinimo != null && r.CantidadMinima == null) return "C-02";
            return "C-01";
        }

        /// <summary>Limpia y valida la regla antes de tocar la base.</summary>
        private static void NormalizarRegla(BonReglaDto r)
        {
            r.Nombre = (r.Nombre ?? "").Trim();
            if (r.Nombre.Length == 0) throw new BonificadosException("El nombre de la regla es obligatorio.");
            if (r.Nombre.Length > LargoDescripcion)
                throw new BonificadosException($"El nombre de la regla admite {LargoDescripcion} caracteres.");
            if (r.Prioridad < 1 || r.Prioridad > 999) throw new BonificadosException("La prioridad va de 1 a 999.");
            if (r.FechaInicio != null && r.FechaFin != null && r.FechaFin.Value.Date < r.FechaInicio.Value.Date)
                throw new BonificadosException("La fecha de fin de la regla debe ser igual o posterior a la de inicio.");

            static List<string> Limpiar(IEnumerable<string>? valores, int largo, string que) =>
                (valores ?? Enumerable.Empty<string>())
                    .Select(v => (v ?? "").Trim()).Where(v => v.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase)
                    .Select(v => v.Length <= largo ? v : throw new BonificadosException($"El código {v} de {que} es demasiado largo."))
                    .ToList();

            r.Canales = Limpiar(r.Canales, 20, "canal");
            r.Regiones = Limpiar(r.Regiones, 20, "región");
            r.Territorios = Limpiar(r.Territorios, 20, "territorio");

            r.Agrupacion = (r.Agrupacion ?? "").Trim().ToUpperInvariant();
            if (!Agrupaciones.Contains(r.Agrupacion))
                throw new BonificadosException("La agrupación debe ser PRODUCTO, LINEA, SUBLINEA o GRUPO_MIXTO.");
            r.AgrupacionValores = Limpiar(r.AgrupacionValores, 32, "la agrupación");
            if (r.AgrupacionValores.Count == 0)
                throw new BonificadosException("Elige al menos un elemento en la agrupación de productos.");
            if (r.Agrupacion == "GRUPO_MIXTO" && r.AgrupacionValores.Count > 1)
                throw new BonificadosException("Una regla usa un solo grupo mixto.");
            if (r.Agrupacion == "SUBLINEA" && r.AgrupacionValores.Any(v => v.Split('|').Length != 2))
                throw new BonificadosException("Cada sublínea debe llegar como \"linea|sublinea\".");

            r.Medida = (r.Medida ?? Unidades).Trim().ToUpperInvariant();
            if (r.Medida is not (Unidades or Monto)) throw new BonificadosException("La medida debe ser UNIDADES o MONTO.");

            if (r.Multiplo is <= 0) r.Multiplo = null;
            if (r.UsaEscalonamiento)
            {
                r.CantidadMinima = null;
                r.MontoMinimo = null;
                r.Multiplo = null;
                r.Beneficios = new List<BonBeneficioDto>();
                r.Niveles = (r.Niveles ?? new List<BonNivelDto>()).OrderBy(n => n.Desde).ToList();
                if (r.Niveles.Count == 0) throw new BonificadosException("Agrega al menos un nivel de escalonamiento.");
                for (var i = 0; i < r.Niveles.Count; i++)
                {
                    var nivel = r.Niveles[i];
                    nivel.Nivel = i + 1;
                    if (nivel.Desde <= 0) throw new BonificadosException($"El nivel {i + 1} debe arrancar en un valor mayor que cero.");
                    if (i > 0 && nivel.Desde == r.Niveles[i - 1].Desde)
                        throw new BonificadosException($"Los niveles {i} y {i + 1} arrancan en el mismo valor.");
                    nivel.Beneficios = (nivel.Beneficios ?? new List<BonProductoCantidadDto>())
                        .Where(b => !string.IsNullOrWhiteSpace(b.ProductoId)).ToList();
                    if (nivel.Beneficios.Count == 0)
                        throw new BonificadosException($"El nivel {i + 1} no tiene productos bonificados.");
                    foreach (var b in nivel.Beneficios)
                    {
                        b.ProductoId = b.ProductoId.Trim();
                        if (!b.IgualAlTotal && b.Cantidad <= 0)
                            throw new BonificadosException($"El nivel {i + 1} tiene una cantidad bonificada en cero.");
                    }
                }
            }
            else
            {
                r.Niveles = new List<BonNivelDto>();
                if (r.CantidadMinima is <= 0) r.CantidadMinima = null;
                if (r.MontoMinimo is <= 0) r.MontoMinimo = null;
                if (r.CantidadMinima == null && r.MontoMinimo == null)
                    throw new BonificadosException("Indica una cantidad mínima, un monto mínimo o ambos.");
                if (r.Multiplo != null && r.CantidadMinima == null)
                    throw new BonificadosException("Para repetir el beneficio por cada N unidades hace falta una cantidad mínima.");

                r.Beneficios = (r.Beneficios ?? new List<BonBeneficioDto>()).ToList();
                if (r.Beneficios.Count == 0) throw new BonificadosException("Agrega al menos un beneficio.");
                for (var i = 0; i < r.Beneficios.Count; i++)
                {
                    var b = r.Beneficios[i];
                    b.Tipo = (b.Tipo ?? "").Trim().ToUpperInvariant();
                    if (!TiposBeneficio.Contains(b.Tipo)) throw new BonificadosException($"El beneficio {i + 1} tiene un tipo desconocido.");
                    if (b.Cantidad <= 0) throw new BonificadosException($"El beneficio {i + 1} tiene cantidad en cero.");
                    b.ProductoId = string.IsNullOrWhiteSpace(b.ProductoId) ? null : b.ProductoId.Trim();
                    b.Grupo = string.IsNullOrWhiteSpace(b.Grupo) ? null : b.Grupo.Trim();
                    b.Opciones = (b.Opciones ?? new List<BonProductoCantidadDto>())
                        .Where(o => !string.IsNullOrWhiteSpace(o.ProductoId)).ToList();
                    b.Opciones.ForEach(o => o.ProductoId = o.ProductoId.Trim());

                    switch (b.Tipo)
                    {
                        case "SPECIFIC_PRODUCT" when b.ProductoId == null:
                            throw new BonificadosException($"Elige el producto del beneficio {i + 1}.");
                        case "LOWEST_VALUE" when b.Grupo == null:
                            throw new BonificadosException($"Elige el grupo del beneficio {i + 1}.");
                        case "MULTIPLE_OPTION" when b.Opciones.Count < 2:
                            throw new BonificadosException($"El beneficio {i + 1} es a elección: necesita al menos dos opciones.");
                    }
                    if (b.Tipo != "SPECIFIC_PRODUCT") b.ProductoId = null;
                    if (b.Tipo != "LOWEST_VALUE") b.Grupo = null;
                    if (b.Tipo != "MULTIPLE_OPTION") b.Opciones = new List<BonProductoCantidadDto>();
                }
            }

            if (r.PrecioMinimoBonificado is <= 0) r.PrecioMinimoBonificado = null;

            r.Frecuencia = (r.Frecuencia ?? "N").Trim().ToUpperInvariant();
            if (r.Frecuencia is not ("N" or "M" or "U"))
                throw new BonificadosException("La frecuencia debe ser N (sin límite), M (por mes) o U (una vez por cliente).");
            if (r.Frecuencia == "N") r.MaximoPorCliente = null;
            else if (r.MaximoPorCliente is null or <= 0) r.MaximoPorCliente = 1;

            r.Stock = (r.Stock ?? new List<BonStockDto>())
                .Where(s => !string.IsNullOrWhiteSpace(s.ProductoId))
                .GroupBy(s => s.ProductoId.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => new BonStockDto { ProductoId = g.Key, Asignado = g.Last().Asignado })
                .ToList();
            if (r.StockLimitado)
            {
                if (r.Stock.Count == 0) throw new BonificadosException("El stock limitado necesita al menos un producto con cupo.");
                if (r.Stock.Any(s => s.Asignado <= 0)) throw new BonificadosException("El cupo de stock debe ser mayor que cero.");
            }
        }

        /// <summary>Que existan el tipo C-xx, los productos, líneas, sublíneas y grupos citados.</summary>
        private static async Task ValidarReferenciasAsync(BonificadosSql sql, string cmpy, BonReglaDto r, string tipo)
        {
            if (await sql.ContarAsync("SELECT COUNT(*) FROM mob_rbontipos WHERE rbt_cmpy = ? AND rbt_tipo = ?", cmpy, tipo) == 0)
                throw new BonificadosException(
                    $"Falta el tipo {tipo} en mob_rbontipos para la compañía {cmpy}. Hay que cargar el catálogo C-01..C-11.", 409);

            var ids = IdsDeProductos(r).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var existentes = await NombresDeProductosAsync(sql, cmpy, ids);
            var faltantes = ids.Where(id => !existentes.ContainsKey(id)).ToList();
            if (faltantes.Count > 0)
                throw new BonificadosException($"No existen estos productos: {string.Join(", ", faltantes.Take(10))}.");

            var grupos = r.Beneficios.Select(b => b.Grupo)
                .Concat(r.Agrupacion == "GRUPO_MIXTO" ? r.AgrupacionValores : Enumerable.Empty<string>())
                .OfType<string>().Distinct().ToList();
            foreach (var grupo in grupos)
            {
                if (await sql.ContarAsync("SELECT COUNT(*) FROM mob_rbongrupo WHERE grp_cmpy = ? AND grp_grupo = ?", cmpy, grupo) == 0)
                    throw new BonificadosException($"No existe el grupo {grupo}.");
            }

            if (r.Agrupacion == "LINEA")
            {
                foreach (var linea in r.AgrupacionValores)
                {
                    if (await sql.ContarAsync("SELECT COUNT(*) FROM mob_rlinea WHERE rln_cmpy = ? AND rln_linea = ?", cmpy, linea) == 0)
                        throw new BonificadosException($"No existe la línea {linea}.");
                }
            }
            else if (r.Agrupacion == "SUBLINEA")
            {
                foreach (var par in r.AgrupacionValores.Select(v => v.Split('|')))
                {
                    if (await sql.ContarAsync(
                            "SELECT COUNT(*) FROM mob_rsublineas WHERE rsb_cmpy = ? AND rsb_linea = ? AND rsb_subl = ?",
                            cmpy, par[0], par[1]) == 0)
                        throw new BonificadosException($"No existe la sublínea {par[1]} de la línea {par[0]}.");
                }
            }
        }

        /// <summary>
        /// Borra las hijas de una regla. Con <paramref name="completo"/> en false
        /// (al editar) deja las condiciones de producto foco, los alcances que
        /// el portal no maneja y el stock, que se reconcilia aparte.
        /// </summary>
        private static async Task BorrarHijasAsync(BonificadosSql sql, string cmpy, string regla, bool completo)
        {
            await sql.EjecutarAsync("DELETE FROM mob_rbonbenefopc WHERE rbo_cmpy = ? AND rbo_regla = ?", cmpy, regla);
            await sql.EjecutarAsync("DELETE FROM mob_rbonregbenef WHERE rbb_cmpy = ? AND rbb_regla = ?", cmpy, regla);
            await sql.EjecutarAsync("DELETE FROM mob_rbonnivelbenef WHERE rnb_cmpy = ? AND rnb_regla = ?", cmpy, regla);
            await sql.EjecutarAsync("DELETE FROM mob_rbonnivel WHERE rbn_cmpy = ? AND rbn_regla = ?", cmpy, regla);

            if (completo)
            {
                await sql.EjecutarAsync("DELETE FROM mob_rbonregcond WHERE rbc_cmpy = ? AND rbc_regla = ?", cmpy, regla);
                await sql.EjecutarAsync("DELETE FROM mob_rbonalcance WHERE rbe_cmpy = ? AND rbe_regla = ?", cmpy, regla);
                await sql.EjecutarAsync("DELETE FROM mob_rbonstock WHERE rbs_cmpy = ? AND rbs_regla = ?", cmpy, regla);
            }
            else
            {
                await sql.EjecutarAsync(
                    "DELETE FROM mob_rbonregcond WHERE rbc_cmpy = ? AND rbc_regla = ? AND (rbc_foco IS NULL OR rbc_foco <> 'S')",
                    cmpy, regla);
                await sql.EjecutarAsync(
                    "DELETE FROM mob_rbonalcance WHERE rbe_cmpy = ? AND rbe_regla = ? AND rbe_tipoalcance IN ('CANAL', 'REGION', 'TERRITORIO')",
                    cmpy, regla);
            }
        }

        private static async Task InsertarHijasAsync(BonificadosSql sql, string cmpy, string regla, BonReglaDto r,
            Dictionary<string, string> nombres)
        {
            string NombreDe(string? id) => id != null && nombres.TryGetValue(id, out var n) ? n : id ?? "";

            foreach (var (tipo, valores) in new[] { ("CANAL", r.Canales), ("REGION", r.Regiones), ("TERRITORIO", r.Territorios) })
            {
                foreach (var valor in valores)
                {
                    await sql.EjecutarAsync(
                        "INSERT INTO mob_rbonalcance (rbe_cmpy, rbe_regla, rbe_tipoalcance, rbe_valor) VALUES (?, ?, ?, ?)",
                        cmpy, regla, tipo, valor);
                }
            }

            // Condiciones: una fila por elemento de la agrupación; las cantidades
            // se suman entre todas (OR) y el mínimo va repetido en cada fila.
            decimal? cantidadMinima = r.CantidadMinima, montoMinimo = r.MontoMinimo;
            if (r.UsaEscalonamiento)
            {
                cantidadMinima = r.Medida == Unidades ? r.Niveles[0].Desde : null;
                montoMinimo = r.Medida == Monto ? r.Niveles[0].Desde : null;
            }

            var seq = Convert.ToInt32(await sql.EscalarAsync(
                "SELECT MAX(rbc_seq) FROM mob_rbonregcond WHERE rbc_cmpy = ? AND rbc_regla = ?", cmpy, regla) ?? 0,
                CultureInfo.InvariantCulture);
            foreach (var valor in r.AgrupacionValores)
            {
                string? grupo = null, linea = null, sublinea = null, producto = null;
                switch (r.Agrupacion)
                {
                    case "GRUPO_MIXTO": grupo = valor; break;
                    case "PRODUCTO": producto = valor; break;
                    case "LINEA": linea = valor; break;
                    case "SUBLINEA":
                        var partes = valor.Split('|');
                        linea = partes[0];
                        sublinea = partes[1];
                        break;
                }

                await sql.EjecutarAsync(
                    "INSERT INTO mob_rbonregcond (rbc_cmpy, rbc_regla, rbc_seq, rbc_operador, rbc_grupo, rbc_linea, rbc_subl, " +
                    "rbc_id, rbc_cantmin, rbc_multiplo, rbc_valmin, rbc_foco) VALUES (?, ?, ?, 'OR', ?, ?, ?, ?, ?, ?, ?, 'N')",
                    cmpy, regla, ++seq, grupo, linea, sublinea, producto, cantidadMinima,
                    r.UsaEscalonamiento ? null : r.Multiplo, montoMinimo);
            }

            // Niveles: "hasta" es el arranque del siguiente menos una unidad (o un centavo).
            var paso = r.Medida == Monto ? 0.01m : 1m;
            for (var i = 0; i < r.Niveles.Count; i++)
            {
                var nivel = r.Niveles[i];
                decimal? hasta = i + 1 < r.Niveles.Count ? r.Niveles[i + 1].Desde - paso : null;
                await sql.EjecutarAsync(
                    "INSERT INTO mob_rbonnivel (rbn_cmpy, rbn_regla, rbn_nivel, rbn_descr, rbn_desde, rbn_hasta) VALUES (?, ?, ?, ?, ?, ?)",
                    cmpy, regla, nivel.Nivel, $"Nivel {nivel.Nivel}", nivel.Desde, hasta);

                var orden = 0;
                foreach (var b in nivel.Beneficios)
                {
                    await sql.EjecutarAsync(
                        "INSERT INTO mob_rbonnivelbenef (rnb_cmpy, rnb_regla, rnb_nivel, rnb_seq, rnb_id, rnb_cant, rnb_formula) " +
                        "VALUES (?, ?, ?, ?, ?, ?, ?)",
                        cmpy, regla, nivel.Nivel, ++orden, b.ProductoId, b.IgualAlTotal ? 0 : b.Cantidad,
                        b.IgualAlTotal ? "IGUAL_TOTAL" : null);
                }
            }

            for (var i = 0; i < r.Beneficios.Count; i++)
            {
                var b = r.Beneficios[i];
                var secuencia = i + 1;
                var descripcion = b.Tipo switch
                {
                    "SPECIFIC_PRODUCT" => NombreDe(b.ProductoId),
                    "SAME_PRODUCT" => "Mismo producto comprado",
                    "LOWEST_VALUE" => $"El de menor valor del grupo {b.Grupo}",
                    _ => $"A elección entre {b.Opciones.Count} productos",
                };

                await sql.EjecutarAsync(
                    "INSERT INTO mob_rbonregbenef (rbb_cmpy, rbb_regla, rbb_seq, rbb_tipobenef, rbb_cant, rbb_id, rbb_grupo, " +
                    "rbb_multiopc, rbb_descr) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)",
                    cmpy, regla, secuencia, b.Tipo, b.Cantidad, b.ProductoId, b.Grupo,
                    SN(b.Tipo == "MULTIPLE_OPTION"), Recortar(descripcion, LargoBeneficio));

                var orden = 0;
                foreach (var o in b.Opciones)
                {
                    await sql.EjecutarAsync(
                        "INSERT INTO mob_rbonbenefopc (rbo_cmpy, rbo_regla, rbo_seqbenef, rbo_orden, rbo_id, rbo_cant, rbo_descr) " +
                        "VALUES (?, ?, ?, ?, ?, ?, ?)",
                        cmpy, regla, secuencia, ++orden, o.ProductoId, b.Cantidad, Recortar(NombreDe(o.ProductoId), LargoBeneficio));
                }
            }
        }

        /// <summary>
        /// Ajusta el cupo sin perder lo consumido: actualiza el asignado de los
        /// productos que siguen, agrega los nuevos y quita solo los que nunca se
        /// consumieron.
        /// </summary>
        private static async Task GuardarStockAsync(BonificadosSql sql, string cmpy, string regla, List<BonStockDto> deseado)
        {
            var actuales = (await sql.ConsultarAsync(
                    "SELECT rbs_id, rbs_consumido FROM mob_rbonstock WHERE rbs_cmpy = ? AND rbs_regla = ?", cmpy, regla))
                .ToDictionary(f => f.TextoO("rbs_id"), f => f.Entero("rbs_consumido") ?? 0, StringComparer.OrdinalIgnoreCase);

            foreach (var s in deseado)
            {
                if (actuales.ContainsKey(s.ProductoId))
                {
                    await sql.EjecutarAsync(
                        "UPDATE mob_rbonstock SET rbs_asignado = ?, rbs_fecha_mod = ?, estado_envio = 0 " +
                        "WHERE rbs_cmpy = ? AND rbs_regla = ? AND rbs_id = ?",
                        s.Asignado, DateTime.Now, cmpy, regla, s.ProductoId);
                }
                else
                {
                    await sql.EjecutarAsync(
                        "INSERT INTO mob_rbonstock (rbs_cmpy, rbs_regla, rbs_id, rbs_asignado, rbs_consumido, rbs_fecha_mod, estado_envio) " +
                        "VALUES (?, ?, ?, ?, 0, ?, 0)",
                        cmpy, regla, s.ProductoId, s.Asignado, DateTime.Now);
                }
            }

            foreach (var (producto, consumido) in actuales)
            {
                if (consumido == 0 && !deseado.Any(s => string.Equals(s.ProductoId, producto, StringComparison.OrdinalIgnoreCase)))
                {
                    await sql.EjecutarAsync("DELETE FROM mob_rbonstock WHERE rbs_cmpy = ? AND rbs_regla = ? AND rbs_id = ?",
                        cmpy, regla, producto);
                }
            }
        }

        /// <summary>Tablas hijas de una regla, en el orden en que se insertan (padres primero).</summary>
        private static readonly (string Tabla, string Cmpy, string Regla)[] TablasDeRegla =
        {
            ("mob_rbonregcond", "rbc_cmpy", "rbc_regla"),
            ("mob_rbonalcance", "rbe_cmpy", "rbe_regla"),
            ("mob_rbonnivel", "rbn_cmpy", "rbn_regla"),
            ("mob_rbonnivelbenef", "rnb_cmpy", "rnb_regla"),
            ("mob_rbonregbenef", "rbb_cmpy", "rbb_regla"),
            ("mob_rbonbenefopc", "rbo_cmpy", "rbo_regla"),
            ("mob_rbonstock", "rbs_cmpy", "rbs_regla"),
        };

        /// <summary>
        /// Copia la regla con todas sus columnas (también las de fase II que el
        /// portal todavía no edita). Los nombres de columna salen de la propia
        /// tabla, no de la petición.
        /// </summary>
        private static async Task CopiarReglaAsync(BonificadosSql sql, string cmpy, string origen, string destino,
            string campana, string? usuario)
        {
            var regla = (await sql.ConsultarAsync("SELECT * FROM mob_rbonreglas WHERE rbr_cmpy = ? AND rbr_regla = ?", cmpy, origen))
                .Single();
            await InsertarFilaAsync(sql, "mob_rbonreglas", regla, new Dictionary<string, object?>
            {
                ["rbr_regla"] = destino,
                ["rbr_campana"] = campana,
                ["rbr_codusu_crea"] = Usuario(usuario),
                ["rbr_fecha_crea"] = DateTime.Now,
                ["rbr_codusu_mod"] = null,
                ["rbr_fecha_mod"] = null,
                ["estado_envio"] = 0,
            });

            foreach (var (tabla, columnaCmpy, columnaRegla) in TablasDeRegla)
            {
                var filas = await sql.ConsultarAsync($"SELECT * FROM {tabla} WHERE {columnaCmpy} = ? AND {columnaRegla} = ?", cmpy, origen);
                foreach (var fila in filas)
                {
                    var cambios = new Dictionary<string, object?> { [columnaRegla] = destino };
                    if (tabla == "mob_rbonstock")
                    {
                        cambios["rbs_consumido"] = 0;
                        cambios["rbs_fecha_mod"] = DateTime.Now;
                        cambios["estado_envio"] = 0;
                    }
                    await InsertarFilaAsync(sql, tabla, fila, cambios);
                }
            }
        }

        private static Task<int> InsertarFilaAsync(BonificadosSql sql, string tabla, Fila fila, Dictionary<string, object?> cambios)
        {
            var columnas = fila.Valores.Keys.ToList();
            var valores = columnas.Select(c => cambios.TryGetValue(c, out var v) ? v : fila.Valores[c]).ToArray();
            return sql.EjecutarAsync(
                $"INSERT INTO {tabla} ({string.Join(", ", columnas)}) VALUES ({BonificadosSql.Marcadores(columnas.Count)})",
                valores);
        }

        /// <summary>Una regla que ya bonificó pedidos no se borra: se desactiva.</summary>
        private static async Task ExigirSinUsoAsync(BonificadosSql sql, string cmpy, string regla, string que)
        {
            var pedidos = await sql.ContarAsync("SELECT COUNT(*) FROM mob_rordbonif WHERE orb_cmpy = ? AND orb_regla = ?", cmpy, regla);
            var consumos = await sql.ContarAsync("SELECT COUNT(*) FROM mob_rbonconsumo WHERE cns_cmpy = ? AND cns_regla = ?", cmpy, regla);
            var eventos = await sql.ContarAsync("SELECT COUNT(*) FROM mob_rbonevento WHERE evt_cmpy = ? AND evt_regla = ?", cmpy, regla);
            if (pedidos + consumos + eventos == 0) return;

            var detalle = new List<string>();
            if (pedidos > 0) detalle.Add($"{pedidos} pedido(s)");
            if (consumos > 0) detalle.Add($"{consumos} registro(s) de consumo");
            if (eventos > 0) detalle.Add($"{eventos} evento(s) QR");
            throw new BonificadosException(
                $"No se puede eliminar {que}: la regla {regla} ya tiene {string.Join(", ", detalle)}. Desactívala en su lugar.", 409);
        }

        // =====================================================================
        // Grupos mixtos
        // =====================================================================

        private static readonly string[] NivelesGrupo = { "LINEA", "SUBLINEA", "REFERENCIA" };

        public async Task<List<BonGrupoEditarDto>> GruposAsync(string cmpy)
        {
            cmpy = Compania(cmpy);
            var sql = await BonificadosSql.AbrirAsync(_contexto);
            return await CargarGruposAsync(sql, cmpy, null);
        }

        public async Task<BonGrupoEditarDto> GrupoAsync(string cmpy, string codigo)
        {
            cmpy = Compania(cmpy);
            codigo = Codigo(codigo, "grupo");
            var sql = await BonificadosSql.AbrirAsync(_contexto);
            return (await CargarGruposAsync(sql, cmpy, codigo)).FirstOrDefault()
                ?? throw new BonificadosException($"No existe el grupo {codigo}.", 404);
        }

        public Task<BonGrupoEditarDto> CrearGrupoAsync(BonGrupoEditarDto datos) => GuardarGrupoAsync(datos, esNuevo: true);

        public Task<BonGrupoEditarDto> ActualizarGrupoAsync(BonGrupoEditarDto datos) => GuardarGrupoAsync(datos, esNuevo: false);

        public async Task EliminarGrupoAsync(string cmpy, string codigo, string? usuario)
        {
            cmpy = Compania(cmpy);
            codigo = Codigo(codigo, "grupo");
            var sql = await BonificadosSql.AbrirAsync(_contexto);
            var anterior = (await CargarGruposAsync(sql, cmpy, codigo)).FirstOrDefault()
                ?? throw new BonificadosException($"No existe el grupo {codigo}.", 404);
            if (anterior.Reglas.Count > 0)
                throw new BonificadosException(
                    $"El grupo {codigo} lo usan las reglas {string.Join(", ", anterior.Reglas.Take(10))}. Quítalo de ellas o desactívalo.", 409);

            sql.Iniciar();
            try
            {
                await sql.EjecutarAsync("DELETE FROM mob_rbongrupodet WHERE grd_cmpy = ? AND grd_grupo = ?", cmpy, codigo);
                await sql.EjecutarAsync("DELETE FROM mob_rbongrupo WHERE grp_cmpy = ? AND grp_grupo = ?", cmpy, codigo);
                sql.Confirmar();
            }
            catch (Exception ex)
            {
                sql.Deshacer();
                throw Traducir(ex);
            }
            await AuditarAsync(sql, cmpy, null, null, "GRUPO ELIMINADO", anterior, null, usuario);
        }

        private async Task<BonGrupoEditarDto> GuardarGrupoAsync(BonGrupoEditarDto datos, bool esNuevo)
        {
            var cmpy = Compania(datos.Cmpy);
            datos.Nombre = (datos.Nombre ?? "").Trim();
            if (datos.Nombre.Length == 0) throw new BonificadosException("El nombre del grupo es obligatorio.");
            if (datos.Nombre.Length > 100) throw new BonificadosException("El nombre del grupo admite 100 caracteres.");

            var elementos = (datos.Elementos ?? new List<BonGrupoElementoDto>())
                .Select(e => new BonGrupoElementoDto { Nivel = (e.Nivel ?? "").Trim().ToUpperInvariant(), Valor = (e.Valor ?? "").Trim() })
                .Where(e => e.Valor.Length > 0)
                .GroupBy(e => (e.Nivel, e.Valor.ToUpperInvariant())).Select(g => g.First())
                .ToList();
            if (elementos.Count == 0) throw new BonificadosException("El grupo necesita al menos un elemento.");
            if (elementos.Count > 999) throw new BonificadosException("Un grupo admite hasta 999 elementos.");
            foreach (var e in elementos)
            {
                if (!NivelesGrupo.Contains(e.Nivel)) throw new BonificadosException($"Nivel de elemento desconocido: {e.Nivel}.");
                if (e.Valor.Length > 21) throw new BonificadosException($"El código {e.Valor} es demasiado largo.");
                if (e.Nivel == "SUBLINEA" && e.Valor.Split('|').Length != 2)
                    throw new BonificadosException("Cada sublínea del grupo debe llegar como \"linea|sublinea\".");
            }

            var sql = await BonificadosSql.AbrirAsync(_contexto);
            await ValidarElementosAsync(sql, cmpy, elementos);

            BonGrupoEditarDto? anterior = null;
            var codigo = "";
            if (!esNuevo)
            {
                codigo = Codigo(datos.Codigo, "grupo");
                anterior = (await CargarGruposAsync(sql, cmpy, codigo)).FirstOrDefault()
                    ?? throw new BonificadosException($"No existe el grupo {codigo}.", 404);
            }

            sql.Iniciar();
            try
            {
                if (esNuevo)
                {
                    codigo = await SiguienteCodigoAsync(sql, "mob_rbongrupo", "grp_cmpy", "grp_grupo", "GRP", cmpy);
                    await sql.EjecutarAsync(
                        "INSERT INTO mob_rbongrupo (grp_cmpy, grp_grupo, grp_descr, grp_activo, estado_envio) VALUES (?, ?, ?, ?, 0)",
                        cmpy, codigo, datos.Nombre, SN(datos.Activo));
                }
                else
                {
                    await sql.EjecutarAsync(
                        "UPDATE mob_rbongrupo SET grp_descr = ?, grp_activo = ?, estado_envio = 0 WHERE grp_cmpy = ? AND grp_grupo = ?",
                        datos.Nombre, SN(datos.Activo), cmpy, codigo);
                    await sql.EjecutarAsync("DELETE FROM mob_rbongrupodet WHERE grd_cmpy = ? AND grd_grupo = ?", cmpy, codigo);
                }

                var seq = 0;
                foreach (var e in elementos)
                {
                    await sql.EjecutarAsync(
                        "INSERT INTO mob_rbongrupodet (grd_cmpy, grd_grupo, grd_seq, grd_nivel, grd_valor) VALUES (?, ?, ?, ?, ?)",
                        cmpy, codigo, ++seq, e.Nivel, e.Valor);
                }
                sql.Confirmar();
            }
            catch (Exception ex)
            {
                sql.Deshacer();
                throw Traducir(ex);
            }

            var guardado = (await CargarGruposAsync(sql, cmpy, codigo)).First();
            await AuditarAsync(sql, cmpy, null, null, esNuevo ? "GRUPO CREADO" : "GRUPO MODIFICADO", anterior, guardado, datos.Usuario);
            return guardado;
        }

        private static async Task ValidarElementosAsync(BonificadosSql sql, string cmpy, List<BonGrupoElementoDto> elementos)
        {
            var productos = elementos.Where(e => e.Nivel == "REFERENCIA").Select(e => e.Valor).ToList();
            var existentes = await NombresDeProductosAsync(sql, cmpy, productos);
            var faltan = productos.Where(p => !existentes.ContainsKey(p)).ToList();
            if (faltan.Count > 0) throw new BonificadosException($"No existen estos productos: {string.Join(", ", faltan.Take(10))}.");

            foreach (var e in elementos.Where(e => e.Nivel == "LINEA"))
            {
                if (await sql.ContarAsync("SELECT COUNT(*) FROM mob_rlinea WHERE rln_cmpy = ? AND rln_linea = ?", cmpy, e.Valor) == 0)
                    throw new BonificadosException($"No existe la línea {e.Valor}.");
            }
            foreach (var par in elementos.Where(e => e.Nivel == "SUBLINEA").Select(e => e.Valor.Split('|')))
            {
                if (await sql.ContarAsync("SELECT COUNT(*) FROM mob_rsublineas WHERE rsb_cmpy = ? AND rsb_linea = ? AND rsb_subl = ?",
                        cmpy, par[0], par[1]) == 0)
                    throw new BonificadosException($"No existe la sublínea {par[1]} de la línea {par[0]}.");
            }
        }

        /// <param name="codigo">Null = todos los grupos de la compañía.</param>
        private static async Task<List<BonGrupoEditarDto>> CargarGruposAsync(BonificadosSql sql, string cmpy, string? codigo)
        {
            var filtro = codigo == null ? "" : " AND grp_grupo = ?";
            var parametros = codigo == null ? new object?[] { cmpy } : new object?[] { cmpy, codigo };
            var grupos = await sql.ConsultarAsync(
                $"SELECT grp_grupo, grp_descr, grp_activo FROM mob_rbongrupo WHERE grp_cmpy = ?{filtro} ORDER BY grp_descr", parametros);
            if (grupos.Count == 0) return new List<BonGrupoEditarDto>();

            var detalle = (await sql.ConsultarAsync(
                    $"SELECT grd_grupo, grd_seq, grd_nivel, grd_valor FROM mob_rbongrupodet WHERE grd_cmpy = ?" +
                    (codigo == null ? "" : " AND grd_grupo = ?") + " ORDER BY grd_grupo, grd_seq", parametros))
                .ToLookup(f => f.TextoO("grd_grupo"));

            // Reglas que lo usan como agrupación o como "menor valor del grupo".
            var usos = (await sql.ConsultarAsync(
                    "SELECT rbc_grupo AS grupo, rbc_regla AS regla FROM mob_rbonregcond WHERE rbc_cmpy = ? AND rbc_grupo IS NOT NULL " +
                    "UNION SELECT rbb_grupo AS grupo, rbb_regla AS regla FROM mob_rbonregbenef WHERE rbb_cmpy = ? AND rbb_grupo IS NOT NULL",
                    cmpy, cmpy))
                .ToLookup(f => f.TextoO("grupo"), f => f.TextoO("regla"));

            // Nombres para mostrar: productos, líneas y sublíneas citados.
            var ids = detalle.SelectMany(g => g).Where(d => d.Texto("grd_nivel") == "REFERENCIA").Select(d => d.TextoO("grd_valor")).Distinct().ToList();
            var productos = await NombresDeProductosAsync(sql, cmpy, ids);
            var lineas = (await sql.ConsultarAsync("SELECT rln_linea, rln_descr FROM mob_rlinea WHERE rln_cmpy = ?", cmpy))
                .ToDictionary(f => f.TextoO("rln_linea"), f => f.Texto("rln_descr") ?? f.TextoO("rln_linea"), StringComparer.OrdinalIgnoreCase);
            var sublineas = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in await sql.ConsultarAsync("SELECT rsb_linea, rsb_subl, rsb_descr FROM mob_rsublineas WHERE rsb_cmpy = ?", cmpy))
                sublineas[$"{f.TextoO("rsb_linea")}|{f.TextoO("rsb_subl")}"] = f.Texto("rsb_descr") ?? f.TextoO("rsb_subl");

            return grupos.Select(g =>
            {
                var cod = g.TextoO("grp_grupo");
                return new BonGrupoEditarDto
                {
                    Cmpy = cmpy,
                    Codigo = cod,
                    Nombre = g.Texto("grp_descr") ?? cod,
                    Activo = g.Texto("grp_activo") != "N",
                    Reglas = usos[cod].Distinct().ToList(),
                    Elementos = detalle[cod].Select(d =>
                    {
                        var nivel = d.TextoO("grd_nivel").ToUpperInvariant();
                        var valor = d.TextoO("grd_valor");
                        var mapa = nivel switch { "REFERENCIA" => productos, "LINEA" => lineas, "SUBLINEA" => sublineas, _ => null };
                        return new BonGrupoElementoDto
                        {
                            Nivel = nivel,
                            Valor = valor,
                            Nombre = mapa != null && mapa.TryGetValue(valor, out var n) ? n : null,
                        };
                    }).ToList(),
                };
            }).ToList();
        }

        // =====================================================================
        // Utilidades
        // =====================================================================

        /// <summary>
        /// Siguiente código con prefijo (CAM000001, REG000001). Se calcula dentro
        /// de la transacción del alta; si dos personas crean a la vez, la llave
        /// primaria rechaza la segunda y Traducir devuelve un 409 para reintentar.
        /// </summary>
        private static async Task<string> SiguienteCodigoAsync(BonificadosSql sql, string tabla, string columnaCmpy,
            string columna, string prefijo, string cmpy)
        {
            var codigos = await sql.ConsultarAsync(
                $"SELECT {columna} AS codigo FROM {tabla} WHERE {columnaCmpy} = ? AND {columna} LIKE ?", cmpy, prefijo + "%");
            var maximo = codigos
                .Select(f => f.TextoO("codigo"))
                .Select(c => int.TryParse(c.Substring(prefijo.Length), NumberStyles.None, CultureInfo.InvariantCulture, out var n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max();
            return prefijo + (maximo + 1).ToString("D6", CultureInfo.InvariantCulture);
        }

        private Task AuditarAsync(BonificadosSql sql, string cmpy, string? regla, string? campana, string accion,
            object? anterior, object? nuevo, string? usuario) =>
            BonificadosAuditoria.RegistrarAsync(sql, _logger, new BonAuditoria
            {
                Cmpy = cmpy,
                Regla = regla,
                Campana = campana,
                Accion = accion,
                Anterior = anterior,
                Nuevo = nuevo,
                Usuario = usuario,
                // rba_codusu tiene 10 caracteres; el usuario del portal completo queda aquí.
                Observacion = string.IsNullOrWhiteSpace(usuario) ? null : $"Portal: {usuario.Trim()}",
            });

        /// <summary>Convierte errores de llave del motor en mensajes que el portal puede mostrar.</summary>
        private static Exception Traducir(Exception ex)
        {
            if (ex is BonificadosException) return ex;
            var mensaje = ex.Message ?? "";
            // Informix: -268/-239 llave duplicada, -691/-692 integridad referencial.
            // DB2: SQLSTATE 23505 duplicada, 23503/23504 referencial.
            if (mensaje.Contains("23505") || mensaje.Contains("-268") || mensaje.Contains("-239"))
                return new BonificadosException("Otro usuario guardó al mismo tiempo y el código quedó repetido. Intenta de nuevo.", 409);
            if (mensaje.Contains("23503") || mensaje.Contains("23504") || mensaje.Contains("-691") || mensaje.Contains("-692"))
                return new BonificadosException(
                    "La base rechazó el cambio porque otros registros dependen de estos datos (por ejemplo, pedidos que ya " +
                    "eligieron una opción del beneficio). Desactiva la regla y crea una nueva. Detalle: " + mensaje, 409);
            return ex;
        }

        private static string Compania(string? cmpy)
        {
            var valor = (cmpy ?? "").Trim();
            if (!PatronCmpy.IsMatch(valor)) throw new BonificadosException("Compañía inválida.");
            return valor;
        }

        private static string Codigo(string? codigo, string que)
        {
            var valor = (codigo ?? "").Trim();
            if (valor.Length == 0 || valor.Length > 15 || !PatronCodigo.IsMatch(valor))
                throw new BonificadosException($"Código de {que} inválido.");
            return valor;
        }

        private static string? Usuario(string? usuario) => Recortar(usuario, LargoUsuario);

        private static string? Recortar(string? texto, int largo)
        {
            if (string.IsNullOrWhiteSpace(texto)) return null;
            var limpio = texto.Trim();
            return limpio.Length <= largo ? limpio : limpio.Substring(0, largo);
        }

        private static string SN(bool valor) => valor ? "S" : "N";

        /// <summary>Las vistas comparan cmp_fecfin &gt;= ahora: a medianoche el último día no contaría.</summary>
        private static DateTime FinDelDia(DateTime fecha) => fecha.Date.AddDays(1).AddSeconds(-1);
    }
}
