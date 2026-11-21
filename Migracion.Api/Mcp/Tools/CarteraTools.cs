using Microsoft.Extensions.Options;
using Migracion.Service.Queries;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Migracion.Api.Mcp.Tools
{
    [McpServerToolType]
    public class CarteraTools : EvaToolBase
    {
        private readonly ICarteraQueries _cartera;

        public CarteraTools(
            ICarteraQueries cartera,
            IOptions<McpSettings> settings, ILogger<CarteraTools> logger) : base(settings, logger)
        {
            _cartera = cartera;
        }

        [McpServerTool(Name = "eva_consultar_cartera", Title = "Consultar cartera (cuentas por cobrar)",
            ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
        [Description(
            "Documentos de cartera (facturas por cobrar) de los clientes de un vendedor, opcionalmente de un solo " +
            "cliente. Cada fila trae: tipo (car_tipo) y número (car_numero) del documento, cliente (car_cust) y " +
            "sucursal (car_succli), fecha (car_fecha) y vencimiento (car_fechaven) en MM/dd/yyyy, días de " +
            "vencido (car_diasven), valor original (car_amt) y saldo pendiente (car_saldo). La respuesta incluye " +
            "'saldo_total' y 'documentos' calculados sobre todas las filas del filtro, no solo sobre la página. " +
            "Las filas van ordenadas de más a menos días de vencido.")]
        public async Task<string> ConsultarCartera(
            [Description("Código del vendedor (rusu_codusu). Se obtiene con eva_listar_vendedores.")] string codigo_vendedor,
            [Description(DescripcionCmpy)] string? cmpy = null,
            [Description("Código del cliente (rctt_cust) para ver solo su cartera. Omite para toda la cartera del vendedor.")] string? codigo_cliente = null,
            [Description("Devuelve solo documentos con car_diasven mayor o igual a este valor. Ejemplo: 1 = solo vencidos, 30 = vencidos hace 30 días o más.")] int? dias_vencido_minimo = null,
            [Description(DescripcionLimite)] int limite = 25,
            [Description(DescripcionDesplazamiento)] int desplazamiento = 0)
        {
            string compania = Cmpy(cmpy);
            string vendedor = McpEntrada.Codigo(codigo_vendedor, "codigo_vendedor");
            string? cliente = McpEntrada.CodigoOpcional(codigo_cliente, "codigo_cliente");
            var (lim, desp) = Pagina(limite, desplazamiento);

            var cartera = await Consultar("eva_consultar_cartera", () => _cartera.GetList(compania, vendedor));

            var filas = (cartera ?? new())
                .Where(d => cliente == null || McpEntrada.Igual(d.car_cust, cliente))
                .Where(d => dias_vencido_minimo == null || McpEntrada.Numero(d.car_diasven) >= dias_vencido_minimo)
                .OrderByDescending(d => McpEntrada.Numero(d.car_diasven))
                .Select(d => new
                {
                    d.car_tipo,
                    d.car_numero,
                    d.car_cust,
                    d.car_succli,
                    d.car_fecha,
                    d.car_fechaven,
                    d.car_diasven,
                    d.car_amt,
                    d.car_saldo,
                    d.car_notacredito,
                    d.car_guia
                })
                .ToList();

            return McpSalida.Pagina(filas, lim, desp,
                extra: new
                {
                    documentos = filas.Count,
                    saldo_total = filas.Sum(d => McpEntrada.Numero(d.car_saldo))
                },
                siVacio: "No hay cartera con ese filtro. Verifica 'codigo_vendedor' y 'codigo_cliente', o quita 'dias_vencido_minimo'.");
        }

        [McpServerTool(Name = "eva_listar_pagos", Title = "Listar pagos y abonos recibidos",
            ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
        [Description(
            "Pagos (recibos de caja / abonos) registrados en los últimos días para una región y territorio, " +
            "opcionalmente de un solo cliente. Cada pago trae: número (csh_transaccion_numero) y estado " +
            "(csh_transaccion_estado) de la transacción, tipo de abono (csh_tipo_abono), cliente (csh_cust), " +
            "fecha (csh_entdt), marca de aplicación (csh_marca: 'S' = ya aplicado en el ERP; cualquier otro " +
            "valor en un pago no rechazado = todavía en tránsito) y la lista 'facturas' con el valor abonado a " +
            "cada una (csh_inv = factura, csh_amt = valor, csh_descto = descuento).")]
        public async Task<string> ListarPagos(
            [Description("Código de región del vendedor (rusu_reg).")] string region,
            [Description("Código de territorio del vendedor (rusu_teri).")] string territorio,
            [Description(DescripcionCmpy)] string? cmpy = null,
            [Description("Código del cliente (rctt_cust) para ver solo sus pagos.")] string? codigo_cliente = null,
            [Description("Cuántos días hacia atrás consultar (1 a 365). Por defecto 30.")] int dias = 30,
            [Description(DescripcionLimite)] int limite = 25,
            [Description(DescripcionDesplazamiento)] int desplazamiento = 0)
        {
            string compania = Cmpy(cmpy);
            string reg = McpEntrada.Codigo(region, "region");
            string teri = McpEntrada.Codigo(territorio, "territorio");
            string? cliente = McpEntrada.CodigoOpcional(codigo_cliente, "codigo_cliente");
            int rango = Math.Clamp(dias, 1, 365);
            var (lim, desp) = Pagina(limite, desplazamiento);

            var pagos = await Consultar("eva_listar_pagos", () => cliente == null
                ? _cartera.GetPaymentByUser(compania, reg, teri, rango)
                : _cartera.GetPaymentByCust(compania, reg, teri, cliente, rango));

            var filas = (pagos ?? new())
                .OrderByDescending(p => p.csh_entdt)
                .Select(p => new
                {
                    p.csh_transaccion_numero,
                    p.csh_transaccion_estado,
                    p.csh_tipo_abono,
                    p.csh_cust,
                    csh_entdt = p.csh_entdt.ToString("yyyy-MM-dd HH:mm"),
                    p.csh_marca,
                    p.csh_num2,
                    p.csh_referencia,
                    p.csh_convenio,
                    total_abonado = (p.facturas ?? new()).Sum(f => f.csh_amt),
                    facturas = (p.facturas ?? new()).Select(f => new
                    {
                        f.csh_inv,
                        f.csh_amt,
                        f.csh_descto,
                        f.csh_saldo
                    })
                })
                .ToList();

            return McpSalida.Pagina(filas, lim, desp,
                siVacio: $"No hay pagos en los últimos {rango} días con ese filtro. Amplía 'dias' o verifica región, territorio y cliente.");
        }
    }
}
