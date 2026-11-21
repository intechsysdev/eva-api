using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rcashrecpE
    {
        /// <summary>
        /// TODO: Esta tabla no esta en el motor de base de datos de pruebas
        /// </summary>
        [MaxLength(2)]
        public string? csh_cmpy { get; set; }
        public string? csh_num { get; set; }
        [MaxLength(20)]
        public string? csh_tele { get; set; }
        [MaxLength(15)]
        public string? csh_cust { get; set; }
       
        public string? csh_entdt { get; set; }
        public string csh_amt { get; set; }
        [MaxLength(20)]
        public string? csh_card { get; set; }
        [MaxLength(4)]
        public string? csh_crdt { get; set; }
      
        //[MaxLength(60)]
        public string? csh_com { get; set; }
        [MaxLength(1)]
        public string? csh_conclnte { get; set; }
        public string? csh_confecha { get; set; }
        public decimal csh_descto { get; set; }
        [MaxLength(5)]
        public string? csh_reg { get; set; }
        [MaxLength(5)]
        public string? csh_teri { get; set; }
        public int csh_num2 { get; set; }
        [MaxLength(1)]
        public string? csh_marca { get; set; }
        public string? csh_date { get; set; }
        [MaxLength(10)]
        public string? csh_time { get; set; }
        
    }
}
