using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rlistpricE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rpri_cmpy { get; set; }
        [MaxLength(2)]
        public string? rpri_lista { get; set; }
    }
}
