using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class mob_rtipoexhibicionE
    {
		[Key]
		public int rtipoexh_id { get; set; }
		public string? rtipoexh_cmpy { get; set; }
		public string? rtipoexh_abr { get; set; }
		public string? rtipoexh_des { get; set; }
		public string? rtipoexh_image { get; set; }
		public string? rtipoexh_updatebyuser { get; set; }
		public DateTime rtipoexh_date { get; set; }
		public string? rtipoexh_time { get; set; }
		public string? rtipoexh_estadoupd { get; set; }
	}
}
