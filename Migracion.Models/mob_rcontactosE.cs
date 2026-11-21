using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rcontactosE
    {
        [MaxLength(2),Required, JsonPropertyName("KEY_rcont_cmpy")]
        public string? rcont_cmpy { get; set; }
        [MaxLength(15),Required, JsonPropertyName("KEY_rcont_codcon")]
        public string? rcont_codcon { get; set; }
        [MaxLength(2),Required, JsonPropertyName("KEY_rcont_tipdoc")]
        public string? rcont_tipdoc { get; set; }
        public string? rcont_numdoc { get; set; }
        public string? rcont_nom1 { get; set; }
        public string? rcont_nom2 { get; set; }
        public string? rcont_ape1 { get; set; }
        public string? rcont_ape2 { get; set; }
        public string? rcont_feccum { get; set; }
        public string? rcont_correo { get; set; }
        public string? rcont_celular { get; set; }
        public string? rcont_date { get; set; }
        public string? rcont_time { get; set; }
        public string? rcont_estadoupd { get; set; }
    }
}
