using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_indicaclienteE
    {//
        [MaxLength(2), Required, JsonPropertyName("KEY_indcust_cmpy")]
        public string? indcust_cmpy { get; set; }
        [MaxLength(10), Required, JsonPropertyName("KEY_indcust_codusu")]
        public string? indcust_codusu { get; set; }
        [MaxLength(15), Required, JsonPropertyName("KEY_indcust_cust")]
        public string? indcust_cust { get; set; }
        [MaxLength(15), Required, JsonPropertyName("KEY_indcust_succli")]
        public string? indcust_succli { get; set; }
        [MaxLength(5), Required]
        public string? indcust_concepto { get; set; }
        [MaxLength(40)]
        public string? indcust_descrip { get; set; }
        [MaxLength(50)]
        public string? indcust_valor { get; set; }
    }
}
