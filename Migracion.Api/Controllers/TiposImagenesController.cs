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
    public class TiposImagenesController : ControllerBase
    {
        private readonly ITiposImagenQueries _tipoImagenesQueries;

        public TiposImagenesController(ITiposImagenQueries tipoImagenesQueries)
        {
            _tipoImagenesQueries = tipoImagenesQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_rtipoimagenE>> GetList(string cmpy)
        {
            return await _tipoImagenesQueries.GetList(cmpy);
        }
    }
}
