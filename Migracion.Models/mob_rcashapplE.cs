using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rcashapplE
    {
        /// <summary>
        /// TODO: esta tabla no esta en el motor de base de datos
        /// </summary>
        [MaxLength(2), Required]
        public string? csa_cmpy { get; set; }
        public string? csa_num { get; set; }
        public string? csa_reg { get; set; }
        public string? csa_teri { get; set; }
        public string? csa_inv { get; set; }
        public string? csa_amt { get; set; }
        public string? csa_dis { get; set; }
        public string? csa_entdt { get; set; }
        [MaxLength(1)]
        public string? csa_marca { get; set; }
        public string? csa_date { get; set; }
        [MaxLength(10)]
        public string? csa_time { get; set; }
        
    }
}
