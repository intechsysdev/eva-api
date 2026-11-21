using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Command
{
    public class mob_rchequeoCommand
    {
        [MaxLength(2), Required]
        public string? rche_cmpy { get; set; }
        [Required]
        public string? rche_consec { get; set; }
        [MaxLength(15), Required]
        public string? rche_cust { get; set; }
        [MaxLength(15), Required]
        public string? rche_succli { get; set; }
        [MaxLength(15), Required]
        public string? rche_codusu { get; set; }
        //[MaxLength(20), Required]
        public string? rche_custdep { get; set; }
        public string? rche_fecche { get; set; }
        //[MaxLength(10), Required]
        public string? rche_horiniche { get; set; }
        public string? rche_obsche { get; set; }
        //[MaxLength(1), Required]
        public string? rche_marca { get; set; }
        public string? rche_horfinche { get; set; }
        public string? rche_date { get; set; }
        public string? rche_time { get; set; }
    }
}
