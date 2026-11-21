using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_reclamosE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rc_cmpy { get; set; }
        public decimal rc_code { get; set; }
        //[MaxLength(60)]
        public string? rc_desc { get; set; }
    }
}
