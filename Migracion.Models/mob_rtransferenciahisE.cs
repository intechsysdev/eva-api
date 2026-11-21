using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Migracion.Models
{
    public class mob_rtransferenciahisE
    {
        [MaxLength(15), Required, JsonPropertyName("KEY_rctt_cust")]
        public string? rctt_cust { get; set; }
        [MaxLength(15), Required, JsonPropertyName("KEY_rctt_succli")]
        public string? rctt_succli { get; set; }
        [MaxLength(2), Required, JsonPropertyName("KEY_rtrans_tipo")]
        public string? rtrans_tipo { get; set; }
        [MaxLength(15), Required, JsonPropertyName("KEY_rtrans_custdep")]
        public string? rtrans_custdep { get; set; }
    }
}
