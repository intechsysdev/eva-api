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
    public class TransferenciasController : ControllerBase
    {
        private readonly ITransferenciasQueries _transferenciasQueries;

        public TransferenciasController(ITransferenciasQueries transferenciasQueries)
        {
            _transferenciasQueries = transferenciasQueries;
        }

        [HttpGet]
        [Route("GetListDetail/{cmpy}/{codUsu}/{tipo}/{id}/{num}")]
        public async Task<List<mob_rtransdetE>> GetListDetail(string cmpy, string codUsu, string tipo, string id, string num)
        {
            return await _transferenciasQueries.GetListDetail(cmpy, codUsu, tipo, id, num);
            
        }

        [HttpGet]
        [Route("GetListDetailHis/{cmpy}/{codUsu}/{num}/{tipo}")]
        public async Task<List<mob_rtransdet_hisE>> GetListDetailHis(string cmpy, string codUsu, string num, string tipo)
        {
            return await _transferenciasQueries.GetListDetailHis(cmpy, codUsu, num, tipo);
            
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{codUsu}/{tipo}/{num}")]
        public async Task<List<mob_rtransferenciaE>> GetList(string cmpy, string codUsu, string tipo, string num)
        {
            return await _transferenciasQueries.GetList(cmpy, codUsu, tipo, num);
            
        }

        [HttpGet]
        [Route("GetListHis/{numero}/{cmpy}/{codUsu}/{tipo}/{cus}/{succli}/{dep}")]
        public async Task<List<mob_rtransferencia_hisE>> GetListHis(string numero,string cmpy, string codUsu, string tipo, string cus, string succli, string dep)
        {
            return await _transferenciasQueries.GetListHis(numero, cmpy, codUsu, tipo, cus, succli, dep);
            
        }

        [HttpGet]
        [Route("GetFullListHis/{numero}/{cmpy}/{codUsu}/{tipo}/{cus}/{succli}/{dep}")]
        public async Task<List<mob_transferencia_full_dto>> GetFullListHis(string numero, string cmpy, string codUsu, string tipo, string cus, string succli, string dep)
        {
            return await _transferenciasQueries.GetFullHis(numero, cmpy, codUsu, tipo, cus, succli, dep);

        }

        [HttpGet]
        [Route("GetListDeposito/{codUsu}")]
        public async Task<List<mob_rtransferenciahisE>> GetListDeposito(string codUsu)
        {
            return await _transferenciasQueries.GetListHisDeposit(codUsu);
            
        }

        [HttpPost]
        [Route("InsertDetail")]
        public int InsertDetalle(mob_rtransdetCommand detalle)
        {
            return _transferenciasQueries.InsertDetail(detalle);
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rtransferenciaE transferencia)
        {
            return _transferenciasQueries.Insert(transferencia);
        }

        [HttpPut]
        [Route("UpdateDetail")]
        public int UpdateDetalle(mob_rtransdetCommand detalle)
        {
            return _transferenciasQueries.UpdateDetail(detalle);
        }

        [HttpPut]
        [Route("Update")]
        public int Update(mob_rtransferenciaE transferencia)
        {
            return _transferenciasQueries.Update(transferencia);
        }

        [HttpDelete]
        [Route("Delete/{cmpy}/{num}/{tipo}/{codUsu}")]
        public int Delete(string cmpy, string num, string tipo, string codusu)
        {
            return _transferenciasQueries.Delete(cmpy, num, tipo, codusu);
        }


        [HttpDelete]
        [Route("DeleteDetailByProd/{cmpy}/{num}/{tipo}/{codUsu}/{id}")]
        public int DeleteDetailByProd(string cmpy, string num, string tipo, string codusu, string id)
        {
            return _transferenciasQueries.DeleteDetailByProd(cmpy, num, tipo, codusu, id);
        }

        [HttpDelete]
        [Route("DeleteDetail/{cmpy}/{num}/{tipo}/{codUsu}")]
        public int DeleteDetail(string cmpy, string num, string tipo, string codusu)
        {
            return _transferenciasQueries.DeleteDetail(cmpy, num, tipo, codusu);
        }
    }
}
