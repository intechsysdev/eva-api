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
    public class TipoDevolucionesController : ControllerBase
    {
        private readonly ITiposDevolucionQueries _tipoDevolucionesQueries;

        public TipoDevolucionesController(ITiposDevolucionQueries tipoDevolucionesQueries)
        {
            _tipoDevolucionesQueries = tipoDevolucionesQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_tipodevE>> GetList(string cmpy)
        {
            return await _tipoDevolucionesQueries.GetList(cmpy);
        }
    }
}
