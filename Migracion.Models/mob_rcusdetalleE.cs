using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rcusdetalleE
    {
        [MaxLength(2),Required,JsonPropertyName("KEY_rcusde_cmpy")]
        public string? rcusde_cmpy { get; set; }
        [MaxLength(15), JsonPropertyName("KEY_rcusde_cus")]
        public string? rcusde_cus { get; set; }
        [MaxLength(15), JsonPropertyName("KEY_rcusde_succli")]
        public string? rcusde_succli { get; set; }
        [MaxLength(5), JsonPropertyName("KEY_rcusde_coddet")]
        public string? rcusde_coddet { get; set; }
        public string? rcusde_desdet { get; set; }
        public string? rcusde_valdet { get; set; }
        public string? rcusde_date { get; set; }
        public string? rcusde_time { get; set; }
        public string? rcusde_estadoupd { get; set; }
    }
}
