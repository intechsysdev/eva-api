using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class mob_rexhibicionE
    {
		[Key]
		public string? rexh_key { get; set; }
		public string? rexh_cmpy { get; set; }
		public int rexh_idstatus { get; set; }
		public string? rexh_iduser { get; set; }
		public string? rexh_codper { get; set; }
		public string? rexh_latitud { get; set; }
		public string? rexh_longitud { get; set; }
		public string? rexh_codusu { get; set; }
		public int rexh_idclase { get; set; }
		public int rexh_idtipo { get; set; }
		public string? rexh_otrotipo { get; set; }
		public string? rexh_cust { get; set; }
		public string? rexh_succli { get; set; }
		public string? rexh_linea { get; set; }
		public string? rexh_subl { get; set; }
		public int rexh_idcategoria { get; set; }
		public string? rexh_otracategoria { get; set; }
		public string? rexh_unasolavez { get; set; }
		public string? rexh_tipofoto { get; set; }
		public string? rexh_observacion { get; set; }
		public string? rexh_premiodeseado { get; set; }
		public string? rexh_createbyemail { get; set; }
		public string? rexh_createbydevice { get; set; }
		public string? rexh_updatebyuser { get; set; }
		public DateTime rexh_dateupdate { get; set; }
		public DateTime rexh_date { get; set; }
		public string? rexh_time { get; set; }
		public string? rexh_note { get; set; }
	}
}
