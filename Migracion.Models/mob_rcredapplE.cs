using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rcredapplE
    {
        ///TODO:No esta en BD
        [MaxLength(2)]
        public string? cda_cmpy { get; set; }
        public string? cda_inv { get; set; }
        public string? cda_num { get; set; }
        public string? cda_amt { get; set; }
        [MaxLength(10)]
        public string? cda_cust { get; set; }
        [MaxLength(10)]
        public string? cda_ship { get; set; }
        [MaxLength(5)]
        public string? cda_reg { get; set; }
        [MaxLength(5)]
        public string? cda_teri { get; set; }
        [MaxLength(1)]
        public string? cda_marca { get; set; }
        public string? cda_date { get; set; }
        [MaxLength(10)]
        public string? cda_time { get; set; }
        //public int estado_envio { get; set; }
    }
}
