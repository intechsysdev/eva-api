using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_tipodocE
    {
        [JsonPropertyName("KEY_rtipodoc_cmpy")]
        [MaxLength(2), Required]
        public string? rtipodoc_cmpy { get; set; }

        [JsonPropertyName("KEY_rtipodoc_cod")]
        [MaxLength(5)]
        public string? rtipodoc_cod { get; set; }
        [MaxLength(40)]
        public string? rtipodoc_desc { get; set; }
        public string? rtipodoc_estadoupd { get; set; }
    }
}
