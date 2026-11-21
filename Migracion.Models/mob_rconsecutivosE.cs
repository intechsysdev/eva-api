using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rconsecutivosE
    {
        [MaxLength(2),Required, JsonPropertyName("KEY_rcons_cmpy")]
        public string? rcons_cmpy { get; set; }
        [MaxLength(10),Required, JsonPropertyName("KEY_rcons_codusu")]
        public string? rcons_codusu { get; set; }
        [MaxLength(2),Required, JsonPropertyName("KEY_rcons_coddoc")]
        public string? rcons_coddoc { get; set; }
        public string? rcons_desdoc { get; set; }
        public string? rcons_consec { get; set; }
        public string? rcons_date { get; set; }
        public string? rcons_time { get; set; }
        public string? rcons_enter { get; set; }
        ///TODO: Validar GET con este ultimo campo

    }
}
