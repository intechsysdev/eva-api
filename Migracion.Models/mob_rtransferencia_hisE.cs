using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rtransferencia_hisE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rtrans_cmpy")]
        public string? rtrans_cmpy { get; set; }
        [JsonPropertyName("KEY_rtrans_num")]
        public string? rtrans_num { get; set; }
        [MaxLength(2)]
        public string? rtrans_tipo { get; set; }
        [MaxLength(15)]
        public string? rtrans_cus { get; set; }
        [MaxLength(15)]
        public string? rtrans_succli { get; set; }
        [MaxLength(10)]
        public string? rtrans_codusu { get; set; }
        [MaxLength(15)]
        public string? rtrans_custdep { get; set; }
        [MaxLength(20)]
        public string? rtrans_coddeposito { get; set; }
        [MaxLength(15)]
        public string? rtrans_codcon { get; set; }
        public string? rtrans_fectrans { get; set; }
        [MaxLength(10)]
        public string? rtrans_horini { get; set; }
        [MaxLength(10)]
        public string? rtrans_horfin { get; set; }
        [MaxLength(1)]
        public string? rtrans_estenvio { get; set; }
        public string? rtrans_fecenvio { get; set; }
        public string? rtrans_vlrparcial { get; set; }
        public string? rtrans_dctogral { get; set; }
        public string? rtrans_totaliva { get; set; }
        public string? rtrans_total { get; set; }
        [MaxLength(200)]
        public string? rtrans_observacion { get; set; }
        public string? rtrans_marca { get; set; }
    }
}
