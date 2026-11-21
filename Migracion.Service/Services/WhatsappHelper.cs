using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Migracion.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static QuestPDF.Helpers.Colors;

namespace Migracion.Service.Services
{
    public interface IWhatsappHelper
    {
        Task WamediaIdAsync(
       string cmpy,
       string towa,
       string datafileBase64, 
       string namefile,
       string numRecibo,
       string nomCliente,
       string vendedorNombre,
       string total,
       string fecha,
       Dictionary<string, string> configuracion,
       string? etiquetaDocumento = null);

        /// <summary>
        /// Envía el mensaje de bienvenida cuando el cliente autoriza (status "A") el
        /// tratamiento de datos. El destino es el número que está autorizando y
        /// <paramref name="app"/> es la app de la pasarela que reportó el callback,
        /// con la que se resuelve a qué compañía pertenece el mensaje.
        /// </summary>
        Task EnviarBienvenidaAutorizacionAsync(string destination, string? app = null);
    }

    public class WhatsappHelper : IWhatsappHelper
    {
        private readonly ILogger<WhatsappHelper> _logger;
        private readonly IConfiguration _configuration;

        /// Indicativos de país por compañía (CO=57, PE=51, EC=593, KR=1). Es el
        /// respaldo: si la configuración de la compañía trae INDICATIVOPAIS, manda esa.
        private static readonly Dictionary<string, string> IndicativosPorCompania =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "RE", "57" },
                { "DD", "51" },
                { "LA", "593" },
                { "KR", "1" }
            };

        public WhatsappHelper(ILogger<WhatsappHelper> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task WamediaIdAsync(
            string cmpy,
       string towa,
       string datafileBase64,
       string namefile,
       string numRecibo,
       string nomCliente,
       string vendedorNombre,
       string total,
       string fecha,
       Dictionary<string, string> configuracion,
       string? etiquetaDocumento = null
        )
        {
            try
            {
                _logger.LogInformation("Iniciando WamediaIdAsync para cmpy: {Cmpy}, towa: {Towa}", cmpy, towa);

                // Se normaliza una sola vez: las claves de cada compañía llegan con
                // espacios de relleno o distinta capitalización según cómo se cargaron.
                var cfg = ConfigHelper.Normalizar(configuracion);

                if (cfg.Count == 0)
                {
                    // Warning y no Information: el log a archivo se escribe desde
                    // Warning, y sin esta línea el envío se perdía sin dejar rastro.
                    _logger.LogWarning(
                        "WhatsApp no enviado: la solicitud no trae configuración (config) para cmpy: {Cmpy}, towa: {Towa}",
                        cmpy, towa);
                    return;
                }

                if (!ConfigHelper.EsSi(cfg, "ENVIARWHASTAPP", "ENVIARWHATSAPP"))
                {
                    _logger.LogWarning(
                        "WhatsApp no enviado: ENVIARWHASTAPP no está en SI para cmpy: {Cmpy} (valor recibido: {Valor}). Parametros recibidos: {Claves}",
                        cmpy, ConfigHelper.Valor(cfg, "ENVIARWHASTAPP", "ENVIARWHATSAPP"), ConfigHelper.Claves(cfg));
                    return;
                }

                // Validate required configuration values
                string whatsappId = ConfigHelper.Valor(cfg, "WHASTAPPID", "WHATSAPPID");
                string whatsappToken = ConfigHelper.Valor(cfg, "WHASTAPPTOKEN", "WHATSAPPTOKEN");
                string waTemplate = ConfigHelper.Valor(cfg, "WHASTAPPTEMPLATERECIBO", "WHATSAPPTEMPLATERECIBO");

                if (string.IsNullOrWhiteSpace(whatsappId))
                {
                    _logger.LogError(
                        "Error: WHASTAPPID no está configurado para cmpy: {Cmpy}. Parametros recibidos: {Claves}",
                        cmpy, ConfigHelper.Claves(cfg));
                    return;
                }

                if (string.IsNullOrWhiteSpace(whatsappToken))
                {
                    _logger.LogError(
                        "Error: WHASTAPPTOKEN no está configurado para cmpy: {Cmpy}. Parametros recibidos: {Claves}",
                        cmpy, ConfigHelper.Claves(cfg));
                    return;
                }

                if (string.IsNullOrWhiteSpace(waTemplate))
                {
                    _logger.LogError(
                        "Error: WHASTAPPTEMPLATERECIBO no está configurado para cmpy: {Cmpy}. Parametros recibidos: {Claves}",
                        cmpy, ConfigHelper.Claves(cfg));
                    return;
                }

                byte[] bytes = Convert.FromBase64String(datafileBase64);

                string mediaUrl = $"https://partner.gupshup.io/partner/app/{whatsappId}/media";
                _logger.LogInformation("Subiendo archivo a WhatsApp. URL: {Url}, Archivo: {FileName}", mediaUrl, namefile);

                var _client = new RestClient(mediaUrl);
                var request = new RestRequest("", Method.Post);
                request.AddHeader("accept", "application/json");
                request.AddHeader("token", whatsappToken);
                request.AlwaysMultipartFormData = true;
                request.AddFile("file", bytes, namefile, "application/pdf");
                request.AddParameter("file_type", "application/pdf");

                var resp = await _client.ExecuteAsync(request);

                if (resp.IsSuccessStatusCode)
                {
                    var filemedia = JsonSerializer.Deserialize<JsonElement>(resp.Content);
                    string mediaId = filemedia.GetProperty("mediaId").GetString();

                    _logger.LogInformation("Archivo subido exitosamente. MediaId: {MediaId}", mediaId);

                    await WasendMessageAsync(
                        cmpy, cfg, towa, waTemplate, mediaId, namefile, numRecibo, nomCliente, vendedorNombre, total, fecha,
                        etiquetaDocumento
                    );
                }
                else
                {
                    _logger.LogError("Error al subir media para cmpy {Cmpy}: {StatusCode} - {Content}", cmpy, resp.StatusCode, resp.Content);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en WamediaIdAsync para cmpy: {Cmpy}, towa: {Towa}", cmpy, towa);
            }
        }

        public async Task WasendMessageAsync(
            string cmpy,
      Dictionary<string, string> configuracion,
       string towa,
       string waTemplate,
       string idFileMedia,
       string namefile,
        string numRecibo,
       string nomCliente,
       string vendedorNombre,
       string total,
       string fecha,
       string? etiquetaDocumento = null)
        {
            try
            {
                _logger.LogInformation("Iniciando WasendMessageAsync para towa: {Towa}, MediaId: {MediaId}", towa, idFileMedia);

                var cfg = ConfigHelper.Normalizar(configuracion);

                // Validate required configuration values
                string whatsappApi = ConfigHelper.Valor(cfg, "WHASTAPPAPI", "WHATSAPPAPI");
                string whatsappPhone = ConfigHelper.Valor(cfg, "WHASTAPPPHONE", "WHATSAPPPHONE");

                if (string.IsNullOrWhiteSpace(whatsappApi))
                {
                    _logger.LogError(
                        "Error: WHASTAPPAPI no está configurado para cmpy: {Cmpy}. Parametros recibidos: {Claves}",
                        cmpy, ConfigHelper.Claves(cfg));
                    return;
                }

                if (string.IsNullOrWhiteSpace(whatsappPhone))
                {
                    _logger.LogError(
                        "Error: WHASTAPPPHONE no está configurado para cmpy: {Cmpy}. Parametros recibidos: {Claves}",
                        cmpy, ConfigHelper.Claves(cfg));
                    return;
                }

                string indicativo = ResolverIndicativo(cmpy, cfg);
                string resctrindwa = NormalizarDestino(towa, indicativo);

                if (resctrindwa.Length == 0)
                {
                    _logger.LogWarning(
                        "WhatsApp no enviado: el cliente no tiene un celular válido (cmpy: {Cmpy}, valor recibido: {Destino})",
                        cmpy, towa);
                    return;
                }

                // La pasarela rechaza el template si alguno de los parámetros va vacío,
                // así que ninguno puede quedar en blanco: un documento sin
                // csh_event_type dejaba vacía la etiqueta y tumbaba todo el envío.
                string etiqueta = !string.IsNullOrWhiteSpace(etiquetaDocumento)
                    ? etiquetaDocumento.Trim()
                    : EtiquetaDesdeNombreArchivo(namefile);

                var parametros = new List<string>
                {
                    Parametro(numRecibo),
                    Parametro(nomCliente),
                    Parametro(vendedorNombre),
                    Parametro(total),
                    Parametro(fecha),
                    Parametro(etiqueta)
                };

                var templateJson = new Dictionary<string, object>
                {
                    { "params", parametros },
                    { "id", waTemplate }
                };

                var messageJson = new Dictionary<string, object>
                {
                    { "document", new Dictionary<string, object> {
                        { "filename", namefile },
                        { "id", idFileMedia }
                    }},
                    { "type", "document" }
                };

                var payload = new Dictionary<string, string>
                {
                    { "template", JsonSerializer.Serialize(templateJson) },
                    { "source", whatsappPhone },
                    { "destination", resctrindwa },
                    { "channel", "whatsapp" }
                };
                if (idFileMedia != "0")
                    payload.Add("message", JsonSerializer.Serialize(messageJson));

                var body = new StringBuilder();
                foreach (var kv in payload)
                {
                    if (body.Length > 0) body.Append('&');
                    body.Append(Uri.EscapeDataString(kv.Key)).Append('=').Append(Uri.EscapeDataString(kv.Value));
                }

                _logger.LogInformation(
                    "Enviando mensaje WhatsApp. Cmpy: {Cmpy}, Destino: {Destino}, Template: {Template}",
                    cmpy, resctrindwa, waTemplate);

                var _client = new RestClient($"https://api.gupshup.io/wa/api/v1/template/msg");
                var request = new RestRequest("", Method.Post);
                request.AddHeader("accept", "application/json");
                request.AddHeader("apikey", whatsappApi);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddStringBody(body.ToString(), DataFormat.None);

                var response = await _client.ExecuteAsync(request);

                // La pasarela responde 200 incluso cuando rechaza el mensaje
                // ({"status":"error"}), así que mirar solo el código HTTP daba por
                // enviado un mensaje que nunca salió.
                if (EnvioAceptado(response))
                {
                    _logger.LogInformation(
                        "Mensaje WhatsApp enviado exitosamente a {Destino} (cmpy {Cmpy}): {StatusCode} - {Content}",
                        resctrindwa, cmpy, response.StatusCode, response.Content);
                }
                else
                {
                    _logger.LogError(
                        "Error al enviar mensaje WhatsApp a {Destino} (cmpy {Cmpy}, template {Template}): {StatusCode} - {Content}",
                        resctrindwa, cmpy, waTemplate, response.StatusCode, response.Content);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en WasendMessageAsync para cmpy: {Cmpy}, towa: {Towa}", cmpy, towa);
            }
        }

        public async Task EnviarBienvenidaAutorizacionAsync(string destination, string? app = null)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(destination))
                {
                    _logger.LogWarning("EnviarBienvenidaAutorizacionAsync: destino vacío, no se envía el mensaje de bienvenida.");
                    return;
                }

                // Las credenciales y el nombre de la empresa salen de appsettings y no
                // del código: cada compañía tiene su propia app y su propio número en
                // la pasarela, y con los valores fijos de RECAMIER el cliente de otra
                // compañía recibía un mensaje que no era de su empresa.
                var remitente = ResolverRemitenteBienvenida(app);

                if (remitente == null)
                {
                    _logger.LogWarning(
                        "No se envía el mensaje de bienvenida a {Destino}: no hay remitente configurado en WhatsAppBienvenida para la app {App}",
                        destination, app ?? "(sin app)");
                    return;
                }

                var messageJson = new Dictionary<string, object>
                {
                    { "type", "text" },
                    { "text", "*¡Gracias por tu autorización!*\n\n" +
                              $"Te damos la bienvenida al canal de comunicación de *{remitente.Empresa}*.\n\n" +
                              "Hemos registrado correctamente tu autorización para el tratamiento de datos personales y la recepción de mensajes informativos a través de este canal." }
                };

                var payload = new Dictionary<string, string>
                {
                    { "channel", "whatsapp" },
                    { "source", remitente.Source },
                    { "destination", destination.Trim() },
                    { "src.name", remitente.SrcName },
                    { "message", JsonSerializer.Serialize(messageJson) }
                };

                var body = new StringBuilder();
                foreach (var kv in payload)
                {
                    if (body.Length > 0) body.Append('&');
                    body.Append(Uri.EscapeDataString(kv.Key)).Append('=').Append(Uri.EscapeDataString(kv.Value));
                }

                _logger.LogInformation("Enviando mensaje de bienvenida (autorización) a: {Destino}", destination);

                var _client = new RestClient("https://api.gupshup.io/wa/api/v1/msg");
                var request = new RestRequest("", Method.Post);
                request.AddHeader("accept", "application/json");
                request.AddHeader("apikey", remitente.ApiKey);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddStringBody(body.ToString(), DataFormat.None);

                var response = await _client.ExecuteAsync(request);

                if (EnvioAceptado(response))
                {
                    _logger.LogInformation("Mensaje de bienvenida enviado exitosamente a {Destino}: {StatusCode} - {Content}", destination, response.StatusCode, response.Content);
                }
                else
                {
                    _logger.LogError("Error al enviar mensaje de bienvenida a {Destino}: {StatusCode} - {Content}", destination, response.StatusCode, response.Content);
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error en EnviarBienvenidaAutorizacionAsync para destino: {Destino}", destination);
            }
        }

        /// <summary>
        /// Indicativo de país del destino. Primero el parámetro INDICATIVOPAIS de la
        /// compañía (así una compañía nueva no obliga a tocar el código) y, si no está,
        /// la tabla por compañía.
        /// </summary>
        private static string ResolverIndicativo(string cmpy, Dictionary<string, string> cfg)
        {
            string configurado = ConfigHelper.Valor(cfg, "INDICATIVOPAIS", "INDICATIVO");
            if (configurado.Length > 0)
                return Regex.Replace(configurado, @"\D", "");

            string clave = CompanyRules.Normalize(cmpy);
            return IndicativosPorCompania.TryGetValue(clave, out var indicativo)
                ? indicativo
                : IndicativosPorCompania[CompanyRules.RECAMIER];
        }

        /// <summary>
        /// Deja el destino en el formato que espera la pasarela: solo dígitos y con
        /// indicativo de país. Si el celular ya viene con el indicativo no se le
        /// vuelve a anteponer, porque el número duplicado no existe y el mensaje se
        /// pierde sin error visible.
        /// </summary>
        private static string NormalizarDestino(string? towa, string indicativo)
        {
            string digitos = Regex.Replace(towa ?? string.Empty, @"\D", "");
            if (digitos.Length == 0) return string.Empty;

            // Prefijo internacional escrito como 00XX.
            if (digitos.StartsWith("00") && digitos.Length > 2)
                digitos = digitos.Substring(2);

            if (indicativo.Length > 0 &&
                digitos.StartsWith(indicativo) &&
                digitos.Length > indicativo.Length + 6)
                return digitos;

            return indicativo + digitos;
        }

        /// <summary>Etiqueta del documento a partir del nombre del archivo (respaldo).</summary>
        private static string EtiquetaDesdeNombreArchivo(string? namefile)
        {
            string etiqueta = (namefile ?? string.Empty).Split('_')[0].Trim();
            return etiqueta.Length > 0 ? etiqueta : "Documento";
        }

        /// <summary>Ningún parámetro del template puede ir vacío: la pasarela rechaza el mensaje.</summary>
        private static string Parametro(string? valor)
        {
            string texto = (valor ?? string.Empty).Trim();
            return texto.Length > 0 ? texto : "-";
        }

        /// <summary>
        /// Indica si la pasarela aceptó el mensaje. Gupshup responde 200/202 con
        /// {"status":"submitted"}, pero ante un rechazo puede devolver igualmente un
        /// 200 con {"status":"error", ...}.
        /// </summary>
        private static bool EnvioAceptado(RestResponse response)
        {
            if (!response.IsSuccessStatusCode) return false;
            if (string.IsNullOrWhiteSpace(response.Content)) return true;

            try
            {
                var cuerpo = JsonSerializer.Deserialize<JsonElement>(response.Content);
                if (cuerpo.ValueKind == JsonValueKind.Object &&
                    cuerpo.TryGetProperty("status", out var estado) &&
                    estado.ValueKind == JsonValueKind.String)
                {
                    string valor = estado.GetString()?.ToLowerInvariant() ?? string.Empty;
                    return valor != "error" && valor != "failed";
                }
            }
            catch
            {
                // Cuerpo no JSON: lo único con lo que se cuenta es el código HTTP.
            }

            return true;
        }

        /// <summary>Remitente del mensaje de bienvenida, leído de appsettings.</summary>
        private sealed class RemitenteBienvenida
        {
            public string App { get; set; } = string.Empty;
            public string ApiKey { get; set; } = string.Empty;
            public string Source { get; set; } = string.Empty;
            public string SrcName { get; set; } = string.Empty;
            public string Empresa { get; set; } = string.Empty;
        }

        /// <summary>
        /// Busca en appsettings (sección WhatsAppBienvenida:Companias) la compañía
        /// cuya app coincide con la que reportó el callback. Si el callback no trae
        /// app se usa la compañía indicada en WhatsAppBienvenida:Default. Si trae una
        /// app que no está registrada, o no hay credenciales, devuelve null y no se
        /// envía: la respuesta viene de otra compañía, y el mensaje saldría con el
        /// nombre y el número de la compañía por defecto.
        /// </summary>
        private RemitenteBienvenida? ResolverRemitenteBienvenida(string? app)
        {
            // Lectura manual de la seccion (y no ConfigurationBinder.Get<T>) para no
            // agregarle a Migracion.Service una dependencia solo por esto.
            var companias = new List<(string Clave, RemitenteBienvenida Remitente)>();

            foreach (var hijo in _configuration.GetSection("WhatsAppBienvenida:Companias").GetChildren())
            {
                companias.Add((hijo.Key, new RemitenteBienvenida
                {
                    App = hijo["App"] ?? string.Empty,
                    ApiKey = hijo["ApiKey"] ?? string.Empty,
                    Source = hijo["Source"] ?? string.Empty,
                    SrcName = hijo["SrcName"] ?? string.Empty,
                    Empresa = hijo["Empresa"] ?? string.Empty
                }));
            }

            if (companias.Count == 0) return null;

            RemitenteBienvenida? remitente = null;

            if (!string.IsNullOrWhiteSpace(app))
            {
                remitente = companias
                    .Where(c => !string.IsNullOrWhiteSpace(c.Remitente.App) &&
                                string.Equals(c.Remitente.App.Trim(), app.Trim(), StringComparison.OrdinalIgnoreCase))
                    .Select(c => c.Remitente)
                    .FirstOrDefault();
            }

            if (remitente == null)
            {
                if (!string.IsNullOrWhiteSpace(app))
                {
                    // Queda en el log para que se registre la app de la compania que
                    // falta. La autorizacion ya quedo guardada: lo unico que se omite
                    // es la bienvenida, mejor que enviarla a nombre de otra empresa.
                    _logger.LogWarning(
                        "La app {App} no esta registrada en WhatsAppBienvenida:Companias; no se envia la bienvenida",
                        app);
                    return null;
                }

                string porDefecto = _configuration["WhatsAppBienvenida:Default"] ?? CompanyRules.RECAMIER;
                remitente = companias
                    .Where(c => string.Equals(c.Clave.Trim(), porDefecto.Trim(), StringComparison.OrdinalIgnoreCase))
                    .Select(c => c.Remitente)
                    .FirstOrDefault();
            }

            if (remitente == null ||
                string.IsNullOrWhiteSpace(remitente.ApiKey) ||
                string.IsNullOrWhiteSpace(remitente.Source))
                return null;

            if (string.IsNullOrWhiteSpace(remitente.SrcName)) remitente.SrcName = remitente.App;

            return remitente;
        }
    }

}
