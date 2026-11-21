using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class mob_rexhibicionfotoE
    {
		[Key]
		public string? rfotoexh_keyfoto { get; set; }
		public string? rfotoexh_cmpy { get; set; }
		public string? rfotoexh_key { get; set; }
		public string? rfotoexh_tipofoto { get; set; }
		public string? rfotoexh_idsnap { get; set; }
		public string? rfotoexh_ideva { get; set; }
		public string? rfotoexh_image { get; set; }
        public string? rfotoexh_cust { get; set; }
        public string? rfotoexh_succli { get; set; }
        public Nullable<DateTime> rfotoexh_date { get; set; }
        public string? rfotoexh_time { get; set; }
        public int rfotoexh_idclase { get; set; }
        public int rfotoexh_idtipo { get; set; }
        public string? rfotoexh_linea { get; set; }
        public string? rfotoexh_subl { get; set; }

    }
}
