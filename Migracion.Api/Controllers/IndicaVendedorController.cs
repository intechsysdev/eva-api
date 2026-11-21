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
    public class IndicaVendedorController : ControllerBase
    {
        private readonly IIndicaVendedorQueries _indicadorVendedorQueries;

        public IndicaVendedorController(IIndicaVendedorQueries indicadorVendedorQueries)
        {
            _indicadorVendedorQueries = indicadorVendedorQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{codusu}")]
        public async Task<List<mob_indicavendedorE>> GetList(string cmpy, string codusu)
        {
            return await _indicadorVendedorQueries.GetList(cmpy, codusu);
            
        }
    }
}
