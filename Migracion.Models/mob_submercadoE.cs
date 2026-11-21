using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_submercadoE
    {
        [JsonPropertyName("KEY_sbm_cmpy")]
        [MaxLength(2), Required]
        public string? sbm_cmpy { get; set; }

        [JsonPropertyName("KEY_sbm_codmer")]
        [MaxLength(10)]
        public string? sbm_codmer { get; set; }

        [JsonPropertyName("KEY_sbm_codigo")]
        [MaxLength(10)]
        public string? sbm_codigo { get; set; }
        [MaxLength(60)]
        public string? sbm_desc { get; set; }
        [MaxLength(255)]
        public string? sbm_mail { get; set; }
    }
}
