using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rtipoimagenE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rtipimg_cmpy")]
        public string? rtipimg_cmpy { get; set; }
        [MaxLength(2), JsonPropertyName("KEY_rtipimg_cod")]
        public string? rtipimg_cod { get; set; }
        [MaxLength(40)]
        public string? rtipimg_des { get; set; }
    }
}
