using Migracion.Models.Bonificados;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Migracion.Service.Bonificados
{
    /// <summary>
    /// Catálogo único de validaciones. La app lo recibe en la descarga
    /// (BonSyncDto.Codigos) para que la alerta local y el rechazo del API
    /// digan lo mismo.
    /// </summary>
    public static class BonificadosCodigos
    {
        public static readonly IReadOnlyList<BonCodigoDto> Todos = new List<BonCodigoDto>
        {
            new() { Codigo = "V-01", Titulo = "Campaña o regla no vigente", Correccion = "Quita el bonificado: la campaña no está activa o el pedido está fuera de sus fechas." },
            new() { Codigo = "V-02", Titulo = "Cliente fuera del alcance", Correccion = "Esta oferta no aplica para el canal, la región, el territorio o el tipo de este cliente." },
            new() { Codigo = "V-03", Titulo = "Producto bonificado no permitido", Correccion = "Elige uno de los productos que otorga la regla." },
            new() { Codigo = "V-04", Titulo = "El pedido no tiene los productos de la regla", Correccion = "Agrega al pedido los productos que exige la regla (incluido el producto foco, si lo hay)." },
            new() { Codigo = "V-05", Titulo = "No cumple la condición de compra", Correccion = "Aumenta la cantidad o el monto comprado, o reduce la cantidad bonificada." },
            new() { Codigo = "V-06", Titulo = "Sin stock para bonificar", Correccion = "Reduce la cantidad bonificada: el cupo de la regla no alcanza." },
            new() { Codigo = "V-07", Titulo = "Precio por debajo del mínimo", Correccion = "El producto bonificado debe tener un precio igual o mayor al mínimo de la regla." },
            new() { Codigo = "V-08", Titulo = "Límite por cliente alcanzado", Correccion = "El cliente ya usó esta oferta las veces permitidas en el periodo." },
            new() { Codigo = "V-09", Titulo = "Falta escaneo QR del evento", Correccion = "Escanea el QR del evento con el cliente antes de bonificar." },
            new() { Codigo = "V-10", Titulo = "Solo aplica en el primer pedido", Correccion = "Esta oferta es para clientes nuevos sin pedidos anteriores." },
        };

        public static BonCodigoDto De(string codigo) => Todos.First(c => c.Codigo == codigo);
    }

    /// <summary>Pedido ya interpretado que recibe el motor.</summary>
    internal sealed class BonPedidoContexto
    {
        public string Cmpy { get; init; } = "";
        public string Reg { get; init; } = "";
        public string Teri { get; init; } = "";
        public decimal? Num { get; init; }
        public DateTime Fecha { get; set; }
        public string Cust { get; init; } = "";
        public string Ship { get; init; } = "";
        public string? Codusu { get; init; }
        /// <summary>Compras agrupadas por producto: cantidad total y precio unitario.</summary>
        public Dictionary<string, (decimal Cantidad, decimal? Precio)> Lineas { get; init; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>yyyyMM del pedido: periodo de consumo de las reglas por mes.</summary>
        public string Periodo => Fecha.ToString("yyyyMM", CultureInfo.InvariantCulture);

        public static string Periodo_(string frecuencia, DateTime fecha) =>
            frecuencia == "U" ? "000000" : fecha.ToString("yyyyMM", CultureInfo.InvariantCulture);
    }

    /// <summary>Opción elegida en un beneficio de opción múltiple (para mob_rbonopcsel).</summary>
    internal sealed record BonSeleccion(string Regla, int SeqBenef, int Orden);

    /// <summary>
    /// Motor de reglas de bonificados (V-01..V-10). Lee todo de la base en el
    /// momento: es la fuente de verdad final; el motor de la app es solo
    /// experiencia de usuario.
    /// </summary>
    internal sealed class BonificadosMotor
    {
        private readonly BonificadosSql _sql;

        public BonificadosMotor(BonificadosSql sql) => _sql = sql;

        /// <summary>Opciones múltiples resueltas en la última evaluación.</summary>
        public List<BonSeleccion> Selecciones { get; } = new();

        private sealed record Producto(string Id, string? Linea, string? Sublinea);

        public async Task<BonResultadoDto> EvaluarAsync(BonPedidoContexto p, List<BonAplicadoDto> bonificados)
        {
            Selecciones.Clear();
            var resultado = new BonResultadoDto();

            // La misma regla puede venir dos veces: se une.
            var aplicaciones = bonificados
                .Where(b => !string.IsNullOrWhiteSpace(b.Regla))
                .GroupBy(b => b.Regla.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => (Regla: g.Key, Items: g.SelectMany(b => b.Items ?? new List<BonItemDto>())
                    .Where(i => !string.IsNullOrWhiteSpace(i.Part) && i.Cantidad > 0)
                    .Select(i => { i.Part = i.Part.Trim(); return i; }).ToList()))
                .ToList();
            if (aplicaciones.Count == 0)
            {
                resultado.Valido = true;
                return resultado;
            }

            var codigos = aplicaciones.Select(a => a.Regla).ToList();
            var datos = await CargarAsync(p, codigos, aplicaciones.SelectMany(a => a.Items).Select(i => i.Part));

            void Violacion(string codigo, string regla, string? part, string mensaje)
            {
                resultado.Violaciones.Add(new BonViolacionDto
                {
                    Codigo = codigo,
                    Regla = regla,
                    Part = part,
                    Mensaje = mensaje,
                    Correccion = BonificadosCodigos.De(codigo).Correccion,
                });
            }

            foreach (var (codigo, items) in aplicaciones)
            {
                var antes = resultado.Violaciones.Count;
                var evaluada = new BonReglaEvaluadaDto { Regla = codigo };
                resultado.Reglas.Add(evaluada);

                if (!datos.Reglas.TryGetValue(codigo, out var regla))
                {
                    Violacion("V-01", codigo, null, $"La regla {codigo} no existe.");
                    continue;
                }
                evaluada.Campana = regla.TextoO("rbr_campana");
                var nombre = regla.Texto("rbr_descr") ?? codigo;

                // ---- V-01 vigencia ----
                var fecha = p.Fecha.Date;
                if (regla.Texto("cmp_estado") != "A")
                    Violacion("V-01", codigo, null, $"La campaña {evaluada.Campana} de \"{nombre}\" no está activa.");
                var cmpIni = regla.Fecha("cmp_fecini")?.Date;
                var cmpFin = regla.Fecha("cmp_fecfin")?.Date;
                if ((cmpIni != null && fecha < cmpIni) || (cmpFin != null && fecha > cmpFin))
                    Violacion("V-01", codigo, null, $"\"{nombre}\" rige del {cmpIni:dd/MM/yyyy} al {cmpFin:dd/MM/yyyy}.");
                if (regla.Texto("rbr_activo") == "N")
                    Violacion("V-01", codigo, null, $"La regla \"{nombre}\" está inactiva.");
                var rIni = regla.Fecha("rbr_fecini")?.Date;
                var rFin = regla.Fecha("rbr_fecfin")?.Date;
                if ((rIni != null && fecha < rIni) || (rFin != null && fecha > rFin))
                    Violacion("V-01", codigo, null, $"\"{nombre}\" solo rige del {rIni:dd/MM/yyyy} al {rFin:dd/MM/yyyy}.");

                // ---- V-02 alcance ----
                foreach (var grupo in datos.Alcance[codigo].GroupBy(a => a.TextoO("rbe_tipoalcance").ToUpperInvariant()))
                {
                    var valores = grupo.Select(a => a.TextoO("rbe_valor")).ToHashSet(StringComparer.OrdinalIgnoreCase);
                    var (valor, que) = grupo.Key switch
                    {
                        "CANAL" => (datos.Canal, "el canal"),
                        "REGION" => (p.Reg, "la región"),
                        "TERRITORIO" => (p.Teri, "el territorio"),
                        "CLIENTE" => (p.Cust, "el cliente"),
                        // Tipos que aún no evalúa el motor: no restringen.
                        _ => ((string?)null, ""),
                    };
                    if (que.Length > 0 && (valor == null || !valores.Contains(valor)))
                        Violacion("V-02", codigo, null, $"\"{nombre}\" no aplica para {que} {valor ?? "(sin dato)"}.");
                }
                var categoria = regla.Texto("rbr_categcli");
                if (categoria != null && !string.Equals(categoria, datos.Canal, StringComparison.OrdinalIgnoreCase))
                    Violacion("V-02", codigo, null, $"\"{nombre}\" es solo para clientes de categoría {categoria}.");
                var diasMin = regla.Entero("rbr_diasmin");
                var diasMax = regla.Entero("rbr_diasmax");
                if (diasMin != null || diasMax != null)
                {
                    var ultima = await UltimaCompraAsync(p);
                    var dias = ultima == null ? int.MaxValue : (int)(fecha - ultima.Value.Date).TotalDays;
                    if ((diasMin != null && dias < diasMin) || (diasMax != null && dias > diasMax))
                        Violacion("V-02", codigo, null,
                            $"\"{nombre}\" es para clientes sin compra entre {diasMin ?? 0} y {(diasMax?.ToString() ?? "∞")} días " +
                            $"({(ultima == null ? "este cliente no tiene compras" : $"última compra hace {dias} días")}).");
                }

                // ---- V-04 productos que califican ----
                var condiciones = datos.Condiciones[codigo].ToList();
                var agrupacion = condiciones.Where(c => !c.SiNo("rbc_foco")).ToList();
                var califican = p.Lineas
                    .Where(l => l.Value.Cantidad > 0 && agrupacion.Any(c => Cumple(datos, l.Key, c)))
                    .ToDictionary(l => l.Key, l => l.Value, StringComparer.OrdinalIgnoreCase);
                var unidades = califican.Sum(l => l.Value.Cantidad);
                var monto = califican.Sum(l => l.Value.Cantidad * (l.Value.Precio ?? 0));
                var sinPrecio = califican.Any(l => l.Value.Precio == null);
                evaluada.Unidades = unidades;
                evaluada.Monto = monto;

                if (unidades <= 0)
                    Violacion("V-04", codigo, null, $"El pedido no tiene productos de \"{nombre}\".");
                foreach (var foco in condiciones.Where(c => c.SiNo("rbc_foco")))
                {
                    if (!p.Lineas.Any(l => l.Value.Cantidad > 0 && Cumple(datos, l.Key, foco)))
                        Violacion("V-04", codigo, null, $"Falta el producto foco {DescribirCondicion(foco)} de \"{nombre}\".");
                }

                // ---- V-03 / V-05 condición y beneficios ----
                foreach (var item in items.Where(i => !datos.Productos.ContainsKey(i.Part)))
                    Violacion("V-03", codigo, item.Part, $"El producto {item.Part} no existe.");

                var niveles = datos.Niveles[codigo].OrderBy(n => n.Decimal("rbn_desde")).ToList();
                var primera = agrupacion.FirstOrDefault();
                if (niveles.Count > 0)
                {
                    var porMonto = primera?.Decimal("rbc_valmin") != null && primera.Decimal("rbc_cantmin") == null;
                    var medida = porMonto ? monto : unidades;
                    var nivel = niveles.LastOrDefault(n => medida >= (n.Decimal("rbn_desde") ?? 0));
                    if (porMonto && sinPrecio)
                        Violacion("V-05", codigo, null, $"Faltan precios de líneas del pedido para calcular el monto de \"{nombre}\".");
                    if (nivel == null)
                    {
                        Violacion("V-05", codigo, null,
                            $"Con {Cifra(medida, porMonto)} no se alcanza el nivel 1 de \"{nombre}\" (desde {Cifra(niveles[0].Decimal("rbn_desde") ?? 0, porMonto)}).");
                    }
                    else
                    {
                        var numero = nivel.Entero("rbn_nivel") ?? 0;
                        evaluada.Nivel = numero;
                        var permitidos = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
                        var bolsas = new List<(string Grupo, decimal Cantidad)>();
                        foreach (var b in datos.BeneficiosNivel[codigo].Where(b => b.Entero("rnb_nivel") == numero))
                        {
                            var cantidad = string.Equals(b.Texto("rnb_formula"), "IGUAL_TOTAL", StringComparison.OrdinalIgnoreCase)
                                ? unidades : b.Decimal("rnb_cant") ?? 0;
                            var id = b.Texto("rnb_id");
                            if (id != null) permitidos[id] = permitidos.GetValueOrDefault(id) + cantidad;
                            else if (b.Texto("rnb_grupo") is { } grupoBenef) bolsas.Add((grupoBenef, cantidad));
                        }

                        var usado = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
                        var usadoBolsa = new decimal[bolsas.Count];
                        foreach (var item in items)
                        {
                            if (permitidos.ContainsKey(item.Part))
                            {
                                usado[item.Part] = usado.GetValueOrDefault(item.Part) + item.Cantidad;
                                continue;
                            }
                            var bolsa = bolsas.FindIndex(b => EnGrupo(datos, item.Part, b.Grupo));
                            if (bolsa >= 0) usadoBolsa[bolsa] += item.Cantidad;
                            else Violacion("V-03", codigo, item.Part, $"{Nombre(datos, item.Part)} no es un bonificado del nivel {numero} de \"{nombre}\".");
                        }
                        foreach (var (part, cantidad) in usado.Where(u => u.Value > permitidos[u.Key]))
                            Violacion("V-05", codigo, part,
                                $"Se bonifican {Cifra(cantidad)} de {Nombre(datos, part)} y el nivel {numero} permite {Cifra(permitidos[part])}.");
                        for (var i = 0; i < bolsas.Count; i++)
                        {
                            if (usadoBolsa[i] > bolsas[i].Cantidad)
                                Violacion("V-05", codigo, null,
                                    $"Se bonifican {Cifra(usadoBolsa[i])} del grupo {bolsas[i].Grupo} y el nivel {numero} permite {Cifra(bolsas[i].Cantidad)}.");
                        }
                    }
                }
                else
                {
                    var cantidadMinima = primera?.Decimal("rbc_cantmin");
                    var montoMinimo = primera?.Decimal("rbc_valmin");
                    var multiplo = primera?.Decimal("rbc_multiplo");
                    if (cantidadMinima != null && unidades < cantidadMinima)
                        Violacion("V-05", codigo, null, $"\"{nombre}\" pide {Cifra(cantidadMinima.Value)} unidades y el pedido tiene {Cifra(unidades)}.");
                    if (montoMinimo != null && sinPrecio)
                        Violacion("V-05", codigo, null, $"Faltan precios de líneas del pedido para calcular el monto de \"{nombre}\".");
                    else if (montoMinimo != null && monto < montoMinimo)
                        Violacion("V-05", codigo, null, $"\"{nombre}\" pide una compra de {Cifra(montoMinimo.Value, true)} y el pedido suma {Cifra(monto, true)}.");

                    var veces = 1;
                    if (cantidadMinima != null && multiplo is > 0 && unidades >= cantidadMinima)
                        veces = 1 + (int)Math.Floor((unidades - cantidadMinima.Value) / multiplo.Value);
                    evaluada.Veces = veces;

                    var beneficios = datos.Beneficios[codigo].OrderBy(b => b.Entero("rbb_seq")).ToList();
                    var usadoPorBeneficio = new Dictionary<int, decimal>();
                    var opcionesElegidas = new Dictionary<int, HashSet<string>>();
                    foreach (var item in items)
                    {
                        var beneficio = item.SeqBenef != null
                            ? beneficios.FirstOrDefault(b => b.Entero("rbb_seq") == item.SeqBenef && Acepta(datos, codigo, b, item, califican))
                            : beneficios.FirstOrDefault(b => Acepta(datos, codigo, b, item, califican));
                        if (beneficio == null)
                        {
                            Violacion("V-03", codigo, item.Part, $"{Nombre(datos, item.Part)} no es un bonificado de \"{nombre}\".");
                            continue;
                        }
                        var seq = beneficio.Entero("rbb_seq") ?? 0;
                        usadoPorBeneficio[seq] = usadoPorBeneficio.GetValueOrDefault(seq) + item.Cantidad;
                        if (TipoBeneficio(beneficio) == "MULTIPLE_OPTION")
                        {
                            if (!opcionesElegidas.TryGetValue(seq, out var elegidas)) opcionesElegidas[seq] = elegidas = new(StringComparer.OrdinalIgnoreCase);
                            elegidas.Add(item.Part);
                        }
                    }

                    foreach (var (seq, cantidad) in usadoPorBeneficio)
                    {
                        var beneficio = beneficios.First(b => b.Entero("rbb_seq") == seq);
                        decimal porVez = beneficio.Decimal("rbb_cant") ?? 1;
                        if (opcionesElegidas.TryGetValue(seq, out var elegidas))
                        {
                            if (elegidas.Count > 1)
                            {
                                Violacion("V-03", codigo, null, $"En \"{nombre}\" se elige una sola opción y llegaron {elegidas.Count}.");
                                continue;
                            }
                            var opcion = datos.Opciones[codigo].First(o => o.Entero("rbo_seqbenef") == seq &&
                                string.Equals(o.Texto("rbo_id"), elegidas.First(), StringComparison.OrdinalIgnoreCase));
                            porVez = opcion.Decimal("rbo_cant") ?? porVez;
                            Selecciones.Add(new BonSeleccion(codigo, seq, opcion.Entero("rbo_orden") ?? 0));
                        }
                        var maximo = porVez * veces;
                        if (cantidad > maximo)
                            Violacion("V-05", codigo, null,
                                $"Se bonifican {Cifra(cantidad)} unidades del beneficio {seq} de \"{nombre}\" y se permiten {Cifra(maximo)}.");
                    }
                }

                // ---- V-06 stock ----
                if (regla.SiNo("rbr_stocklimit"))
                {
                    var stock = datos.Stock[codigo].ToDictionary(s => s.TextoO("rbs_id"), StringComparer.OrdinalIgnoreCase);
                    foreach (var porProducto in items.GroupBy(i => i.Part, StringComparer.OrdinalIgnoreCase))
                    {
                        var cantidad = porProducto.Sum(i => i.Cantidad);
                        if (!stock.TryGetValue(porProducto.Key, out var fila))
                        {
                            Violacion("V-06", codigo, porProducto.Key, $"{Nombre(datos, porProducto.Key)} no tiene cupo asignado en \"{nombre}\".");
                            continue;
                        }
                        var disponible = (fila.Decimal("rbs_asignado") ?? 0) - (fila.Decimal("rbs_consumido") ?? 0);
                        if (cantidad > disponible)
                            Violacion("V-06", codigo, porProducto.Key,
                                $"Quedan {Cifra(Math.Max(0, disponible))} unidades de {Nombre(datos, porProducto.Key)} y se piden {Cifra(cantidad)}.");
                    }
                }

                // ---- V-07 precio mínimo del bonificado ----
                if (regla.Decimal("rbr_preciominbon") is { } precioMinimo)
                {
                    foreach (var item in items)
                    {
                        if (item.PrecioLista == null)
                            Violacion("V-07", codigo, item.Part, $"No llegó el precio de {Nombre(datos, item.Part)} para comprobar el mínimo de {Cifra(precioMinimo, true)}.");
                        else if (item.PrecioLista < precioMinimo)
                            Violacion("V-07", codigo, item.Part,
                                $"{Nombre(datos, item.Part)} cuesta {Cifra(item.PrecioLista.Value, true)} y el mínimo es {Cifra(precioMinimo, true)}.");
                    }
                }

                // ---- V-08 límite por cliente ----
                var frecuencia = regla.Texto("rbr_frecuencia") ?? "N";
                if (frecuencia is "M" or "U")
                {
                    var periodo = BonPedidoContexto.Periodo_(frecuencia, p.Fecha);
                    var usadas = datos.Consumos.TryGetValue((codigo, periodo), out var v) ? v : 0;
                    var maximo = regla.Entero("rbr_maxclimes") ?? 1;
                    if (usadas + 1 > maximo)
                        Violacion("V-08", codigo, null,
                            $"El cliente ya usó \"{nombre}\" {usadas} {(usadas == 1 ? "vez" : "veces")}" +
                            $"{(frecuencia == "M" ? " este mes" : "")} y el máximo es {maximo}.");
                }

                // ---- V-09 QR de evento ----
                if (regla.SiNo("rbr_reqqr"))
                {
                    var escaneos = await _sql.ContarAsync(
                        "SELECT COUNT(*) FROM mob_rboneventoscan s, mob_rbonevento e " +
                        "WHERE s.evs_cmpy = e.evt_cmpy AND s.evs_evento = e.evt_evento " +
                        "AND e.evt_cmpy = ? AND e.evt_regla = ? AND s.evs_cust = ? AND s.evs_resultado = 'V'",
                        p.Cmpy, codigo, p.Cust);
                    if (escaneos == 0)
                        Violacion("V-09", codigo, null, $"\"{nombre}\" exige el QR del evento y este cliente no tiene un escaneo válido.");
                }

                // ---- V-10 primer pedido ----
                if (regla.SiNo("rbr_soloprimped") && await TienePedidosPreviosAsync(p))
                    Violacion("V-10", codigo, null, $"\"{nombre}\" es solo para el primer pedido y el cliente ya tiene pedidos.");

                evaluada.Aceptada = resultado.Violaciones.Count == antes;
            }

            // ---- reglas no acumulables ----
            if (aplicaciones.Count > 1)
            {
                foreach (var (codigo, _) in aplicaciones)
                {
                    if (datos.Reglas.TryGetValue(codigo, out var regla) && !regla.SiNo("rbr_acumulable"))
                    {
                        Violacion("V-05", codigo, null, $"\"{regla.Texto("rbr_descr") ?? codigo}\" no es acumulable con otras reglas en el mismo pedido.");
                        resultado.Reglas.First(r => r.Regla == codigo).Aceptada = false;
                    }
                }
            }

            resultado.Valido = resultado.Violaciones.Count == 0;
            return resultado;
        }

        // ---------------------------------------------------------------------
        // Reglas de pertenencia
        // ---------------------------------------------------------------------

        private static string TipoBeneficio(Fila b) =>
            b.SiNo("rbb_multiopc") ? "MULTIPLE_OPTION" : (b.Texto("rbb_tipobenef") ?? "SPECIFIC_PRODUCT").ToUpperInvariant();

        private static bool Acepta(Datos d, string regla, Fila beneficio, BonItemDto item,
            Dictionary<string, (decimal Cantidad, decimal? Precio)> califican)
        {
            switch (TipoBeneficio(beneficio))
            {
                case "SAME_PRODUCT":
                    return califican.ContainsKey(item.Part);
                case "LOWEST_VALUE":
                    var grupo = beneficio.Texto("rbb_grupo");
                    if (grupo == null || !EnGrupo(d, item.Part, grupo)) return false;
                    // Si se compró y hay precios, debe ser el más barato de lo comprado en el grupo.
                    var comprados = califican.Where(c => EnGrupo(d, c.Key, grupo) && c.Value.Precio != null).ToList();
                    if (!comprados.Any() || !califican.TryGetValue(item.Part, out var propio) || propio.Precio == null) return true;
                    return propio.Precio <= comprados.Min(c => c.Value.Precio);
                case "MULTIPLE_OPTION":
                    var seq = beneficio.Entero("rbb_seq");
                    return d.Opciones[regla].Any(o => o.Entero("rbo_seqbenef") == seq &&
                        string.Equals(o.Texto("rbo_id"), item.Part, StringComparison.OrdinalIgnoreCase) &&
                        (item.Orden == null || o.Entero("rbo_orden") == item.Orden));
                default:
                    return string.Equals(beneficio.Texto("rbb_id"), item.Part, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>¿El producto comprado cumple esta fila de condición?</summary>
        private static bool Cumple(Datos d, string part, Fila c)
        {
            if (c.Texto("rbc_grupo") is { } grupo) return EnGrupo(d, part, grupo);
            if (!d.Productos.TryGetValue(part, out var producto)) return false;
            if (c.Texto("rbc_id") is { } id) return Igual(id, producto.Id);
            if (c.Texto("rbc_subl") is { } sublinea)
                return Igual(sublinea, producto.Sublinea) && (c.Texto("rbc_linea") is not { } l || Igual(l, producto.Linea));
            if (c.Texto("rbc_linea") is { } linea) return Igual(linea, producto.Linea);
            return false;
        }

        private static bool EnGrupo(Datos d, string part, string grupo)
        {
            if (!d.Productos.TryGetValue(part, out var producto)) return false;
            return d.Grupos[grupo].Any(e =>
            {
                var valor = e.TextoO("grd_valor");
                switch (e.TextoO("grd_nivel").ToUpperInvariant())
                {
                    case "REFERENCIA": return Igual(valor, producto.Id);
                    case "LINEA": return Igual(valor, producto.Linea);
                    case "SUBLINEA":
                        var partes = valor.Split('|');
                        return partes.Length == 2
                            ? Igual(partes[0], producto.Linea) && Igual(partes[1], producto.Sublinea)
                            : Igual(valor, producto.Sublinea);
                    default: return false; // CATEGORIA aún no se evalúa.
                }
            });
        }

        private static bool Igual(string? a, string? b) =>
            a != null && b != null && string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase);

        private static string DescribirCondicion(Fila c) =>
            c.Texto("rbc_id") ?? c.Texto("rbc_grupo") ?? c.Texto("rbc_subl") ?? c.Texto("rbc_linea") ?? "?";

        private static string Nombre(Datos d, string part) =>
            d.Nombres.TryGetValue(part, out var n) ? $"{n} ({part})" : part;

        private static string Cifra(decimal valor, bool dinero = false) =>
            (dinero ? "$" : "") + valor.ToString(valor == Math.Floor(valor) ? "#,0" : "#,0.##", CultureInfo.GetCultureInfo("es-CO"));

        // ---------------------------------------------------------------------
        // Datos
        // ---------------------------------------------------------------------

        private sealed class Datos
        {
            public Dictionary<string, Fila> Reglas { get; } = new(StringComparer.OrdinalIgnoreCase);
            public ILookup<string, Fila> Alcance { get; set; } = Vacio();
            public ILookup<string, Fila> Condiciones { get; set; } = Vacio();
            public ILookup<string, Fila> Niveles { get; set; } = Vacio();
            public ILookup<string, Fila> BeneficiosNivel { get; set; } = Vacio();
            public ILookup<string, Fila> Beneficios { get; set; } = Vacio();
            public ILookup<string, Fila> Opciones { get; set; } = Vacio();
            public ILookup<string, Fila> Stock { get; set; } = Vacio();
            public ILookup<string, Fila> Grupos { get; set; } = Vacio();
            public Dictionary<string, Producto> Productos { get; } = new(StringComparer.OrdinalIgnoreCase);
            public Dictionary<string, string> Nombres { get; } = new(StringComparer.OrdinalIgnoreCase);
            public Dictionary<(string, string), int> Consumos { get; } = new();
            public string? Canal { get; set; }

            private static ILookup<string, Fila> Vacio() => Array.Empty<Fila>().ToLookup(f => "", StringComparer.OrdinalIgnoreCase);
        }

        private async Task<Datos> CargarAsync(BonPedidoContexto p, List<string> reglas, IEnumerable<string> bonificados)
        {
            var d = new Datos();
            var marcadores = BonificadosSql.Marcadores(reglas.Count);
            object?[] Con(params object?[] inicio) => inicio.Concat(reglas).ToArray();

            foreach (var r in await _sql.ConsultarAsync(
                         "SELECT r.*, c.cmp_estado, c.cmp_fecini, c.cmp_fecfin FROM mob_rbonreglas r, mob_rboncampana c " +
                         "WHERE r.rbr_cmpy = c.cmp_cmpy AND r.rbr_campana = c.cmp_campana AND r.rbr_cmpy = ? " +
                         $"AND r.rbr_regla IN ({marcadores})", Con(p.Cmpy)))
                d.Reglas[r.TextoO("rbr_regla")] = r;

            async Task<ILookup<string, Fila>> Hijas(string tabla, string cmpy, string regla) =>
                (await _sql.ConsultarAsync($"SELECT * FROM {tabla} WHERE {cmpy} = ? AND {regla} IN ({marcadores})", Con(p.Cmpy)))
                .ToLookup(f => f.TextoO(regla), StringComparer.OrdinalIgnoreCase);

            d.Alcance = await Hijas("mob_rbonalcance", "rbe_cmpy", "rbe_regla");
            d.Condiciones = await Hijas("mob_rbonregcond", "rbc_cmpy", "rbc_regla");
            d.Niveles = await Hijas("mob_rbonnivel", "rbn_cmpy", "rbn_regla");
            d.BeneficiosNivel = await Hijas("mob_rbonnivelbenef", "rnb_cmpy", "rnb_regla");
            d.Beneficios = await Hijas("mob_rbonregbenef", "rbb_cmpy", "rbb_regla");
            d.Opciones = await Hijas("mob_rbonbenefopc", "rbo_cmpy", "rbo_regla");
            d.Stock = await Hijas("mob_rbonstock", "rbs_cmpy", "rbs_regla");

            var grupos = d.Condiciones.SelectMany(g => g).Select(c => c.Texto("rbc_grupo"))
                .Concat(d.Beneficios.SelectMany(g => g).Select(b => b.Texto("rbb_grupo")))
                .Concat(d.BeneficiosNivel.SelectMany(g => g).Select(b => b.Texto("rnb_grupo")))
                .OfType<string>().Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (grupos.Count > 0)
            {
                var parametros = new object?[] { p.Cmpy }.Concat(grupos).ToArray();
                d.Grupos = (await _sql.ConsultarAsync(
                        $"SELECT grd_grupo, grd_nivel, grd_valor FROM mob_rbongrupodet WHERE grd_cmpy = ? AND grd_grupo IN ({BonificadosSql.Marcadores(grupos.Count)})",
                        parametros))
                    .ToLookup(f => f.TextoO("grd_grupo"), StringComparer.OrdinalIgnoreCase);
            }

            // Productos del pedido y de los bonificados: línea y sublínea para la pertenencia.
            var partes = p.Lineas.Keys.Concat(bonificados).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            foreach (var tanda in partes.Chunk(100))
            {
                var parametros = new object?[] { p.Cmpy }.Concat(tanda).ToArray();
                foreach (var f in await _sql.ConsultarAsync(
                             $"SELECT rpr_id, rpr_desc, rpr_linea, rpr_subl FROM mob_rproductos WHERE rpr_cmpy = ? AND rpr_id IN ({BonificadosSql.Marcadores(tanda.Length)})",
                             parametros))
                {
                    var id = f.TextoO("rpr_id");
                    d.Productos[id] = new Producto(id, f.Texto("rpr_linea"), f.Texto("rpr_subl"));
                    d.Nombres[id] = f.Texto("rpr_desc") ?? id;
                }
            }

            // Canal (tipo) del cliente; si es nuevo puede estar todavía en mob_rcustship.
            d.Canal = (await _sql.ConsultarAsync("SELECT rctt_type FROM mob_rcustmain WHERE rctt_cmpy = ? AND rctt_cust = ?", p.Cmpy, p.Cust))
                .Select(f => f.Texto("rctt_type")).FirstOrDefault(t => t != null);
            if (d.Canal == null)
            {
                try
                {
                    d.Canal = (await _sql.ConsultarAsync("SELECT rctt_type FROM mob_rcustship WHERE rctt_cmpy = ? AND rctt_cust = ?", p.Cmpy, p.Cust))
                        .Select(f => f.Texto("rctt_type")).FirstOrDefault(t => t != null);
                }
                catch
                {
                    // mob_rcustship puede no tener la columna en todas las compañías.
                }
            }

            foreach (var f in await _sql.ConsultarAsync(
                         "SELECT cns_regla, cns_periodo, cns_veces FROM mob_rbonconsumo WHERE cns_cmpy = ? AND cns_cust = ? " +
                         $"AND cns_periodo IN (?, '000000') AND cns_regla IN ({marcadores})",
                         Con(p.Cmpy, p.Cust, p.Periodo)))
            {
                var clave = (f.TextoO("cns_regla"), f.TextoO("cns_periodo"));
                d.Consumos[clave] = d.Consumos.GetValueOrDefault(clave) + (f.Entero("cns_veces") ?? 0);
            }

            return d;
        }

        private async Task<DateTime?> UltimaCompraAsync(BonPedidoContexto p)
        {
            var filas = await _sql.ConsultarAsync("SELECT MAX(orh_entdt) AS ultima FROM mob_rordhead_his WHERE orh_cmpy = ? AND orh_cust = ?", p.Cmpy, p.Cust);
            return filas.FirstOrDefault()?.Fecha("ultima");
        }

        private async Task<bool> TienePedidosPreviosAsync(BonPedidoContexto p)
        {
            if (await _sql.ContarAsync("SELECT COUNT(*) FROM mob_rordhead_his WHERE orh_cmpy = ? AND orh_cust = ?", p.Cmpy, p.Cust) > 0)
                return true;
            // El propio pedido ya está en mob_rordhead cuando se revalida al recibirlo.
            return p.Num == null
                ? await _sql.ContarAsync("SELECT COUNT(*) FROM mob_rordhead WHERE orh_cmpy = ? AND orh_cust = ?", p.Cmpy, p.Cust) > 0
                : await _sql.ContarAsync(
                    "SELECT COUNT(*) FROM mob_rordhead WHERE orh_cmpy = ? AND orh_cust = ? AND NOT (orh_num = ? AND orh_reg = ? AND orh_teri = ?)",
                    p.Cmpy, p.Cust, p.Num, p.Reg, p.Teri) > 0;
        }
    }
}
