using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Persistence.DataBase;
using Migracion.Service.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Migracion.Service.Queries
{
    public interface IRecibosCajasQueries
    {
        Task<List<mob_rcashrecpE>> GetListProntoPago(string cmpy, string reg, string teri, string num, string entdt);
        Task<List<mob_rcashapplE>> GetList(string cmpy, string reg, string teri, string num, string inv, string entdt);
        int InsertProntoPago(mob_rcashrecpE prontoPago);
        int Insert(mob_rcashapplE recibo);
        Task<byte[]> GenerarReciboCajaPdf(string cmpy, string reg, string teri, string num, string cust, string succli);
    }
    public class RecibosCajasQueries : IRecibosCajasQueries
    {
        private readonly ApplicationDbContext _context;
        public RecibosCajasQueries(
          ApplicationDbContext context)
        {
            _context = context;
            
        }

        public async Task<List<mob_rcashrecpE>> GetListProntoPago(string cmpy, string reg, string teri, string num, string entdt)
        {
            List<mob_rcashrecpE> lstAcciones = null;
            try
            {

                lstAcciones = await _context.mob_rcashrecp.Where(x => x.csh_cmpy == cmpy
                && x.csh_reg == reg && x.csh_teri == teri && x.csh_num == num &&
                x.csh_entdt == entdt
                ).ToListAsync();

                lstAcciones.ForEach(x =>
                {
                    x.csh_entdt = Convert.ToDateTime(x.csh_entdt).ToString("MM/dd/yyyy");
                    x.csh_date = Convert.ToDateTime(x.csh_date).ToString("MM/dd/yyyy");
                    x.csh_confecha = Convert.ToDateTime(x.csh_confecha).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rcashrecpE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

        public async Task<List<mob_rcashapplE>> GetList(string cmpy, string reg, string teri, string num, string inv, string entdt)
        {
            List<mob_rcashapplE> lstAcciones = null;
            try
            {

                lstAcciones = await _context.mob_rcashappl.Where(x => x.csa_cmpy == cmpy
                && x.csa_reg == reg && x.csa_teri == teri && x.csa_num == num &&
                 x.csa_inv == inv && x.csa_entdt == entdt
                ).ToListAsync();

                lstAcciones.ForEach(x =>
                {
                    x.csa_entdt = Convert.ToDateTime(x.csa_entdt).ToString("MM/dd/yyyy");
                    x.csa_date = Convert.ToDateTime(x.csa_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rcashapplE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

        public int InsertProntoPago(mob_rcashrecpE prontoPago)
        {
            int result = 0;
            try
            {

                // Normalizamos el valor de csh_amt, eliminando las comas y manejando el formato decimal
                string montoStr = prontoPago.csh_amt;

                // Si contiene coma (probablemente miles), reemplazamos la coma para eliminarla
                if (montoStr.Contains(","))
                {
                    montoStr = montoStr.Replace(",", "");
                }

                // Ahora intentamos convertirlo a decimal
                decimal monto;
                if (!decimal.TryParse(montoStr, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out monto))
                {
                    throw new Exception("El monto ingresado no tiene un formato válido.");
                }

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * FROM mob_rcashrecp WHERE csh_cmpy = '" + prontoPago.csh_cmpy + "' AND  csh_reg = '" + prontoPago.csh_reg + "' AND csh_teri = '" + prontoPago.csh_teri + "' AND csh_num = '" + prontoPago.csh_num + "' AND csh_entdt = '" + prontoPago.csh_entdt + "'";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rcashrecp (csh_cmpy,csh_num,csh_tele,csh_cust,csh_entdt,csh_amt,csh_card,csh_crdt,csh_com,csh_conclnte,csh_confecha,csh_descto,csh_reg,csh_teri,csh_num2,csh_marca,csh_date,csh_time) values ( @csh_cmpy, @csh_num, @csh_tele, @csh_cust, @csh_entdt, @csh_amt, @csh_card, @csh_crdt, @csh_com, @csh_conclnte, @csh_confecha, @csh_descto, @csh_reg, @csh_teri, @csh_num2, @csh_marca, @csh_date, @csh_time)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("csh_cmpy", prontoPago.csh_cmpy),
                                 new DB2Parameter("csh_num", prontoPago.csh_num),
                                 new DB2Parameter("csh_tele", prontoPago.csh_tele),
                                 new DB2Parameter("csh_cust", prontoPago.csh_cust),
                                 new DB2Parameter("csh_entdt", prontoPago.csh_entdt),
                                 new DB2Parameter("csh_amt", montoStr),
                                 new DB2Parameter("csh_card", prontoPago.csh_card),
                                 new DB2Parameter("csh_crdt", prontoPago.csh_crdt),
                                 new DB2Parameter("csh_com", prontoPago.csh_com),
                                 new DB2Parameter("csh_conclnte", prontoPago.csh_conclnte),
                                 new DB2Parameter("csh_confecha", prontoPago.csh_confecha),
                                 new DB2Parameter("csh_descto", prontoPago.csh_descto),
                                 new DB2Parameter("csh_reg", prontoPago.csh_reg),
                                 new DB2Parameter("csh_teri", prontoPago.csh_teri),
                                 new DB2Parameter("csh_num2", prontoPago.csh_num2),
                                 new DB2Parameter("csh_marca", prontoPago.csh_marca),
                                 new DB2Parameter("csh_date", prontoPago.csh_date),
                                 new DB2Parameter("csh_time", prontoPago.csh_time)

                                 );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int Insert(mob_rcashapplE recibo)
        {
            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * FROM mob_rcashappl WHERE csa_cmpy = '" + recibo.csa_cmpy + "' AND csa_reg = '" + recibo.csa_reg + "' AND csa_teri = '" + recibo.csa_teri + "' AND csa_num = '" + recibo.csa_num + "' AND csa_inv = '" + recibo.csa_inv + "' AND csa_entdt = '" + recibo.csa_entdt + "'";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                       if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rcashappl (csa_cmpy,csa_num,csa_reg,csa_teri,csa_inv,csa_amt,csa_dis,csa_entdt,csa_marca,csa_date,csa_time) values ( @csa_cmpy, @csa_num, @csa_reg, @csa_teri, @csa_inv, @csa_amt, @csa_dis, @csa_entdt, @csa_marca, @csa_date, @csa_time)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("csa_cmpy", recibo.csa_cmpy),
                                 new DB2Parameter("csa_num", recibo.csa_num),
                                 new DB2Parameter("csa_reg", recibo.csa_reg),
                                 new DB2Parameter("csa_teri", recibo.csa_teri),
                                 new DB2Parameter("csa_inv", recibo.csa_inv),
                                 new DB2Parameter("csa_amt", recibo.csa_amt),
                                 new DB2Parameter("csa_dis", recibo.csa_dis),
                                 new DB2Parameter("csa_entdt", recibo.csa_entdt),
                                 new DB2Parameter("csa_marca", recibo.csa_marca),
                                 new DB2Parameter("csa_date", recibo.csa_date),
                                 new DB2Parameter("csa_time", recibo.csa_time)
                                 );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public async Task<byte[]> GenerarReciboCajaPdf(string cmpy, string reg, string teri, string num, string cust, string succli)
        {
            try
            {
                
                QuestPDF.Settings.License = LicenseType.Community;
                var vendedor = $"Vendedor: {num}";
                var territorio = $"Región: {reg} - Territorio: {teri}";
                var reciboNumero = $"RC-{cmpy}-{num}";
                var fechaHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                var cliente = $"Cliente: {cust} - Sucursal: {succli}";
                var compania = $"Compañía: {cmpy}";
                
                
                var facturas = new List<(string NumFactura, decimal ValorDescuento, decimal Saldo)>
                {
                    ("FAC-001234", 50000.00m, 450000.00m),
                    ("FAC-001235", 25000.00m, 175000.00m),
                    ("FAC-001236", 0.00m, 320000.00m)
                };

                var totalDescuento = facturas.Sum(f => f.ValorDescuento);
                var totalSaldo = facturas.Sum(f => f.Saldo);

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.Margin(40);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        page.Header()
                            .BorderBottom(1)
                            .Padding(10)
                            .Column(column =>
                            {
                                column.Item().AlignCenter().Text("RECIBO DE CAJA")
                                    .FontSize(18)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken2);

                                column.Item().PaddingTop(5).AlignCenter().Text(reciboNumero)
                                    .FontSize(14)
                                    .Bold();

                                column.Item().PaddingTop(3).AlignCenter().Text(compania)
                                    .FontSize(10);
                            });

                        page.Content()
                            .PaddingVertical(20)
                            .Column(column =>
                            {
                                // Información del Vendedor y Territorio
                                column.Item().Row(row =>
                                {
                                    row.RelativeItem().Column(col =>
                                    {
                                        col.Item().Text(text =>
                                        {
                                            text.Span("Vendedor: ").Bold();
                                            text.Span(num);
                                        });

                                        col.Item().PaddingTop(5).Text(text =>
                                        {
                                            text.Span("Territorio: ").Bold();
                                            text.Span($"{reg} / {teri}");
                                        });
                                    });

                                    row.RelativeItem().Column(col =>
                                    {
                                        col.Item().AlignRight().Text(text =>
                                        {
                                            text.Span("Fecha/Hora: ").Bold();
                                            text.Span(fechaHora);
                                        });
                                    });
                                });

                                // Información del Cliente
                                column.Item().PaddingTop(15).Text(text =>
                                {
                                    text.Span("Cliente: ").Bold();
                                    text.Span(cust);
                                });

                                column.Item().PaddingTop(5).Text(text =>
                                {
                                    text.Span("Sucursal: ").Bold();
                                    text.Span(succli);
                                });

                                // Separador
                                column.Item().PaddingTop(20).PaddingBottom(10).LineHorizontal(1);

                                // Título de Facturas
                                column.Item().PaddingBottom(10).Text("DETALLE DE FACTURAS")
                                    .FontSize(12)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken1);

                                // Tabla de Facturas
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1.5f);
                                        columns.RelativeColumn(1.5f);
                                    });

                                    // Header
                                    table.Header(header =>
                                    {
                                        header.Cell().Background(Colors.Blue.Darken2)
                                            .Padding(8)
                                            .Text("Factura")
                                            .FontColor(Colors.White)
                                            .Bold();

                                        header.Cell().Background(Colors.Blue.Darken2)
                                            .Padding(8)
                                            .AlignRight()
                                            .Text("Valor Descuento")
                                            .FontColor(Colors.White)
                                            .Bold();

                                        header.Cell().Background(Colors.Blue.Darken2)
                                            .Padding(8)
                                            .AlignRight()
                                            .Text("Saldo")
                                            .FontColor(Colors.White)
                                            .Bold();
                                    });

                                    // Rows
                                    foreach (var factura in facturas)
                                    {
                                        table.Cell()
                                            .BorderBottom(0.5f)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .Padding(8)
                                            .Text(factura.NumFactura);

                                        table.Cell()
                                            .BorderBottom(0.5f)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .Padding(8)
                                            .AlignRight()
                                            .Text($"${factura.ValorDescuento:N2}");

                                        table.Cell()
                                            .BorderBottom(0.5f)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .Padding(8)
                                            .AlignRight()
                                            .Text($"${factura.Saldo:N2}");
                                    }

                                    // Totales
                                    table.Cell()
                                        .Background(Colors.Grey.Lighten3)
                                        .Padding(8)
                                        .Text("TOTALES")
                                        .Bold();

                                    table.Cell()
                                        .Background(Colors.Grey.Lighten3)
                                        .Padding(8)
                                        .AlignRight()
                                        .Text($"${totalDescuento:N2}")
                                        .Bold();

                                    table.Cell()
                                        .Background(Colors.Grey.Lighten3)
                                        .Padding(8)
                                        .AlignRight()
                                        .Text($"${totalSaldo:N2}")
                                        .Bold();
                                });

                                // Observaciones
                                column.Item().PaddingTop(20).Text("Observaciones:")
                                    .Bold();
                                
                                column.Item().PaddingTop(5).Border(1)
                                    .BorderColor(Colors.Grey.Lighten1)
                                    .Padding(10)
                                    .MinHeight(60)
                                    .Text($"Recibo generado para compañía {cmpy}, región {reg}, territorio {teri}.");
                            });

                        page.Footer()
                            .AlignCenter()
                            .DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken1))
                            .Text(text =>
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

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar el PDF: {ex.Message}");
            }
        }
    }
}
