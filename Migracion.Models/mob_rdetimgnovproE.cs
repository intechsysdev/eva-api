using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rdetimgnovproE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rdtimgpr_cmpy { get; set; }
        public decimal rdtimgpr_consec { get; set; }
        public decimal rdtimgpr_conimg { get; set; }
        public Byte[]? rdtimgpr_imagen { get; set; }
    }
}
