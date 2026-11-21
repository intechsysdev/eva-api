using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rtransdetE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rtransd_cmpy")]
        public string? rtransd_cmpy { get; set; }
        [JsonPropertyName("KEY_rtransd_num")]
        public string? rtransd_num { get; set; }
        [MaxLength(2), JsonPropertyName("KEY_rtransd_tipo")]
        public string? rtransd_tipo { get; set; }
        [MaxLength(20)]
        public string? rtransd_codprodep { get; set; }
        [MaxLength(21), JsonPropertyName("KEY_rtransd_id")]
        public string? rtransd_id { get; set; }
        [MaxLength(10), JsonPropertyName("KEY_rtransd_codusu")]
        public string? rtransd_codusu { get; set; }
        [MaxLength(4)]
        public string? rtransd_linea { get; set; }
        public string? rtransd_cant { get; set; }
        public string? rtransd_vlrparcial { get; set; }
        public string? rtransd_dctogral { get; set; }
        public string? rtransd_vlrtotaliva { get; set; }
        public string? rtransd_vlrtotal { get; set; }
        
    }
}
