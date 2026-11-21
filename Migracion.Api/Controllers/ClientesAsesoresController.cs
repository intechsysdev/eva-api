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
    public class ClientesAsesoresController : ControllerBase
    {
        private readonly IClientesAsesoresQueries _clientesAsesoresQueries;

        public ClientesAsesoresController(IClientesAsesoresQueries clientesAsesoresQueries)
        {
            _clientesAsesoresQueries = clientesAsesoresQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{codusu}/{ruta}")]
        public async Task<List<mob_rcustasesE>> GetList(string cmpy, string codusu, string ruta)
        {
            return await _clientesAsesoresQueries.GetList(cmpy, codusu, ruta);
            
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rcustasesE asesor)
        {
            return _clientesAsesoresQueries.Insert(asesor);
        }
    }
}
