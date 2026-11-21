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
    public class PerfilesController : ControllerBase
    {
        private readonly IPerfilesQueries _perfilesQueries;

        public PerfilesController(IPerfilesQueries perfilesQueries)
        {
            _perfilesQueries = perfilesQueries;
        }

        [HttpGet]
        [Route("GetList")]
        public async Task<List<mob_rperfilesE>> GetList()
        {
            return await _perfilesQueries.GetList();
        }

        [HttpGet]
        [Route("GetListContacts/{cmpy}")]
        public async Task<List<mob_rperfilcontactoE>> GetListContacts(string cmpy)
        {
            return await _perfilesQueries.GetListContacts(cmpy);
            
        }

    }
}
