using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rcustransE
    {
        [MaxLength(2), Required]
        public string? rctt_cmpy { get; set; }
        [MaxLength(5)]
        public string? rctt_reg { get; set; }
        [MaxLength(15), Required]
        public string? rctt_cust { get; set; }
        [MaxLength(5)]
        public string? rctt_tipdoc { get; set; }
        [MaxLength(15)]
        public string? rctt_resal { get; set; }
        [MaxLength(15), Required]
        public string? rctt_succli { get; set; }
        public string? rctt_razsoc { get; set; }
        public string? rctt_nomcli { get; set; }
        public string? rctt_addr { get; set; }
        public string? rctt_tele { get; set; }
        public string? rctt_celular { get; set; }
        public string? rctt_fax { get; set; }
        public string? rctt_dist { get; set; }
        public string? rctt_sic { get; set; }
        public string? rctt_codusu { get; set; }
        public string? rctt_ruta { get; set; }
        public string? rctt_sec { get; set; }
        public string? rctt_clinvo { get; set; }
        public string? rctt_date { get; set; }
        public string? rctt_estrato { get; set; }
        public string? rctt_latitud { get; set; }
        public string? rctt_longitud { get; set; }
        public string? rctt_frevisita { get; set; }
        public string? rctt_horsugvis { get; set; }
        public string? rctt_codabc { get; set; }
        public string? rctt_type { get; set; }
        public string? rctt_zip { get; set; }
        public string? rctt_zip4 { get; set; }
        public string? rctt_vot { get; set; }
        public string? rctt_limit { get; set; }
        public string? rctt_level { get; set; }
        public string? rctt_bal { get; set; }
        public string? rctt_cont { get; set; }
        public string? rctt_feccum { get; set; }
        public string? rctt_pedmini { get; set; }
        public string? rctt_enter { get; set; }
        public string? rctt_diasent { get; set; }
        public string? rctt_tax { get; set; }
        public string? rctt_nomcan { get; set; }
        public string? rctt_schrg { get; set; }
        public string? rctt_diavisita { get; set; }
        public Byte[]? rctt_imagen { get; set; }
        public string? rctt_urlimagen { get; set; }
        public int estado_envio { get; set; }
    }
}
