using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rmediovisitaE
    {
        /// <summary>
        /// TODO: Falta agregar los Required
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_rmedio_cmpy")]
        public string? rmedio_cmpy { get; set; }
        [JsonPropertyName("KEY_rmedio_codmedvis")]
        public string? rmedio_codmedvis { get; set; }
        //[MaxLength(60)] 
        public string? rmedio_desmedvis { get; set; }
        public DateTime rmedio_date { get; set; }
        public string? rmedio_time { get; set; }
        public string? rmedio_estadoupd { get; set; }
        /// <summary>
        /// 'S' cuando el medio exige validar la ubicación del vendedor contra la
        /// del cliente al registrar la visita (la app muestra la distancia). En la
        /// tabla es CHAR(1) NOT NULL DEFAULT 'N'.
        /// </summary>
        public string? rmedio_reqgeo { get; set; }

    }
}
