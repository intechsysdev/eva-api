using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rinvenincpreE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rininpre_cmpy { get; set; }
        [MaxLength(5)]
        public string? rrininpre_codpre { get; set; }
        [MaxLength(5)]
        public string? rininpre_codinc { get; set; }
        public DateTime rininpre_fecent { get; set; }
        [MaxLength(10)]
        public string? rininpre_codusu { get; set; }
        public decimal rininpre_canpre { get; set; }
        public decimal rininpre_valpre { get; set; }
    }
}
