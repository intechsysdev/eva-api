using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rconperclienteE
    {
        [MaxLength(2),Required, JsonPropertyName("KEY_rconperc_cmpy")]
        public string? rconperc_cmpy { get; set; }
        [MaxLength(15),Required, JsonPropertyName("KEY_rconperc_cus")]
        public string? rconperc_cus { get; set; }
        [MaxLength(15),Required, JsonPropertyName("KEY_rconperc_succli")]
        public string? rconperc_succli { get; set; }
        [MaxLength(15),Required, JsonPropertyName("KEY_rconperc_codcon")]
        public string? rconperc_codcon { get; set; }
        [MaxLength(2),Required, JsonPropertyName("KEY_rconperc_codper")]
        public string? rconperc_codper { get; set; }
        
    }
}
