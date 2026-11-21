using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rordline_hisE
    {
        [JsonPropertyName("KEY_orl_cmpy")]
        [MaxLength(2), Required]
        public string? orl_cmpy { get; set; }

        [JsonPropertyName("KEY_orl_reg")]
        [MaxLength(5)]
        public string? orl_reg { get; set; }

        [JsonPropertyName("KEY_orl_teri")]
        [MaxLength(5)]
        public string? orl_teri { get; set; }

        [JsonPropertyName("KEY_orl_num")]
        public string? orl_num { get; set; }

        [JsonPropertyName("KEY_orl_entdt")]
        public string? orl_entdt { get; set; }
        [MaxLength(10)]
        public string? orl_time { get; set; }
        [JsonPropertyName("KEY_orl_part")]
        [MaxLength(21)]
        public string? orl_part { get; set; }
        public string? orl_qord { get; set; }
        //public string? orl_qordb { get; set; }
        [MaxLength(1)]
        public string? orl_txbl { get; set; }
        [MaxLength(1)]
        [JsonPropertyName("KEY_orl_level")]
        public string? orl_level { get; set; }
        [MaxLength(1)]
        public string? orl_marca { get; set; }
        public string? orl_date { get; set; }
        //public decimal orl_vlrparcial { get; set; }
        //public decimal orl_dctogral { get; set; }
        //public decimal orl_vlrtotaliva { get; set; }
        //public decimal orl_vlrtotal { get; set; }
    }
}
