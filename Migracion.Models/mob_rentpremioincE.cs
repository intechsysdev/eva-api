using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rentpremioincE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rentpreinc_cmpy { get; set; }
        public decimal rentpreinc_consec { get; set; }
        [MaxLength(15)]
        public string? rentpreinc_cust { get; set; }
        [MaxLength(15)]
        public string? rentpreinc_succli { get; set; }
        [MaxLength(10)]
        public string? rentpreinc_codusu { get; set; }
        [MaxLength(10)]
        public string? rentpreinc_coddep { get; set; }
        public DateTime rentpreinc_fecent { get; set; }
        [MaxLength(10)]
        public string? rentpreinc_horinient { get; set; }
        [MaxLength(10)]
        public string? rentpreinc_horfinent { get; set; }
        [MaxLength(80)]
        public string? rentpreinc_obsent { get; set; }
        [MaxLength(15)]
        public string? rentpreinc_codcon { get; set; }
        public Byte[]? rentpreinc_firmacon { get; set; }
    }
}
