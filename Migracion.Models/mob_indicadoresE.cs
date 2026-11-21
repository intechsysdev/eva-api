using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_indicadoresE
    {//
        /// <summary>
        /// TODO: Api de prueba falla
        /// </summary>
        [MaxLength(2), Required, JsonPropertyName("KEY_indcust_cmpy")]
        public string? indcust_cmpy { get; set; }
        [MaxLength(5), Required]
        public string? indcust_codusu { get; set; }
        [JsonPropertyName("KEY_indcust_concepto")]
        public string? indcust_concepto { get; set; }
        [MaxLength(40)]
        public string? indcust_descrip { get; set; }
    }
}
