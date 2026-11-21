using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class mob_rmaterialpopE
    {
        [Key]
        public int rmaterialpop_idmaterial { get; set; }
        public string? rmaterialpop_cmpy { get; set; }
        public string? rmaterialpop_des { get; set; }
        public string? rmaterialpop_image { get; set; }
        public string? rmarcapop_activo { get; set; }
        public string? rmaterialpop_updatebyuser { get; set; }
        public DateTime rmaterialpop_date { get; set; }
        public string? rmaterialpop_time { get; set; }
        public string? rmaterialpop_estadoupd { get; set; }
    }
}
