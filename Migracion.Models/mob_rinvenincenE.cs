using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rinvenincenE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rinvinc_cmpy { get; set; }
        [MaxLength(5)]
        public string? rinvinc_codinc { get; set; }
        public DateTime rinvinc_fecent { get; set; }
        [MaxLength(10)]
        public string? rinvinc_codusu { get; set; }
        public decimal rinvinc_cantidad { get; set; }
        public decimal rinvinc_valor { get; set; }
    }
}
