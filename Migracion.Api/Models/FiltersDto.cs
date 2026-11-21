using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Migracion.Api.Models
{
    public class FiltersDto
    {
        public string? fecIni { get; set; }
        public string? fecFin { get; set; }
        public string[] Territorios { get; set; }
        public string[] Regiones { get; set; }
        public string[] Perfiles { get; set; }
        public string[] Vendedores { get; set; }
        public int duracionVisitaDesde { get; set; }
        public int duracionVisitaHasta { get; set; }
        public string? horaDesde { get; set; }
        public string? horaHasta { get; set; }
        public int cantVisitaDesde { get; set; }
        public int cantVisitaHasta { get; set; }
    }
}
