using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Persistence.DataBase;
using Migracion.Service.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TiposMovimientosController : ControllerBase
    {
        private readonly ITipoMovimientoQueries _tipoMovimientosQueries;

        public TiposMovimientosController(ITipoMovimientoQueries tipoMovimientosQueries)
        {
            _tipoMovimientosQueries = tipoMovimientosQueries;
        }

        [HttpGet]
        [Route("GetList")]
        public async Task<List<mob_docvisitaE>> GetList()
        {
            return await _tipoMovimientosQueries.GetList();
        }
    }
}
