using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rperfilcontactoE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rpcont_cmpy")]
        public string? rpcont_cmpy { get; set; }
        [MaxLength(2), JsonPropertyName("KEY_rpcont_codper")]
        public string? rpcont_codper { get; set; }
        [MaxLength(50)]
        public string? rpcont_desper { get; set; }
        public string? rpcont_date { get; set; }
        public string? rpcont_time { get; set; }
        public string? rpcont_estadoupd { get; set; }
    }
}
