using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rdetentreqpremioE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rdtreqpr_cmpy { get; set; }
        public decimal rdtreqpr_consec { get; set; }
        [MaxLength(10)]
        public string? rdtreqpr_codusu { get; set; }
        [MaxLength(5)]
        public string? rdtreqpr_codpre { get; set; }
        [MaxLength(5)]
        public string? rdtreqpr_codreqpre { get; set; }
        public Byte[]? rdtreqpr_imgreqpre { get; set; }
        [MaxLength(5)]
        public string? rdtreqpr_codinc { get; set; }
    }
}
