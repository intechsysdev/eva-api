using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rfrecuenciavisitaE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rfrec_cmpy")]
        public string? rfrec_cmpy { get; set; }
        [MaxLength(2), JsonPropertyName("KEY_rfrec_codfre")]
        public string? rfrec_codfre { get; set; }
        [MaxLength(40)]
        public string? rfrec_desfre { get; set; }
        public string? rfrec_diasfre { get; set; }
        public DateTime rfrec_date { get; set; }
        public string? rfrec_time { get; set; }
        public string? rfrec_estadoupd { get; set; }
    }
}
