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
    public class CuotaVentasController : ControllerBase
    {
        private readonly ICuotasAsesoriasQueries _cuotasAsesoriasQueries;

        public CuotaVentasController(ICuotasAsesoriasQueries cuotasAsesoriasQueries)
        {
            _cuotasAsesoriasQueries = cuotasAsesoriasQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{codusu}")]
        public async Task<List<mob_rcliproE>> GetList(string cmpy, string codusu)
        {
            return await _cuotasAsesoriasQueries.GetList(cmpy, codusu);
            
        }
    }
}
