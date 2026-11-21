using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rdetentreqincentivoE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rdtreqinc_cmpy { get; set; }
        public decimal rdtreqinc_consec { get; set; }
        [MaxLength(10)]
        public string? rdtreqinc_codusu { get; set; }
        [MaxLength(5)]
        public string? rdtreqinc_codinc { get; set; }
        [MaxLength(5)]
        public string? rdtreqinc_codreqinc { get; set; }
        public Byte[]? rdtreqinc_imgreqinc { get; set; }
    }
}
