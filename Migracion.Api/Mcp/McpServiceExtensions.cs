using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Migracion.Api.Mcp.Tools;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Protocol;

namespace Migracion.Api.Mcp
{
    /// <summary>
    /// Servidor MCP de EVA: expone consultas de solo lectura del API a modelos de IA
    /// por el protocolo Model Context Protocol (transporte Streamable HTTP en /mcp).
    /// Reutiliza los mismos servicios *Queries de los controladores.
    /// </summary>
    public static class McpServiceExtensions
    {
        public const string Ruta = "/mcp";
        public const string RutaExplorador = "/mcp-ui";
        private const string Politica = "McpAcceso";

        private const string Instrucciones =
            "Servidor de consulta de EVA, el sistema de fuerza de ventas (clientes, cartera, pedidos, productos, " +
            "inventario, visitas e indicadores). Todas las herramientas son de solo lectura.\n" +
            "Cómo empezar: casi todo se consulta por vendedor. Llama primero a eva_listar_vendedores para obtener " +
            "su código (rusu_codusu), región (rusu_reg) y territorio (rusu_teri), y úsalos en las demás herramientas. " +
            "Los clientes se identifican por código (rctt_cust) y sucursal (rctt_succli).\n" +
            "Fechas: los argumentos de fecha van en formato AAAA-MM-DD; las fechas dentro de los resultados vienen " +
            "como MM/dd/yyyy (mes primero).\n" +
            "Los campos de las respuestas conservan el nombre de la columna de la base; cada herramienta explica " +
            "los principales. Los campos vacíos se omiten.\n" +
            "Las listas van paginadas: si 'hay_mas' es true, repite la llamada con 'desplazamiento' igual a " +
            "'siguiente_desplazamiento'. Los valores de dinero no traen símbolo: pesos en RE, soles en DD, " +
            "dólares en LA y KR.";

        public static WebApplicationBuilder AddEvaMcp(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<McpSettings>(builder.Configuration.GetSection(McpSettings.Seccion));

            // Esquema propio para /mcp; el esquema por defecto (Basic) de los controladores no cambia.
            builder.Services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, McpApiKeyAuthenticationHandler>(
                    McpApiKeyAuthenticationHandler.Esquema, null);

            builder.Services.AddAuthorization(options =>
                options.AddPolicy(Politica, politica => politica
                    .AddAuthenticationSchemes(McpApiKeyAuthenticationHandler.Esquema)
                    .RequireAuthenticatedUser()));

            builder.Services
                .AddMcpServer(options =>
                {
                    options.ServerInfo = new Implementation
                    {
                        Name = "eva-mcp-server",
                        Title = "EVA - Fuerza de ventas",
                        Version = "1.0.0"
                    };
                    options.ServerInstructions = Instrucciones;
                })
                // Sin sesión: cada petición es independiente, así funciona igual con
                // varios procesos de IIS o tras un reinicio del sitio.
                .WithHttpTransport(options => options.SessionMode = HttpServerSessionMode.Stateless)
                .WithTools<CatalogoTools>()
                .WithTools<ClientesTools>()
                .WithTools<CarteraTools>()
                .WithTools<PedidosTools>()
                .WithTools<ProductosTools>()
                .WithTools<VisitasTools>()
                .WithTools<IndicadoresTools>();

            return builder;
        }

        public static WebApplication MapEvaMcp(this WebApplication app)
        {
            var settings = app.Services.GetRequiredService<IOptions<McpSettings>>().Value;

            if (!settings.Habilitado)
            {
                // Sin llave el endpoint no existe: nunca queda abierto por descuido.
                app.Logger.LogWarning(
                    "MCP deshabilitado: configure Mcp:ApiKey (mínimo {Minimo} caracteres) para publicar {Ruta}.",
                    McpSettings.LongitudMinimaLlave, Ruta);
                return app;
            }

            app.MapMcp(Ruta).RequireAuthorization(Politica);

            if (settings.Explorador)
            {
                // Anónimo como Swagger: la página no trae datos, pide la llave para consultar.
                app.MapGet(RutaExplorador, (HttpContext http) => McpUi.Responder(http))
                    .ExcludeFromDescription();
            }
            app.Logger.LogInformation("MCP habilitado en {Ruta} ({Compania}).", Ruta,
                string.IsNullOrWhiteSpace(settings.Compania) ? "sin compañía por defecto" : "compañía " + settings.Compania);

            return app;
        }
    }
}
