using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_accionesE
    {
        [JsonPropertyName("KEY_cmpy")]
        public string? cmpy { get; set; }
        
        [JsonPropertyName("KEY_acctip")]
        public string? acctip { get; set; }

        public string? accdesc { get; set; }
        public string? acc_estadoupd { get; set; }

    }
}
