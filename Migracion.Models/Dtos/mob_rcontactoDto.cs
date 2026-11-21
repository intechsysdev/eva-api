using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Migracion.Models.Dtos
{
    public class mob_rcontactoDto
    {
        public string? rcont_cmpy { get; set; }
        public string? rcont_codcon { get; set; }
        public string? rcont_codtip { get; set; }
        public string? rcont_imagen { get; set; }
        public string? rcont_date { get; set; }
        public string? rcont_time { get; set; }
        public string? rcont_estadoupd { get; set; }
    }
}
