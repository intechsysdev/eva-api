namespace Migracion.Api.Mcp
{
    /// <summary>
    /// Configuración del servidor MCP (sección "Mcp" de appsettings o variables de
    /// entorno Mcp__ApiKey, Mcp__Compania, Mcp__LimiteMaximo).
    /// </summary>
    public class McpSettings
    {
        public const string Seccion = "Mcp";

        /// <summary>Longitud mínima de la llave: una llave corta se adivina por fuerza bruta.</summary>
        public const int LongitudMinimaLlave = 24;

        /// <summary>
        /// Llave que debe enviar el cliente MCP en "Authorization: Bearer ..." (o en
        /// "X-Api-Key"). Vacía o muy corta = el endpoint /mcp no se publica.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Compañía que se usa cuando la herramienta no recibe "cmpy". Cada servidor
        /// atiende una compañía, así el modelo no tiene que adivinarla.
        /// </summary>
        public string Compania { get; set; } = string.Empty;

        /// <summary>Máximo de filas que una herramienta devuelve por llamada.</summary>
        public int LimiteMaximo { get; set; } = 100;

        public bool Habilitado =>
            !string.IsNullOrWhiteSpace(ApiKey) && ApiKey.Trim().Length >= LongitudMinimaLlave;
    }
}
