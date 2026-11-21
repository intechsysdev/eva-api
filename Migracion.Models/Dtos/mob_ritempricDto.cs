using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Migracion.Models.Dtos
{
    public class mob_ritempricDto
    {
       [JsonPropertyName("KEY_rpri_cmpy")]
        public string? rpri_cmpy { get; set; }
       
       [JsonPropertyName("KEY_rpri_lista")]
        public string? rpri_lista { get; set; }

    }
}
