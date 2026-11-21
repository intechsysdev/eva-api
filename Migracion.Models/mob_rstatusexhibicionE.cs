using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class mob_rstatusexhibicionE
    {
		[Key]
		public int rstatusexh_id { get; set; }
		public string? rstatusexh_cmpy { get; set; }
		public string? rstatusexh_abr { get; set; }
		public string? rstatusexh_des { get; set; }
		public string? rstatusexh_image { get; set; }
		public DateTime rstatusexh_date { get; set; }
		public string? rstatusexh_time { get; set; }
		public string? rstatusexh_estadoupd { get; set; }
	}
}
