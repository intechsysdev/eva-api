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
    public class MenuPpalController : ControllerBase
    {
        private readonly IMenuPpalQueries _menuppalQueries;

        public MenuPpalController(IMenuPpalQueries menuppalQueries)
        {
            _menuppalQueries = menuppalQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{codper}/{idioma}")]
        public async Task<List<mob_rmenuppalE>> GetList(string cmpy, string codper, string idioma)
        {
            return await _menuppalQueries.GetList(cmpy, codper, idioma);
        }
        ///TODO: Terminar correctamente (parametros en el where)
    }
}
