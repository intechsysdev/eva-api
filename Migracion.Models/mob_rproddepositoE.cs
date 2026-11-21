using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rproddepositoE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rprodep_cmpy")]
        public string? rprodep_cmpy { get; set; }
        [MaxLength(15), JsonPropertyName("KEY_rprodep_plu")]
        public string? rprodep_plu { get; set; }
        [MaxLength(10), JsonPropertyName("KEY_rprodep_custdep")]
        public string? rprodep_custdep { get; set; }
        [MaxLength(21), JsonPropertyName("KEY_rprodep_id")]
        public string? rprodep_id { get; set; }
        public string? rprodep_precio { get; set; }
        public string? rprodep_preciopvp { get; set; }
        public string? rprodep_iva { get; set; }
        public string? rprodep_pordcto { get; set; }
        public DateTime rprodep_date { get; set; }
        public string? rprodep_time { get; set; }
        public string? rprodep_estadoupd { get; set; }
    }
}
