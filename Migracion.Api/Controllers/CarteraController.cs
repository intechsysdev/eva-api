using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Models.Dtos;
using Migracion.Persistence.DataBase;
using Migracion.Service.Queries;
using Migracion.Service.Services;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using static QuestPDF.Helpers.Colors;

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CarteraController : ControllerBase
    {
        private readonly ICarteraQueries _carteraQueries;
        private readonly IWhatsappHelper _whatsappHelper;
        private readonly IEmailHelper _emailHelper;

        public CarteraController(ICarteraQueries carteraQueries, IWhatsappHelper whatsappHelper, IEmailHelper emailHelper)
        {
            _carteraQueries = carteraQueries;
            _whatsappHelper = whatsappHelper;
            _emailHelper = emailHelper;
        }


        // Clasificación del documento, con el mismo criterio que usa GenerarReciboCajaPdf.
        // De ella salen el prefijo del nombre del archivo y la etiqueta que viaja como
        // parámetro del template de WhatsApp y en el asunto del correo. csh_event_type y
        // csh_cash_received son opcionales (la integración de Perú no los envía) y en ese
        // caso el tipo se deduce de csh_tipo_abono y csh_referencia.
        private static TipoDocumentoRecaudo GetTipoDocumento(mob_rcashrecp_payment payment) =>
            DocumentoRecaudo.Determinar(
                payment.csh_num2,
                payment.csh_event_type,
                payment.csh_cash_received,
                payment.csh_tipo_abono,
                payment.csh_referencia);

        [HttpGet]
        [Route("GetList/{cmpy}/{codusu}")]
        public async Task<List<mob_carteraE>> GetList(string cmpy, string codusu)
        {
            return await _carteraQueries.GetList(cmpy, codusu);

        }


        [AllowAnonymous]
        [HttpPost]
        [Route("GenerateCashRecp")]
        public async Task<ApiResponse<string>> GenerateCashRecp(mob_rcashrecp_payment payment)
        {
            try
            {
                int result = _carteraQueries.InsertCash(payment);

                // Comparación tolerante: el tipo de abono y el estado los arma cada
                // integración y llegan con espacios de relleno o distinta capitalización,
                // así que una comparación exacta dejaba pagos sin notificar.
                bool esEfectivo = string.Equals(payment.csh_tipo_abono?.Trim(), "EFECTIVO", StringComparison.OrdinalIgnoreCase);
                bool estaRechazada = string.Equals(payment.csh_transaccion_estado?.Trim(), "RECHAZADA", StringComparison.OrdinalIgnoreCase);

                if (esEfectivo && !estaRechazada)
                {
                    if (result >= 1)
                    {
                        var (pdf_result, numRecibo, nomCliente, cliente_email, vendedorNombre, vendedorEmail, total, fecha, cod_usu) = _carteraQueries.GenerarReciboCajaPdf(payment);

                        // Convert PDF bytes to base64 string
                        string pdfBase64 = Convert.ToBase64String(pdf_result);

                        // Generate filename for the PDF based on document type
                        var tipoDocumento = GetTipoDocumento(payment);
                        string filenamePrefix = DocumentoRecaudo.Prefijo(tipoDocumento);
                        string etiquetaDocumento = DocumentoRecaudo.Etiqueta(tipoDocumento);
                        string filename = $"{filenamePrefix}_{payment.csh_cmpy}_{payment.csh_transaccion_numero}_{DateTime.Now:yyyyMMddHHmmss}.pdf";


                        if (numRecibo != string.Empty)
                        {
                            // Send via WhatsApp
                            await _whatsappHelper.WamediaIdAsync(
                                payment.csh_cmpy,
                                payment.csh_phone,
                                pdfBase64,
                                filename,
                                numRecibo.Trim(),
                                nomCliente.Trim(),
                                vendedorNombre.Trim(),
                                total,
                                fecha,
                                payment.config,
                                etiquetaDocumento
                            );

                            await _emailHelper.EmailLogAsync(
                                cod_usu,
                                cliente_email,
                                vendedorEmail,
                                numRecibo.Trim(),
                                pdfBase64,
                                filename,
                                payment.config,
                                etiquetaDocumento
                            );
                        }


                    }


                }


                return ApiResponse<string>.Success(result.ToString());
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Error(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("UpdateCashRecp")]
        public async Task<ApiResponse<string>> UpdateCashRecp(mob_rcashrecp_payment_updated payment)
        {
            try
            {
                int result = _carteraQueries.UpdateEstadoYMarca(payment);

                if (result >= 1)
                {
                    // Get the payment data to regenerate the PDF
                    var paymentData = _carteraQueries.GetPaymentByTransaccion(payment.csh_cmpy, payment.csh_transaccion_numero);

                    if (paymentData != null)
                    {
                        // Update the state in the payment data for PDF generation
                        paymentData.csh_transaccion_estado = payment.csh_transaccion_estado;
                        paymentData.config = payment.config;
                        // Generate the final PDF
                        var (pdf_result, numRecibo, nomCliente, cliente_email, vendedorNombre, vendedorEmail, total, fecha, cod_usu) = _carteraQueries.GenerarReciboCajaPdf(paymentData);

                        // Convert PDF bytes to base64 string
                        string pdfBase64 = Convert.ToBase64String(pdf_result);

                        // Generate filename for the PDF based on document type
                        var tipoDocumento = GetTipoDocumento(paymentData);
                        string filenamePrefix = DocumentoRecaudo.Prefijo(tipoDocumento);
                        string etiquetaDocumento = DocumentoRecaudo.Etiqueta(tipoDocumento);
                        string filename = $"{filenamePrefix}_{payment.csh_cmpy}_{numRecibo}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                        // Send via WhatsApp. No se descarta aquí por config vacía: el helper
                        // deja en el log cuál parámetro faltó, que es lo que antes no se veía.
                        if (!string.IsNullOrEmpty(paymentData.csh_phone))
                        {

                            if (numRecibo != string.Empty)
                            {
                                await _whatsappHelper.WamediaIdAsync(
                                payment.csh_cmpy,
                                paymentData.csh_phone,
                                pdfBase64,
                                filename,
                                numRecibo,
                                nomCliente,
                                vendedorNombre,
                                total,
                                fecha,
                                payment.config,
                                etiquetaDocumento
                            );

                                await _emailHelper.EmailLogAsync(
                                    cod_usu,
                                cliente_email,
                                vendedorEmail,
                                numRecibo.Trim(),
                                pdfBase64,
                                filename,
                                payment.config,
                                etiquetaDocumento
                            );

                            }


                        }
                    }
                }


                return ApiResponse<string>.Success(result.ToString());
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Error(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("UpdatePaymentsFromTransfers")]
        public async Task<ApiResponse<string>> UpdatePaymentsFromTransfers([FromBody] Dictionary<string, string> config)
        {
            try
            {
                var updatedPayments = _carteraQueries.UpdatePaymentsFromTransfers();

                if (updatedPayments.Count == 0)
                {
                    return ApiResponse<string>. Success("0", "No payments found to update");
                }

                int successCount = 0;
                var errors = new List<string>();

                foreach (var payment in updatedPayments)
                {
                    try
                    {
                        payment.config = config;

                        // Generate the PDF
                        var (pdf_result, numRecibo, nomCliente, cliente_email, vendedorNombre, vendedorEmail, total, fecha, cod_usu) = _carteraQueries.GenerarReciboCajaPdf(payment);

                        // Convert PDF bytes to base64 string
                        string pdfBase64 = Convert.ToBase64String(pdf_result);

                        // Generate filename for the PDF based on document type
                        var tipoDocumento = GetTipoDocumento(payment);
                        string filenamePrefix = DocumentoRecaudo.Prefijo(tipoDocumento);
                        string etiquetaDocumento = DocumentoRecaudo.Etiqueta(tipoDocumento);
                        string filename = $"{filenamePrefix}_{payment.csh_cmpy}_{payment.csh_num2}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                        // Send via WhatsApp if phone is available
                        if (!string.IsNullOrEmpty(payment.csh_phone))
                        {

                            if (numRecibo != string.Empty)
                            {
                                await _whatsappHelper.WamediaIdAsync(
                                                               payment.csh_cmpy,
                                                               payment.csh_phone,
                                                               pdfBase64,
                                                               filename,
                                                               numRecibo?.Trim(),
                                                               nomCliente?.Trim(),
                                                               vendedorNombre?.Trim(),
                                                               total,
                                                               fecha,
                                                               config,
                                                               etiquetaDocumento
                                                           );

                                await _emailHelper.EmailLogAsync(
                                    cod_usu,
                                cliente_email,
                                vendedorEmail,
                                numRecibo.Trim(),
                                pdfBase64,
                                filename,
                                payment.config,
                                etiquetaDocumento
                            );

                            }


                        }

                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error processing payment {payment.csh_transaccion_numero}: {ex.Message}");
                    }
                }

                return ApiResponse<string>.Success("1");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Error(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetPaymentsByUser/{cmpy}/{reg}/{teri}/{dias}")]
        public async Task<List<mob_rcashrecp_payment>> GetPaymentsByUser(string cmpy, string reg, string teri, int dias)
        {
            return await _carteraQueries.GetPaymentByUser(cmpy, reg, teri, dias);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetCustomerInvoicesWompy/{cmpy}/{customerCode}")]
        public async Task<ActionResult<List<CustomerInvoiceWompyDto>>> GetCustomerInvoicesWompy(string cmpy, string customerCode)
        {
            // La compañía decide qué procedure se ejecuta, así que sin ella no se puede resolver
            // la consulta: se responde 400 con el motivo en vez de un 500 opaco.
            if (string.IsNullOrWhiteSpace(cmpy))
            {
                return BadRequest("El parámetro de compañía (cmpy) es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(customerCode))
            {
                return BadRequest("El parámetro de cliente (customerCode) es obligatorio.");
            }

            return await _carteraQueries.GetCustomerInvoicesWompy(cmpy, customerCode);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetPaymentsByCust/{cmpy}/{reg}/{teri}/{cust}/{dias}")]
        public async Task<List<mob_rcashrecp_payment>> GetPaymentsByCust(string cmpy, string reg, string teri, string cust, int dias)
        {
            return await _carteraQueries.GetPaymentByCust(cmpy, reg, teri, cust, dias);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("CheckPaymentExists/{cmpy}/{transaccionNumero}/{inv}")]
        public ApiResponse<bool> CheckPaymentExists(string cmpy, string transaccionNumero, int inv)
        {
            try
            {
                bool exists = _carteraQueries.CheckPaymentExists(cmpy, transaccionNumero, inv);
                return ApiResponse<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Error(ex.Message);
            }
        }
    }
}
