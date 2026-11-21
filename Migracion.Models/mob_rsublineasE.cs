using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rsublineasE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rsb_cmpy")]
        public string? rsb_cmpy { get; set; }
        [MaxLength(4), JsonPropertyName("KEY_rsb_linea")]
        public string? rsb_linea { get; set; }
        [MaxLength(6), JsonPropertyName("KEY_rsb_subl")]
        public string? rsb_subl { get; set; }
        [MaxLength(40)]
        public string? rsb_descr { get; set; }
    }
}
