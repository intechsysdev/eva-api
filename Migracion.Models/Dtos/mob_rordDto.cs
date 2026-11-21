using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Dtos
{
    public class mob_rordDto
    {
        public mob_rordheadE encabezado { get; set; }
        public List<mob_rordlineE> detalle { get; set; }
    }
}
