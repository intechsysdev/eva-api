using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rfabricanteE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rfab_cmpy")]
        public string? rfab_cmpy { get; set; }
        [MaxLength(20), JsonPropertyName("KEY_rfab_codfab")]
        public string? rfab_codfab { get; set; }
        [MaxLength(80)]
        public string? rfab_desfab { get; set; }
    }
}
