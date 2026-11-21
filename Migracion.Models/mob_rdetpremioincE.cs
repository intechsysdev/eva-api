using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rdetpremioincE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rdtprinc_cmpy { get; set; }
        public decimal rdtprinc_consec { get; set; }
        [MaxLength(10)]
        public string? rdtprinc_codusu { get; set; }
        [MaxLength(5)]
        public string? rdtprinc_codinc { get; set; }
        [MaxLength(5)]
        public string? rdtprinc_codpre { get; set; }
        public decimal rdtprinc_vlrpre { get; set; }
        public decimal rdtprinc_cntpre { get; set; }
    }
}
