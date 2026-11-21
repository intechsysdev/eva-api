using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rcliproE
    {
        [JsonPropertyName("KEY_rcp_cmpy")]
        [MaxLength(2),Required]
        public string? rcp_cmpy { get; set; }

        [JsonPropertyName("KEY_rcp_codusu")]
        [MaxLength(10),Required]
        public string? rcp_codusu { get; set; }

        [JsonPropertyName("KEY_rcp_id")]
        [MaxLength(21),Required]
        public string? rcp_id { get; set; }

        [JsonPropertyName("KEY_rcp_cust")]
        [MaxLength(15),Required]
        public string? rcp_cust { get; set; }

        [JsonPropertyName("KEY_rctt_cmpy")]
        [MaxLength(15),Required]
        public string? rcp_succli { get; set; }
        public string? rcp_ur { get; set; }
        public string? rcp_pesos { get; set; }
        public string? rcp_uer { get; set; }
        public string? rcp_uevlr { get; set; }
    }
}
