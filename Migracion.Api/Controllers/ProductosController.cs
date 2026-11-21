using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Api.Models;
using Migracion.Models;
using Migracion.Persistence.DataBase;
using Migracion.Service.Queries;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductosQueries _productosQueries;

        public ProductosController(IProductosQueries productosQueries)
        {
            _productosQueries = productosQueries;
        }

        /// <summary>
        /// Apunta rpr_urlimagen al archivo recien cargado.
        /// </summary>
        /// <remarks>
        /// La URL llega armada desde el SPA, que la deduce de las fotos que ya
        /// se muestran. No se compone aqui con FileSettings.UrlPath porque esa
        /// configuracion es la carpeta de disco donde FileUpload escribe
        /// (C:\Imagenes\eva), no la direccion web de las imagenes.
        /// </remarks>
        [HttpPost]
        [Route("UpdateImagen")]
        public ActionResult<int> UpdateImagen([FromBody] ActualizarImagenDto datos)
        {
            if (datos == null || string.IsNullOrWhiteSpace(datos.cmpy) ||
                string.IsNullOrWhiteSpace(datos.id) || string.IsNullOrWhiteSpace(datos.url))
            {
                return BadRequest("cmpy, id y url son obligatorios.");
            }

            var url = datos.url.Trim();

            // rpr_urlimagen esta declarada con 100 caracteres: si se pasa, DB2
            // truncaria la URL en silencio y la foto quedaria inalcanzable.
            if (url.Length > 100)
            {
                return BadRequest($"La URL resultante tiene {url.Length} caracteres y el campo admite 100: {url}");
            }

            var filas = _productosQueries.UpdateImagen(datos.cmpy, datos.id, url);
            if (filas == 0) return NotFound("No se encontró el producto.");
            return filas;
        }

        [HttpGet]
        [Route("GetListPoints/{cmpy}")]
        public async Task<List<mob_rprodpuntoE>> GetListPoints(string cmpy)
        {
            return await _productosQueries.GetProdPuntosLis(cmpy);

        }

        [HttpGet]
        [Route("GetListDeposits/{cmpy}/{custdep}")]
        public async Task<List<mob_rproddepositoE>> GetListDeposits(string cmpy, string custdep)
        {
            return await _productosQueries.GetListDeposits(cmpy, custdep);
            
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{linea}")]
        public async Task<List<mob_rproductosE>> GetList(string cmpy, string linea)
        {
            return await _productosQueries.GetList(cmpy, linea);
            
        }

        [HttpGet]
        [Route("GetListBySubL/{cmpy}/{linea}/{subl}")]
        public async Task<List<mob_rproductosE>> GetListBySubL(string cmpy, string linea, string subl)
        {
            return await _productosQueries.GetListBySubL(cmpy, linea, subl);
            
        }

        [HttpGet]
        [Route("GetListByVendedor/{cmpy}/{codUsu}")]
        public async Task<List<mob_rprodvendedorE>> GetListByVendedor(string cmpy, string codUsu)
        {
            return await _productosQueries.GetListByVendedor(cmpy, codUsu);
            
        }

        [HttpGet]
        [Route("GetListAll/{cmpy}")]
        public async Task<List<mob_rprodvendedorE>> GetListAll(string cmpy)
        {
            return await _productosQueries.GetListAll(cmpy);

        }


        [HttpGet]
        [Route("GetListByVendedorLinea/{cmpy}/{codUsu}/{linea}")]
        public async Task<List<mob_rprodvendedorE>> GetListByVendedorLinea(string cmpy, string codUsu, string linea)
        {
            return await _productosQueries.GetListByVendedorLinea(cmpy, codUsu, linea);
            
        }

        [HttpGet]
        [Route("GetListByVendedorSubLinea/{cmpy}/{codUsu}/{linea}/{subl}")]
        public async Task<List<mob_rprodvendedorE>> GetListByVendedorLinea(string cmpy, string codUsu, string linea, string subl)
        {
            return await _productosQueries.GetListByVendedorSubLinea(cmpy, codUsu, linea, subl);
            
        }

        [HttpGet]
        [Route("GetListSubLines/{cmpy}")]
        public async Task<List<mob_rsublineasE>> GetListSubLines(string cmpy)
        {
            return await _productosQueries.GetListSubLines(cmpy);
            
        }

        [HttpGet]
        [Route("GetListSubLinesByCodUsu/{cmpy}/{codUsu}")]
        public async Task<List<mob_rsublineasE>> GetListSubLinesByCodUsu(string cmpy, string codUsu)
        {
            return await _productosQueries.GetListSubLinesByCodUsu(cmpy, codUsu);
            
        }

        [HttpGet]
        [Route("GetProdGancho/{cmpy}/{reg}/{teri}/{cust}/{succli}")]
        public async Task<List<mob_prodganchoE>> GetProdGancho(string cmpy, string reg, string teri, string cust, string succli)
        {
            return await _productosQueries.GetProdGancho(cmpy, reg, teri, cust, succli);
            
        }
    }
}
