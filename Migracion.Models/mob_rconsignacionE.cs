using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rconsignacionE
    {
        [JsonPropertyName("KEY_mcn_cmpy")]
        [MaxLength(2),Required]
        public string? mcn_cmpy { get; set; }

        [JsonPropertyName("KEY_mcn_consec")]
        [Required]
        public Int32 mcn_consec { get; set; }

        [JsonPropertyName("KEY_mcn_reg")]
        [MaxLength(5),Required]
        public string? mcn_reg { get; set; }

        [JsonPropertyName("KEY_mcn_teri")]
        [MaxLength(5),Required]
        public string? mcn_teri { get; set; }
        public string? mcn_mbanum { get; set; }
        public string? mcn_numcom { get; set; }
        public string? mcn_fecmvto { get; set; }
        public string? mcn_feccon { get; set; }
        public decimal mcn_efectivo { get; set; }
        public decimal mcn_cheque { get; set; }
        public Int16 mcn_cantcheq { get; set; }
        public string? mcn_feceva { get; set; }
        public string? mcn_horaeva { get; set; }
        public string? mcn_marca { get; set; }
        public string? mcn_date { get; set; }
        //public string? estado_envio { get; set; }
    }
}
