using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class MovimientoMaterialDto
    {
        [Key]
        public int rpop_idpop { get; set; }
        public string? rdetallepop_cmpy { get; set; }
        public int rdetallepop_idpop { get; set; }
        public int rdetallepop_idmarca { get; set; }
        public int rdetallepop_idmaterial { get; set; }
        public string? rdetallepop_idfotosnap { get; set; }
        public string? rdetallepop_idfotoeva { get; set; }
        public string? rdetallepop_image { get; set; }
        public string? rdetallepop_date { get; set; }
        public string? rdetallepop_time { get; set; }
        public string? rpop_latitud { get; set; }
        public string? rpop_longitud { get; set; }
        public string? rusu_nombre { get; set; }
        public string? rusu_reg { get; set; }
        public string? reg_desc { get; set; }
        public string? rusu_teri { get; set; }
        public string? ter_desc { get; set; }
        public string? rpop_codper { get; set; }
        public string? rper_desper { get; set; }
        public string? rpop_codusu { get; set; }
        public string? rmarcapop_marca { get; set; }
        public string? rmarcapop_activo { get; set; }
        public string? rpop_observacion { get; set; }
        public string? rpop_createbyemail { get; set; }
        public string? rpop_createbydevice { get; set; }
        public string? rpop_updatebyuser { get; set; }
        public string? rpop_date { get; set; }
        public string? rpop_time { get; set; }
        public string? rctt_razsoc { get; set; }
        public string? rctt_nomcli { get; set; }
        public string? rctt_addr { get; set; }
        public string? rctt_latitud { get; set; }
        public string? rctt_longitud { get; set; }
        public string? rmaterialpop_des { get; set; }
    }
}
