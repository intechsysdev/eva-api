using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Migracion.Models;
using Migracion.Models.Dtos;
using Migracion.Persistence.DataBase;
using Migracion.Service.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Migracion.Service.Queries
{
    public interface ICarteraQueries
    {
        Task<List<mob_carteraE>> GetList(string car_cmpy, string car_codusu);
        (byte[] pdf,string numRecibo,string nomCliente, string emailCliente, string vendedorNombre,string vendedorEmail, string total,string fecha, string cod_usu) GenerarReciboCajaPdf(mob_rcashrecp_payment item);
        int InsertCash(mob_rcashrecp_payment payment);
        int UpdateEstadoYMarca(mob_rcashrecp_payment_updated updated);
        mob_rcashrecp_payment GetPaymentByTransaccion(string cmpy, string transaccionNumero);
        List<mob_rcashrecp_payment> UpdatePaymentsFromTransfers();
        Task<List<mob_rcashrecp_payment>> GetPaymentByUser(string cmpy, string reg, string teri, int dias);
        Task<List<CustomerInvoiceWompyDto>> GetCustomerInvoicesWompy(string cmpy, string customerCode);
        Task<List<mob_rcashrecp_payment>> GetPaymentByCust(string cmpy, string reg, string teri,string cust, int dias);
        bool CheckPaymentExists(string cmpy, string transaccionNumero, int inv);
    }
    public class CarteraQueries : ICarteraQueries
    {
        private readonly ApplicationDbContext _context;
        private readonly IUsuariosQueries _usuariosQueries;
        private readonly IClientesQueries _clientesQueries;
        public CarteraQueries(
          ApplicationDbContext context, IUsuariosQueries usuariosQueries, IClientesQueries clientesQueries)
        {
            _context = context;
            _usuariosQueries = usuariosQueries;
            _clientesQueries = clientesQueries;
        }

        public async Task<List<mob_carteraE>> GetList(string car_cmpy, string car_codusu)
        {
            List<mob_carteraE> lstCartera = null;
            try
            {
                

                lstCartera = await _context.mob_cartera.Where(x => x.car_cmpy == car_cmpy && x.car_codusu == car_codusu).ToListAsync();
                lstCartera.ForEach(x =>
                {
                    x.car_fecha = Convert.ToDateTime(x.car_fecha).ToString("MM/dd/yyyy");
                    x.car_fechaven = Convert.ToDateTime(x.car_fechaven).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstCartera), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstCartera = JsonSerializer.Deserialize<List<mob_carteraE>>(responseJson);

            } 
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstCartera;
        }

        public (byte[] pdf, string numRecibo, string nomCliente,string emailCliente, string vendedorNombre, string vendedorEmail, string total, string fecha, string cod_usu) GenerarReciboCajaPdf(mob_rcashrecp_payment item)
        {
            try
            {
                List<mob_rusuariosE> lstVendedores = _usuariosQueries.GetListAll(item.csh_cmpy, "2").Result;
                mob_rcustmainDto client = _clientesQueries.GetbyCust(item.csh_cmpy.Trim(), item.csh_cust.Trim()).Result;
                string vendedor_result = lstVendedores.FirstOrDefault(v => v.rusu_teri.Trim() == item.csh_teri.Trim())?.rusu_nombre.Trim() ?? "N/A";
                string vendedor_email = lstVendedores.FirstOrDefault(v => v.rusu_teri.Trim() == item.csh_teri.Trim())?.rusu_correo.Trim() ?? "N/A";
                string vendedor_cod_usu = lstVendedores.FirstOrDefault(v => v.rusu_teri.Trim() == item.csh_teri.Trim())?.rusu_codusu.Trim() ?? "N/A";
                string cliente_result = client.rctt_razsoc ?? "N/A";
                string cliente_email = client.rctt_mail ?? "N/A";

                QuestPDF.Settings.License = LicenseType.Community;

                // Obtener el nombre de la empresa desde config. La busqueda se hace
                // con ConfigHelper porque las claves llegan con espacios de relleno o
                // distinta capitalizacion segun como se cargaron las filas de cada compania.
                string empresaNombre = ConfigHelper.Valor(item.config, "EMPRESA");

                var vendedor = vendedor_result;
                var territorio = $"{item.csh_teri}";

                string reciboNumero = string.Empty;
                string observacion = string.Empty;

                // Determinar el tipo de documento. La clasificacion vive en
                // DocumentoRecaudo para que el PDF, el nombre del archivo y la etiqueta
                // que viaja en el template de WhatsApp salgan siempre del mismo criterio.
                // csh_event_type/csh_cash_received son opcionales: cuando la integracion
                // no los envia (Peru) el tipo se deduce de csh_tipo_abono y csh_referencia,
                // porque sin esa deduccion el documento salia sin titulo ni observaciones.
                var tipoDocumento = DocumentoRecaudo.Determinar(
                    item.csh_num2,
                    item.csh_event_type,
                    item.csh_cash_received,
                    item.csh_tipo_abono,
                    item.csh_referencia);

                bool esReciboCaja = tipoDocumento == TipoDocumentoRecaudo.ReciboCaja;
                bool esRecaudoEfectivo = tipoDocumento == TipoDocumentoRecaudo.RecaudoEfectivo;
                bool esReferenciaCorresponsal = tipoDocumento == TipoDocumentoRecaudo.ReferenciaCorresponsal;

                // Perú (DD) muestra el Código CIP; Colombia (RE) mantiene convenio + referencia
                bool usaCodigoCip = CompanyRules.UsaCodigoCip(item.csh_cmpy);

                // Definir colores según el tipo de documento
                // Recaudo Efectivo: Ámbar/Naranja #F59E0B
                // Referencia Corresponsal: Azul #2563EB
                // Recibo de Caja (Definitivo): Verde #16A34A
                string colorPrincipal;
                if (esReciboCaja)
                {
                    colorPrincipal = "#16A34A";
                }
                else if (esRecaudoEfectivo)
                {
                    colorPrincipal = "#F59E0B";
                }
                else if (esReferenciaCorresponsal)
                {
                    colorPrincipal = "#2563EB";
                }
                else
                {
                    colorPrincipal = "#16A34A";
                }

                // Definir títulos según el tipo de documento
                string tituloDocumento;
                string etiquetaRecibo;
                string etiquetaTotal;
                string numeroDocumento;

                if (esReciboCaja)
                {
                    tituloDocumento = "RECIBO DE CAJA";
                    etiquetaRecibo = "Recibo No.";
                    etiquetaTotal = "TOTAL PAGADO";
                    numeroDocumento = item.csh_num2?.ToString() ?? string.Empty;
                }
                else if (esRecaudoEfectivo)
                {
                    tituloDocumento = "SOPORTE DE RECAUDO EN TRÁNSITO - EFECTIVO";
                    etiquetaRecibo = "Soporte de recaudo No.";
                    etiquetaTotal = "TOTAL PAGADO";
                    numeroDocumento = item.csh_transaccion_numero;
                }
                else if (esReferenciaCorresponsal)
                {
                    tituloDocumento = usaCodigoCip
                        ? "REFERENCIA DE PAGO - CÓDIGO CIP"
                        : "REFERENCIA PARA PAGO EN CORRESPONSAL BANCARIO";
                    etiquetaRecibo = usaCodigoCip ? "Código CIP" : "Referencia";
                    etiquetaTotal = "TOTAL";
                    numeroDocumento = item.csh_referencia ?? string.Empty;
                }
                else
                {
                    tituloDocumento = "";
                    etiquetaRecibo = "";
                    etiquetaTotal = "";
                    numeroDocumento = "";
                }

                switch (item.csh_transaccion_estado)
                {
                    case "INICIADA":
                        reciboNumero = item.csh_transaccion_numero ?? string.Empty;


                        if (esReciboCaja) {
                            observacion = "Este documento certifica el valor entregado al vendedor como pago. Una vez el pago sea procesado y aplicado a su estado de cuenta, recibirá automáticamente el comprobante final. Gracias por su confianza.";
                        }
                        else if (esRecaudoEfectivo) {
                            observacion = "Este documento certifica el valor entregado como pago. Por favor, conserve este recibo como comprobante de su transacción.";
                        }
                        else if (esReferenciaCorresponsal)
                        {
                            observacion = usaCodigoCip
                                ? "Realice el pago con el código CIP indicado en este documento, a través de banca por internet, agente o agencia autorizada (BCP, BBVA, Interbank, Scotiabank, Caja Arequipa, BanBif, Kasnet, Tambo, Western Union, Diashop, entre otros). Si su factura tiene descuento, este puede cambiar si el pago se realiza después de la fecha de generación del código."
                                : "Acérquese a un corresponsal bancario de Bancolombia y realice el pago con el número de referencia indicado en este documento. Si su factura tiene descuento, este puede cambiar si el pago se realiza después de la fecha de generación de la referencia de pago.";
                        }
                        break;

                    case "COMPLETADA":
                        reciboNumero = item.csh_num2?.ToString() ?? string.Empty;
                        observacion = "Este recibo de caja certifica la aplicación del pago a las facturas correspondientes y la actualización de saldos.";
                        break;
                }

              
                var fechaHora = item.csh_entdt.ToString("dd/MM/yyyy");

                var cliente = cliente_result;

                var facturas = item.facturas
                    .Select(f => (
                        NumFactura: f.csh_inv,
                        ValorAplicado: f.csh_amt,
                        Disponible: f.csh_saldo_disponible))
                    .ToList();

                // SALDOPAGOS ("SI"/"NO") en la configuración de la compañía decide si
                // el recibo muestra el saldo de cada factura. Llega en item.config,
                // que arma Seguridad.API; sin el parámetro se omite, como antes.
                //
                // El recibo lo lee el cliente: solo se le muestra lo que le queda por
                // pagar después de este pago (csh_saldo_disponible = saldo menos lo
                // que está en tránsito, que incluye este pago). El saldo anterior y
                // el valor en tránsito son datos internos y no van en el recibo.
                bool mostrarSaldos = ConfigHelper.EsSi(item.config, "SALDOPAGOS");

                // Nulo se imprime como guion: un saldo que no llegó no es un saldo
                // en cero, y en un recibo esa diferencia la lee el cliente.
                string Monto(decimal? valor) => valor.HasValue ? CompanyRules.Moneda(item.csh_cmpy, valor.Value, item.config) : "—";

                var totalPagado = facturas.Sum(f => f.ValorAplicado);

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.Margin(40);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        page.Header().Column(headerColumn =>
                        {
                            // Para RECIBO DE CAJA normal: encabezado con fondo del color correspondiente
                            if (!esRecaudoEfectivo && !esReferenciaCorresponsal)
                            {
                                // Barra superior con nombre de empresa en fondo del color del tipo
                                headerColumn.Item().Background(colorPrincipal).Padding(10).AlignCenter()
                                    .Text(empresaNombre).FontSize(16).Bold().FontColor(Colors.White);
                                
                                // Título RECIBO DE CAJA en azul (original)
                                headerColumn.Item().PaddingTop(10).AlignCenter()
                                    .Text(tituloDocumento).FontSize(18).Bold().FontColor(Colors.Blue.Medium);
                                
                                // Número del documento
                                headerColumn.Item().PaddingTop(5).AlignCenter().Text(text =>
                                {
                                    text.Span("No. ").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                                    text.Span(reciboNumero).FontSize(14).Bold();
                                });
                                
                                headerColumn.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            }
                            else
                            {
                                // Para los otros tipos: encabezado con fondo del color correspondiente
                                headerColumn.Item().Column(column =>
                                {
                                    if (!string.IsNullOrEmpty(empresaNombre))
                                    {
                                        column.Item().Background(colorPrincipal).Padding(10).AlignCenter()
                                            .Text(empresaNombre).FontSize(16).Bold().FontColor(Colors.White);
                                        column.Item().PaddingTop(5);
                                    }
                                    
                                    column.Item().AlignCenter().Container()
                                        .Padding(8)
                                        .Text(tituloDocumento)
                                        .FontSize(14).Bold().FontColor(Colors.Black);
                                    
                                    column.Item().PaddingTop(5).AlignCenter().Text(text =>
                                    {
                                        text.Span("No. ").FontSize(14).Bold().FontColor(Colors.Black);
                                        text.Span(numeroDocumento).FontSize(14).Bold().FontColor(Colors.Black);
                                    });
                                });
                                
                                headerColumn.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            }
                        });

                        page.Content().PaddingVertical(20).Column(column =>
                        {
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text(text => { text.Span("Vendedor: ").Bold(); text.Span(vendedor); });
                                    col.Item().PaddingTop(5).Text(text => { text.Span("Territorio: ").Bold(); text.Span(territorio); });
                                });
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().AlignRight().Text(text => { text.Span("Fecha: ").Bold(); text.Span(fechaHora); });
                                });
                            });

                            column.Item().PaddingTop(15).Text(text => { text.Span("Cliente: ").Bold(); text.Span($"{item.csh_cust.Trim()} - {cliente}"); });
                            column.Item().PaddingTop(5).Text(text => { text.Span("No. Transacción:" + " ").Bold(); text.Span(item.csh_transaccion_numero); });

                            if (usaCodigoCip)
                            {
                                // Perú (DD): el Código CIP reemplaza el par convenio/referencia
                                if (!string.IsNullOrEmpty(item.csh_referencia))
                                {
                                    column.Item().PaddingTop(5).Text(text => { text.Span("Código CIP: ").Bold(); text.Span(item.csh_referencia); });
                                }
                            }
                            else
                            {
                                // Mostrar convenio si tiene valor
                                if (!string.IsNullOrEmpty(item.csh_convenio))
                                {
                                    column.Item().PaddingTop(5).Text(text => { text.Span("Convenio: ").Bold(); text.Span(item.csh_convenio); });
                                }

                                // Mostrar referencia si tiene valor
                                if (!string.IsNullOrEmpty(item.csh_referencia))
                                {
                                    column.Item().PaddingTop(5).Text(text => { text.Span("Referencia: ").Bold(); text.Span(item.csh_referencia); });
                                }
                            }


                            column.Item().PaddingTop(20).PaddingBottom(10).LineHorizontal(1);
                            column.Item().PaddingBottom(10).Text("DETALLE DE FACTURAS").FontSize(12).Bold().FontColor(Colors.Blue.Darken1);

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1.5f);
                                    if (mostrarSaldos)
                                    {
                                        columns.RelativeColumn(1.5f);
                                    }
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(colorPrincipal).Padding(8).Text("# Factura").FontColor(Colors.White).Bold();
                                    header.Cell().Background(colorPrincipal).Padding(8).AlignRight().Text("Valor Aplicado").FontColor(Colors.White).Bold();
                                    if (mostrarSaldos)
                                    {
                                        header.Cell().Background(colorPrincipal).Padding(8).AlignRight().Text("Saldo Pendiente").FontColor(Colors.White).Bold();
                                    }
                                });

                                foreach (var factura in facturas)
                                {
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(8).Text(factura.NumFactura.ToString());
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(8).AlignRight().Text(Monto(factura.ValorAplicado));
                                    if (mostrarSaldos)
                                    {
                                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(8).AlignRight().Text(Monto(factura.Disponible));
                                    }
                                }

                               
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(8).Text(etiquetaTotal).Bold();
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(8).AlignRight().Text(Monto(totalPagado)).Bold();
                                if (mostrarSaldos)
                                {
                                    // El saldo es por factura: la fila de total lo deja en blanco
                                    // para no presentar una suma como si fuera la deuda del cliente.
                                    table.Cell().Background(Colors.Grey.Lighten3);
                                }
                            });

                            if (!string.IsNullOrEmpty(observacion))
                            {
                                // Observaciones con borde redondeado y texto rojo para todos los tipos
                                column.Item().PaddingTop(20).Border(1).BorderColor(Colors.Orange.Lighten2).Padding(10).MinHeight(60)
                                    .Text(text =>
                                    {
                                        text.Span("Observaciones: ").FontColor(Colors.Red.Medium);
                                        text.Span(observacion).FontColor(Colors.Red.Medium);
                                    });
                            }
                        });

                        page.Footer().AlignCenter().DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken1)).Text(text =>
                        {
                            text.Span("Generado el: ");
                            text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).Bold();
                            text.Span(" | Página ");
                            text.CurrentPageNumber();
                            text.Span(" de ");
                            text.TotalPages();
                        });
                    });
                });

                return (document.GeneratePdf(), reciboNumero, cliente_result, cliente_email, vendedor_result, vendedor_email, totalPagado.ToString("N2"), fechaHora, vendedor_cod_usu);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar el PDF: {ex.Message}");
            }
        }

        public int InsertCash(mob_rcashrecp_payment payment)
        {
            int result = 0;
            try
            {
                var insertCommandText = @"INSERT INTO mob_rcashrecp_payment (
            csh_cmpy,
            csh_transaccion_numero,
            csh_transaccion_estado,
            csh_tipo_abono,
            csh_cust,
            csh_phone,
            csh_entdt,
            csh_amt,
            csh_inv,
            csh_descto,
            csh_saldo,
            csh_pago_en_transito,
            csh_saldo_disponible,
            csh_reg,
            csh_teri,
            csh_num2,
            csh_marca,
            csh_date,
            csh_time,
            csh_eva,
            csh_referencia,
            csh_convenio,
            csh_cash_received,
            csh_event_type
        ) VALUES (
            @csh_cmpy,
            @csh_transaccion_numero,
            @csh_transaccion_estado,
            @csh_tipo_abono,
            @csh_cust,
            @csh_phone,
            @csh_entdt,
            @csh_amt,
            @csh_inv,
            @csh_descto,
            @csh_saldo,
            @csh_pago_en_transito,
            @csh_saldo_disponible,
            @csh_reg,
            @csh_teri,
            @csh_num2,
            @csh_marca,
            @csh_date,
            @csh_time,
            @csh_eva,
            @csh_referencia,
            @csh_convenio,
            @csh_cash_received,
            @csh_event_type
        )";

                var updateCommandText = @"UPDATE mob_rcashrecp_payment SET
            csh_transaccion_estado = @csh_transaccion_estado,
            csh_tipo_abono = @csh_tipo_abono,
            csh_cust = @csh_cust,
            csh_phone = @csh_phone,
            csh_entdt = @csh_entdt,
            csh_amt = @csh_amt,
            csh_descto = @csh_descto,
            csh_saldo = @csh_saldo,
            csh_pago_en_transito = @csh_pago_en_transito,
            csh_saldo_disponible = @csh_saldo_disponible,
            csh_reg = @csh_reg,
            csh_teri = @csh_teri,
            csh_num2 = @csh_num2,
            csh_marca = @csh_marca,
            csh_date = @csh_date,
            csh_time = @csh_time,
            csh_eva = @csh_eva,
            csh_referencia = @csh_referencia,
            csh_convenio = @csh_convenio,
            csh_cash_received = @csh_cash_received,
            csh_event_type = @csh_event_type
        WHERE csh_cmpy = @csh_cmpy 
          AND csh_transaccion_numero = @csh_transaccion_numero 
          AND csh_inv = @csh_inv";

                var checkExistsCommandText = @"SELECT COUNT(*) FROM mob_rcashrecp_payment 
                    WHERE csh_cmpy = @csh_cmpy 
                      AND csh_transaccion_numero = @csh_transaccion_numero 
                      AND csh_inv = @csh_inv";

                // Si tienes varios facturas, debes hacer inserts/updates para cada una
                if (payment.facturas != null && payment.facturas.Count > 0)
                {
                    foreach (var factura in payment.facturas)
                    {
                        // Verificar si existe el registro
                        bool exists = false;
                        using (var command = _context.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = checkExistsCommandText;
                            command.Parameters.Add(new DB2Parameter("csh_cmpy", payment.csh_cmpy));
                            command.Parameters.Add(new DB2Parameter("csh_transaccion_numero", payment.csh_transaccion_numero));
                            command.Parameters.Add(new DB2Parameter("csh_inv", factura.csh_inv));

                            _context.Database.OpenConnection();
                            var countResult = command.ExecuteScalar();
                            exists = Convert.ToInt32(countResult) > 0;
                        }

                        var commandText = exists ? updateCommandText : insertCommandText;

                        result += _context.Database.ExecuteSqlRaw(
                            commandText,
                            new DB2Parameter("csh_cmpy", payment.csh_cmpy),
                            new DB2Parameter("csh_transaccion_numero", payment.csh_transaccion_numero),
                            new DB2Parameter("csh_transaccion_estado", payment.csh_transaccion_estado),
                            new DB2Parameter("csh_tipo_abono", payment.csh_tipo_abono),
                            new DB2Parameter("csh_cust", payment.csh_cust),
                            new DB2Parameter("csh_phone", payment.csh_phone),
                            new DB2Parameter("csh_entdt", payment.csh_entdt),
                            new DB2Parameter("csh_amt", factura.csh_amt),
                            new DB2Parameter("csh_inv", factura.csh_inv),
                            new DB2Parameter("csh_descto", factura.csh_descto),
                            new DB2Parameter("csh_saldo", (object)factura.csh_saldo ?? DBNull.Value),
                            new DB2Parameter("csh_pago_en_transito", (object)factura.csh_pago_en_transito ?? DBNull.Value),
                            new DB2Parameter("csh_saldo_disponible", (object)factura.csh_saldo_disponible ?? DBNull.Value),
                            new DB2Parameter("csh_reg", payment.csh_reg),
                            new DB2Parameter("csh_teri", payment.csh_teri),
                            new DB2Parameter("csh_num2", payment.csh_num2),
                            new DB2Parameter("csh_marca", payment.csh_marca),
                            new DB2Parameter("csh_date", payment.csh_date),
                            new DB2Parameter("csh_time", payment.csh_time),
                            new DB2Parameter("csh_eva", payment.csh_eva),
                            new DB2Parameter("csh_referencia", payment.csh_referencia ?? (object)DBNull.Value),
                            new DB2Parameter("csh_convenio", payment.csh_convenio ?? (object)DBNull.Value),
                            new DB2Parameter("csh_cash_received", payment.csh_cash_received ?? (object)DBNull.Value),
                            new DB2Parameter("csh_event_type", payment.csh_event_type ?? (object)DBNull.Value)
                        );
                    }
                }
                else // Si no tiene facturas, puedes insertar valores "vacíos" para csh_inv, csh_amt y csh_descto
                {
                    result = _context.Database.ExecuteSqlRaw(
                        insertCommandText,
                        new DB2Parameter("csh_cmpy", payment.csh_cmpy),
                        new DB2Parameter("csh_transaccion_numero", payment.csh_transaccion_numero),
                        new DB2Parameter("csh_transaccion_estado", payment.csh_transaccion_estado),
                        new DB2Parameter("csh_tipo_abono", payment.csh_tipo_abono),
                        new DB2Parameter("csh_cust", payment.csh_cust),
                        new DB2Parameter("csh_phone", payment.csh_phone),
                        new DB2Parameter("csh_entdt", payment.csh_entdt),
                        new DB2Parameter("csh_amt", DBNull.Value),
                        new DB2Parameter("csh_inv", DBNull.Value),
                        new DB2Parameter("csh_descto", DBNull.Value),
                        new DB2Parameter("csh_saldo", DBNull.Value),
                        new DB2Parameter("csh_pago_en_transito", DBNull.Value),
                        new DB2Parameter("csh_saldo_disponible", DBNull.Value),
                        new DB2Parameter("csh_reg", payment.csh_reg),
                        new DB2Parameter("csh_teri", payment.csh_teri),
                        new DB2Parameter("csh_num2", payment.csh_num2),
                        new DB2Parameter("csh_marca", payment.csh_marca),
                        new DB2Parameter("csh_date", payment.csh_date),
                        new DB2Parameter("csh_time", payment.csh_time),
                        new DB2Parameter("csh_eva", payment.csh_eva),
                        new DB2Parameter("csh_referencia", payment.csh_referencia ?? (object)DBNull.Value),
                        new DB2Parameter("csh_convenio", payment.csh_convenio ?? (object)DBNull.Value),
                        new DB2Parameter("csh_cash_received", payment.csh_cash_received ?? (object)DBNull.Value),
                        new DB2Parameter("csh_event_type", payment.csh_event_type ?? (object)DBNull.Value)
                    );
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int UpdateEstadoYMarca(mob_rcashrecp_payment_updated updated)
        {
            int result = 0;
            int maxRetries = 3;
            int retryDelayMs = 200;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var connection = _context.Database.GetDbConnection();
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Establecer LOCK MODE WAIT para esperar hasta 5 segundos por el bloqueo
                            using (var lockCommand = connection.CreateCommand())
                            {
                                lockCommand.Transaction = transaction;
                                lockCommand.CommandText = "SET LOCK MODE TO WAIT 5";
                                lockCommand.ExecuteNonQuery();
                            }

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = @"UPDATE mob_rcashrecp_payment
                                            SET csh_transaccion_estado = @csh_transaccion_estado
                                            WHERE csh_cmpy = @csh_cmpy
                                              AND csh_transaccion_numero = @csh_transaccion_numero";
                                command.Parameters.Add(new DB2Parameter("csh_transaccion_estado", updated.csh_transaccion_estado));
                                command.Parameters.Add(new DB2Parameter("csh_cmpy", updated.csh_cmpy));
                                command.Parameters.Add(new DB2Parameter("csh_transaccion_numero", updated.csh_transaccion_numero));
                                result = command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            return result; // Éxito, salir del bucle de reintentos
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception ex) when (ex.Message.Contains("record is locked") && attempt < maxRetries - 1)
                {
                    // Esperar antes de reintentar con backoff exponencial
                    System.Threading.Thread.Sleep(retryDelayMs * (attempt + 1));
                    continue;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error actualizando estado y marca después de {attempt + 1} intentos: {ex.Message}", ex);
                }
            }

            return result;
        }



        public mob_rcashrecp_payment GetPaymentByTransaccion(string cmpy, string transaccionNumero)
        {
            try
            {
                var commandText = @"SELECT csh_cmpy, csh_transaccion_numero, csh_transaccion_estado, 
                                           csh_tipo_abono, csh_cust, csh_phone, csh_entdt, 
                                           csh_amt, csh_inv, csh_descto, 
                                           csh_saldo, csh_pago_en_transito, csh_saldo_disponible, 
                                           csh_reg, csh_teri, 
                                           csh_num2, csh_marca, csh_date, csh_time, csh_eva,csh_referencia,csh_convenio,csh_cash_received,csh_event_type
                                    FROM mob_rcashrecp_payment 
                                    WHERE csh_cmpy = @csh_cmpy 
                                      AND csh_transaccion_numero = @csh_transaccion_numero";

                mob_rcashrecp_payment payment = null;
                var facturas = new List<Factura>();

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = commandText;
                    command.Parameters.Add(new DB2Parameter("csh_cmpy", cmpy));
                    command.Parameters.Add(new DB2Parameter("csh_transaccion_numero", transaccionNumero));

                    _context.Database.OpenConnection();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (payment == null)
                            {
                                payment = new mob_rcashrecp_payment
                                {
                                    csh_cmpy = reader["csh_cmpy"]?.ToString(),
                                    csh_transaccion_numero = reader["csh_transaccion_numero"]?.ToString(),
                                    csh_transaccion_estado = reader["csh_transaccion_estado"]?.ToString(),
                                    csh_tipo_abono = reader["csh_tipo_abono"]?.ToString(),
                                    csh_cust = reader["csh_cust"]?.ToString(),
                                    csh_phone = reader["csh_phone"]?.ToString(),
                                    csh_entdt = reader["csh_entdt"] != DBNull.Value ? Convert.ToDateTime(reader["csh_entdt"]) : DateTime.MinValue,
                                    csh_reg = reader["csh_reg"]?.ToString(),
                                    csh_teri = reader["csh_teri"]?.ToString(),
                                    csh_num2 = reader["csh_num2"] != DBNull.Value ? Convert.ToInt32(reader["csh_num2"]) : null,
                                    csh_marca = reader["csh_marca"]?.ToString(),
                                    csh_date = reader["csh_date"] != DBNull.Value ? Convert.ToDateTime(reader["csh_date"]) : DateTime.MinValue,
                                    csh_time = reader["csh_time"]?.ToString(),
                                    csh_eva = reader["csh_eva"]?.ToString(),
                                    csh_referencia = reader["csh_referencia"] != DBNull.Value ? reader["csh_referencia"].ToString() : null,
                                    csh_convenio = reader["csh_convenio"] != DBNull.Value ? reader["csh_convenio"].ToString() : null,
                                    csh_cash_received = reader["csh_cash_received"] != DBNull.Value ? reader["csh_cash_received"].ToString() : null,
                                    csh_event_type = reader["csh_event_type"] != DBNull.Value ? reader["csh_event_type"].ToString() : null,
                                    facturas = new List<Factura>()
                                };
                            }

                            // Add factura details
                            if (reader["csh_inv"] != DBNull.Value)
                            {
                                facturas.Add(new Factura
                                {
                                    csh_inv = Convert.ToInt32(reader["csh_inv"]),
                                    csh_amt = reader["csh_amt"] != DBNull.Value ? Convert.ToDecimal(reader["csh_amt"]) : 0,
                                    csh_descto = reader["csh_descto"] != DBNull.Value ? Convert.ToDecimal(reader["csh_descto"]) : 0,
                                    csh_saldo = reader["csh_saldo"] != DBNull.Value ? Convert.ToDecimal(reader["csh_saldo"]) : (decimal?)null,
                                    csh_pago_en_transito = reader["csh_pago_en_transito"] != DBNull.Value ? Convert.ToDecimal(reader["csh_pago_en_transito"]) : (decimal?)null,
                                    csh_saldo_disponible = reader["csh_saldo_disponible"] != DBNull.Value ? Convert.ToDecimal(reader["csh_saldo_disponible"]) : (decimal?)null
                                });
                            }
                        }
                    }
                }

                if (payment != null)
                {
                    payment.facturas = facturas;
                }

                return payment;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el pago: {ex.Message}");
            }
        }

        public List<mob_rcashrecp_payment> UpdatePaymentsFromTransfers()
        {
            var updatedPayments = new List<mob_rcashrecp_payment>();
            int maxRetries = 3;
            int retryDelayMs = 200;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var connection = _context.Database.GetDbConnection();
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Establecer LOCK MODE WAIT para esperar hasta 5 segundos por el bloqueo
                            using (var lockCommand = connection.CreateCommand())
                            {
                                lockCommand.Transaction = transaction;
                                lockCommand.CommandText = "SET LOCK MODE TO WAIT 5";
                                lockCommand.ExecuteNonQuery();
                            }

                            // First, get the transaction numbers that will be updated
                            var selectCommandText = @"SELECT DISTINCT 
       p.csh_cmpy, 
       p.csh_transaccion_numero
FROM mob_rcashrecp_payment p
INNER JOIN control_transferencias c 
    ON c.ctr_transaccion_numero = p.csh_transaccion_numero
   AND c.ctr_factura_numero = p.csh_inv
WHERE p.csh_marca = 'N'
  AND c.ctr_pago_marca <> 0
AND p.csh_transaccion_estado <> 'RECHAZADA'";

                            var transactionsToUpdate = new List<(string cmpy, string transaccionNumero)>();

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = selectCommandText;
                                using (var reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        transactionsToUpdate.Add((
                                            reader["csh_cmpy"]?.ToString(),
                                            reader["csh_transaccion_numero"]?.ToString()
                                        ));
                                    }
                                }
                            }

                            if (transactionsToUpdate.Count == 0)
                            {
                                transaction.Commit();
                                return updatedPayments;
                            }

                            // Execute the update
                            var updateCommandText = @"UPDATE mob_rcashrecp_payment
                                          SET csh_transaccion_estado = 'COMPLETADA',
                                              csh_num2 = (SELECT ctr_pago_marca 
                                                          FROM control_transferencias 
                                                          WHERE control_transferencias.ctr_transaccion_numero = mob_rcashrecp_payment.csh_transaccion_numero 
                                                          AND ( control_transferencias.ctr_pago_marca IS NOT NULL
                                                          OR control_transferencias.ctr_pago_marca <> 0)
                                                          AND control_transferencias.ctr_factura_numero = mob_rcashrecp_payment.csh_inv
                                                            ),
                                              csh_marca = 'S'
                                          WHERE csh_marca = 'N'
                                            AND EXISTS (SELECT 1 
                                                        FROM control_transferencias 
                                                        WHERE control_transferencias.ctr_transaccion_numero = mob_rcashrecp_payment.csh_transaccion_numero 
                                                        AND ( control_transferencias.ctr_pago_marca IS NOT NULL
                                                        OR control_transferencias.ctr_pago_marca <> 0)
                                                        AND control_transferencias.ctr_factura_numero = mob_rcashrecp_payment.csh_inv
                                                        AND csh_transaccion_estado <> 'RECHAZADA'
                                                )";

                            using (var updateCommand = connection.CreateCommand())
                            {
                                updateCommand.Transaction = transaction;
                                updateCommand.CommandText = updateCommandText;
                                updateCommand.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            // Get the full payment data for each updated transaction (outside transaction)
                            foreach (var trans in transactionsToUpdate)
                            {
                                var payment = GetPaymentByTransaccion(trans.cmpy, trans.transaccionNumero);
                                if (payment != null)
                                {
                                    payment.csh_transaccion_estado = "COMPLETADA";
                                    payment.csh_marca = "S";
                                    updatedPayments.Add(payment);
                                }
                            }

                            return updatedPayments; // Éxito, salir del bucle de reintentos
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception ex) when (ex.Message.Contains("record is locked") && attempt < maxRetries - 1)
                {
                    // Esperar antes de reintentar con backoff exponencial
                    System.Threading.Thread.Sleep(retryDelayMs * (attempt + 1));
                    continue;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al actualizar pagos desde transferencias después de {attempt + 1} intentos: {ex.Message}", ex);
                }
            }

            return updatedPayments;
        }

        public async Task<List<mob_rcashrecp_payment>> GetPaymentByUser(string cmpy, string reg, string teri, int dias)
        {
            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var commandText = @"SELECT csh_cmpy, csh_transaccion_numero, csh_transaccion_estado, 
                                           csh_tipo_abono, csh_cust, csh_phone, csh_entdt, 
                                           csh_amt, csh_inv, csh_descto, 
                                           csh_saldo, csh_pago_en_transito, csh_saldo_disponible, 
                                           csh_reg, csh_teri, 
                                           csh_num2, csh_marca, csh_date, csh_time, csh_eva,csh_referencia,csh_convenio,csh_cash_received,csh_event_type
                                    FROM view_mob_pagos 
                                    WHERE csh_cmpy = @csh_cmpy 
                                      AND csh_reg = @csh_reg
                                      AND csh_teri = @csh_teri
                                      AND csh_date >= @fecha_limite
                                    ORDER BY csh_transaccion_numero, csh_entdt DESC";

                    var payments = new List<mob_rcashrecp_payment>();

                    // Calcular la fecha límite en C#
                    var fechaLimite = DateTime.Now.Date.AddDays(-dias);

                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.Transaction = transaction.GetDbTransaction();
                        command.Parameters.Add(new DB2Parameter("csh_cmpy", cmpy));
                        command.Parameters.Add(new DB2Parameter("csh_reg", reg));
                        command.Parameters.Add(new DB2Parameter("csh_teri", teri));
                        command.Parameters.Add(new DB2Parameter("fecha_limite", fechaLimite));

                        await _context.Database.OpenConnectionAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            mob_rcashrecp_payment current = null;
                            var facturas = new List<Factura>();
                            string lastTrans = null;

                            while (await reader.ReadAsync())
                            {
                                var trans = reader["csh_transaccion_numero"]?.ToString();

                                if (current == null || trans != lastTrans)
                                {
                                    if (current != null)
                                    {
                                        current.facturas = facturas;
                                        payments.Add(current);
                                        facturas = new List<Factura>();
                                    }

                                    current = new mob_rcashrecp_payment
                                    {
                                        csh_cmpy = Convert.ToString(reader["csh_cmpy"])?.Trim(),
                                        csh_transaccion_numero = Convert.ToString(reader["csh_transaccion_numero"])?.Trim(),
                                        csh_transaccion_estado = Convert.ToString(reader["csh_transaccion_estado"])?.Trim(),
                                        csh_tipo_abono = Convert.ToString(reader["csh_tipo_abono"])?.Trim(),
                                        csh_cust = Convert.ToString(reader["csh_cust"])?.Trim(),
                                        csh_phone = Convert.ToString(reader["csh_phone"])?.Trim(),
                                        csh_entdt = reader["csh_entdt"] != DBNull.Value ? Convert.ToDateTime(reader["csh_entdt"]) : DateTime.MinValue,
                                        csh_reg = Convert.ToString(reader["csh_reg"])?.Trim(),
                                        csh_teri = Convert.ToString(reader["csh_teri"])?.Trim(),
                                        csh_num2 = reader["csh_num2"] != DBNull.Value ? Convert.ToInt32(reader["csh_num2"]) : null,
                                        csh_marca = Convert.ToString(reader["csh_marca"])?.Trim(),
                                        csh_date = reader["csh_date"] != DBNull.Value ? Convert.ToDateTime(reader["csh_date"]) : DateTime.MinValue,
                                        csh_time = Convert.ToString(reader["csh_time"])?.Trim(),
                                        csh_eva = Convert.ToString(reader["csh_eva"])?.Trim(),
                                        csh_referencia = reader["csh_referencia"] != DBNull.Value ? Convert.ToString(reader["csh_referencia"])?.Trim() : null,
                                        csh_convenio = reader["csh_convenio"] != DBNull.Value ? Convert.ToString(reader["csh_convenio"])?.Trim() : null,
                                        csh_cash_received = reader["csh_cash_received"] != DBNull.Value ? Convert.ToString(reader["csh_cash_received"])?.Trim() : null,
                                        csh_event_type = reader["csh_event_type"] != DBNull.Value ? Convert.ToString(reader["csh_event_type"])?.Trim() : null,
                                        facturas = new List<Factura>()
                                    };

                                    lastTrans = trans;
                                }

                                // Add factura details
                                if (reader["csh_inv"] != DBNull.Value)
                                {
                                    facturas.Add(new Factura
                                    {
                                        csh_inv = Convert.ToInt32(reader["csh_inv"]),
                                        csh_amt = reader["csh_amt"] != DBNull.Value ? Convert.ToDecimal(reader["csh_amt"]) : 0,
                                        csh_descto = reader["csh_descto"] != DBNull.Value ? Convert.ToDecimal(reader["csh_descto"]) : 0,
                                        csh_saldo = reader["csh_saldo"] != DBNull.Value ? Convert.ToDecimal(reader["csh_saldo"]) : (decimal?)null,
                                        csh_pago_en_transito = reader["csh_pago_en_transito"] != DBNull.Value ? Convert.ToDecimal(reader["csh_pago_en_transito"]) : (decimal?)null,
                                        csh_saldo_disponible = reader["csh_saldo_disponible"] != DBNull.Value ? Convert.ToDecimal(reader["csh_saldo_disponible"]) : (decimal?)null
                                    });
                                }
                            }

                            if (current != null)
                            {
                                current.facturas = facturas;
                                payments.Add(current);
                            }
                        }
                    }
                    transaction.Commit();
                    return payments;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception($"Error al obtener los pagos por usuario: {ex.Message}");
                }
            }
        }

        public async Task<List<CustomerInvoiceWompyDto>> GetCustomerInvoicesWompy(string cmpy, string customerCode)
        {
            var invoices = new List<CustomerInvoiceWompyDto>();

            var cmpyNormalizado = CompanyRules.Normalize(cmpy);
            var customerCodeNormalizado = (customerCode ?? string.Empty).Trim();

            if (cmpyNormalizado.Length == 0)
            {
                throw new ArgumentException("No se recibió el parámetro de compañía (cmpy).", nameof(cmpy));
            }

            if (customerCodeNormalizado.Length == 0)
            {
                throw new ArgumentException("No se recibió el parámetro de cliente (customerCode).", nameof(customerCode));
            }

            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    // El procedure depende de la compañía logueada en la eva: para "RE"
                    // se usa el de Wompy (solo customer_code); para el resto, el del portal
                    // (customer_code + cmpy). Ambos devuelven los mismos campos, así que
                    // solo cambia el nombre del procedure y sus parámetros.
                    bool esRe = cmpyNormalizado == CompanyRules.RECAMIER;

                    var procedureName = esRe
                        ? "proc_get_customer_invoices_wompy"
                        : "proc_get_customer_invoices_portal";

                    // Llamar al stored procedure usando EXECUTE PROCEDURE para Informix/DB2
                    var commandText = esRe
                        ? $@"EXECUTE PROCEDURE {procedureName}(@p_customer_code)"
                        : $@"EXECUTE PROCEDURE {procedureName}(@p_customer_code, @p_company)";

                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.Transaction = transaction.GetDbTransaction();
                        command.Parameters.Add(new DB2Parameter("p_customer_code", customerCodeNormalizado));

                        if (!esRe)
                        {
                            command.Parameters.Add(new DB2Parameter("p_company", cmpyNormalizado));
                        }

                        await _context.Database.OpenConnectionAsync();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // saldo_disponible se busca por nombre y no por posición:
                            // las cinco primeras columnas se leen por índice, pero esta
                            // se agregó después y el procedure de cada compañía puede
                            // tenerla en otro lugar o no tenerla todavía.
                            int idxSaldoDisponible = -1;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (string.Equals(reader.GetName(i)?.Trim(), "saldo_disponible", StringComparison.OrdinalIgnoreCase))
                                {
                                    idxSaldoDisponible = i;
                                    break;
                                }
                            }

                            while (await reader.ReadAsync())
                            {
                                invoices.Add(new CustomerInvoiceWompyDto
                                {
                                    InNum = reader[0] != DBNull.Value ? Convert.ToString(reader[0])?.Trim() : string.Empty,
                                    ValorFactura = reader[1] != DBNull.Value ? Convert.ToDecimal(reader[1]) : 0,
                                    Saldo = reader[2] != DBNull.Value ? Convert.ToDecimal(reader[2]) : 0,
                                    InDate = reader[3] != DBNull.Value ? Convert.ToDateTime(reader[3]) : DateTime.MinValue,
                                    InMisc = reader[4] != DBNull.Value ? Convert.ToString(reader[4])?.Trim() : string.Empty,
                                    SaldoDisponible = idxSaldoDisponible >= 0 && reader[idxSaldoDisponible] != DBNull.Value
                                        ? Convert.ToDecimal(reader[idxSaldoDisponible])
                                        : (decimal?)null
                                });
                            }
                        }
                    }

                    transaction.Commit();
                    return invoices;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Se deja constancia de qué procedure y con qué parámetros se intentó ejecutar:
                    // sin esto el error llega al log sin la compañía y no se puede diagnosticar.
                    throw new Exception(
                        $"Error al obtener las facturas del cliente '{customerCodeNormalizado}' de la compañía '{cmpyNormalizado}': {ex.Message}",
                        ex);
                }
            }
        }

        public async Task<List<mob_rcashrecp_payment>> GetPaymentByCust(string cmpy, string reg, string teri,string cust, int dias)
        {
            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var commandText = @"SELECT csh_cmpy, csh_transaccion_numero, csh_transaccion_estado, 
                                           csh_tipo_abono, csh_cust, csh_phone, csh_entdt, 
                                           csh_amt, csh_inv, csh_descto, 
                                           csh_saldo, csh_pago_en_transito, csh_saldo_disponible, 
                                           csh_reg, csh_teri, 
                                           csh_num2, csh_marca, csh_date, csh_time, csh_eva,csh_referencia,csh_convenio,csh_cash_received,csh_event_type
                                    FROM view_mob_pagos 
                                    WHERE csh_cmpy = @csh_cmpy 
                                      AND csh_reg = @csh_reg
                                      AND csh_teri = @csh_teri
                                      AND csh_cust = @csh_cust
                                      AND csh_date >= @fecha_limite
                                    ORDER BY csh_transaccion_numero, csh_entdt DESC";

                    var payments = new List<mob_rcashrecp_payment>();

                    // Calcular la fecha límite en C#
                    var fechaLimite = DateTime.Now.Date.AddDays(-dias);

                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.Transaction = transaction.GetDbTransaction();
                        command.Parameters.Add(new DB2Parameter("csh_cmpy", cmpy));
                        command.Parameters.Add(new DB2Parameter("csh_reg", reg));
                        command.Parameters.Add(new DB2Parameter("csh_teri", teri));
                        command.Parameters.Add(new DB2Parameter("csh_cust", cust));
                        command.Parameters.Add(new DB2Parameter("fecha_limite", fechaLimite));

                        await _context.Database.OpenConnectionAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            mob_rcashrecp_payment current = null;
                            var facturas = new List<Factura>();
                            string lastTrans = null;

                            while (await reader.ReadAsync())
                            {
                                var trans = reader["csh_transaccion_numero"]?.ToString();

                                if (current == null || trans != lastTrans)
                                {
                                    if (current != null)
                                    {
                                        current.facturas = facturas;
                                        payments.Add(current);
                                        facturas = new List<Factura>();
                                    }

                                    current = new mob_rcashrecp_payment
                                    {
                                        csh_cmpy = Convert.ToString(reader["csh_cmpy"])?.Trim(),
                                        csh_transaccion_numero = Convert.ToString(reader["csh_transaccion_numero"])?.Trim(),
                                        csh_transaccion_estado = Convert.ToString(reader["csh_transaccion_estado"])?.Trim(),
                                        csh_tipo_abono = Convert.ToString(reader["csh_tipo_abono"])?.Trim(),
                                        csh_cust = Convert.ToString(reader["csh_cust"])?.Trim(),
                                        csh_phone = Convert.ToString(reader["csh_phone"])?.Trim(),
                                        csh_entdt = reader["csh_entdt"] != DBNull.Value ? Convert.ToDateTime(reader["csh_entdt"]) : DateTime.MinValue,
                                        csh_reg = Convert.ToString(reader["csh_reg"])?.Trim(),
                                        csh_teri = Convert.ToString(reader["csh_teri"])?.Trim(),
                                        csh_num2 = reader["csh_num2"] != DBNull.Value ? Convert.ToInt32(reader["csh_num2"]) : null,
                                        csh_marca = Convert.ToString(reader["csh_marca"])?.Trim(),
                                        csh_date = reader["csh_date"] != DBNull.Value ? Convert.ToDateTime(reader["csh_date"]) : DateTime.MinValue,
                                        csh_time = Convert.ToString(reader["csh_time"])?.Trim(),
                                        csh_eva = Convert.ToString(reader["csh_eva"])?.Trim(),
                                        csh_referencia = reader["csh_referencia"] != DBNull.Value ? Convert.ToString(reader["csh_referencia"])?.Trim() : null,
                                        csh_convenio = reader["csh_convenio"] != DBNull.Value ? Convert.ToString(reader["csh_convenio"])?.Trim() : null,
                                        csh_cash_received = reader["csh_cash_received"] != DBNull.Value ? Convert.ToString(reader["csh_cash_received"])?.Trim() : null,
                                        csh_event_type = reader["csh_event_type"] != DBNull.Value ? Convert.ToString(reader["csh_event_type"])?.Trim() : null,
                                        facturas = new List<Factura>()
                                    };

                                    lastTrans = trans;
                                }

                                // Add factura details
                                if (reader["csh_inv"] != DBNull.Value)
                                {
                                    facturas.Add(new Factura
                                    {
                                        csh_inv = Convert.ToInt32(reader["csh_inv"]),
                                        csh_amt = reader["csh_amt"] != DBNull.Value ? Convert.ToDecimal(reader["csh_amt"]) : 0,
                                        csh_descto = reader["csh_descto"] != DBNull.Value ? Convert.ToDecimal(reader["csh_descto"]) : 0,
                                        csh_saldo = reader["csh_saldo"] != DBNull.Value ? Convert.ToDecimal(reader["csh_saldo"]) : (decimal?)null,
                                        csh_pago_en_transito = reader["csh_pago_en_transito"] != DBNull.Value ? Convert.ToDecimal(reader["csh_pago_en_transito"]) : (decimal?)null,
                                        csh_saldo_disponible = reader["csh_saldo_disponible"] != DBNull.Value ? Convert.ToDecimal(reader["csh_saldo_disponible"]) : (decimal?)null
                                    });
                                }
                            }

                            if (current != null)
                            {
                                current.facturas = facturas;
                                payments.Add(current);
                            }
                        }
                    }
                    transaction.Commit();
                    return payments;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception($"Error al obtener los pagos por usuario: {ex.Message}");
                }
            }
        }

        public bool CheckPaymentExists(string cmpy, string transaccionNumero, int inv)
        {
            try
            {
                var checkExistsCommandText = @"SELECT COUNT(*) FROM mob_rcashrecp_payment 
                    WHERE csh_cmpy = @csh_cmpy 
                      AND csh_transaccion_numero = @csh_transaccion_numero 
                      AND csh_inv = @csh_inv";

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = checkExistsCommandText;
                    command.Parameters.Add(new DB2Parameter("csh_cmpy", cmpy));
                    command.Parameters.Add(new DB2Parameter("csh_transaccion_numero", transaccionNumero));
                    command.Parameters.Add(new DB2Parameter("csh_inv", inv));

                    _context.Database.OpenConnection();
                    var countResult = command.ExecuteScalar();
                    return Convert.ToInt32(countResult) > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar si existe el pago: {ex.Message}");
            }
        }

    }
}
