using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Migracion.Api.Mcp
{
    /// <summary>
    /// Respuestas de las herramientas en JSON compacto: sin nulos, sin textos vacíos
    /// y sin el relleno de espacios de las columnas CHAR, para no gastar contexto
    /// del modelo en datos que no dicen nada. Los campos conservan el nombre de la
    /// columna (rctt_cust, car_saldo...), el mismo que usan el API y la app.
    /// </summary>
    internal static class McpSalida
    {
        private static readonly JsonSerializerOptions Opciones = new()
        {
            // Tildes y eñes tal cual, no como ñ
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false
        };

        public static string Json(object? valor)
        {
            JsonNode? nodo = JsonSerializer.SerializeToNode(valor, Opciones);
            return (Limpiar(nodo) ?? new JsonObject()).ToJsonString(Opciones);
        }

        /// <summary>
        /// Una página de resultados con los datos que el modelo necesita para pedir
        /// la siguiente. "extra" agrega campos al nivel superior (por ejemplo totales
        /// calculados sobre todas las filas, no solo sobre la página).
        /// </summary>
        public static string Pagina<T>(
            IReadOnlyCollection<T> filas, int limite, int desplazamiento,
            object? extra = null, string? siVacio = null)
        {
            var pagina = filas.Skip(desplazamiento).Take(limite).ToList();
            bool hayMas = desplazamiento + pagina.Count < filas.Count;

            var salida = new JsonObject
            {
                ["total"] = filas.Count,
                ["cantidad"] = pagina.Count,
                ["desplazamiento"] = desplazamiento,
                ["hay_mas"] = hayMas
            };
            if (hayMas)
                salida["siguiente_desplazamiento"] = desplazamiento + pagina.Count;
            if (filas.Count == 0 && siVacio != null)
                salida["mensaje"] = siVacio;

            if (extra != null && JsonSerializer.SerializeToNode(extra, Opciones) is JsonObject campos)
            {
                foreach (var campo in campos.ToList())
                {
                    campos.Remove(campo.Key);
                    salida[campo.Key] = campo.Value;
                }
            }

            salida["elementos"] = JsonSerializer.SerializeToNode(pagina, Opciones);
            return Limpiar(salida)!.ToJsonString(Opciones);
        }

        public static string Mensaje(string mensaje) =>
            new JsonObject { ["mensaje"] = mensaje }.ToJsonString(Opciones);

        private static JsonNode? Limpiar(JsonNode? nodo)
        {
            switch (nodo)
            {
                case JsonObject objeto:
                    foreach (var propiedad in objeto.ToList())
                    {
                        if (EsVacio(propiedad.Value))
                        {
                            objeto.Remove(propiedad.Key);
                        }
                        else if (TextoRecortado(propiedad.Value) is string recortado)
                        {
                            objeto[propiedad.Key] = recortado;
                        }
                        else
                        {
                            Limpiar(propiedad.Value);
                        }
                    }
                    break;

                case JsonArray arreglo:
                    for (int i = 0; i < arreglo.Count; i++)
                    {
                        if (TextoRecortado(arreglo[i]) is string recortado)
                            arreglo[i] = recortado;
                        else
                            Limpiar(arreglo[i]);
                    }
                    break;
            }
            return nodo;
        }

        private static bool EsVacio(JsonNode? nodo) =>
            nodo is null ||
            (nodo.GetValueKind() == JsonValueKind.String && string.IsNullOrWhiteSpace(nodo.GetValue<string>()));

        /// <summary>El texto sin espacios sobrantes, o null si no es texto o ya está limpio.</summary>
        private static string? TextoRecortado(JsonNode? nodo)
        {
            if (nodo is null || nodo.GetValueKind() != JsonValueKind.String) return null;
            string original = nodo.GetValue<string>();
            string recortado = original.Trim();
            return recortado.Length == original.Length ? null : recortado;
        }
    }
}
