using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rcheqinventariosE
    {
        [JsonPropertyName("KEY_rcinv_cmpy")]
        [MaxLength(2),Required]
        public string? rcinv_cmpy { get; set; }
        [JsonPropertyName("KEY_rcinv_consec")]
        [Required]
        public int rcinv_consec { get; set; }

        [JsonPropertyName("KEY_rcinv_codusu")]
        [MaxLength(10),Required]
        public string? rcinv_codusu { get; set; }
        [MaxLength(15),Required]
        public string? rcinv_cust { get; set; }
        [MaxLength(15),Required]
        public string? rcinv_succli { get; set; }
        //[MaxLength(15),Required]
        public string? rcinv_custdep { get; set; }
        public string? rcinv_feccheq { get; set; }
        //[MaxLength(10),Required]
        public string? rcinv_horaini { get; set; }
        public string? rcinv_horafin { get; set; }
        public string? rcinv_observacion { get; set; }
        //[MaxLength(1),Required]
        public string? rcinv_marca { get; set; }
        //[Required]
        public string? rcinv_date { get; set; }
        public string? rcinv_time { get; set; }
        /// <summary>
        /// TODO:Este campo no esta en la base de datos
        /// </summary>
        //public string? rcinv_num2 { get; set; }
        //public int estado_envio { get; set; }
    }
}
