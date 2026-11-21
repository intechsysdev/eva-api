using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rrmalineE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rll_cmpy { get; set; }
        [MaxLength(5)]
        public string? rll_reg { get; set; }
        [MaxLength(5)]
        public string? rll_teri { get; set; }
        public string? rll_num { get; set; }
        public string? rll_entdt { get; set; }
        [MaxLength(10)]
        public string? rll_time { get; set; }
        [MaxLength(21)]
        public string? rll_part { get; set; }
        public decimal rll_qord { get; set; }
       
        [MaxLength(1)]
        public string? rll_txbl { get; set; }
        [MaxLength(1)]
        public string? rll_level { get; set; }
        [MaxLength(1)]
        public string? rll_marca { get; set; }
        public string? rll_date { get; set; }
       
    }
}
