using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_ritemfabriE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? ritefab_cmpy { get; set; }
        [MaxLength(21)]
        public string? ritefab_codprofab { get; set; }
        [MaxLength(20)]
        public string? ritefab_codfab { get; set; }
        [MaxLength(80)]
        public string? ritefab_desprofab { get; set; }
        [MaxLength(20)]
        public string? ritefab_eanprofab { get; set; }
        [MaxLength(4)]
        public string? ritefab_linea { get; set; }
        [MaxLength(6)]
        public string? ritefab_subl { get; set; }
        [MaxLength(8)]
        public string? ritefab_invnt { get; set; }
        [MaxLength(100)]
        public string? ritefab_urlimagen { get; set; }
        public Byte[]? ritefab_imagen { get; set; }
        public int estado_envio { get; set; }
        [MaxLength(20)]
        public string? ritefab_present { get; set; }
    }
}
