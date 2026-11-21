using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class mob_rcashrecp_paymentE
    {
        public string csh_cmpy { get; set; }                  // CHAR(2) NOT NULL
        public string csh_transaccion_numero { get; set; }    // CHAR(50) NOT NULL
        public string csh_transaccion_estado { get; set; }    // CHAR(50) NOT NULL
        public string csh_tipo_abono { get; set; }            // CHAR(50) NOT NULL
        public string csh_cust { get; set; }                  // CHAR(15) NOT NULL
        public DateTime csh_entdt { get; set; }               // DATE NOT NULL
        public decimal csh_amt { get; set; }                  // DECIMAL(16,4) NOT NULL
        public int csh_inv { get; set; }                      // INTEGER NOT NULL
        public decimal csh_descto { get; set; }               // DECIMAL(16,2) NOT NULL
        public string csh_reg { get; set; }                   // CHAR(5) NOT NULL
        public string csh_teri { get; set; }                  // CHAR(5) NOT NULL
        public int csh_num2 { get; set; }                     // INTEGER NOT NULL
        public string csh_marca { get; set; }                 // CHAR(1) NOT NULL
        public DateTime csh_date { get; set; }                // DATE NOT NULL
        public string csh_time { get; set; }                  // CHAR(10) NOT NULL
    }
}
