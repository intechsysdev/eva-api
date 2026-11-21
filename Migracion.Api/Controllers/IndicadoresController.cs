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
    public class IndicadoresController : ControllerBase
    {
        private readonly IIndicadoresQueries _indicadoresQueries;

        public IndicadoresController(IIndicadoresQueries indicadoresQueries)
        {
            _indicadoresQueries = indicadoresQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{codusu}")]
        public async Task<List<mob_indicadoresE>> GetList(string cmpy, string codusu)
        {
            return await _indicadoresQueries.GetList(cmpy, codusu);   
        }
    }
}
