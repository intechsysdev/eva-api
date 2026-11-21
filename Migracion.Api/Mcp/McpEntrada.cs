using ModelContextProtocol;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Migracion.Api.Mcp
{
    /// <summary>
    /// Validación de los argumentos que llegan por MCP. Los escribe un modelo, no
    /// la app, y varias consultas de Migracion.Service arman el SQL concatenando
    /// texto: aquí solo pasan códigos con caracteres de lista blanca, de modo que
    /// ningún argumento puede cerrar una comilla ni añadir SQL.
    /// Los errores salen como McpException para que el modelo lea el motivo y corrija.
    /// </summary>
    internal static class McpEntrada
    {
        public const string FormatoFechaEntrada = "yyyy-MM-dd";

        /// <summary>Formato de fecha que usan las consultas y los datos de EVA.</summary>
        public const string FormatoFechaDatos = "MM/dd/yyyy";

        private static readonly Regex PatronCodigo = new(@"^[A-Za-z0-9_.\-]{1,30}$", RegexOptions.Compiled);
        private static readonly Regex PatronCompania = new(@"^[A-Za-z0-9]{1,4}$", RegexOptions.Compiled);

        public static string Compania(string? cmpy, McpSettings settings)
        {
            string valor = string.IsNullOrWhiteSpace(cmpy) ? settings.Compania : cmpy;
            valor = (valor ?? string.Empty).Trim().ToUpperInvariant();

            if (valor.Length == 0)
                throw new McpException(
                    "Falta la compañía. Envía 'cmpy' con el código: RE (Recamier, Colombia), " +
                    "DD (Dermodis, Perú), LA (Lansey) o KR (Keramer).");

            if (!PatronCompania.IsMatch(valor))
                throw new McpException(
                    $"'cmpy' no es un código de compañía válido: '{Recortar(cmpy)}'. Usa RE, DD, LA o KR.");

            return valor;
        }

        public static string Codigo(string? valor, string nombre)
        {
            string limpio = (valor ?? string.Empty).Trim();
            if (limpio.Length == 0)
                throw new McpException($"Falta '{nombre}'.");

            if (!PatronCodigo.IsMatch(limpio))
                throw new McpException(
                    $"'{nombre}' debe ser un código de hasta 30 caracteres con solo letras, números, " +
                    $"punto, guion o guion bajo. Recibido: '{Recortar(valor)}'.");

            return limpio;
        }

        public static string? CodigoOpcional(string? valor, string nombre) =>
            string.IsNullOrWhiteSpace(valor) ? null : Codigo(valor, nombre);

        public static string[] Codigos(string[]? valores, string nombre, int maximo)
        {
            var lista = (valores ?? Array.Empty<string>())
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => Codigo(v, nombre))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (lista.Length == 0)
                throw new McpException($"'{nombre}' debe traer al menos un código.");
            if (lista.Length > maximo)
                throw new McpException($"'{nombre}' admite máximo {maximo} códigos por llamada; divide la consulta.");

            return lista;
        }

        public static DateTime Fecha(string? valor, string nombre)
        {
            if (!DateTime.TryParseExact((valor ?? string.Empty).Trim(), FormatoFechaEntrada,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
            {
                throw new McpException(
                    $"'{nombre}' debe ser una fecha con formato AAAA-MM-DD (ejemplo: 2026-11-20). " +
                    $"Recibido: '{Recortar(valor)}'.");
            }
            return fecha;
        }

        /// <summary>Rango de fechas ya convertido al formato de los datos (MM/dd/yyyy).</summary>
        public static (string inicio, string fin) Rango(string? fechaInicio, string? fechaFin, int maximoDias)
        {
            var inicio = Fecha(fechaInicio, "fecha_inicio");
            var fin = Fecha(fechaFin, "fecha_fin");

            if (fin < inicio)
                throw new McpException("'fecha_fin' no puede ser anterior a 'fecha_inicio'.");
            if ((fin - inicio).TotalDays > maximoDias)
                throw new McpException(
                    $"El rango máximo es de {maximoDias} días. Divide la consulta en rangos más cortos.");

            return (inicio.ToString(FormatoFechaDatos, CultureInfo.InvariantCulture),
                    fin.ToString(FormatoFechaDatos, CultureInfo.InvariantCulture));
        }

        public static (int limite, int desplazamiento) Pagina(int limite, int desplazamiento, McpSettings settings)
        {
            int maximo = Math.Max(1, settings.LimiteMaximo);
            return (Math.Clamp(limite, 1, maximo), Math.Max(0, desplazamiento));
        }

        /// <summary>Texto de búsqueda libre: se compara en memoria, nunca viaja al SQL.</summary>
        public static string? Busqueda(string? buscar)
        {
            string limpio = (buscar ?? string.Empty).Trim();
            if (limpio.Length == 0) return null;
            if (limpio.Length > 80)
                throw new McpException("'buscar' admite máximo 80 caracteres.");
            return limpio;
        }

        /// <summary>Contiene, sin distinguir mayúsculas ni tildes.</summary>
        public static bool Contiene(string? campo, string buscar) =>
            !string.IsNullOrEmpty(campo) &&
            CultureInfo.InvariantCulture.CompareInfo.IndexOf(
                campo, buscar, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;

        /// <summary>Igualdad de códigos: las columnas CHAR llegan con espacios de relleno.</summary>
        public static bool Igual(string? campo, string codigo) =>
            string.Equals((campo ?? string.Empty).Trim(), codigo, StringComparison.OrdinalIgnoreCase);

        public static decimal Numero(string? valor) =>
            decimal.TryParse((valor ?? string.Empty).Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var n)
                ? n
                : 0m;

        private static string Recortar(string? valor)
        {
            string texto = (valor ?? string.Empty).Trim();
            return texto.Length <= 40 ? texto : texto[..40] + "…";
        }
    }
}
