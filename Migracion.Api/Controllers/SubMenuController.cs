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
    public class SubMenuController : ControllerBase
    {
        private readonly ISubMenuQueries _subMenuQueries;

        public SubMenuController(ISubMenuQueries subMenuQueries)
        {
            _subMenuQueries = subMenuQueries;
        }

        [HttpGet]
        [Route("GetListByCodUsu/{cmpy}/{codPer}/{idioma}")]
        public async Task<List<mob_rsubmenuE>> GetListByCodUsu(string cmpy, string codPer, string idioma)
        {
            return await _subMenuQueries.GetList(cmpy, codPer, idioma);
        }

    }
}
