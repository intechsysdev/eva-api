using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_ritempricE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rpri_cmpy")]
        public string? rpri_cmpy { get; set; }
        [MaxLength(21), JsonPropertyName("KEY_rpri_id")]
        public string? rpri_id { get; set; }
        [MaxLength(2), JsonPropertyName("KEY_rpri_lista")]
        public string? rpri_lista { get; set; }
        public string? rpri_price { get; set; }
    }
}
