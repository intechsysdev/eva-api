using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models.Dtos;
using Migracion.Persistence.DataBase;
using Migracion.Service.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductosIvaController : ControllerBase
    {
        private readonly IProductosIvaQueries _productosIvaQueries;

        public ProductosIvaController(IProductosIvaQueries productosIvaQueries)
        {
            _productosIvaQueries = productosIvaQueries;
        }

        [HttpGet]
        [Route("GetListByVendedor/{cmpy}/{codUsu}")]
        public async Task<List<mob_rprodvendedorDto>> GetListByVendedor(string cmpy, string codUsu)
        {
            return await _productosIvaQueries.GetListByVendedor(cmpy, codUsu);
            
        }
    }
}
