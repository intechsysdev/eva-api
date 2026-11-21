using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_restzonaE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rez_cmpy")]
        public string? rez_cmpy { get; set; }
        [MaxLength(10), JsonPropertyName("KEY_rez_codusu")]
        public string? rez_codusu { get; set; }
        [MaxLength(21), JsonPropertyName("KEY_rez_id")]
        public string? rez_id { get; set; }
        public string? rez_urrv { get; set; }
        public string? rez_ur { get; set; }
    }
}
