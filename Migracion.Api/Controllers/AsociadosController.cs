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
    public class AsociadosController : ControllerBase
    {
        private readonly IAsociadoQueries _asociadoQueries;

        public AsociadosController(IAsociadoQueries asociadoQueries)
        {
            _asociadoQueries = asociadoQueries;
        }

        [HttpGet]
        [Route("GetListByCodUsu/{cmpy}/{codUsu}")]
        public async Task<List<mob_rasociadoE>> GetListByCodUsu(string cmpy, string codUsu)
        {
            return await _asociadoQueries.GetListByCodUsu(cmpy, codUsu);
            
        }

        [HttpGet]
        [Route("GetListByReg/{cmpy}/{reg}")]
        public async Task<List<mob_rasociadoE>> GetListByReg(string cmpy, string reg)
        {
            return await _asociadoQueries.GetListByReg(cmpy, reg);
            
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rasociadoE asociado)
        {
            return _asociadoQueries.InsertAsociado(asociado);
        }

        [HttpPut]
        [Route("Update")]
        public int Update(mob_rasociadoE asociado)
        {
            return _asociadoQueries.Update(asociado);
        }

        [HttpDelete]
        [Route("Delete/{cmpy}/{cust}/{succli}")]
        public int Delete(string cmpy, string cust, string succli)
        {
            return _asociadoQueries.Delete(cmpy, cust, succli);
        }

        [HttpDelete]
        [Route("DeleteContact/{cmpy}/{custd}/{succli}/{codcon}")]
        public int DeleteContact(string cmpy, string custd, string succli, string codcon)
        {
            return _asociadoQueries.DeleteContact(cmpy, custd, succli, codcon);
        }

        [HttpDelete]
        [Route("DeleteDepositContact/{cmpy}/{custd}/{codcon}")]
        public int DeleteContact(string cmpy, string custd, string codcon)
        {
            return _asociadoQueries.DeleteDepositContact(cmpy, custd, codcon);
        }

    }
}
