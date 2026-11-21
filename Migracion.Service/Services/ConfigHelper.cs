namespace Migracion.Service.Services
{
    /// <summary>
    /// Lectura tolerante de la configuración por compañía.
    ///
    /// El diccionario <c>config</c> que llega en el body lo arma Seguridad.API a partir
    /// del SP <c>GetListSettingsByCompany</c>, una fila por parámetro y por compañía.
    /// Como las filas de cada compañía se cargaron por separado, no siempre coinciden
    /// carácter por carácter con las de RE: llegan con espacios de relleno, con otra
    /// combinación de mayúsculas/minúsculas o con la clave bien escrita
    /// (WHATSAPP...) en vez de la histórica mal escrita (WHASTAPP...).
    ///
    /// Un <c>Dictionary&lt;string,string&gt;</c> corriente compara claves con
    /// sensibilidad a mayúsculas y no recorta espacios, así que esas diferencias hacían
    /// que el parámetro "no existiera" para una compañía y sí para otra. Aquí se
    /// normaliza una sola vez para que la búsqueda sea igual de robusta en todas.
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// Devuelve una copia con las claves y los valores recortados y con búsqueda
        /// insensible a mayúsculas. Nunca devuelve null: si no hay configuración,
        /// devuelve un diccionario vacío para que quien lo use no tenga que validar.
        /// </summary>
        public static Dictionary<string, string> Normalizar(Dictionary<string, string>? configuracion)
        {
            var normalizada = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (configuracion == null) return normalizada;

            foreach (var kv in configuracion)
            {
                if (string.IsNullOrWhiteSpace(kv.Key)) continue;
                // Indexador y no Add: dos claves que solo difieren en mayúsculas
                // colisionan al normalizar y Add lanzaría excepción.
                normalizada[kv.Key.Trim()] = kv.Value?.Trim() ?? string.Empty;
            }

            return normalizada;
        }

        /// <summary>
        /// Primer valor no vacío de las claves indicadas. Recibe varias claves para
        /// admitir los alias históricos (WHASTAPPID / WHATSAPPID) sin obligar a
        /// corregir las filas ya cargadas en la base.
        /// </summary>
        public static string Valor(Dictionary<string, string>? configuracion, params string[] claves)
        {
            if (configuracion == null || configuracion.Count == 0) return string.Empty;

            foreach (var clave in claves)
            {
                // Busqueda directa primero: es el caso normal y no recorre el diccionario.
                if (configuracion.TryGetValue(clave, out var valor) && !string.IsNullOrWhiteSpace(valor))
                    return valor.Trim();

                // Y si no aparece, comparando sin espacios ni mayusculas, para que
                // funcione tambien sobre un diccionario que no venga normalizado.
                foreach (var kv in configuracion)
                {
                    if (kv.Key != null &&
                        kv.Key.Trim().Equals(clave, StringComparison.OrdinalIgnoreCase) &&
                        !string.IsNullOrWhiteSpace(kv.Value))
                        return kv.Value.Trim();
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Interpreta un parámetro de tipo interruptor. Se aceptan las variantes con
        /// las que se han cargado las filas ("SI", "S", "1", "TRUE") para que un
        /// parámetro escrito distinto no apague la funcionalidad en silencio.
        /// </summary>
        public static bool EsSi(Dictionary<string, string>? configuracion, params string[] claves)
        {
            string valor = Valor(configuracion, claves);
            if (valor.Length == 0) return false;

            return valor.Equals("SI", StringComparison.OrdinalIgnoreCase)
                || valor.Equals("S", StringComparison.OrdinalIgnoreCase)
                || valor.Equals("YES", StringComparison.OrdinalIgnoreCase)
                || valor.Equals("Y", StringComparison.OrdinalIgnoreCase)
                || valor.Equals("TRUE", StringComparison.OrdinalIgnoreCase)
                || valor == "1";
        }

        /// <summary>
        /// Nombres de las claves recibidas, para dejar constancia en el log de qué
        /// llegó cuando falta un parámetro. Solo nombres: los valores traen tokens
        /// y claves de API y no deben quedar escritos en el archivo de log.
        /// </summary>
        public static string Claves(Dictionary<string, string>? configuracion)
        {
            if (configuracion == null || configuracion.Count == 0) return "(sin configuración)";
            return string.Join(", ", configuracion.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase));
        }
    }
}
