using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Dtos
{
    public class mob_rvisitSummaryDto
    {
        public string? rvis_cmpy { get; set; }
        public string? rusu_nombre { get; set; }
        public string? rusu_codper { get; set; }
        public string? rper_desper { get; set; }
        public string? rusu_reg { get; set; }
        public string? rusu_teri { get; set; }
        public string? rvis_fecvis { get; set; }
        public string? minrvis_horinivis { get; set; }
        public string? maxrvis_horinivis { get; set; }
        public string? rvis_enter { get; set; }
        /// <summary>
        /// Clientes distintos visitados en el dia. Antes contaba filas de
        /// mob_rvisita, que es una por movimiento, no por visita: el numero no
        /// cuadraba con las filas de Efectuadas.
        /// </summary>
        public string? cantvisitas { get; set; }
        /// <summary>
        /// Movimientos registrados en el dia (lo que cantvisitas traia antes).
        /// Se conserva porque el dato sigue siendo util junto al de visitas.
        /// </summary>
        public string? cantmovimientos { get; set; }
        public string? tiempoVis { get; set; }
        public string? reg_desc { get; set; }
        public string? ter_desc { get; set; }
    }
}
