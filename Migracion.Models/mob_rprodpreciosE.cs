using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_rprodpreciosE
    {
        [JsonPropertyName("KEY_rpropre_cmpy")]
        [MaxLength(2), Required]
        public string? rpropre_cmpy { get; set; }

        [JsonPropertyName("KEY_rpropre_codfab")]
        [MaxLength(20)]
        public string? rpropre_codfab { get; set; }
        [JsonPropertyName("KEY_rpropre_codigo")]
        [MaxLength(21)]
        public string? rpropre_codigo { get; set; }
        [MaxLength(20)]
        public string? rpropre_eanprofab { get; set; }
        [MaxLength(80)]
        public string? rpropre_desc { get; set; }
        [MaxLength(10)]
        public string? rpropre_marca { get; set; }
        [MaxLength(10)]
        public string? rpropre_mercado { get; set; }
        [MaxLength(10)]
        public string? rpropre_submercado { get; set; }
        [MaxLength(200)]
        public string? rpropre_urlimagen { get; set; }
        public Byte[]? rpropre_imagen { get; set; }
        [MaxLength(20)]
        public string? rpropre_present { get; set; }
        public string? rpropre_porcprecinf { get; set; }
        public string? rpropre_porcprecsup { get; set; }
        public string? rpropre_precant { get; set; }
        public string? rpropre_precact { get; set; }
        public string? rpropre_porcestrategia { get; set; }
        [MaxLength(1)]
        public string? rpropre_estado { get; set; }
        [MaxLength(10)]
        public string? rpropre_codusu { get; set; }
        [MaxLength(200)]
        public string? rpropre_observacion { get; set; }
        //public string? estado_envio { get; set; }
    }
}
