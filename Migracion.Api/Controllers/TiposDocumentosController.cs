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
    public class TiposDocumentosController : ControllerBase
    {
        private readonly ITiposDocumentosQueries _tiposDocumentosQueries;

        public TiposDocumentosController(ITiposDocumentosQueries tiposDocumentosQueries)
        {
            _tiposDocumentosQueries = tiposDocumentosQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_tipodocE>> GetList(string cmpy)
        {
            return await _tiposDocumentosQueries.GetList(cmpy);
        }
    }
}
