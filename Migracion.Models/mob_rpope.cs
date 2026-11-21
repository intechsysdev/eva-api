using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class mob_rpope
    {
        [Key]
        public int rpop_idpop { get; set; }
        public string? rpop_cmpy { get; set; }
        public string? rpop_iduser { get; set; }
        public string? rpop_codper { get; set; }
        public string? rpop_latitud { get; set; }
        public string? rpop_longitud { get; set; }
        public string? rpop_codusu { get; set; }
        public string? rpop_cust { get; set; }
        public string? rpop_succli { get; set; }
        public string? rpop_observacion { get; set; }
        public string? rpop_createbyemail { get; set; }
        public string? rpop_createbydevice { get; set; }
        public string? rpop_updatebyuser { get; set; }
        public DateTime rpop_date { get; set; }
        public string? rpop_time { get; set; }

    }
}
