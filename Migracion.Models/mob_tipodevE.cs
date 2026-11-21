using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class mob_tipodevE
    {
        [JsonPropertyName("KEY_cmpy")]
        public string? cmpy { get; set; }
        [JsonPropertyName("KEY_codigo")]
        public string? codigo { get; set; }
        public string? descrip { get; set; }
    }
}
