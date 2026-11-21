using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Dtos
{
    public class mob_pedido_full_Dto
    {
        public mob_rordhead_hisE encabezado { get; set; }
        public List<mob_rordline_hisE> list_detail { get; set; }
    }
}
