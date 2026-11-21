using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_carteraE
    {

        [MaxLength(2), Required, JsonPropertyName("KEY_car_cmpy")]
        public string? car_cmpy { get; set; }
        [MaxLength(10), JsonPropertyName("KEY_car_codusu")]
        public string? car_codusu { get; set; }
        [MaxLength(2), Required, JsonPropertyName("KEY_car_tipo")]
        public string? car_tipo { get; set; }
        [Required, JsonPropertyName("KEY_car_numero")]
        public string? car_numero { get; set; }
        [MaxLength(15)]
        public string? car_cust { get; set; }
        [MaxLength(15)]
        public string? car_succli { get; set; }
        [MaxLength(20)]
        public string? car_guia { get; set; }
        public string? car_fecha { get; set; }
        public string? car_diasven { get; set; }
        public string? car_amt { get; set; }
        public string? car_saldo { get; set; }
        public string? car_notacredito { get; set; }
        [MaxLength(1)]
        public string? car_aplicanc { get; set; }
        public string? car_fechaven { get; set; }
        public string? car_porciva { get; set; }

    }
}
