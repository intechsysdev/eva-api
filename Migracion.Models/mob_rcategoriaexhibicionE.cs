using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class mob_rcategoriaexhibicionE
    {
		[Key]
		public int rcateexh_id { get; set; }
		public string? rcateexh_cmpy { get; set; }
		public string? rcateexh_abr { get; set; }
		public string? rcateexh_des { get; set; }
		public string? rcateexh_image { get; set; }
		public string? rcateexh_updatebyuser { get; set; }
		public DateTime rcateexh_date { get; set; }
		public string? rcateexh_time { get; set; }
		public string? rcateexh_estadoupd { get; set; }
	}
}
