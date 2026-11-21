using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rmenuppalE
    {
        [MaxLength(2), Required, JsonPropertyName("KEY_rmen_cmpy")]
        public string? rmen_cmpy { get; set; }
        [MaxLength(2), Required, JsonPropertyName("KEY_rmen_codmen")]
        public string? rmen_codmen { get; set; }
        [MaxLength(50), Required]
        public string? rmen_desmen { get; set; }
        public string? rmen_cabecera { get; set; }
        [MaxLength(80), Required]
        public string? rmen_aplicacion { get; set; }
        public string? rmen_date { get; set; }
        public string? rmen_time { get; set; }
        public string? rmen_estadoupd { get; set; }

    }
}
