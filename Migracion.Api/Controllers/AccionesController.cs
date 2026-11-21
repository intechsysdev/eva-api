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
    public class AccionesController : ControllerBase
    {
        private readonly IAccionesQueries _accionesQueries;

        public AccionesController(IAccionesQueries accionesQueries)
        {
            _accionesQueries = accionesQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_accionesE>> GetList(string cmpy)
        {
            return await _accionesQueries.GetList(cmpy);
        }
    }
}
