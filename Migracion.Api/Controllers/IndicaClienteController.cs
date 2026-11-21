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
    public class IndicaClienteController : ControllerBase
    {
        private readonly IIndicaClienteQueries _indicaClienteQueries;

        public IndicaClienteController(IIndicaClienteQueries indicaClienteQueries)
        {
            _indicaClienteQueries = indicaClienteQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{codusu}/{concepto}")]
        public async Task<List<mob_indicaclienteE>> GetList(string cmpy, string codusu, string concepto)
        {
            return await _indicaClienteQueries.GetList(cmpy, codusu, concepto);   
        }
    }
}
