using System.Reflection;
using System.Security.Cryptography;

namespace Migracion.Api.Mcp
{
    /// <summary>
    /// Explorador web de las herramientas MCP (el equivalente a Swagger para /mcp):
    /// una sola página, incrustada en el ensamblado, que lista las herramientas y
    /// permite ejecutarlas. La página no contiene datos ni la llave: quien la abre
    /// debe escribir la llave, y todas las consultas pasan por /mcp con su misma
    /// autenticación.
    /// </summary>
    internal static class McpUi
    {
        private const string MarcaNonce = "__NONCE__";

        private static readonly Lazy<string> Plantilla = new(() =>
        {
            var ensamblado = Assembly.GetExecutingAssembly();
            string recurso = ensamblado.GetManifestResourceNames()
                .Single(nombre => nombre.EndsWith("McpUi.html", StringComparison.OrdinalIgnoreCase));
            using var lector = new StreamReader(ensamblado.GetManifestResourceStream(recurso)!);
            return lector.ReadToEnd();
        });

        public static IResult Responder(HttpContext http)
        {
            // Solo se ejecutan el script y el estilo de la página (nonce nuevo por petición)
            // y solo puede llamar a este mismo servidor.
            string nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            var cabeceras = http.Response.Headers;
            cabeceras.ContentSecurityPolicy =
                $"default-src 'none'; connect-src 'self'; script-src 'nonce-{nonce}'; style-src 'nonce-{nonce}'; " +
                "base-uri 'none'; form-action 'none'; frame-ancestors 'none'";
            cabeceras.XContentTypeOptions = "nosniff";
            cabeceras["Referrer-Policy"] = "no-referrer";
            cabeceras.CacheControl = "no-store";

            return Results.Content(Plantilla.Value.Replace(MarcaNonce, nonce), "text/html; charset=utf-8");
        }
    }
}
