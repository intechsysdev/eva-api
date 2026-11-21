using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_novedadpreciosE
    {
        [JsonPropertyName("KEY_novp_cmpy")]
        [MaxLength(2), Required]
        public string? novp_cmpy { get; set; }

        [JsonPropertyName("KEY_novp_codigo")]
        [MaxLength(10), Required]
        public string? novp_codigo { get; set; }
        //[MaxLength(60), Required]
        public string? novp_descripcion { get; set; }
        //[MaxLength(10), Required]
        //public string? novp_enter { get; set; }
        //[Required]
        //public DateTime novp_date { get; set; }
        //[MaxLength(10), Required]
        //public string? novp_time { get; set; }
    }
}
