using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rrutasE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rruta_cmpy")]
        public string? rruta_cmpy { get; set; }
        [MaxLength(10), JsonPropertyName("KEY_rruta_codusu")]
        public string? rruta_codusu { get; set; }
        [MaxLength(15), JsonPropertyName("KEY_rruta_cus")]
        public string? rruta_cus { get; set; }
        [MaxLength(15), JsonPropertyName("KEY_rruta_succli")]
        public string? rruta_succli { get; set; }
        public string? rruta_ruta { get; set; }
        public string? rruta_sec { get; set; }
        public string? rruta_fecha { get; set; }
        public string? rruta_date { get; set; }
        public string? rruta_time { get; set; }
        public string? rruta_estadoupd { get; set; }

    }
}
