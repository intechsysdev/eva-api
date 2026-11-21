using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rdetentincenE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rdeninc_cmpy { get; set; }
        public decimal rdeninc_consec { get; set; }
        [MaxLength(10)]
        public string? rdeninc_codusu { get; set; }
        [MaxLength(5)]
        public string? rdeninc_codinc { get; set; }
        public decimal rdeninc_canent { get; set; }
    }
}
