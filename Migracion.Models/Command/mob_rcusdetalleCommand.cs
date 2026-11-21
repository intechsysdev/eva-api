using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Command
{
    public class mob_rcusdetalleCommand
    {
        [MaxLength(2), Required]
        public string? rcusde_cmpy { get; set; }
        [MaxLength(15)]
        public string? rcusde_cus { get; set; }
        [MaxLength(15)]
        public string? rcusde_succli { get; set; }
        [MaxLength(5)]
        public string? rcusde_coddet { get; set; }
        public string? rcusde_desdet { get; set; }
        public string? rcusde_valdet { get; set; }
        public string? rcusde_date { get; set; }
        public string? rcusde_time { get; set; }
        public string? rcusde_estadoupd { get; set; }
        public string? rcusde_enter { get; set; }

    }
}
