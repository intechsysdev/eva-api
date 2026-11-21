using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_marcaE
    {
        [JsonPropertyName("KEY_mar_cmpy")]
        [MaxLength(2), Required]
        public string? mar_cmpy { get; set; }

        [JsonPropertyName("KEY_mar_codfab")]
        [MaxLength(20), Required]
        public string? mar_codfab { get; set; }
        [JsonPropertyName("KEY_mar_marca")]
        [MaxLength(10), Required]
        public string? mar_marca { get; set; }
        //[MaxLength(60), Required]
        public string? mar_desc { get; set; }
        //[MaxLength(10), Required]
        //public string? mar_enter { get; set; }
        //[Required]
        //public DateTime mar_date { get; set; }
        //[MaxLength(10), Required]
        //public string? mar_time { get; set; }
    }
}
