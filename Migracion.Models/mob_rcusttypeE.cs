using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rcusttypeE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_cu_cmpy")]
        public string? cu_cmpy { get; set; }
        [MaxLength(3), JsonPropertyName("KEY_cu_type")]
        public string? cu_type { get; set; }
        [MaxLength(40)]
        public string? cu_desc { get; set; }
        public string? cu_pedminnuevo { get; set; }
    }
}
