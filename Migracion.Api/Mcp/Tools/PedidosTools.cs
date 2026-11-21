using Microsoft.Extensions.Options;
using Migracion.Service.Queries;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Migracion.Api.Mcp.Tools
{
    [McpServerToolType]
    public class PedidosTools : EvaToolBase
    {
        private const string GlosarioPedido =
            "Encabezado: número del pedido (orh_num), cliente (orh_cust), sucursal (orh_ship), fecha del pedido " +
            "(orh_entdt) y de despacho (orh_shpdt) en MM/dd/yyyy, comentario (orh_com) y orden de compra (orh_po). " +
            "Líneas: producto (orl_part), cantidad pedida (orl_qord) y lista de precios (orl_level).";

        private readonly IPedidosQueries _pedidos;

        public PedidosTools(
            IPedidosQueries pedidos,
            IOptions<McpSettings> settings, ILogger<PedidosTools> logger) : base(settings, logger)
        {
            _pedidos = pedidos;
        }

        [McpServerTool(Name = "eva_ultimos_pedidos_cliente", Title = "Últimos pedidos de un cliente",
            ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
        [Description(
            "Devuelve los pedidos más recientes de un cliente (del más nuevo al más antiguo), con o sin sus " +
            "líneas de producto. " + GlosarioPedido + " Con incluir_detalle=true cada elemento trae 'encabezado' " +
            "y 'list_detail', y se omiten los pedidos que no tienen líneas.")]
        public async Task<string> UltimosPedidosCliente(
            [Description("Código de región del vendedor que atiende al cliente (rusu_reg).")] string region,
            [Description("Código de territorio del vendedor que atiende al cliente (rusu_teri).")] string territorio,
            [Description("Código del cliente (rctt_cust).")] string codigo_cliente,
            [Description("Sucursal del cliente (rctt_succli), tal como aparece en eva_buscar_clientes.")] string sucursal,
            [Description(DescripcionCmpy)] string? cmpy = null,
            [Description("Cuántos pedidos traer (1 a 20). Por defecto 5.")] int cantidad = 5,
            [Description("true para incluir las líneas de producto de cada pedido. Por defecto false.")] bool incluir_detalle = false)
        {
            string compania = Cmpy(cmpy);
            string reg = McpEntrada.Codigo(region, "region");
            string teri = McpEntrada.Codigo(territorio, "territorio");
            string cliente = McpEntrada.Codigo(codigo_cliente, "codigo_cliente");
            string ship = McpEntrada.Codigo(sucursal, "sucursal");
            int numero = Math.Clamp(cantidad, 1, 20);

            const string sinPedidos =
                "El cliente no tiene pedidos con esos datos. Verifica región, territorio y sucursal: deben ser los del vendedor y la sucursal del cliente.";

            if (incluir_detalle)
            {
                var completos = await Consultar("eva_ultimos_pedidos_cliente",
                    () => _pedidos.GetFull(numero, compania, reg, teri, cliente, ship));
                return McpSalida.Pagina(completos ?? new(), numero, 0, siVacio: sinPedidos);
            }

            var encabezados = await Consultar("eva_ultimos_pedidos_cliente",
                () => _pedidos.GetHis(numero, compania, reg, teri, cliente, ship));
            return McpSalida.Pagina(encabezados ?? new(), numero, 0, siVacio: sinPedidos);
        }

        [McpServerTool(Name = "eva_obtener_pedido", Title = "Obtener un pedido con sus líneas",
            ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
        [Description(
            "Devuelve un pedido por su número, con el encabezado y todas sus líneas de producto. " + GlosarioPedido +
            " El número de pedido es único dentro de una región y territorio, por eso se piden los tres datos.")]
        public async Task<string> ObtenerPedido(
            [Description("Código de región del pedido (orh_reg).")] string region,
            [Description("Código de territorio del pedido (orh_teri).")] string territorio,
            [Description("Número del pedido (orh_num), tal como aparece en eva_ultimos_pedidos_cliente.")] string numero_pedido,
            [Description(DescripcionCmpy)] string? cmpy = null)
        {
            string compania = Cmpy(cmpy);
            string reg = McpEntrada.Codigo(region, "region");
            string teri = McpEntrada.Codigo(territorio, "territorio");
            string numero = McpEntrada.Codigo(numero_pedido, "numero_pedido");

            var encabezados = await Consultar("eva_obtener_pedido",
                () => _pedidos.GetByReg(compania, reg, teri, numero));

            if (encabezados == null || encabezados.Count == 0)
                return McpSalida.Mensaje(
                    $"No existe el pedido {numero} en la región {reg} y territorio {teri} de {compania}. " +
                    "Confirma el número con eva_ultimos_pedidos_cliente.");

            var lineas = await Consultar("eva_obtener_pedido",
                () => _pedidos.GetDetail(compania, reg, teri, numero));

            return McpSalida.Json(new
            {
                encabezado = encabezados[0],
                cantidad_lineas = lineas?.Count ?? 0,
                lineas = lineas ?? new()
            });
        }
    }
}
