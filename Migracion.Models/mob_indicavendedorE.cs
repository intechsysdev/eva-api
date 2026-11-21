using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_indicavendedorE
    {//
        [MaxLength(2), Required, JsonPropertyName("KEY_indven_cmpy")]
        public string? indven_cmpy { get; set; }
        [MaxLength(10), Required, JsonPropertyName("KEY_indven_codusu")]
        public string? indven_codusu { get; set; }
        [MaxLength(5), Required, JsonPropertyName("KEY_indven_concepto")]
        public string? indven_concepto { get; set; }
        [MaxLength(40)]
        public string? indven_descrip { get; set; }
        [MaxLength(50)]
        public string? indven_valor { get; set; }
    }
}
