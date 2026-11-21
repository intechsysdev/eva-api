using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Models.Command;
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
    public class ProductosPreciosController : ControllerBase
    {
        private readonly IProductosPreciosQueries _productosPreciosQueries;

        public ProductosPreciosController(IProductosPreciosQueries productosPreciosQueries)
        {
            _productosPreciosQueries = productosPreciosQueries;
        }

        [HttpGet]
        [Route("GetListByMarca/{cmpy}/{codfab}/{marca}")]
        public async Task<List<mob_rprodpreciosE>> GetListByMarca(string cmpy, string codfab, string marca)
        {
            return await _productosPreciosQueries.GetListByMarca(cmpy, codfab, marca);
            
        }

        [HttpGet]
        [Route("GetListByCodigo/{cmpy}/{codfab}/{codigo}")]
        public async Task<List<mob_rprodpreciosE>> GetListByCodigo(string cmpy, string codfab, string codigo)

        {
            return await _productosPreciosQueries.GetListByCodigo(cmpy, codfab, codigo);
            
        }

        [HttpGet]
        [Route("GetListByEan/{cmpy}/{ean}")]
        public async Task<List<mob_rprodpreciosE>> GetListByEan(string cmpy, string ean)

        {
            return await _productosPreciosQueries.GetListByEan(cmpy, ean);
            
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rprodpreciosCommand precio)
        {
            return _productosPreciosQueries.Insert(precio);
        }

        [HttpPut]
        [Route("Update")]
        public int Update(mob_rprodpreciosCommand item)
        {
            return _productosPreciosQueries.Update(item);
        }
    }
}
