using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rsubmenuE
    {
        [MaxLength(2), Required, JsonPropertyName("KEY_rsubm_cmpy")]
        public string? rsubm_cmpy { get; set; }
        [MaxLength(2), JsonPropertyName("KEY_rsubm_codmen")]
        public string? rsubm_codmen { get; set; }
        [MaxLength(2), JsonPropertyName("KEY_rsubm_codsubmen")]
        public string? rsubm_codsubmen { get; set; }
        [MaxLength(50)]
        public string? rsubm_dessubmen { get; set; }
        public string? rsubm_cabecera { get; set; }
        [MaxLength(80)]
        public string? rsubm_aplicacion { get; set; }
        public DateTime rsubm_date { get; set; }
        public string? rsubm_time { get; set; }
        public string? rsubm_estadoupd { get; set; }
        public string? rsubm_icono_movil { get; set; }
    }
}
