using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rdetentobsE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        public string? rdentob_cmpy { get; set; }
        public decimal dentob_consec { get; set; }
        [MaxLength(10)]
        public string? dentob_codusu { get; set; }
        [MaxLength(21)]
        public string? dentob_codproobs { get; set; }
        public decimal dentob_cantobs { get; set; }
    }
}
