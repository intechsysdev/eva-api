using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class mob_rclasexhibicionE
    {
		[Key]
		public int rclasexh_id { get; set; }
		public string? rclasexh_cmpy { get; set; }
		public string? rclasexh_abr { get; set; }
		public string? rclasexh_des { get; set; }
		public string? rclasexh_image { get; set; }
		public string? rclasexh_updatebyuser { get; set; }
		public DateTime rclasexh_date { get; set; }
		public string? rclasexh_time { get; set; }
		public string? rclasexh_estadoupd { get; set; }
	}
}
