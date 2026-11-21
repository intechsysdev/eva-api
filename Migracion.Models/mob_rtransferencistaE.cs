using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rtransferencistaE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rtra_cmpy { get; set; }
        [MaxLength(5)]
        public string? rtra_transf { get; set; }
        [MaxLength(80)]
        public string? rtra_nom { get; set; }
        [MaxLength(5)]
        public string? rtra_teri { get; set; }
        [MaxLength(5)]
        public string? rtra_reg { get; set; }
    }
}
