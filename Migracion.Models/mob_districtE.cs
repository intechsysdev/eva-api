using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_districtE
    {//
        [MaxLength(2), Required, JsonPropertyName("KEY_dis_cmpy")]
        public string? dis_cmpy { get; set; }
        [MaxLength(10), Required, JsonPropertyName("KEY_dis_dist")]
        public string? dis_dist { get; set; }
        [MaxLength(40), Required]
        public string? dis_desc { get; set; }
        //[MaxLength(10), Required]
        //public string? dis_enter { get; set; }
        //public DateTime dis_date { get; set; }
        //[MaxLength(10)]
        //public string? dis_time { get; set; }
        //[MaxLength(1)]
        //public string? dis_estadoupd { get; set; }
    }
}
