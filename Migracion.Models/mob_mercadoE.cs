using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_mercadoE
    {
        [JsonPropertyName("KEY_mer_cmpy")]
        [MaxLength(2), Required]
        public string? mer_cmpy { get; set; }
        
        [JsonPropertyName("KEY_mer_codigo")]
        [MaxLength(10), Required]
        public string? mer_codigo { get; set; }
        //[MaxLength(60), Required]
        public string? mer_desc { get; set; }
        //[MaxLength(10), Required]
        //public string? mer_enter { get; set; }
        //[Required]
        //public DateTime mer_date { get; set; }
        //[MaxLength(10), Required]
        //public string? mer_time { get; set; }
    }
}
