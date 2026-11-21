using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rtipoincentivoE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rtipinc_cmpy { get; set; }
        [MaxLength(5)]
        public string? rtipinc_codtipincpre { get; set; }
        //[MaxLength(60)]
        public string? rtipinc_destipinc { get; set; }
    }
}
