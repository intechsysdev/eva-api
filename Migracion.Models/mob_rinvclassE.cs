using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rinvclassE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_ric_cmpy")]
        public string? ric_cmpy { get; set; }
        [MaxLength(4), JsonPropertyName("KEY_ric_linea")]
        public string? ric_linea { get; set; }
        [MaxLength(6), JsonPropertyName("KEY_ric_subl")]
        public string? ric_subl { get; set; }
        [MaxLength(8), JsonPropertyName("KEY_ric_class")]
        public string? ric_class { get; set; }
        [MaxLength(40)]
        public string? ric_descr { get; set; }
    }
}
