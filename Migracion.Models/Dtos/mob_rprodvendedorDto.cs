using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Migracion.Models.Dtos
{
    public class mob_rprodvendedorDto
    {
        [JsonPropertyName("KEY_rprodven_cmpy")]
        public string? rprodven_cmpy { get; set; }
        [JsonPropertyName("KEY_rprodven_id")]
        public string? rprodven_id { get; set; }
        [JsonPropertyName("KEY_rprodven_codusu")]
        public string? rprodven_codusu { get; set; }
        public string? rprodven_tax { get; set; }
    }
}
