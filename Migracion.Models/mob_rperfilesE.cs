using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rperfilesE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rper_cmpy")]
        public string? rper_cmpy { get; set; }
        [MaxLength(10), JsonPropertyName("KEY_rper_codper")]
        public string? rper_codper { get; set; }
        [MaxLength(40)]
        public string? rper_desper { get; set; }
        public string? rper_estadoupd { get; set; }
        public string? rper_date { get; set; }
        public string? rper_time { get; set; }

    }
}
