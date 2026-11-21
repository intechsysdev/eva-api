using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rproductosE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rpr_cmpy")]
        public string? rpr_cmpy { get; set; }
        [MaxLength(21), JsonPropertyName("KEY_rpr_id")]
        public string? rpr_id { get; set; }
        [MaxLength(40)]
        public string? rpr_desc { get; set; }
        [MaxLength(4)]
        public string? rpr_linea { get; set; }
        [MaxLength(6)]
        public string? rpr_subl { get; set; }
        public string? rpr_nuevo { get; set; }
        [MaxLength(2147483647)]
        public string? rpr_componentes { get; set; }
        [MaxLength(2147483647)]
        public string? rpr_aplicacion { get; set; }
        [MaxLength(2147483647)]
        public string? rpr_mediospubli { get; set; }
        [MaxLength(2147483647)]
        public string? rpr_urlvideoclip { get; set; }
        [MaxLength(100)]
        public string? rpr_urlimagen { get; set; }
        public string? rpr_imagen { get; set; }
        public DateTime rpr_date { get; set; }
        public string? rpr_time { get; set; }
        public string? rpr_estadoupd { get; set; }
        [MaxLength(20)]
        public string? rpr_ean { get; set; }
    }
}
