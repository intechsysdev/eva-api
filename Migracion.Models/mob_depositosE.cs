using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_depositosE
    {//
        [MaxLength(2), Required, JsonPropertyName("KEY_cm_cmpy")]
        public string? cm_cmpy { get; set; }
        [MaxLength(10), Required, JsonPropertyName("KEY_cm_cust")]
        public string? cm_cust { get; set; }
        [MaxLength(40), Required]
        public string? cm_name { get; set; }
        //[MaxLength(10)]
        //public string? cm_enter { get; set; }
        //public DateTime cm_date { get; set; }
        //[MaxLength(10)]
        //public string? cm_time { get; set; }
        //[MaxLength(1)]
        //public string? cm_estadoupd { get; set; }
    }
}
