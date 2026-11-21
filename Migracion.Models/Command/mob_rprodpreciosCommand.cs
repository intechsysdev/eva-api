using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Command
{
    public class mob_rprodpreciosCommand
    {
        /// <summary>
        /// TODO: Validar ultimos campos
        /// </summary>
        [MaxLength(2), Required]
        public string? rpropre_cmpy { get; set; }
        [MaxLength(20)]
        public string? rpropre_codfab { get; set; }
        [MaxLength(21)]
        public string? rpropre_codigo { get; set; }
        [MaxLength(80)]
        public string? rpropre_desc { get; set; }
        public string? rpropre_eanprofab { get; set; }
        public string? rpropre_marca { get; set; }
        public string? rpropre_mercado { get; set; }
        public string? rpropre_submercado { get; set; }
        public string? rpropre_urlimagen { get; set; }
        public string? rpropre_present { get; set; }
        public decimal rpropre_porcprecinf { get; set; }
        public decimal rpropre_porcprecsup { get; set; }
        public decimal rpropre_precant { get; set; }
        public decimal rpropre_precact { get; set; }
        public decimal rpropre_porcestrategia { get; set; }
        public string? rpropre_codusu { get; set; }
        public string? rpropre_observacion { get; set; }
        public string? rpropre_estado { get; set; }
        public string? rpropre_nuevo { get; set; }
        public string? rpropre_feccreacion { get; set; }
        public string? rpropre_enter { get; set; }
        public string? rpropre_date { get; set; }
        public string? rpropre_time { get; set; }
        //public string? estado_envio { get; set; }
    }
}
