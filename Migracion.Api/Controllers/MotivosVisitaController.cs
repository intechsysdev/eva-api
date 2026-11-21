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
    public class MotivosVisitaController : ControllerBase
    {
        private readonly IMotivisQueries _motivisQueries;

        public MotivosVisitaController(IMotivisQueries motivisQueries)
        {
            _motivisQueries = motivisQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_motivisE>> GetList(string cmpy)
        {
            return await _motivisQueries.GetList(cmpy);
            
        }
    }
}
