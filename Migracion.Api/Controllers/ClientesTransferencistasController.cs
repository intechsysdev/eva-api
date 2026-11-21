using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models.Command;
using Migracion.Models.Dtos;
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
    public class ClientesTransferencistasController : ControllerBase
    {
        private readonly ITransferencistasQueries _clientestranferencistasQueries;

        public ClientesTransferencistasController(ITransferencistasQueries clientestranferencistasQueries)
        {
            _clientestranferencistasQueries = clientestranferencistasQueries;
        }

        [HttpGet]
        [Route("GetListByCodUsu/{cmpy}/{codUsu}")]
        public async Task<List<mob_rcustransDto>> GetListByCodUsu(string cmpy, string codUsu)
        {
            return await _clientestranferencistasQueries.GetListByCodUsu(cmpy, codUsu);
            
        }

        [HttpGet]
        [Route("GetListByRuta/{cmpy}/{codUsu}/{codRuta}")]
        public async Task<List<mob_rcustransDto>> GetListByRuta(string cmpy, string codUsu, string codRuta)
        {
            return await _clientestranferencistasQueries.GetListByRuta(cmpy, codUsu, codRuta);
            
        }

        [HttpGet]
        [Route("GetListByTipoDoc/{cmpy}/{cus}/{succli}/{tipoDoc}")]
        public async Task<List<mob_rcustransDto>> GetListByTipoDoc(string cmpy, string cus, string succli, string tipoDoc)
        {
            return await _clientestranferencistasQueries.GetListByTipoDoc(cmpy, cus, succli, tipoDoc);
            
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rcustransCommand item)
        {
            return _clientestranferencistasQueries.Insert(item);
        }

        [HttpPut]
        [Route("Update")]
        public int Update(mob_rcustransCommand client)
        {
            return _clientestranferencistasQueries.Update(client);
        }

        [HttpDelete]
        [Route("Delete")]
        public int Delete(string cmpy, string cust, string succli, string codusu)
        {
            return _clientestranferencistasQueries.Delete(cmpy, cust, succli, codusu);
        }
    }
}
