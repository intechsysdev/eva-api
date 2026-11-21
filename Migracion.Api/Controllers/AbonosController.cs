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
    public class AbonosController : ControllerBase
    {
        private readonly IAbonosQueries _abonosQueries;

        public AbonosController(IAbonosQueries abonosQueries)
        {
            _abonosQueries = abonosQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{teri}/{reg}")]
        public async Task<List<mob_rabonosE>> GetList(string cmpy, string teri, string reg)
        {
            return await _abonosQueries.GetList(cmpy, teri, reg);
        }

        [HttpGet]
        [Route("GetExceptionList/{cmpy}/{codUsu}")]
        public async Task<List<mob_rexcepcionabonosE>> GetExceptionList(string cmpy, string codUsu)
        {
            return await _abonosQueries.GetExceptionList(cmpy, codUsu);
        }
    }
}
