using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rrcheckposE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? cph_cmpy { get; set; }
        public string? cph_num { get; set; }
        [MaxLength(20)]
        public string? cph_tele { get; set; }
        [MaxLength(10)]
        public string? cph_cust { get; set; }
        [MaxLength(10)]
        public string? cph_ship { get; set; }
        public string? cph_entdt { get; set; }
        public decimal cph_amt { get; set; }
        [MaxLength(20)]
        public string? cph_card { get; set; }
        [MaxLength(6)]
        public string? cph_crdt { get; set; }
        //[MaxLength(60)]
        public string? cph_com { get; set; }
        public string? cph_consigna { get; set; }
        [MaxLength(5)]
        public string? cph_reg { get; set; }
        [MaxLength(5)]
        public string? cph_teri { get; set; }
        [MaxLength(1)]
        public string? cph_marca { get; set; }
        public string? cph_date { get; set; }
        [MaxLength(10)]
        public string? cph_time { get; set; }
        //public int estado_envio { get; set; }
    }
}
