using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Command
{
    public class mob_rcontactoclienteCommand
    {
        [MaxLength(2), Required]
        public string? rconcli_cmpy { get; set; }
        [MaxLength(15), Required]
        public string? rconcli_cus { get; set; }
        [MaxLength(15), Required]
        public string? rconcli_succli { get; set; }
        [MaxLength(15), Required]
        public string? rconcli_codcon { get; set; }
        public string? rconcli_tipdoc { get; set; }
        [MaxLength(20)]
        public string? rconcli_telefono { get; set; }
        public string? rconcli_celular { get; set; }
        public string? rconcli_correo { get; set; }
        public string? rconcli_enter { get; set; }
        public string? rconcli_date { get; set; }
        public string? rconcli_time { get; set; }
        public string? rconcli_estadoupd { get; set; }
    }
}
