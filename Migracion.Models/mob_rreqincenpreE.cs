using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rreqincenpreE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rrqinpr_cmpy { get; set; }
        [MaxLength(5)]
        public string? rrqinpr_codpre { get; set; }
        [MaxLength(5)]
        public string? rrqinpr_codinc { get; set; }
        [MaxLength(80)]
        public string? rrqinpr_des { get; set; }
        public string? rrqinpr_oblig { get; set; }
        public string? rrqinpr_fotooblig { get; set; }
    }
}
