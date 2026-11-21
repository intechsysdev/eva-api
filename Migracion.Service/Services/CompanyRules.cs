using System.Globalization;

namespace Migracion.Service.Services
{
    /// <summary>
    /// Reglas de negocio que dependen del código de compañía (csh_cmpy).
    /// Centraliza las diferencias por país para no dispersar comparaciones sueltas por el código.
    /// </summary>
    public static class CompanyRules
    {
        /// Recamier S.A. - Colombia
        public const string RECAMIER = "RE";

        /// Dermodis S.A.C. - Perú
        public const string DERMODIS = "DD";

        /// Lansey
        public const string LANSEY = "LA";

        /// Keramer
        public const string KERAMER = "KR";

        public static string Normalize(string? cmpy) => (cmpy ?? string.Empty).Trim().ToUpperInvariant();

        /// <summary>
        /// Perú (DD) recauda con Código CIP (Código Identificador de Pago) en vez del
        /// par convenio/referencia que usa el corresponsal bancario en Colombia.
        /// </summary>
        public static bool UsaCodigoCip(string? cmpy) => Normalize(cmpy) == DERMODIS;

        /// <summary>
        /// Símbolo de moneda: soles en DD, pesos en RE y dólares en LA y KR. Es la
        /// misma regla de CompanyRules.simboloMoneda en la app.
        /// </summary>
        public static string SimboloMoneda(string? cmpy) => Normalize(cmpy) == DERMODIS ? "S/" : "$";

        /// <summary>
        /// Monto con el formato de la compañía, igual que formatMoneda en la app:
        /// decimales y separador decimal salen de NUMERODECIMALES y CARACTERDECIMALES,
        /// y los miles son el separador opuesto al decimal. "S/ 1,234.50" lleva espacio
        /// y "$1.234" no; el signo de un negativo va antes del símbolo.
        ///
        /// Si falta NUMERODECIMALES se dejan 2 decimales y no 0 como en la app: en un
        /// recibo, perder los centavos por un parámetro ausente altera el valor pagado.
        /// </summary>
        public static string Moneda(string? cmpy, decimal valor, Dictionary<string, string>? configuracion)
        {
            bool hayDecimales = int.TryParse(ConfigHelper.Valor(configuracion, "NUMERODECIMALES"), out int decimales);
            if (!hayDecimales || decimales < 0) decimales = 2;

            bool decimalConComa = ConfigHelper.Valor(configuracion, "CARACTERDECIMALES") == ",";
            var formato = new NumberFormatInfo
            {
                NumberDecimalSeparator = decimalConComa ? "," : ".",
                NumberGroupSeparator = decimalConComa ? "." : ","
            };

            string simbolo = SimboloMoneda(cmpy);
            string prefijo = simbolo.Length > 1 ? $"{simbolo} " : simbolo;
            string numero = Math.Abs(valor).ToString($"N{decimales}", formato);
            return valor < 0 ? $"-{prefijo}{numero}" : $"{prefijo}{numero}";
        }
    }
}
