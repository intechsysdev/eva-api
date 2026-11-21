using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models.Dtos
{
    public class mob_rcustmainSimp
    {
        [MaxLength(2), Required, JsonPropertyName("KEY_rctt_cmpy")]
        public string? rctt_cmpy { get; set; }
        [MaxLength(5), Required, JsonPropertyName("KEY_rctt_cust")]
        public string? rctt_cust { get; set; }
        [MaxLength(5), Required, JsonPropertyName("KEY_rctt_succli")]
        public string? rctt_succli { get; set; }
    }
}
