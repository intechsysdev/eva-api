using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class mob_rclasetipoeexhibicionE
    {
		public string? rclastipoexh_cmpy { get; set; }
		public int rclastipoexh_idclase { get; set; }
		public int rclastipoexh_idtipo { get; set; }
		public string? rclastipoexh_updatebyuser { get; set; }
		public DateTime rclastipoexh_date { get; set; }
		public string? rclastipoexh_time { get; set; }
		public string? rclastipoexh_estadoupd { get; set; }
	}

	public class mob_rclasetipoeexhibicionDto
	{
		public int rclastipoexh_idclase { get; set; }
		public int rclastipoexh_idtipo { get; set; }
        public string? rclasexh_des { get; set; }
        public string? rtipoexh_des { get; set; }
    }
}
