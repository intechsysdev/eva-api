using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Migracion.Models
{
    public class mob_cupocompromisoE
    {
        [MaxLength(2), Required]
        public string? mcc_cmpy { get; set; }
        [MaxLength(10), Required]
        public string? mcc_reg { get; set; }
        [MaxLength(10), Required]
        public string? mcc_teri { get; set; }
        [Required]
        public string? mcc_consecutivo { get; set; }
        [MaxLength(2)]
        public string? mcc_tiposolicitud { get; set; }
        [MaxLength(2)]
        public string? mcc_compromisopago { get; set; }
        [MaxLength(15)]
        public string? mcc_cust { get; set; }
        [MaxLength(15)]
        public string? mcc_ship { get; set; }
        public string? mcc_fechaeva { get; set; }
        public string? mcc_horaeva { get; set; }
        public decimal mcc_valorcompromiso { get; set; }
        public string? mcc_fechacompromiso { get; set; }
        public decimal mcc_valornotacredito { get; set; }
        public string? mcc_fechanotacredito { get; set; }
        [MaxLength(200)]
        public string? mcc_observacioncompromisonc { get; set; }
        [MaxLength(1)]
        public string? mcc_tipocupo { get; set; }
        public decimal mcc_valorcupo { get; set; }
        //[MaxLength(200)]
        public string? mcc_observacioncupo { get; set; }
        [MaxLength(1)]
        public string? mcc_marca { get; set; }
        public string? mcc_fechaibes { get; set; }
        //public string? mcc_horaibes { get; set; }
        //public string? mcc_enter { get; set; }

        // Campos de resultado provenientes de la vista view_mob_cupocompromiso
        // (gestión de Cartera en IBES). Solo se poblan al consultar via GetByClient;
        // no forman parte de la tabla base mob_cupocompromiso.
        [MaxLength(1)]
        public string? mcc_estado_compromiso { get; set; }
        public string? mcc_fecha_apro_compromiso { get; set; }
        public string? mcc_obs_cartera_compromiso { get; set; }
        [MaxLength(1)]
        public string? mcc_estado_cupo { get; set; }
        public string? mcc_fecha_apro_cupo { get; set; }
        public decimal mcc_valor_cupo_aprobado { get; set; }
        public string? mcc_obs_cartera_cupo { get; set; }
        public string? mcc_estado_final { get; set; }
    }
}
