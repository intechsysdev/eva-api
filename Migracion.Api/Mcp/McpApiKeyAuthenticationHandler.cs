using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace Migracion.Api.Mcp
{
    /// <summary>
    /// Autenticación del endpoint /mcp con una llave propia, separada del usuario
    /// Basic que usan las apps: así se puede dar o quitar acceso a un modelo sin
    /// tocar las credenciales de los dispositivos.
    /// Acepta "Authorization: Bearer {llave}" (lo que envían los clientes MCP) y
    /// "X-Api-Key: {llave}".
    /// </summary>
    public class McpApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string Esquema = "McpApiKey";

        private readonly McpSettings _settings;

        public McpApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IOptions<McpSettings> settings) : base(options, logger, encoder)
        {
            _settings = settings.Value;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!_settings.Habilitado)
                return Task.FromResult(AuthenticateResult.Fail("MCP sin llave configurada"));

            string? recibida = LeerLlave();
            if (string.IsNullOrEmpty(recibida))
                return Task.FromResult(AuthenticateResult.NoResult());

            if (!SonIguales(recibida, _settings.ApiKey.Trim()))
            {
                Logger.LogWarning("MCP: llave inválida desde {Ip}", Context.Connection.RemoteIpAddress);
                return Task.FromResult(AuthenticateResult.Fail("Llave MCP inválida"));
            }

            var identidad = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "mcp") }, Scheme.Name);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identidad), Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.Headers.WWWAuthenticate = "Bearer";
            return Task.CompletedTask;
        }

        private string? LeerLlave()
        {
            string? autorizacion = Request.Headers.Authorization;
            const string prefijo = "Bearer ";
            if (!string.IsNullOrEmpty(autorizacion) &&
                autorizacion.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase))
            {
                return autorizacion[prefijo.Length..].Trim();
            }

            string? cabecera = Request.Headers["X-Api-Key"];
            return string.IsNullOrWhiteSpace(cabecera) ? null : cabecera.Trim();
        }

        /// <summary>
        /// Comparación en tiempo constante. Se comparan los hash para que la
        /// longitud de la llave tampoco se pueda medir por el tiempo de respuesta.
        /// </summary>
        private static bool SonIguales(string a, string b) =>
            CryptographicOperations.FixedTimeEquals(
                SHA256.HashData(Encoding.UTF8.GetBytes(a)),
                SHA256.HashData(Encoding.UTF8.GetBytes(b)));
    }
}
