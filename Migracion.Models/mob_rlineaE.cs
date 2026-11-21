using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rlineaE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rln_cmpy")]
        public string? rln_cmpy { get; set; }
        [MaxLength(4), JsonPropertyName("KEY_rln_linea")]
        public string? rln_linea { get; set; }
        [MaxLength(40)]
        public string? rln_descr { get; set; }
    }
}
