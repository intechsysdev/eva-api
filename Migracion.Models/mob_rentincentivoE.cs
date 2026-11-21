using System;
using System.ComponentModel.DataAnnotations;


namespace Migracion.Models
{
    public class mob_rentincentivoE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rentinc_cmpy { get; set; }
        public decimal rentinc_consec { get; set; }
        [MaxLength(15)]
        public string? rentinc_cust { get; set; }
        [MaxLength(15)]
        public string? rentinc_succli { get; set; }
        [MaxLength(10)]
        public string? rentinc_codusu { get; set; }
        [MaxLength(10)]
        public string? rentinc_coddep { get; set; }
        public DateTime rentinc_fecent { get; set; }
        [MaxLength(10)]
        public string? rentinc_horinient { get; set; }
        [MaxLength(10)]
        public string? rentinc_horfinent { get; set; }
        [MaxLength(80)]
        public string? rentinc_obsent { get; set; }
        [MaxLength(15)]
        public string? rentinc_codcon { get; set; }
        public Byte[]? rentinc_firmacon { get; set; }
    }
}
