using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rbancosE
    {
        [MaxLength(2), Required, JsonPropertyName("KEY_rbn_cmpy")]
        public string? rbn_cmpy { get; set; }
        [MaxLength(4), Required, JsonPropertyName("KEY_rbn_crdt")]
        public string? rbn_crdt { get; set; }
        [MaxLength(20)]
        public string? rbn_descrip { get; set; }
    }
}
