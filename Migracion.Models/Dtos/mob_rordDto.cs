using Migracion.Models.Bonificados;
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

        /// <summary>
        /// Opcional: bonificados aplicados en la app. Las versiones que no lo
        /// mandan siguen funcionando igual. Se registra y revalida después de
        /// guardar el pedido (ver PedidosQueries.InsertFull).
        /// </summary>
        public BonPedidoDto? bonificacion { get; set; }
    }
}
