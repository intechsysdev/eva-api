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
    public class FrecuenciaVisitasController : ControllerBase
    {
        private readonly IFrecuenciaVisitaQueries _frecuenciaQueries;

        public FrecuenciaVisitasController(IFrecuenciaVisitaQueries frecuenciaQueries)
        {
            _frecuenciaQueries = frecuenciaQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_rfrecuenciavisitaE>> GetList(string cmpy)
        {
            return await _frecuenciaQueries.GetList(cmpy);
            
        }
    }
}
