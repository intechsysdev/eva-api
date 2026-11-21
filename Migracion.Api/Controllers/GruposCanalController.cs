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
    public class GruposCanalController : ControllerBase
    {
        private readonly IGruposCanalQueries _gruposCanalQueries;

        public GruposCanalController(IGruposCanalQueries gruposCanalQueries)
        {
            _gruposCanalQueries = gruposCanalQueries;
        }
        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<grupo_canalE>> GetList(string cmpy)
        {
            return await _gruposCanalQueries.GetList(cmpy);
            

        }

        [HttpGet]
        [Route("GetListByIdentity/{code}")]
        public async Task<List<grupo_canalE>> GetListByIdentity(string code)
        {
            return await _gruposCanalQueries.GetListByIdentity(code);
        }


    }
}
