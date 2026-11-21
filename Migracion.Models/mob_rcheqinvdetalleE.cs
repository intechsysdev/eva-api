using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rcheqinvdetalleE
    {
        [MaxLength(2),Required]
        public string? rcinvdet_cmpy { get; set; }
        [Required]
        public int rcinvdet_consec { get; set; }
        [Required]
        public string? rcinvdet_codusu { get; set; }
        [Required]
        public string? rcinvdet_id { get; set; }
        public int rcinvdet_inventlineal { get; set; }
        public int rcinvdet_inventbodega { get; set; }
        public string? rcinvdet_observacion { get; set; }
        public string? rcinvdet_marca { get; set; }
        public string? rcinvdet_date { get; set; }
        public string? rcinvdet_time { get; set; }
        /// <summary>
        /// TODO: Este campo no existe en la base de datos
        /// TODO: Validar los nuevos campos date y time en el get
        /// </summary>
        //public string? estado_envio { get; set; }
    }
}
