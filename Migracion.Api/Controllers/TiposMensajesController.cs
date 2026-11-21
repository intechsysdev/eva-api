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
    public class TiposMensajesController : ControllerBase
    {
        private readonly ITiposMensajesQueries _tiposMensajesQueries;

        public TiposMensajesController(ITiposMensajesQueries tiposMensajesQueries)
        {
            _tiposMensajesQueries = tiposMensajesQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_tipomensajesE>> GetList(string cmpy)
        {
            return await _tiposMensajesQueries.GetList(cmpy);
        }
    }
}
