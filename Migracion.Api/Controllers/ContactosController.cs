using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
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
    public class ContactosController : ControllerBase
    {
        private readonly IContactosQueries _contactQueries;

        public ContactosController(IContactosQueries contactQueries)
        {
            _contactQueries = contactQueries;
        }

        [HttpGet]
        [Route("GetListByCodUsu/{cmpy}/{codUsu}")]
        public async Task<List<mob_rcontactosE>> GetListByCodUsu(string cmpy, string codUsu)
        {
            return await _contactQueries.GetListByCodUsu(cmpy, codUsu);
            
        }

        [HttpGet]
        [Route("GetContact/{cmpy}/{codcon}/{tipodoc}")]
        public async Task<List<mob_rcontactosE>> GetContact(string cmpy, string codcon, string tipodoc)
        {
            return await _contactQueries.GetContact(cmpy, codcon, tipodoc);
            
        }
        [HttpGet]
        [Route("GetListByClient/{cmpy}/{codUsu}")]
        public async Task<List<mob_rcontactoclienteE>> GetListByClient(string cmpy, string codUsu)
        {
            return await _contactQueries.GetListByClient(cmpy, codUsu);
            
        }

        [HttpPost]
        [Route("InsertClients")]
        public int InsertClients(mob_rcontactoclienteCommand item)
        {
            return _contactQueries.InsertClients(item);
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rcontactoCommand item)
        {
            return _contactQueries.Insert(item);
        }

        [HttpPut]
        [Route("UpdateCliente")]
        public int UpdateCliente(mob_rcontactoclienteCommand contactoCliente)
        {
            return _contactQueries.UpdateCliente(contactoCliente);
        }

        [HttpPut]
        [Route("UpdateByTipoDoc")]
        public int UpdateByTipoDoc(mob_rcontactoCommand contacto)
        {
            return _contactQueries.UpdateByTipoDoc(contacto);
        }

        [HttpPut]
        [Route("Update")]
        public int Update(mob_rcontactoCommand contacto)
        {
            return _contactQueries.Update(contacto);
        }

        [HttpDelete]
        [Route("Delete/{cmpy}/{cus}/{succli}")]
        public int Delete(string cmpy, string cus, string succli)
        {
            return _contactQueries.Delete(cmpy, cus, succli);
        }

        [HttpDelete]
        [Route("DeleteByContact/{cmpy}/{cus}/{succli}/{codcon}/{tipdoc}")]
        public int DeleteByContact(string cmpy, string cus, string succli, string codcon, string tipdoc)
        {
            return _contactQueries.DeleteByContact(cmpy, cus, succli, codcon, tipdoc);
        }
    }
}
