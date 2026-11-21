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
    public class CiudadesController : ControllerBase
    {
        private readonly IDistrictQueries _districtQueries;

        public CiudadesController(IDistrictQueries districtQueries)
        {
            _districtQueries = districtQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_districtE>> GetList(string cmpy)
        {
            return await _districtQueries.GetList(cmpy);
            
        }
    }
}
