using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Migracion.Models.Dtos
{
    public class mob_rcustshipDto
    {
        [JsonPropertyName("KEY_rctt_cmpy")]
        public string? rctt_cmpy { get; set; }
        [JsonPropertyName("KEY_rctt_reg")]
        public string? rctt_reg { get; set; }
        [JsonPropertyName("KEY_rctt_cust")]
        public string? rctt_cust { get; set; }
        [JsonPropertyName("KEY_rctt_tipdoc")]
        public string? rctt_tipdoc { get; set; }
        public string? rctt_resal { get; set; }
        [JsonPropertyName("KEY_rctt_succli")]
        public string? rctt_succli { get; set; }
        public string? rctt_razsoc { get; set; }
        public string? rctt_nomcli { get; set; }
        public string? rctt_addr { get; set; }
        public string? rctt_tele { get; set; }
        public string? rctt_celular { get; set; }
        public string? rctt_fax { get; set; }
        public string? rctt_dist { get; set; }
        public string? rctt_sic { get; set; }
        public string? rctt_codusu { get; set; }
        public decimal rctt_ruta { get; set; }
        public decimal rctt_sec { get; set; }
        public string? rctt_clinvo { get; set; }
        public string? rctt_date { get; set; }
        public string? rctt_estrato { get; set; }
        public string? rctt_latitud { get; set; }
        public string? rctt_longitud { get; set; }
        public string? rctt_frevisita { get; set; }
        public string? rctt_horsugvis { get; set; }
        public string? rctt_codabc { get; set; }
        public string? rctt_type { get; set; }
        public string? rctt_zip { get; set; }
        public string? rctt_zip4 { get; set; }
        public string? rctt_vot { get; set; }
        public decimal rctt_limit { get; set; }
        public string? rctt_level { get; set; }
        public decimal rctt_bal { get; set; }
        public string? rctt_cont { get; set; }
        public string? rctt_feccum { get; set; }
        public decimal rctt_pedmini { get; set; }
        public decimal rctt_diasent { get; set; }
        public decimal rctt_tax { get; set; }
        public string? rctt_nomcan { get; set; }
        public decimal rctt_schrg { get; set; }
        public decimal rctt_diavisita { get; set; }
        public string? rctt_imagen { get; set; }
        public string? rctt_urlimagen { get; set; }
        public string? rctt_mail { get; set; }
        public string? rctt_mppago { get; set; }
        public decimal rctt_vpmin { get; set; }
        public string? rctt_zone { get; set; }
        public string? rctt_entdt { get; set; }
        public string? rctt_time { get; set; }
        public string? rctt_estadoupd { get; set; }
    }
}
