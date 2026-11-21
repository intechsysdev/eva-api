using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rreqincentE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rreqinc_cmpy { get; set; }
        [MaxLength(5)]
        public string? rreqinc_codinc { get; set; }
        [MaxLength(80)]
        public string? rreqinc_des { get; set; }
        public string? rreqinc_oblig { get; set; }
        public string? rreqinc_fotooblig { get; set; }
    }
}
