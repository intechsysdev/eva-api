using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rcontacimageE
    {
        /// <summary>
        /// TODO: Validar tamaño Byte[]
        /// </summary>
        [MaxLength(2),Required, JsonPropertyName("KEY_rcont_cmpy")]
        public string? rcont_cmpy { get; set; }
        [MaxLength(15),Required, JsonPropertyName("KEY_rcont_codcon")]
        public string? rcont_codcon { get; set; }
        [MaxLength(2),Required, JsonPropertyName("KEY_rcont_codtip")]
        public string? rcont_codtip { get; set; }
        
    }
}
