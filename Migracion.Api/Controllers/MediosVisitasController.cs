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
    public class MediosVisitasController : ControllerBase
    {
        private readonly IMediosVisitasQueries _mediosVisitaQueries;

        public MediosVisitasController(IMediosVisitasQueries mediosVisitaQueries)
        {
            _mediosVisitaQueries = mediosVisitaQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_rmediovisitaE>> GetList(string cmpy)
        {
            return await _mediosVisitaQueries.GetList(cmpy);
            
        }
    }
}
