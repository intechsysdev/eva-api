using Microsoft.Extensions.Options;
using ModelContextProtocol;

namespace Migracion.Api.Mcp.Tools
{
    /// <summary>
    /// Base de las herramientas MCP: compañía por defecto, paginación y un único
    /// punto donde los errores de la base se registran completos en el log y al
    /// modelo le llega solo un mensaje útil, sin detalles internos.
    /// </summary>
    public abstract class EvaToolBase
    {
        protected const string DescripcionCmpy =
            "Código de compañía: RE (Recamier, Colombia), DD (Dermodis, Perú), LA (Lansey), KR (Keramer). " +
            "Si se omite se usa la compañía configurada en este servidor.";

        protected const string DescripcionLimite = "Máximo de filas a devolver (1 a 100). Por defecto 25.";

        protected const string DescripcionDesplazamiento =
            "Filas a saltar para paginar. Usa el 'siguiente_desplazamiento' de la respuesta anterior.";

        protected readonly McpSettings Settings;
        private readonly ILogger _logger;

        protected EvaToolBase(IOptions<McpSettings> settings, ILogger logger)
        {
            Settings = settings.Value;
            _logger = logger;
        }

        protected string Cmpy(string? cmpy) => McpEntrada.Compania(cmpy, Settings);

        protected (int limite, int desplazamiento) Pagina(int limite, int desplazamiento) =>
            McpEntrada.Pagina(limite, desplazamiento, Settings);

        protected async Task<T> Consultar<T>(string herramienta, Func<Task<T>> consulta)
        {
            try
            {
                return await consulta();
            }
            catch (McpException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MCP: la herramienta {Herramienta} falló", herramienta);
                throw new McpException(
                    "No se pudo consultar EVA en este momento (falla del servidor o de la base de datos). " +
                    "Reintenta en unos minutos; si el error continúa, pide al administrador revisar el log del API.");
            }
        }
    }
}
