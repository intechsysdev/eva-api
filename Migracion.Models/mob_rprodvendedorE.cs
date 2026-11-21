using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rprodvendedorE
    {
        [JsonPropertyName("KEY_rpr_cmpy")]
        public string? rpr_cmpy { get; set; }
        [JsonPropertyName("KEY_rpr_id")]
        public string? rpr_id { get; set; }
        public string? rpr_desc { get; set; }
        public string? rpr_linea { get; set; }
        public string? rpr_subl { get; set; }
        public string? rpr_nuevo { get; set; }
        public string? rpr_componentes { get; set; }
        public string? rpr_aplicacion { get; set; }
        public string? rpr_mediospubli { get; set; }
        public string? rpr_urlvideoclip { get; set; }
        public string? rpr_urlimagen { get; set; }
        public string? rpr_imagen { get; set; }
        public string? rpr_date { get; set; }
        public string? rpr_time { get; set; }
        public string? rpr_estadoupd { get; set; }
        public string? rpr_ean { get; set; }
    }
}
