using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Dtos
{
    public class mob_transferencia_full_dto
    {
        public mob_rtransferencia_hisE item { get; set; }
        public List<mob_rtransdet_hisE> detail { get; set; }
    }
}
