using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_motivisE
    {//
        [MaxLength(2), Required, JsonPropertyName("KEY_cmpy")]
        public string? cmpy { get; set; }
        [Required, JsonPropertyName("KEY_motcod")]
        public string? motcod { get; set; }
        //[MaxLength(60), Required]
        public string? motdes { get; set; }
        public string? mot_estadoupd { get; set; }
    }
}
