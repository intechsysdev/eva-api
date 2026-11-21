using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rnovproductoE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rnovpro_cmpy { get; set; }
        public decimal rnovpro_consec { get; set; }
        [MaxLength(15)]
        public string? rnovpro_cust { get; set; }
        [MaxLength(15)]
        public string? rnovpro_succli { get; set; }
        [MaxLength(10)]
        public string? rnovpro_codusu { get; set; }
        [MaxLength(21)]
        public string? rnovpro_id { get; set; }
        public DateTime rnovpro_fecnovpro { get; set; }
        [MaxLength(10)]
        public string? rnovpro_horininovpro { get; set; }
        [MaxLength(10)]
        public string? rnovpro_horfinnovpro { get; set; }
        [MaxLength(300)]
        public string? rnovpro_obsnovpro { get; set; }
        [MaxLength(100)]
        public string? rnovpro_correo { get; set; }
    }
}
