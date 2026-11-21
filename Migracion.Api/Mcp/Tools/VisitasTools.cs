using Microsoft.Extensions.Options;
using Migracion.Service.Queries;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Migracion.Api.Mcp.Tools
{
    [McpServerToolType]
    public class VisitasTools : EvaToolBase
    {
        /// <summary>Tope del rango de fechas: las consultas de visitas recorren toda la tabla del periodo.</summary>
        private const int MaximoDias = 93;

        private const int MaximoVendedores = 50;

        private readonly IVisitasQueries _visitas;

        public VisitasTools(
            IVisitasQueries visitas,
            IOptions<McpSettings> settings, ILogger<VisitasTools> logger) : base(settings, logger)
        {
            _visitas = visitas;
        }

        [McpServerTool(Name = "eva_resumen_visitas", Title = "Resumen diario de visitas por vendedor",
            ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
        [Description(
            "Resumen de visitas: una fila por vendedor y por día dentro del rango. Cada fila trae: vendedor " +
            "(rvis_enter = código, rusu_nombre), perfil (rper_desper), región y territorio (reg_desc, ter_desc), " +
            "fecha (rvis_fecvis, MM-dd-yyyy), hora de la primera y la última visita (minrvis_horinivis, " +
            "maxrvis_horinivis), clientes distintos visitados (cantvisitas), movimientos registrados " +
            "(cantmovimientos) y tiempo entre la primera y la última visita (tiempoVis). Sirve para medir la " +
            "actividad del equipo; para el detalle cliente por cliente usa eva_listar_visitas. Rango máximo: 93 días.")]
        public async Task<string> ResumenVisitas(
            [Description("Fecha inicial del rango, formato AAAA-MM-DD.")] string fecha_inicio,
            [Description("Fecha final del rango (incluida), formato AAAA-MM-DD.")] string fecha_fin,
            [Description(DescripcionCmpy)] string? cmpy = null,
            [Description("Código del vendedor (rusu_codusu) para ver solo su resumen. Omite para todo el equipo.")] string? codigo_vendedor = null,
            [Description(DescripcionLimite)] int limite = 25,
            [Description(DescripcionDesplazamiento)] int desplazamiento = 0)
        {
            string compania = Cmpy(cmpy);
            var (inicio, fin) = McpEntrada.Rango(fecha_inicio, fecha_fin, MaximoDias);
            string? vendedor = McpEntrada.CodigoOpcional(codigo_vendedor, "codigo_vendedor");
            var (lim, desp) = Pagina(limite, desplazamiento);

            var resumen = await Consultar("eva_resumen_visitas", () => _visitas.GetListSummary(inicio, fin));

            var filas = (resumen ?? new())
                .Where(r => McpEntrada.Igual(r.rvis_cmpy, compania))
                .Where(r => vendedor == null || McpEntrada.Igual(r.rvis_enter, vendedor))
                .OrderBy(r => r.rusu_nombre).ThenBy(r => r.rvis_fecvis)
                .Select(r => new
                {
                    r.rvis_enter,
                    r.rusu_nombre,
                    r.rper_desper,
                    r.reg_desc,
                    r.ter_desc,
                    r.rvis_fecvis,
                    r.minrvis_horinivis,
                    r.maxrvis_horinivis,
                    r.cantvisitas,
                    r.cantmovimientos,
                    r.tiempoVis
                })
                .ToList();

            return McpSalida.Pagina(filas, lim, desp,
                extra: new
                {
                    visitas_total = filas.Sum(f => McpEntrada.Numero(f.cantvisitas))
                },
                siVacio: "No hay visitas en ese rango. Verifica las fechas y el código del vendedor.");
        }

        [McpServerTool(Name = "eva_listar_visitas", Title = "Listar visitas a clientes",
            ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
        [Description(
            "Visitas hechas por uno o varios vendedores en un rango de fechas: una fila por cliente visitado y " +
            "por día. Cada fila trae: fecha (rvis_fecvis, MM/dd/yyyy), hora de inicio y fin (rvis_horinivis, " +
            "rvis_horfinvis), duración (tiempoVis), vendedor (rvis_enter, rusu_nombre), cliente (rvis_cust), " +
            "sucursal (rctt_succli), razón social y nombre comercial (rctt_razsoc, rctt_nomcli), dirección " +
            "(rctt_addr), ciudad (dis_desc) y coordenadas donde se registró la visita (rvis_latitud, " +
            "rvis_longitud). Rango máximo: 93 días; máximo 50 vendedores por llamada.")]
        public async Task<string> ListarVisitas(
            [Description("Códigos de los vendedores (rusu_codusu) a consultar. Se obtienen con eva_listar_vendedores.")] string[] codigos_vendedores,
            [Description("Fecha inicial del rango, formato AAAA-MM-DD.")] string fecha_inicio,
            [Description("Fecha final del rango (incluida), formato AAAA-MM-DD.")] string fecha_fin,
            [Description(DescripcionCmpy)] string? cmpy = null,
            [Description("Código del cliente (rctt_cust) para ver solo las visitas a ese cliente.")] string? codigo_cliente = null,
            [Description(DescripcionLimite)] int limite = 25,
            [Description(DescripcionDesplazamiento)] int desplazamiento = 0)
        {
            string compania = Cmpy(cmpy);
            string[] vendedores = McpEntrada.Codigos(codigos_vendedores, "codigos_vendedores", MaximoVendedores);
            var (inicio, fin) = McpEntrada.Rango(fecha_inicio, fecha_fin, MaximoDias);
            string? cliente = McpEntrada.CodigoOpcional(codigo_cliente, "codigo_cliente");
            var (lim, desp) = Pagina(limite, desplazamiento);

            // codcust: "1" = transferencistas, "3" = asesores; cualquier otro valor cruza
            // contra mob_rcustmain, que es la tabla de clientes de las demás herramientas.
            var visitas = await Consultar("eva_listar_visitas",
                () => _visitas.GetListGroup(vendedores, "0", inicio, fin, cliente ?? string.Empty));

            var filas = (visitas ?? new())
                .Where(v => McpEntrada.Igual(v.rvis_cmpy, compania))
                .Select(v => new
                {
                    v.rvis_fecvis,
                    v.rvis_horinivis,
                    v.rvis_horfinvis,
                    v.tiempoVis,
                    v.rvis_enter,
                    v.rusu_nombre,
                    v.rvis_cust,
                    v.rctt_succli,
                    v.rctt_razsoc,
                    v.rctt_nomcli,
                    v.rctt_addr,
                    v.dis_desc,
                    v.rvis_latitud,
                    v.rvis_longitud
                })
                .ToList();

            return McpSalida.Pagina(filas, lim, desp,
                siVacio: "No hay visitas con ese filtro. Verifica las fechas y los códigos de vendedor.");
        }
    }
}
