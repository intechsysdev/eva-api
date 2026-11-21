using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class mob_rmarcaspopE
    {
        [Key]
        public int rmarcapop_idmarca { get; set; }
        public string? rmarcapop_cmpy { get; set; }
        public string? rmarcapop_linea { get; set; }
        public string? rmarcapop_subl { get; set; }
        public string? rmarcapop_marca { get; set; }
        public string? rmarcapop_activo { get; set; }
        public string? rmarcapop_updatebyuser { get; set; }
        public DateTime rmarcapop_date { get; set; }
        public string? rmarcapop_time { get; set; }
        public string? rmarcapop_estadoupd { get; set; }

    }
}
