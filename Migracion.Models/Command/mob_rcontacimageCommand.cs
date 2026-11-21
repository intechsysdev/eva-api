using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Command
{
    public class mob_rcontacimageCommand
    {
        [MaxLength(2), Required]
        public string? rcont_cmpy { get; set; }
        [MaxLength(15), Required]
        public string? rcont_codcon { get; set; }
        [MaxLength(2), Required]
        public string? rcont_codtip { get; set; }
        public string? rcont_enter { get; set; }
        public string? rcont_date { get; set; }
        public string? rcont_time { get; set; }
        public string? rcont_estadoupd { get; set; }

    }
}
