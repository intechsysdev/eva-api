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
    public class LineasController : ControllerBase
    {
        private readonly ILineasQueries _lineasQueries;

        public LineasController(ILineasQueries lineasQueries)
        {
            _lineasQueries = lineasQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_rlineaE>> GetList(string cmpy)
        {
            return await _lineasQueries.GetList(cmpy);
            
        }
    }
}
