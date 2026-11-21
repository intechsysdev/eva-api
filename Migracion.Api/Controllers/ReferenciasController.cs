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
    public class ReferenciasController : ControllerBase
    {
        private readonly IReferenciasQueries _referenciasQueries;

        public ReferenciasController(IReferenciasQueries referenciasQueries)
        {
            _referenciasQueries = referenciasQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_rinvclassE>> GetList(string cmpy)
        {
            return await _referenciasQueries.GetList(cmpy);
            
        }
    }
}
