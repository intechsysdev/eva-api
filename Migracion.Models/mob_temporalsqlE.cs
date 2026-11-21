using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_temporalsqlE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(60)]
        public string? timeSQL { get; set; }
        [MaxLength(60)]
        public string? tipo { get; set; }
        [MaxLength(200)]
        public string? servicio { get; set; }
        [MaxLength(15)]
        public string? tabla { get; set; }
        [MaxLength(60)]
        public string? sql { get; set; }
        public int estado_envio { get; set; }
    }
}
