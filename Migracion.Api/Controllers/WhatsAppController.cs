using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Migracion.Api.Models;
using Migracion.Models;
using Migracion.Models.Command;
using Migracion.Service.Commands;
using Migracion.Service.Queries;
using Migracion.Service.Services;
using System.Text.Json.Nodes;

namespace Migracion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsAppController : ControllerBase
    {

        private readonly IMessageCommand _message;
        private readonly IOptionConsentQueries _optionConsent;
        private readonly IWhatsappHelper _whatsappHelper;
        private readonly ILogger<WhatsAppController> _logger;

        public WhatsAppController(IMessageCommand message, IOptionConsentQueries optionConsent, IWhatsappHelper whatsappHelper, ILogger<WhatsAppController> logger)
        {
            _message = message;
            _optionConsent = optionConsent;
            _whatsappHelper = whatsappHelper;
            _logger = logger;
        }

        [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromBody] GupshupCallback message)
        {
            if (message == null)
            {
                return BadRequest("El mensaje no es válido.");
            }


            await ProcesarMensajeAsync(message);

            return Ok();
        }
        private async Task ProcesarMensajeAsync(GupshupCallback message)
        {
            TResult result = _message.Insert(message);

            // Procesar respuesta de consentimiento de protección de datos (quick reply).
            await ProcesarConsentimiento(message);
        }

        /// <summary>
        /// Si el callback es una respuesta de botón al template de protección de datos
        /// ("Autorizo" / "No autorizo"), actualiza el estado del consentimiento del celular.
        /// </summary>
        private async Task ProcesarConsentimiento(GupshupCallback message)
        {
            try
            {

                var payload = message.payload;
                if (payload == null) return;

                var innerPayload = payload["payload"] as JsonObject;

                // El texto del botón puede llegar en distintos campos según el tipo de
                // respuesta que envíe Gupshup:
                //   - quick reply de un template  -> payload.payload.text
                //   - interactive button/list reply -> payload.payload.title
                //   - algunos formatos              -> payload.payload.reply
                //   - respuesta escrita como texto   -> payload.payload.text / payload.text
                // Se leen en orden hasta encontrar uno con valor.
                string? text = LeerTexto(innerPayload, "text")
                               ?? LeerTexto(innerPayload, "title")
                               ?? LeerTexto(innerPayload, "reply")
                               ?? LeerTexto(payload, "text");

                string? status = MapearStatus(text);

                // No es una respuesta de consentimiento reconocida. Se registra el
                // payload interno para poder ver en el log exactamente qué llegó
                // (campo y estructura) cuando un cliente responde y no se actualiza.
                if (status == null)
                {
                    _logger.LogInformation(
                        "Callback WhatsApp sin respuesta de consentimiento reconocida: type={Type} texto='{Texto}' payload={Payload}",
                        LeerTexto(payload, "type"), text, innerPayload?.ToJsonString());
                    return;
                }

                var sender = payload["sender"] as JsonObject;
                string? phone = sender?["phone"]?.GetValue<string>()
                                 ?? payload["source"]?.GetValue<string>();
                // dial_code es el número sin indicativo de país; country_code es el indicativo.
                string? dialCode = sender?["dial_code"]?.GetValue<string>();
                string? countryCode = sender?["country_code"]?.GetValue<string>();

                // Construir variantes del número para que el match sea preciso aunque
                // en la tabla el celular esté guardado sin el código de país.
                var celulares = new List<string>();
                if (!string.IsNullOrWhiteSpace(phone)) celulares.Add(phone);
                if (!string.IsNullOrWhiteSpace(dialCode)) celulares.Add(dialCode);

                // Si el celular no trae dial_code pero sí indicativo, derivar el número local.
                if (!string.IsNullOrWhiteSpace(phone) &&
                    !string.IsNullOrWhiteSpace(countryCode) &&
                    phone.StartsWith(countryCode))
                {
                    celulares.Add(phone.Substring(countryCode.Length));
                }

                var updateResult = _optionConsent.UpdateStatusByCelular(status, celulares.ToArray());
                if (updateResult.IsSuccess)
                {
                    _logger.LogInformation(
                        "Consentimiento WhatsApp actualizado: status={Status} celulares={Celulares} filas={Message}",
                        status, string.Join(",", celulares), updateResult.Message);
                }
                else
                {

                    _logger.LogWarning(
                        "Consentimiento WhatsApp NO actualizado: status={Status} celulares={Celulares} motivo={Message}",
                        status, string.Join(",", celulares), updateResult.Message);
                }

                // Si el cliente autoriza (A) y el estado quedó efectivamente registrado
                // (el UPDATE afectó al menos una fila), enviarle el mensaje de bienvenida.
                // No se confirma una autorización que no se pudo guardar: de lo contrario
                // el cliente recibiría "Gracias por tu autorización" y aun así seguiría
                // recibiendo el template de autorización (el estado seguiría distinto de 'A').
                if (status == "A" && updateResult.IsSuccess && !string.IsNullOrWhiteSpace(phone))
                {
                    await _whatsappHelper.EnviarBienvenidaAutorizacionAsync(phone, message.app);
                }
            }
            catch (Exception ex)
            {
                // No interrumpir el flujo del callback ante un payload inesperado.
                _logger.LogError(ex, "Error procesando consentimiento del callback de WhatsApp");
            }
        }

        /// <summary>
        /// Lee una propiedad de texto de un JsonObject sin lanzar excepción si la
        /// propiedad no existe o no es una cadena (algunos payloads traen el valor
        /// como número u otro tipo). Devuelve null si no hay valor utilizable.
        /// </summary>
        private static string? LeerTexto(JsonObject? obj, string prop)
        {
            if (obj == null) return null;
            if (!obj.TryGetPropertyValue(prop, out var node) || node == null) return null;
            try { return node.GetValue<string>(); }
            catch { return node.ToString(); }
        }

        /// <summary>
        /// Traduce el texto del botón de consentimiento al estado interno:
        /// "Autorizo"/"Acepto" -> "A", "No autorizo"/"No acepto" -> "R". Cualquier otro
        /// texto -> null. Los botones dependen de la plantilla de cada país: un texto
        /// no reconocido deja el opt-in sin actualizar y solo se ve en el log.
        /// </summary>
        private static string? MapearStatus(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            string t = text.Trim();
            if (string.Equals(t, "Autorizo", System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(t, "Acepto", System.StringComparison.OrdinalIgnoreCase))
                return "A";
            if (string.Equals(t, "No autorizo", System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(t, "No acepto", System.StringComparison.OrdinalIgnoreCase))
                return "R";
            return null;
        }
    }
}
