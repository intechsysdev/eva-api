using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rusuariosE
    {
        [JsonPropertyName("KEY_rusu_cmpy")]
        [MaxLength(2), Required]
        public string? rusu_cmpy { get; set; }
        [JsonPropertyName("KEY_rusu_codusu")]
        [MaxLength(10)]
        public string? rusu_codusu { get; set; }
        [MaxLength(10)]
        public string? rusu_passwd { get; set; }
        [MaxLength(80)]
        public string? rusu_nombre { get; set; }
        [MaxLength(10)]
        public string? rusu_codper { get; set; }
        [MaxLength(70)]
        public string? rusu_correo { get; set; }
        [MaxLength(5)]
        public string? rusu_reg { get; set; }
        [MaxLength(5)]
        public string? rusu_teri { get; set; }
        public string? rusu_date { get; set; }
        public string? rusu_time { get; set; }
        public string? rusu_estadoupd { get; set; }
        public string? rusu_cedula { get; set; }
    }
}
