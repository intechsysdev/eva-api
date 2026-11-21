using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Persistence.DataBase;
using Migracion.Service.Queries;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ReglasProntopagoController : ControllerBase
    {

        private readonly IReglasProntopagoQueries _reglasProntopagoQueries;

        public ReglasProntopagoController(IReglasProntopagoQueries reglasProntopagoQueries)
        {
            _reglasProntopagoQueries = reglasProntopagoQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_reglasprontopagoE>> GetList(string cmpy)
        {
            return await _reglasProntopagoQueries.GetList(cmpy);
            
        }
    }
}
