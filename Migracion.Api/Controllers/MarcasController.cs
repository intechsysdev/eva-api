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
    public class MarcasController : ControllerBase
    {
        private readonly IMarcasQueries _marcasQueries;

        public MarcasController(IMarcasQueries marcasQueries)
        {
            _marcasQueries = marcasQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_marcaE>> GetList(string cmpy)
        {
            return await _marcasQueries.GetList(cmpy);
            
        }
    }
}
