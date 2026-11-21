using Microsoft.Extensions.Options;
using Migracion.Service.Queries;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Migracion.Api.Mcp.Tools
{
    [McpServerToolType]
    public class ClientesTools : EvaToolBase
    {
        private readonly IClientesQueries _clientes;

        public ClientesTools(
            IClientesQueries clientes,
            IOptions<McpSettings> settings, ILogger<ClientesTools> logger) : base(settings, logger)
        {
            _clientes = clientes;
        }

        [McpServerTool(Name = "eva_buscar_clientes", Title = "Buscar clientes de un vendedor",
            ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
        [Description(
            "Lista los clientes asignados a un vendedor, con búsqueda por nombre o código. Cada fila trae: " +
            "código del cliente (rctt_cust), sucursal (rctt_succli), razón social (rctt_razsoc), nombre comercial " +
            "(rctt_nomcli), dirección (rctt_addr), teléfonos (rctt_tele, rctt_celular), ruta (rctt_ruta), región " +
            "(rctt_reg) y canal (rctt_nomcan). Un mismo cliente puede aparecer con varias sucursales. " +
            "Para la ficha completa de un cliente usa eva_obtener_cliente.")]
        public async Task<string> BuscarClientes(
            [Description("Código del vendedor (rusu_codusu). Se obtiene con eva_listar_vendedores.")] string codigo_vendedor,
            [Description(DescripcionCmpy)] string? cmpy = null,
            [Description("Texto a buscar en razón social, nombre comercial o código del cliente (sin distinguir mayúsculas ni tildes).")] string? buscar = null,
            [Description(DescripcionLimite)] int limite = 25,
            [Description(DescripcionDesplazamiento)] int desplazamiento = 0)
        {
            string compania = Cmpy(cmpy);
            string vendedor = McpEntrada.Codigo(codigo_vendedor, "codigo_vendedor");
            string? texto = McpEntrada.Busqueda(buscar);
            var (lim, desp) = Pagina(limite, desplazamiento);

            var clientes = await Consultar("eva_buscar_clientes",
                () => _clientes.GetListByCodUsu(compania, vendedor));

            var filas = (clientes ?? new())
                .Where(c => texto == null
                    || McpEntrada.Contiene(c.rctt_razsoc, texto)
                    || McpEntrada.Contiene(c.rctt_nomcli, texto)
                    || McpEntrada.Contiene(c.rctt_cust, texto))
                .OrderBy(c => c.rctt_razsoc)
                .Select(c => new
                {
                    c.rctt_cust,
                    c.rctt_succli,
                    c.rctt_razsoc,
                    c.rctt_nomcli,
                    c.rctt_addr,
                    c.rctt_tele,
                    c.rctt_celular,
                    c.rctt_ruta,
                    c.rctt_reg,
                    c.rctt_nomcan
                })
                .ToList();

            return McpSalida.Pagina(filas, lim, desp,
                siVacio: "El vendedor no tiene clientes con ese filtro. Verifica 'codigo_vendedor' con eva_listar_vendedores o quita 'buscar'.");
        }

        [McpServerTool(Name = "eva_obtener_cliente", Title = "Obtener la ficha de un cliente",
            ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
        [Description(
            "Devuelve la ficha completa de un cliente por su código: identificación, razón social, dirección, " +
            "contacto (rctt_tele, rctt_celular, rctt_mail), vendedor asignado (rctt_codusu), ruta, región, canal, " +
            "coordenadas (rctt_latitud, rctt_longitud), frecuencia y día de visita y demás columnas rctt_* de " +
            "mob_rcustmain. Si el cliente tiene varias sucursales devuelve la primera; para verlas todas usa " +
            "eva_buscar_clientes con el vendedor del cliente.")]
        public async Task<string> ObtenerCliente(
            [Description("Código del cliente (rctt_cust), tal como aparece en eva_buscar_clientes.")] string codigo_cliente,
            [Description(DescripcionCmpy)] string? cmpy = null)
        {
            string compania = Cmpy(cmpy);
            string cliente = McpEntrada.Codigo(codigo_cliente, "codigo_cliente");

            var ficha = await Consultar("eva_obtener_cliente", () => _clientes.GetbyCust(compania, cliente));

            return ficha == null
                ? McpSalida.Mensaje($"No existe el cliente '{cliente}' en la compañía {compania}. Búscalo con eva_buscar_clientes para confirmar el código.")
                : McpSalida.Json(ficha);
        }
    }
}
