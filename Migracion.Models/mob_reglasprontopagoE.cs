using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_reglasprontopagoE
    {
        [JsonPropertyName("KEY_mrp_cmpy")]
        [MaxLength(2), Required]
        public string? mrp_cmpy { get; set; }

        [JsonPropertyName("KEY_mrp_type")]
        [MaxLength(3)]
        public string? mrp_type { get; set; }

        [JsonPropertyName("KEY_mrp_tipo")]
        [MaxLength(1)]
        public string? mrp_tipo { get; set; }

        [JsonPropertyName("KEY_mrp_dini")]
        public string? mrp_dini { get; set; }
        [JsonPropertyName("KEY_mrp_dfin")]
        public string? mrp_dfin { get; set; }
        public string? mrp_porc { get; set; }
    }
}
