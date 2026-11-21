using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_robsequiosE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? robseq_cmpy { get; set; }
        [MaxLength(21)]
        public string? robseq_codproobs { get; set; }
        //[MaxLength(60)]
        public string? robseq_desproobs { get; set; }
    }
}
