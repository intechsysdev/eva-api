using Microsoft.Extensions.Options;
using Migracion.Service.Queries;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Migracion.Api.Mcp.Tools
{
    [McpServerToolType]
    public class IndicadoresTools : EvaToolBase
    {
        private readonly IIndicadoresGestionQueries _indicadores;

        public IndicadoresTools(
            IIndicadoresGestionQueries indicadores,
            IOptions<McpSettings> settings, ILogger<IndicadoresTools> logger) : base(settings, logger)
        {
            _indicadores = indicadores;
        }

        [McpServerTool(Name = "eva_tablero_indicadores", Title = "Tablero de indicadores de gestión",
            ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
        [Description(
            "Tablero de indicadores de gestión de un vendedor (territorio y región) para un mes. Devuelve " +
            "'secciones' con seis bloques: resumen, ventas, cobros, efectividad, pedidos y clientes; cada uno " +
            "es un objeto columna -> valor tal como lo entrega su procedimiento sp_mob_ind_*, con los valores " +
            "sin formato (la moneda y los porcentajes no vienen formateados). Si una sección falla no llega " +
            "y su nombre aparece en 'secciones_con_error'; las demás siguen siendo válidas.")]
        public async Task<string> TableroIndicadores(
            [Description("Año del periodo, por ejemplo 2026.")] int anio,
            [Description("Mes del periodo, de 1 a 12.")] int mes,
            [Description("Código de territorio del vendedor (rusu_teri).")] string territorio,
            [Description("Código de región del vendedor (rusu_reg).")] string region,
            [Description(DescripcionCmpy)] string? cmpy = null)
        {
            string compania = Cmpy(cmpy);
            string teri = McpEntrada.Codigo(territorio, "territorio");
            string reg = McpEntrada.Codigo(region, "region");

            if (mes < 1 || mes > 12)
                throw new McpException("'mes' debe estar entre 1 y 12.");
            if (anio < 2000 || anio > 2100)
                throw new McpException("'anio' fuera de rango: usa un año de cuatro dígitos, por ejemplo 2026.");

            var tablero = await Consultar("eva_tablero_indicadores",
                () => _indicadores.GetTablero(compania, anio, mes, teri, reg));

            // El detalle del error de cada procedimiento (texto de la base) se queda en
            // el servidor: al modelo solo le sirve saber qué secciones no llegaron.
            return McpSalida.Json(new
            {
                tablero.cmpy,
                tablero.anio,
                mes = tablero.per,
                tablero.teri,
                tablero.reg,
                tablero.secciones,
                secciones_con_error = tablero.errores.Keys.ToArray()
            });
        }
    }
}
