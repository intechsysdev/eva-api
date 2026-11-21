using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rentreobseE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? renob_cmpy { get; set; }
        public decimal renob_consec { get; set; }
        [MaxLength(15)]
        public string? renob_cust { get; set; }
        [MaxLength(15)]
        public string? renob_succli { get; set; }
        [MaxLength(10)]
        public string? renob_coddep { get; set; }
        [MaxLength(20)]
        public string? renob_coddepaso { get; set; }
        [MaxLength(15)]
        public string? renob_codcon { get; set; }
        [MaxLength(10)]
        public string? renob_codusu { get; set; }
        public DateTime renob_fecobs { get; set; }
        [MaxLength(10)]
        public string? renob_horiniobs { get; set; }
        [MaxLength(10)]
        public string? renob_horfinobs { get; set; }
        [MaxLength(80)]
        public string? renob_obsobs { get; set; }
        public Byte[]? renob_fircon { get; set; }
    }
}
