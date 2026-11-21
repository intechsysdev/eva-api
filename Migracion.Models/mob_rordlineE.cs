using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rordlineE
    {
        [JsonPropertyName("KEY_orl_cmpy")]
        [MaxLength(2), Required]
        public string? orl_cmpy { get; set; }
        [MaxLength(5)]

        [JsonPropertyName("KEY_orl_reg")]
        public string? orl_reg { get; set; }

        [JsonPropertyName("KEY_orl_teri")]
        [MaxLength(5)]
        public string? orl_teri { get; set; }

        [JsonPropertyName("KEY_orl_num")]
        public string? orl_num { get; set; }

        [JsonPropertyName("KEY_orl_entdt")]
        public string? orl_entdt { get; set; }
        public string? orl_time { get; set; }
        public string? orl_part { get; set; }
        public decimal orl_qord { get; set; }
        //public decimal orl_qordb { get; set; }
        public string? orl_txbl { get; set; }
        public string? orl_level { get; set; }
        public string? orl_marca { get; set; }
        public string? orl_date { get; set; }
        //public string? orl_vlrparcial { get; set; }
        //public string? orl_dctogral { get; set; }
        //public string? orl_vlrtotaliva { get; set; }
        //public string? orl_vlrtotal { get; set; }
        //public int estado_envio { get; set; }
    }
}
