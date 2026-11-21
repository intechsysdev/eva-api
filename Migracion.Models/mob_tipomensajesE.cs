using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_tipomensajesE
    {
        [JsonPropertyName("KEY_tmen_cmpy")]
        [MaxLength(2), Required]
        public string? tmen_cmpy { get; set; }

        [JsonPropertyName("KEY_tmen_codigo")]
        [MaxLength(10)]
        public string? tmen_codigo { get; set; }
        [MaxLength(40)]
        public string? tmen_desc { get; set; }
        [MaxLength(255)]
        public string? tmen_mail { get; set; }
    }
}
