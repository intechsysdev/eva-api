using System;
using System.ComponentModel.DataAnnotations;

namespace Migracion.Models
{
    public class mob_rperfilmenuppalE
    {
        [MaxLength(2), Required]
        public string? rpfm_cmpy { get; set; }
        [MaxLength(10), Required]
        public string? rpfm_codper { get; set; }
        [MaxLength(2), Required]
        public string? rpfm_codmen { get; set; }
        [MaxLength(2), Required]
        public string? rpfm_codsubmen { get; set; }
    }
}
