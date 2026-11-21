using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rabonosE
    {
        [JsonPropertyName("KEY_mabo_cmpy")]
        [MaxLength(2), Required]
        public string? mabo_cmpy { get; set; }

        [JsonPropertyName("KEY_mabo_teri")]
        [MaxLength(5), Required]
        public string? mabo_teri { get; set; }

        [JsonPropertyName("KEY_mabo_reg")]
        [MaxLength(5), Required]
        public string? mabo_reg { get; set; }

        [JsonPropertyName("KEY_mabo_inv")]
        [Required]
        public decimal mabo_inv{ get; set; }

        [JsonPropertyName("KEY_mabo_recaj")]
        [MaxLength(2), Required]
        public decimal mabo_recaj { get; set; }
        [Required]
        public string? mabo_fecrc { get; set; }
        [Required]
        public decimal mabo_vlrc { get; set; }
        [Required]
        public decimal mabo_orden { get; set; }
        [MaxLength(1), Required]
        public string? mabo_flag { get; set; }
    }
}
