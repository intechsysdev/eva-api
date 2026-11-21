using Microsoft.Extensions.Options;
using Migracion.Models.Dtos;
using Migracion.Service.Queries;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Migracion.Api.Mcp.Tools
{
    [McpServerToolType]
    public class ProductosTools : EvaToolBase
    {
        private readonly IProductosQueries _productos;
        private readonly IInventariosQueries _inventarios;

        public ProductosTools(
            IProductosQueries productos, IInventariosQueries inventarios,
            IOptions<McpSettings> settings, ILogger<ProductosTools> logger) : base(settings, logger)
        {
            _productos = productos;
            _inventarios = inventarios;
        }

        [McpServerTool(Name = "eva_buscar_productos", Title = "Buscar productos del catálogo",
            ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
        [Description(
            "Busca productos del catálogo de la compañía por descripción, código o código de barras. Cada fila " +
            "trae: código (rpr_id), descripción (rpr_desc), línea (rpr_linea), sublínea (rpr_subl), código de " +
            "barras (rpr_ean), marca de producto nuevo (rpr_nuevo) y URL de la imagen (rpr_urlimagen). " +
            "Para existencias usa eva_consultar_inventario con el rpr_id.")]
        public async Task<string> BuscarProductos(
            [Description(DescripcionCmpy)] string? cmpy = null,
            [Description("Texto a buscar en la descripción, el código o el EAN (sin distinguir mayúsculas ni tildes).")] string? buscar = null,
            [Description("Código de línea (rpr_linea) para filtrar.")] string? linea = null,
            [Description(DescripcionLimite)] int limite = 25,
            [Description(DescripcionDesplazamiento)] int desplazamiento = 0)
        {
            string compania = Cmpy(cmpy);
            string? texto = McpEntrada.Busqueda(buscar);
            string? filtroLinea = McpEntrada.CodigoOpcional(linea, "linea");
            var (lim, desp) = Pagina(limite, desplazamiento);

            var productos = await Consultar("eva_buscar_productos", () => _productos.GetListAll(compania));

            var filas = (productos ?? new())
                .Where(p => filtroLinea == null || McpEntrada.Igual(p.rpr_linea, filtroLinea))
                .Where(p => texto == null
                    || McpEntrada.Contiene(p.rpr_desc, texto)
                    || McpEntrada.Contiene(p.rpr_id, texto)
                    || McpEntrada.Contiene(p.rpr_ean, texto))
                .OrderBy(p => p.rpr_desc)
                .Select(p => new
                {
                    p.rpr_id,
                    p.rpr_desc,
                    p.rpr_linea,
                    p.rpr_subl,
                    p.rpr_ean,
                    p.rpr_nuevo,
                    p.rpr_urlimagen
                })
                .ToList();

            return McpSalida.Pagina(filas, lim, desp,
                siVacio: "No hay productos con ese filtro. Prueba con menos palabras en 'buscar' o sin 'linea'.");
        }

        [McpServerTool(Name = "eva_consultar_inventario", Title = "Consultar existencias de inventario",
            ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
        [Description(
            "Existencias de inventario por producto. Con 'codigo_producto' devuelve ese producto; sin él lista " +
            "el inventario de la compañía con búsqueda por descripción. Cada fila trae: producto (cod_producto, " +
            "desc_producto), línea y sublínea con su descripción, unidades disponibles (total_inventario), " +
            "estado del stock (estado_stock) y disponibilidad (disponibilidad).")]
        public async Task<string> ConsultarInventario(
            [Description(DescripcionCmpy)] string? cmpy = null,
            [Description("Código del producto (rpr_id / cod_producto) para consultar uno solo.")] string? codigo_producto = null,
            [Description("Texto a buscar en la descripción o el código del producto. Se ignora si envías 'codigo_producto'.")] string? buscar = null,
            [Description(DescripcionLimite)] int limite = 25,
            [Description(DescripcionDesplazamiento)] int desplazamiento = 0)
        {
            string compania = Cmpy(cmpy);
            string? producto = McpEntrada.CodigoOpcional(codigo_producto, "codigo_producto");
            string? texto = McpEntrada.Busqueda(buscar);
            var (lim, desp) = Pagina(limite, desplazamiento);

            if (producto != null)
            {
                var uno = await Consultar("eva_consultar_inventario",
                    () => _inventarios.GetByProducto(compania, producto));
                return uno == null
                    ? McpSalida.Mensaje($"El producto '{producto}' no tiene registro de inventario en {compania}. Confirma el código con eva_buscar_productos.")
                    : McpSalida.Json(uno);
            }

            var inventario = await Consultar("eva_consultar_inventario",
                () => _inventarios.GetListByCompany(compania));

            List<InventarioStockDto> filas = (inventario ?? new())
                .Where(i => texto == null
                    || McpEntrada.Contiene(i.desc_producto, texto)
                    || McpEntrada.Contiene(i.cod_producto, texto))
                .OrderBy(i => i.desc_producto)
                .ToList();

            return McpSalida.Pagina(filas, lim, desp,
                siVacio: "No hay inventario con ese filtro. Prueba con menos palabras en 'buscar'.");
        }
    }
}
