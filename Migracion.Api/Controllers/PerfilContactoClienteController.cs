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
    public class PerfilContactoClienteController : ControllerBase
    {
        private readonly IConPerClienteQueries _conperClientesQueries;

        public PerfilContactoClienteController(IConPerClienteQueries conperClientesQueries)
        {
            _conperClientesQueries = conperClientesQueries;
        }

        [HttpGet]
        [Route("GetListByReg/{cmpy}/{codUsu}")]
        public async Task<List<mob_rconperclienteE>> GetList(string cmpy, string codUsu)
        {
            return await _conperClientesQueries.GetList(cmpy, codUsu);
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rconperclienteCommand contacto)
        {
            return _conperClientesQueries.Insert(contacto);
        }

        [HttpDelete]
        [Route("Delete/{cmpy}/{cus}/{succli}")]
        public int Delete(string cmpy, string cus, string succli)
        {
            return _conperClientesQueries.Delete(cmpy, cus, succli);
        }

        [HttpDelete]
        [Route("DeleteByContacto/{cmpy}/{cus}/{succli}/{codcon}")]
        public int DeleteByContacto(string cmpy, string cus, string succli, string codcon)
        {
            return _conperClientesQueries.DeleteByContacto(cmpy, cus, succli, codcon);
        }

        [HttpDelete]
        [Route("DeleteByPerfil/{cmpy}/{cus}/{codcon}/{codper}")]
        public int DeleteByPerfil(string cmpy, string cus, string codcon, string codper)
        {
            return _conperClientesQueries.DeleteByPerfil(cmpy, cus, codcon, codper);
        }
    }
}
