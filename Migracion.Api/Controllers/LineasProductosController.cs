using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class LineasProductosController : ControllerBase
    {
        private readonly ILineasProductosQueries _lineaProductosQueries;

        public LineasProductosController(ILineasProductosQueries lineaProductosQueries)
        {
            _lineaProductosQueries = lineaProductosQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_rlineaE>> GetList(string cmpy)
        {
            return await _lineaProductosQueries.GetList(cmpy);
            
        }

        [HttpGet]
        [Route("GetListByCodUse/{cmpy}/{codUsu}")]
        public async Task<List<mob_rlineaE>> GetListByCodUse(string cmpy, string codUsu)
        {
            return await _lineaProductosQueries.GetListByCodUse(cmpy, codUsu);
            
        }
    }
}
