using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_bankacctE
    {
        [JsonPropertyName("KEY_mba_cmpy")]
        [MaxLength(2), Required]
        public string? mba_cmpy { get; set; }
        [JsonPropertyName("KEY_mba_anum")]
        [MaxLength(25), Required]
        public string? mba_anum { get; set; }
        [MaxLength(40), Required]
        public string? mba_name { get; set; }
    }
}
