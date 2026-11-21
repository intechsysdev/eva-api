using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rvisitaE
    {
        [JsonPropertyName("KEY_rvis_cmpy")]
        [MaxLength(2), Required]
        public string? rvis_cmpy { get; set; }
        [JsonPropertyName("KEY_rvis_consec")]
        public string? rvis_consec { get; set; }
        [MaxLength(15)]
        public string? rvis_cust { get; set; }
        [MaxLength(15)]
        public string? rvis_succli { get; set; }
        [JsonPropertyName("KEY_rvis_codusu")]
        [MaxLength(15)]
        public string? rvis_codusu { get; set; }
        [MaxLength(10)]
        public string? rvis_coddep { get; set; }
        [MaxLength(20)]
        public string? rvis_coddepaso { get; set; }
        [MaxLength(15)]
        public string? rvis_codcon { get; set; }
        public string? rvis_codmedvis { get; set; }
        public string? rvis_codmotncom { get; set; }
        public string? rvis_codmotvis { get; set; }
        public string? rvis_codaccaseg { get; set; }
        [MaxLength(5)]
        public string? rvis_coddoc { get; set; }
        public string? rvis_consdoc { get; set; }
        public string? rvis_fecvis { get; set; }
        [MaxLength(10)]
        public string? rvis_horinivis { get; set; }
        [MaxLength(10)]
        public string? rvis_horfinvis { get; set; }
        public string? rvis_fecprovis { get; set; }
        [MaxLength(10)]
        public string? rvis_horprovis { get; set; }
        //[MaxLength(60)]
        public string? rvis_obsvis { get; set; }
        [MaxLength(20)]
        public string? rvis_latitud { get; set; }
        [MaxLength(20)]
        public string? rvis_longitud { get; set; }
        public string? rvis_enter { get; set; }

        public string? rvis_date { get; set; }
        public string? rvis_time { get; set; }
        public string? rvis_estadoupd { get; set; }


    }
}
