using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rasociadoE
    {
        //
        [MaxLength(2), Required, JsonPropertyName("KEY_rasoc_cmpy")]
        public string? rasoc_cmpy { get; set; }
        [MaxLength(10), Required, JsonPropertyName("KEY_rasoc_custd")]
        public string? rasoc_custd { get; set; }
        [MaxLength(15), Required, JsonPropertyName("KEY_rasoc_codcon")]
        public string? rasoc_codcon { get; set; }
        [MaxLength(15), Required, JsonPropertyName("KEY_rasoc_cust")]
        public string? rasoc_cust { get; set; }
        [MaxLength(15), Required, JsonPropertyName("KEY_rasoc_succli")]
        public string? rasoc_succli { get; set; }
        [MaxLength(20), Required, JsonPropertyName("KEY_rasoc_coddeposito")]
        public string? rasoc_coddeposito { get; set; }
        [MaxLength(5)]
        public string? rasoc_estado { get; set; }
        [MaxLength(30)]
        public string? rasoc_desestado { get; set; }
        public string? rasoc_date { get; set; }
        public string? rasoc_time { get; set; }
        public string? rasoc_estadoupd { get; set; }
        public string? rasoc_enter { get; set; }

    }
}
