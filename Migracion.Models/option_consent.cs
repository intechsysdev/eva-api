using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class option_consent
    {
        public int woc_id { get; set; }
        [MaxLength(10), Required]
        public string? woc_cmpy { get; set; }
        [MaxLength(10), Required(AllowEmptyStrings = true)]
        public string? woc_reg { get; set; }
        [MaxLength(20), Required(AllowEmptyStrings = true)]
        public string? woc_cust { get; set; }
        [MaxLength(15)]
        public string? woc_succli { get; set; }
        [MaxLength(20), Required]
        public string? woc_celular { get; set; }
        [MaxLength(1)]
        public string? woc_status { get; set; }
        [MaxLength(30), Required]
        public string? woc_channel { get; set; }
        [MaxLength(50)]
        public string? woc_source_app { get; set; }
        [MaxLength(20)]
        public string? woc_source_version { get; set; }
        public long? woc_ref_waevalog { get; set; }
        [MaxLength(45)]
        public string? woc_ip_address { get; set; }
        [MaxLength(200)]
        public string? woc_device_info { get; set; }
        [MaxLength(4000), Required]
        public string? woc_consent_text { get; set; }
        [MaxLength(10), Required]
        public string? woc_consent_version { get; set; }
        public DateTime? woc_optin_dt { get; set; }
        public DateTime? woc_optout_dt { get; set; }
        [MaxLength(20)]
        public string? woc_codusu { get; set; }
        public DateTime? woc_entdt { get; set; }
        public DateTime? woc_updt { get; set; }
    }
}
