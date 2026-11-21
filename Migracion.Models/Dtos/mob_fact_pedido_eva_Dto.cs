using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Dtos
{
    public class mob_fact_pedido_eva_Dto
    {
        public string? orh_cmpy { get; set; }
        public string? orh_reg { get; set; }
        public string? orh_teri { get; set; }
        public string? fecha { get; set; }
        public string? num_pedido_eva { get; set; }
        public string? cliente_cod { get; set; }
        public string? nombre_cliente { get; set; }
        public string? marca { get; set; }
        public int ped_ibes { get; set; }
        public int fact_ibes { get; set; }
    }
}
