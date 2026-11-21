using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rrelacreclamoE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? rrelrec_cmpy { get; set; }
        public string? rrelrec_consec { get; set; }
        [MaxLength(15)]
        public string? rrelrec_cust { get; set; }
        [MaxLength(15)]
        public string? rrelrec_succli { get; set; }
        [MaxLength(10)]
        public string? rrelrec_coddep { get; set; }
        [MaxLength(20)]
        public string? rrelrec_codcon { get; set; }
        [MaxLength(10)]
        public string? rrelrec_codusu { get; set; }
        [MaxLength(21)]
        public string? rrelrec_id { get; set; }
        public string? rrelrec_code { get; set; }
        public string? rrelrec_cant { get; set; }
        [MaxLength(21)]
        public string? rrelrec_lote { get; set; }
        [MaxLength(20)]
        public string? rrelrec_recibido { get; set; }
        [MaxLength(20)]
        public string? rrelrec_origen { get; set; }
        public DateTime rrelrec_fecrec { get; set; }
        [MaxLength(10)]
        public string? rrelrec_horinirec { get; set; }
        [MaxLength(10)]
        public string? rrelrec_horfinrec { get; set; }
        [MaxLength(300)]
        public string? rrelrec_obsrec { get; set; }
        public Byte[]? rrelrec_imgrecl { get; set; }
    }
}
