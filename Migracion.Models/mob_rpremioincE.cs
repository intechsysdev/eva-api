using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rpremioincE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rpreinc_cmpy { get; set; }
        [MaxLength(5)]
        public string? rpreinc_codpre { get; set; }
        [MaxLength(5)]
        public string? rpreinc_codinc { get; set; }
        [MaxLength(5)]
        public string? rpreinc_codtipincpre { get; set; }
        [MaxLength(80)]
        public string? rpreinc_despre { get; set; }
    }
}
