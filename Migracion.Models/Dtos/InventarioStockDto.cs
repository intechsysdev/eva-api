using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Dtos
{
    public class InventarioStockDto
    {
        public string? linea { get; set; }
        public string? desc_linea { get; set; }
        public string? sublinea { get; set; }
        public string? desc_sublinea { get; set; }
        public string? cod_producto { get; set; }
        public string? desc_producto { get; set; }
        public decimal total_inventario { get; set; }
        public string? estado_stock { get; set; }
        public string? disponibilidad { get; set; }
    }
}
