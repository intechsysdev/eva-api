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
    public class ConsecutivosController : ControllerBase
    {
        private readonly IConsecutivosQueries _consecutivosQueries;

        public ConsecutivosController(IConsecutivosQueries consecutivosQueries)
        {
            _consecutivosQueries = consecutivosQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{codUsu}")]
        public async Task<List<mob_rconsecutivosE>> GetList(string cmpy, string codUsu)
        {
            return await _consecutivosQueries.GetList(cmpy, codUsu);
            
        }

        [HttpPut]
        [Route("Update")]
        public int Update(mob_rconsecutivosE consecutivo)
        {
            return _consecutivosQueries.Update(consecutivo);
        }

        [HttpGet]
        [Route("GetLastConsecutive/{codDoc}/{codUsu}/{reg}/{teri}")]
        public async Task<string> GetLastConsecutive(string codDoc, string codUsu, string reg, string teri)
        {
            return await _consecutivosQueries.GetLastConsecutive(codDoc, codUsu, reg, teri);
        }
    }
}
