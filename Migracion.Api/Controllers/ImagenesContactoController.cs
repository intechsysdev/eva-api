using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Models.Command;
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
    public class ImagenesContactoController : ControllerBase
    {
        private readonly IContactImageQueries _contactImageQueries;

        public ImagenesContactoController(IContactImageQueries contactImageQueries)
        {
            _contactImageQueries = contactImageQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{codUsu}")]
        public async Task<List<mob_rcontacimageE>> GetList(string cmpy, string codUsu)
        {
            return await _contactImageQueries.GetList(cmpy, codUsu);
            
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rcontacimageCommand contacto)
        {
            return _contactImageQueries.Insert(contacto);
        }

        [HttpDelete]
        [Route("Delete/{cmpy}/{codtip}/{codcon}")]
        public int Delete(string cmpy, string codtip, string codcon)
        {
            return _contactImageQueries.Delete(cmpy, codtip, codcon);
        }
    }
}
