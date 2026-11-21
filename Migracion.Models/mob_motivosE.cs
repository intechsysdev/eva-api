using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_motivosE
    {//
        [MaxLength(2), Required, JsonPropertyName("KEY_cmpy")]
        public string? cmpy { get; set; }
        [Required, JsonPropertyName("KEY_movcod")]
        public string? movcod { get; set; }
        //[MaxLength(60), Required]
        public string? movdesc { get; set; }
        public string? mov_estadoupd { get; set; }
    }
}
