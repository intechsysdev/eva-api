using System.Text.Json;

namespace Migracion.Api.Middleware
{
    /// <summary>
    /// Manejo global de excepciones: registra el error completo en el log
    /// (con método, ruta y traceId para poder rastrearlo desde el dispositivo)
    /// y devuelve una respuesta JSON uniforme.
    /// Los errores de conectividad/bloqueo de la base de datos se devuelven
    /// como 503 + Retry-After para que la app EVA sepa que debe esperar y
    /// reintentar más tarde, en lugar de un 500 genérico.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var traceId = context.TraceIdentifier;
                _logger.LogError(ex,
                    "Error no controlado {Method} {Path} desde {Ip} (traceId: {TraceId})",
                    context.Request.Method,
                    context.Request.Path,
                    context.Connection.RemoteIpAddress,
                    traceId);

                if (context.Response.HasStarted)
                {
                    throw;
                }

                var (statusCode, mensaje) = Clasificar(ex);

                context.Response.Clear();
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";
                if (statusCode == StatusCodes.Status503ServiceUnavailable)
                {
                    // Indica a la app cuánto esperar antes de reintentar
                    context.Response.Headers["Retry-After"] = "120";
                }

                var body = JsonSerializer.Serialize(new { error = mensaje, traceId });
                await context.Response.WriteAsync(body);
            }
        }

        private static (int statusCode, string mensaje) Clasificar(Exception ex)
        {
            for (Exception? e = ex; e != null; e = e.InnerException)
            {
                var msg = e.Message ?? string.Empty;
                bool esProblemaTransitorio =
                    e is System.Net.Sockets.SocketException ||
                    e is TimeoutException ||
                    // DB2/Informix: fallo de comunicación con el servidor de datos
                    msg.Contains("SQL30081", StringComparison.OrdinalIgnoreCase) ||
                    msg.Contains("record is locked", StringComparison.OrdinalIgnoreCase) ||
                    msg.Contains("lock timeout", StringComparison.OrdinalIgnoreCase) ||
                    (msg.Contains("connection", StringComparison.OrdinalIgnoreCase) &&
                     (msg.Contains("pool", StringComparison.OrdinalIgnoreCase) ||
                      msg.Contains("refused", StringComparison.OrdinalIgnoreCase) ||
                      msg.Contains("timeout", StringComparison.OrdinalIgnoreCase)));

                if (esProblemaTransitorio)
                {
                    return (StatusCodes.Status503ServiceUnavailable,
                        "Servicio temporalmente no disponible, reintente más tarde");
                }
            }

            return (StatusCodes.Status500InternalServerError, "Error interno del servidor");
        }
    }
}
