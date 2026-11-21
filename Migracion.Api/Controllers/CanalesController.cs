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
    public class CanalesController : ControllerBase
    {
        private readonly ICanalesQueries _canalesQueries;

        public CanalesController(ICanalesQueries canalesQueries)
        {
            _canalesQueries = canalesQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_rcusttypeE>> GetList(string cmpy)
        {
            return await _canalesQueries.GetList(cmpy);
            
        }
    }
}
