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
    public class FabricantesController : ControllerBase
    {
        private readonly IFabricantesQueries _fabricantesQueries;

        public FabricantesController(IFabricantesQueries fabricantesQueries)
        {
            _fabricantesQueries = fabricantesQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_rfabricanteE>> GetList(string cmpy)
        {
            return await _fabricantesQueries.GetList(cmpy);
            
        }
    }
}
