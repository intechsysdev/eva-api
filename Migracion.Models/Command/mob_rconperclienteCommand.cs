using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Command
{
    public class mob_rconperclienteCommand
    {
        [MaxLength(2), Required]
        public string? rconperc_cmpy { get; set; }
        [MaxLength(15), Required]
        public string? rconperc_cus { get; set; }
        [MaxLength(15), Required]
        public string? rconperc_succli { get; set; }
        [MaxLength(15), Required]
        public string? rconperc_codcon { get; set; }
        [MaxLength(2), Required]
        public string? rconperc_codper { get; set; }
        public string? rconperc_enter { get; set; }
        public string? rconperc_date { get; set; }
        public string? rconperc_time { get; set; }
        public string? rconperc_estadoupd { get; set; }

    }
}
