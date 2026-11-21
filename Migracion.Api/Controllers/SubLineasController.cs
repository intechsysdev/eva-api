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
    public class SubLineasController : ControllerBase
    {
        private readonly ISubLineasQueries _sublineasQueries;

        public SubLineasController(ISubLineasQueries sublineasQueries)
        {
            _sublineasQueries = sublineasQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_rsublineasE>> GetList(string cmpy)
        {
            return await _sublineasQueries.GetList(cmpy);
            
        }

        [HttpGet]
        [Route("GetListByLinea/{cmpy}/{linea}")]
        public async Task<List<mob_rsublineasE>> GetListByLinea(string cmpy, string linea)
        {
            return await _sublineasQueries.GetListByLinea(cmpy, linea);
        }

    }
}

