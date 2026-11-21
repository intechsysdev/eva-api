using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_mdiasE
    {
        //
        [MaxLength(2), Required]
        public string? rdia_cmpy { get; set; }
        [Required]
        public decimal rdia_cod { get; set; }
        [MaxLength(20), Required]
        public string? rdia_des { get; set; }
        //[MaxLength(10), Required]
        //public string? rdia_enter { get; set; }
        //[Required]
        //public DateTime rdia_date { get; set; }
        //[MaxLength(10), Required]
        //public string? rdia_time { get; set; }
        //[MaxLength(1)]
        //public string? rdia_estadoupd { get; set; }
        //[MaxLength(2147483647)]
        //public Byte[]? imagen { get; set; }
        ///TODO:Que pasa con este ultimo campo
    }
}
