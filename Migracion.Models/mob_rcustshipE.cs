using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class mob_rcustshipE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2)]
        [JsonPropertyName("KEY_rctt_cmpy")]
        public string? rctt_cmpy { get; set; }
        [MaxLength(5), JsonPropertyName("KEY_rctt_reg")]
        public string? rctt_reg { get; set; }
        [MaxLength(15), JsonPropertyName("KEY_rctt_cust")]
        public string? rctt_cust { get; set; }
        [MaxLength(5), JsonPropertyName("KEY_rctt_tipdoc")]
        public string? rctt_tipdoc { get; set; }
        [MaxLength(15)]
        public string? rctt_resal { get; set; }
        [MaxLength(15), JsonPropertyName("KEY_rctt_succli")]
        public string? rctt_succli { get; set; }
        [MaxLength(80)]
        public string? rctt_razsoc { get; set; }
        [MaxLength(80)]
        public string? rctt_nomcli { get; set; }
        [MaxLength(80)]
        public string? rctt_addr { get; set; }
        [MaxLength(20)]
        public string? rctt_tele { get; set; }
        [MaxLength(20)]
        public string? rctt_celular { get; set; }
        [MaxLength(20)]
        public string? rctt_fax { get; set; }
        [MaxLength(10)]
        public string? rctt_dist { get; set; }
        [MaxLength(5)]
        public string? rctt_sic { get; set; }
        [MaxLength(10)]
        public string? rctt_codusu { get; set; }
        public string? rctt_ruta { get; set; }
        public string? rctt_sec { get; set; }
        [MaxLength(1)]
        public string? rctt_clinvo { get; set; }
        public string? rctt_date { get; set; }
        [MaxLength(5)]
        public string? rctt_estrato { get; set; }
        [MaxLength(20)]
        public string? rctt_latitud { get; set; }
        [MaxLength(20)]
        public string? rctt_longitud { get; set; }
        [MaxLength(4)]
        public string? rctt_frevisita { get; set; }
        [MaxLength(10)]
        public string? rctt_horsugvis { get; set; }
        [MaxLength(3)]
        public string? rctt_codabc { get; set; }
        [MaxLength(5)]
        public string? rctt_type { get; set; }
        [MaxLength(10)]
        public string? rctt_zip { get; set; }
        [MaxLength(10)]
        public string? rctt_zip4 { get; set; }
        [MaxLength(12)]
        public string? rctt_vot { get; set; }
        public string? rctt_limit { get; set; }
        [MaxLength(2)]
        public string? rctt_level { get; set; }
        public string? rctt_bal { get; set; }
        [MaxLength(40)]
        public string? rctt_cont { get; set; }
        public DateTime rctt_feccum { get; set; }
        public string? rctt_pedmini { get; set; }
        public string? rctt_diasent { get; set; }
        public string? rctt_tax { get; set; }
        [MaxLength(40)]
        public string? rctt_nomcan { get; set; }
        public string? rctt_schrg { get; set; }
        public string? rctt_diavisita { get; set; }
        [MaxLength(2147483647)]
        public Byte[]? rctt_imagen { get; set; }
        [MaxLength(200)]
        public string? rctt_urlimagen { get; set; }
        [MaxLength(80)]
        public string? rctt_mail { get; set; }
        [MaxLength(1)]
        public string? rctt_mppago { get; set; }
        public string? rctt_vpmin { get; set; }
        [MaxLength(10)]
        public string? rctt_zone { get; set; }
        public int estado_envio { get; set; }
    }
}
