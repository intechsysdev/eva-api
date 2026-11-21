using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Migracion.Api.Models
{
    public class FilterClienteDto
    {
        public string[] regiones { get; set; }
        public string[] ciudades { get; set; }
        public string[] usuarios { get; set; }
        public string[] canales { get; set; }
        public string? cmpy { get; set; }
    }
}
