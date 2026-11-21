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
    public class ActualizacionController : ControllerBase
    {
        private readonly IActualizacionQueries _actualizacionQueries;

        public ActualizacionController(IActualizacionQueries actualizacionQueries)
        {
            _actualizacionQueries = actualizacionQueries;
        }

        [HttpGet]
        [Route("GetList")]
        public async Task<List<mob_ractualiza>> GetList()
        {
            return await _actualizacionQueries.GetList();
            
        }
    }
}
