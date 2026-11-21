using System;

namespace Migracion.Models.Dtos
{
    /// <summary>
    /// DTO para el resultado del stored procedure proc_get_customer_invoices_wompy
    /// Representa las facturas pendientes de un cliente para pagos Wompy
    /// </summary>
    public class CustomerInvoiceWompyDto
    {
        /// <summary>
        /// Número de factura (in_num)
        /// </summary>
        public string InNum { get; set; }

        /// <summary>
        /// Valor total de la factura (in_amt)
        /// </summary>
        public decimal ValorFactura { get; set; }

        /// <summary>
        /// Saldo pendiente de la factura (xin_mont - xin_paga)
        /// </summary>
        public decimal Saldo { get; set; }

        /// <summary>
        /// Fecha de la factura (in_date)
        /// </summary>
        public DateTime InDate { get; set; }

        /// <summary>
        /// Tipo de factura: FR (con guías de entrega) o DB (débito)
        /// </summary>
        public string InMisc { get; set; }

        /// <summary>
        /// Saldo descontando los pagos que todavía no aplica el ERP (columna
        /// saldo_disponible del procedure). Es el saldo que muestra la app. Nulo
        /// si el procedure de la compañía no la devuelve.
        /// </summary>
        public decimal? SaldoDisponible { get; set; }
    }
}
