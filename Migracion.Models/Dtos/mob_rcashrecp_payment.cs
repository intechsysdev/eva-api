using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models.Dtos
{
    public class Factura
    {
        public int csh_inv { get; set; } 
        public decimal csh_amt { get; set; }
        public decimal csh_descto { get; set; }

        /// <summary>Saldo de la factura. Nulo cuando el origen no lo informa.</summary>
        public decimal? csh_saldo { get; set; }

        /// <summary>Pagos de la factura que aún no se aplican.</summary>
        public decimal? csh_pago_en_transito { get; set; }

        /// <summary>Saldo que queda disponible descontando lo que está en tránsito.</summary>
        public decimal? csh_saldo_disponible { get; set; }
    }

    public class mob_rcashrecp_payment
    {
        public string csh_cmpy { get; set; }
        public string csh_transaccion_numero { get; set; }
        public string csh_transaccion_estado { get; set; }
        public string csh_tipo_abono { get; set; }
        public string csh_cust { get; set; }
        public string csh_phone { get; set; }
        public DateTime csh_entdt { get; set; }
        public List<Factura> facturas { get; set; }
        public string csh_reg { get; set; }
        public string csh_teri { get; set; }
        public int? csh_num2 { get; set; } 
        public string csh_marca { get; set; }
        public DateTime csh_date { get; set; }
        public string csh_time { get; set; }
        public string csh_eva { get; set; }
        public string? csh_referencia { get; set; }
        public string? csh_convenio { get; set; }
        public string? csh_event_type { get; set; }
        public string? csh_cash_received { get; set; }

        public Dictionary<string,string>? config { get; set; }

    }

    public class mob_rcashrecp_payment_updated
    {
        public string csh_cmpy { get; set; }
        public string csh_transaccion_numero { get; set; }
        public string csh_transaccion_estado { get; set; }
        public int? csh_num2 { get; set; }
        public string? csh_marca { get; set; }
        public Dictionary<string, string>? config { get; set; }
    }
}
