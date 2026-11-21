using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Command
{
    public class mob_rtransdetCommand
    {
        [MaxLength(2), Required, JsonPropertyName("KEY_rtransd_cmpy")]
        public string? rtransd_cmpy { get; set; }
        [JsonPropertyName("KEY_rtransd_num")]
        public string? rtransd_num { get; set; }
        [MaxLength(2)]
        [JsonPropertyName("KEY_rtransd_tipo")]
        public string? rtransd_tipo { get; set; }
        [MaxLength(20)]
        public string? rtransd_codprodep { get; set; }
        [JsonPropertyName("KEY_rtransd_id")]
        public string? rtransd_id { get; set; }
        [JsonPropertyName("KEY_rtransd_codusu")]
        public string? rtransd_codusu { get; set; }
        [MaxLength(4)]
        public string? rtransd_linea { get; set; }
        public decimal rtransd_cant { get; set; }
        public decimal rtransd_vlrparcial { get; set; }
        public decimal rtransd_dctogral { get; set; }
        public decimal rtransd_vlrtotaliva { get; set; }
        public decimal rtransd_vlrtotal { get; set; }
        public string? rtransd_enter { get; set; }
        public string? rtransd_date { get; set; }
        public string? rtransd_time { get; set; }
        public string? rtransd_estadoupd { get; set; }
    }
}
