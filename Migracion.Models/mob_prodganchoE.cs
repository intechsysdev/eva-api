using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_prodganchoE
    {
        [JsonPropertyName("KEY_pgan_cmpy")]
        [MaxLength(2), Required]
        public string? pgan_cmpy { get; set; }

        [JsonPropertyName("KEY_pgan_reg")]
        [MaxLength(5), Required]
        public string? pgan_reg { get; set; }

        [JsonPropertyName("KEY_pgan_teri")]
        [MaxLength(5), Required]
        public string? pgan_teri { get; set; }

        [JsonPropertyName("KEY_pgan_cust")]
        [MaxLength(15), Required]
        public string? pgan_cust { get; set; }

        [JsonPropertyName("KEY_pgan_succli")]
        [MaxLength(15), Required]
        public string? pgan_succli { get; set; }

        [JsonPropertyName("KEY_pgan_id")]
        [MaxLength(21), Required]
        public string? pgan_id{ get; set; }
        [MaxLength(40),Required]
        public string? pgan_desc { get; set; }
    }
}
