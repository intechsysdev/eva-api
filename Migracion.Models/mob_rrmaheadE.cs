using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rrmaheadE
    {
        [JsonPropertyName("KEY_rmh_cmpy")]
        [MaxLength(2), Required]
        public string? rmh_cmpy { get; set; }

        [JsonPropertyName("KEY_rmh_reg")]
        [MaxLength(5)]
        public string? rmh_reg { get; set; }

        [JsonPropertyName("KEY_rmh_teri")]
        [MaxLength(5)]
        public string? rmh_teri { get; set; }

        [JsonPropertyName("KEY_rmh_num")]
        public string? rmh_num { get; set; }
        public string? rmh_entdt { get; set; }
        [MaxLength(10)]
        public string? rmh_time { get; set; }
        //[MaxLength(60)]
        public string? rmh_com { get; set; }
        [MaxLength(10)]
        public string? rmh_cust { get; set; }
        [MaxLength(10)]
        public string? rmh_ship { get; set; }
        [MaxLength(5)]
        public string? rmh_job { get; set; }
        [MaxLength(5)]
        public string? rmh_tipo { get; set; }
        [MaxLength(10)]
        public string? rmh_memo { get; set; }
        public int rmh_num2 { get; set; }
        [MaxLength(1)]
        public string? rmh_marca { get; set; }
        public string? rmh_date { get; set; }
       
    }
}
