using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rinvenobseE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rinvobs_cmpy { get; set; }
        [MaxLength(10)]
        public string? rinvobs_codusu { get; set; }
        [MaxLength(21)]
        public string? rinvobs_codproobs { get; set; }
        public decimal rinvobs_cantobs { get; set; }
    }
}
