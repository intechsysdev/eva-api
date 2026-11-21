using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rtransferenciaE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// TODO: No hay API de prueba
        /// </summary>
        [MaxLength(2), JsonPropertyName("KEY_rtrans_cmpy")]
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
        public decimal rtrans_vlrparcial { get; set; }
        public decimal rtrans_dctogral { get; set; }
        public decimal rtrans_totaliva { get; set; }
        public decimal rtrans_total { get; set; }
        [MaxLength(200)]
        public string? rtrans_observacion { get; set; }
        [MaxLength(1)]
        public string? rtrans_marca { get; set; }
        public string? rtrans_latitud { get; set; }
        public string? rtrans_longitud { get; set; }
        public string? rtrans_enter { get; set; }
        public string? rtrans_date { get; set; }
        public string? rtrans_time { get; set; }
        public string? rtrans_estadoupd { get; set; }


    }
}
