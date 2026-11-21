using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Persistence.DataBase;
using Migracion.Service.Queries;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MensajesController : ControllerBase
    {
        private readonly IMensajesQueries _mensajesQueries;

        public MensajesController(IMensajesQueries mensajesQueries)
        {
            _mensajesQueries = mensajesQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{codusu}/{consec}/{fecha}")]
        public async Task<List<mob_rmensajesE>> GetList(string cmpy, string codusu, string consec, string fecha)
        {
            return await _mensajesQueries.GetList(cmpy, codusu, consec, fecha);
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rmensajesE mensaje)
        {
            return _mensajesQueries.Insert(mensaje);
        }
    }
}
