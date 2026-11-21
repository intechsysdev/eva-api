using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rinsentivosE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rinsen_cmpy { get; set; }
        [MaxLength(5)]
        public string? rinsen_codinc { get; set; }
        [MaxLength(5)]
        public string? rinsen_codtipinc { get; set; }
        //[MaxLength(60)]
        public string? rinsen_desinc { get; set; }
        public string? rinsen_reqseg { get; set; }
        public string? rinsen_reqpre { get; set; }
        public decimal rinsen_valpre { get; set; }
    }
}
