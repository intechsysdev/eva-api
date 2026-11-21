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
    public class PresupuestosController : ControllerBase
    {
        private readonly IPresupuestosQueries _presupuestosQueries;

        public PresupuestosController(IPresupuestosQueries presupuestosQueries)
        {
            _presupuestosQueries = presupuestosQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{codUsu}")]
        public async Task<List<mob_restzonaE>> GetList(string cmpy, string codUsu)
        {
            return await _presupuestosQueries.GetListByCodUsu(cmpy, codUsu);
            
        }
    }
}
