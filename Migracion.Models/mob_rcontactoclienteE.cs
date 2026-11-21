using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rcontactoclienteE
    {
        [MaxLength(2),Required,JsonPropertyName("KEY_rconcli_cmpy")]
        public string? rconcli_cmpy { get; set; }
        [MaxLength(15),Required, JsonPropertyName("KEY_rconcli_cus")]
        public string? rconcli_cus { get; set; }
        [MaxLength(15),Required, JsonPropertyName("KEY_rconcli_succli")]
        public string? rconcli_succli { get; set; }
        [MaxLength(15),Required, JsonPropertyName("KEY_rconcli_codcon")]
        public string? rconcli_codcon { get; set; }
        public string? rconcli_telefono { get; set; }
        public string? rconcli_celular { get; set; }
        public string? rconcli_correo { get; set; }
        public string? rconcli_date { get; set; }
        public string? rconcli_time { get; set; }
        public string? rconcli_estadoupd { get; set; }
        public string? rcont_tipdoc { get; set; }
        public string? rcont_numdoc { get; set; }
        public string? rcont_nom1 { get; set; }
        public string? rcont_nom2 { get; set; }
        public string? rcont_ape1 { get; set; }
        public string? rcont_ape2 { get; set; }
        public string? rcont_feccum { get; set; }
        public string? rcont_correo { get; set; }
        public string? rcont_celular { get; set; }
    }
}