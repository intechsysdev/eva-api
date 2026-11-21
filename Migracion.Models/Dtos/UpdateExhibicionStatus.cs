using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Dtos
{
    public class UpdateExhibicionStatus
    {
        public string? cod_exhi { get; set; }
        public int rstatusexh_id { get; set; }
        public string? rexh_observacion { get; set; }
        public string? rexh_updatebyuser { get; set; }

    }
}
