using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rordheadE
    {
        [MaxLength(2), Required, JsonPropertyName("KEY_orh_cmpy")]
        public string? orh_cmpy { get; set; }

        [MaxLength(5), JsonPropertyName("KEY_orh_reg")]
        public string? orh_reg { get; set; }

        [MaxLength(5), JsonPropertyName("KEY_orh_teri")]
        public string? orh_teri { get; set; }

        [JsonPropertyName("KEY_orh_num")]
        public string? orh_num { get; set; }
        public string? orh_entdt { get; set; }
        [MaxLength(10)]
        public string? orh_time { get; set; }
        //[MaxLength(60)]
        public string? orh_com { get; set; }
        public string? orh_shpdt { get; set; }
        [MaxLength(15)]
        public string? orh_cust { get; set; }
        [MaxLength(15)]
        public string? orh_ship { get; set; }
        [MaxLength(20)]
        public string? orh_po { get; set; }
        public int orh_num2 { get; set; }
        [MaxLength(1)]
        public string? orh_marca { get; set; }
        public string? orh_date { get; set; }
        
    }
}
