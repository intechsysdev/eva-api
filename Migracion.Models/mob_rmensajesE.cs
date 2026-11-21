using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rmensajesE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required]
        public string? mens_cmpy { get; set; }
        [MaxLength(12)]
        public string? mens_consec { get; set; }
        [MaxLength(15)]
        public string? mens_codusu { get; set; }
        [MaxLength(10)]
        public string? mens_tipo { get; set; }
        [MaxLength(10)]
        public string? mens_mercado { get; set; }
        [MaxLength(10)]
        public string? mens_submercado { get; set; }
        [MaxLength(15)]
        public string? mens_cust { get; set; }
        [MaxLength(15)]
        public string? mens_succli { get; set; }
        [MaxLength(20)]
        public string? mens_coddep { get; set; }
        public string? mens_fecha { get; set; }
        [MaxLength(10)]
        public string? mens_hora { get; set; }
        [MaxLength(255)]
        public string? mens_mensaje { get; set; }
        [MaxLength(300)]
        public string? mens_urlimagen { get; set; }
        //public Byte[]? mens_imagen { get; set; }
        //public int estado_envio { get; set; }
    }
}