using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Service.Services
{
    public interface IEmailHelper
    {
        Task EmailLogAsync(
            string cod_usu,
            string email_cliente,
            string email_vendedor,
            string num_recibo,
            string base64Pdf,
            string filename,
            Dictionary<string, string> configuracion,
            string? etiquetaDocumento = null);
    }

    public class EmailHelper : IEmailHelper
    {
        private readonly ILogger<EmailHelper> _logger;

        public EmailHelper(ILogger<EmailHelper> logger)
        {
            _logger = logger;
        }

        public async Task EmailLogAsync(
            string cod_usu,
            string email_cliente,
            string email_vendedor,
            string num_recibo,
            string base64Pdf,
            string filename,
            Dictionary<string, string> configuracion,
            string? etiquetaDocumento = null)
        {
            try
            {
                _logger.LogInformation("Iniciando EmailLogAsync para usuario: {CodUsu}", num_recibo);

                // Igual que en el envío por WhatsApp: las claves de cada compañía llegan
                // con espacios de relleno o distinta capitalización según cómo se cargaron.
                var cfg = ConfigHelper.Normalizar(configuracion);

                if (cfg.Count == 0)
                {
                    _logger.LogWarning(
                        "Correo no enviado: la solicitud no trae configuración (config) para el documento: {CodUsu}",
                        num_recibo);
                    return;
                }

                // Validate required configuration values
                string empresa = ConfigHelper.Valor(cfg, "EMPRESA");
                string dominioEmail = ConfigHelper.Valor(cfg, "DOMINIOEMAIL");
                string smsApi = ConfigHelper.Valor(cfg, "SMSAPI");
                string emailCartera = ConfigHelper.Valor(cfg, "EMAILCARTERA");
                // Copia interna de seguimiento. Estaba fija en el código con una cuenta de
                // RECAMIER; ahora cada compañía puede apuntar a su propio buzón con el
                // parámetro EMAILCOPIA, y mientras no lo tenga se conserva el de siempre.
                string emailCopia = ConfigHelper.Valor(cfg, "EMAILCOPIA");
                if (emailCopia.Length == 0) emailCopia = "evarecamier@gmail.com";

                if (string.IsNullOrWhiteSpace(empresa))
                {
                    _logger.LogError(
                        "Error: EMPRESA no está configurado. Parametros recibidos: {Claves}",
                        ConfigHelper.Claves(cfg));
                    return;
                }

                if (string.IsNullOrWhiteSpace(dominioEmail))
                {
                    _logger.LogError(
                        "Error: DOMINIOEMAIL no está configurado. Parametros recibidos: {Claves}",
                        ConfigHelper.Claves(cfg));
                    return;
                }

                if (string.IsNullOrWhiteSpace(smsApi))
                {
                    _logger.LogError(
                        "Error: SMSAPI no está configurado. Parametros recibidos: {Claves}",
                        ConfigHelper.Claves(cfg));
                    return;
                }

                // Etiqueta del documento: llega calculada desde el controlador. El
                // respaldo es el prefijo del nombre del archivo, que queda vacío cuando
                // el pago no trae csh_event_type.
                string etiqueta = !string.IsNullOrWhiteSpace(etiquetaDocumento)
                    ? etiquetaDocumento.Trim()
                    : EtiquetaDesdeNombreArchivo(filename);

                string fromEmail = $"{empresa} - {etiqueta} <eva@{dominioEmail}>";
                string subject = $"EVA - Usuario: {cod_usu} - {etiqueta} # : {num_recibo}";
                string htmlContent = "<h1>Resumen </h1><p>Informacion de pago adjunto</p>";

                // Recipients. Se descartan los vacíos y el "N/A" con el que se rellena
                // el correo cuando el cliente o el vendedor no lo tienen registrado.
                List<string> destinatarios = new List<string>
                {
                    email_cliente,
                    email_vendedor,
                    emailCopia,
                    emailCartera
                }
                .Where(EsCorreoUtilizable)
                .Select(correo => correo.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

                if (destinatarios.Count == 0)
                {
                    _logger.LogWarning(
                        "Correo no enviado: no hay destinatarios válidos para el documento {NumRecibo}",
                        num_recibo);
                    return;
                }

                // Decode base64 to bytes
                byte[] bytes = Convert.FromBase64String(base64Pdf);

                _logger.LogInformation("Enviando email con PDF para usuario: {CodUsu}", num_recibo);

                var client = new RestClient("https://m5gej.api.infobip.com/email/2/send");
                var request = new RestRequest("", Method.Post);

                // Headers
                request.AddHeader("Authorization", $"App {smsApi}");
                request.AddHeader("Accept", "application/json");

                request.AlwaysMultipartFormData = true;

                // Form fields
                request.AddParameter("from", fromEmail);
                request.AddParameter("subject", subject);
                request.AddParameter("html", htmlContent);
                request.AddParameter("text", "");

                // Add recipients
                foreach (var correo in destinatarios)
                {
                    request.AddParameter("to", correo);
                }


                request.AddFile("attachment", bytes, filename, "application/pdf");

                // Send request
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email enviado exitosamente: {StatusCode} - {Content}", response.StatusCode, response.Content);
                }
                else
                {
                    _logger.LogError("Error al enviar email: {StatusCode} - {Content}", response.StatusCode, response.Content);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en EmailLogAsync para usuario: {CodUsu}", num_recibo);
            }
        }

        /// <summary>
        /// Descarta los destinatarios vacíos y los "N/A" que ponen las consultas cuando
        /// el cliente o el vendedor no tienen correo registrado: enviarlos hace que el
        /// proveedor rechace la solicitud completa y nadie reciba el soporte.
        /// </summary>
        private static bool EsCorreoUtilizable(string? correo)
        {
            if (string.IsNullOrWhiteSpace(correo)) return false;

            string valor = correo.Trim();
            if (valor.Equals("N/A", StringComparison.OrdinalIgnoreCase)) return false;

            return valor.Contains('@');
        }

        /// <summary>Etiqueta del documento a partir del nombre del archivo (respaldo).</summary>
        private static string EtiquetaDesdeNombreArchivo(string? filename)
        {
            string etiqueta = (filename ?? string.Empty).Split('_')[0].Trim();
            return etiqueta.Length > 0 ? etiqueta : "Documento";
        }
    }
}
