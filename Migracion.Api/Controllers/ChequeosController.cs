using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Models.Command;
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
    public class ChequeosController : ControllerBase
    {
        private readonly IRCheDetalleQueries _rCheDetalleQueries;

        public ChequeosController(IRCheDetalleQueries rCheDetalleQueries)
        {
            _rCheDetalleQueries = rCheDetalleQueries;
        }

        [HttpGet]
        [Route("GetListDetail/{cmpy}/{codusu}/{consec}")]
        public async Task<List<mob_rcheqdetalleE>> GetList(string cmpy, string codusu, string consec)
        {
            return await _rCheDetalleQueries.GetList(cmpy, codusu, consec);
            
        }

        [HttpGet]
        [Route("GetListDetailByDate/{cmpy}/{codusu}/{fechaIni}/{fechaFin}")]
        public async Task<List<mob_rcheqdetalleE>> GetListDetailByDate(string cmpy, string codusu, string fechaIni, string fechaFin)
        {
            return await _rCheDetalleQueries.GetListDetailByDate(cmpy, codusu, fechaIni, fechaFin);
            
        }

        [HttpGet]
        [Route("GetListChequeos/{cmpy}/{cust}/{succli}/{consec}")]
        public async Task<List<mob_rchequeoE>> GetListChequeos(string cmpy, string cust, string succli, string consec)
        {
            return await _rCheDetalleQueries.GetListChequeos(cmpy, cust, succli, consec);
            
        }

        [HttpGet]
        [Route("GetListChequeosByDate/{cmpy}/{codusu}/{fechaIni}/{fechaFin}")]
        public async Task<List<mob_rchequeoE>> GetListChequeosByDate(string cmpy, string codusu, string fechaIni, string fechaFin)
        {
            return await _rCheDetalleQueries.GetListByDate(cmpy, codusu, fechaIni, fechaFin);
            
        }

        [HttpPost]
        [Route("InsertDetail")]
        public int InsertDetail(mob_rcheqdetalleCommand chequedetalle)
        {
            return _rCheDetalleQueries.InsertDetail(chequedetalle);
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rchequeoCommand chequeo)
        {
            return _rCheDetalleQueries.Insert(chequeo);
        }

        [HttpPut]
        [Route("UpdateDetail/{cmpy}/{codusu}")]
        public int UpdateDetail(mob_rcheqdetalleCommand chequedetalle, string cmpy, string codusu)
        {
            return _rCheDetalleQueries.UpdateDetail(chequedetalle, codusu, cmpy);
        }

        [HttpPut]
        [Route("Update")]
        public int Update(mob_rchequeoCommand chequeo)
        {
            return _rCheDetalleQueries.Update(chequeo);
        }

        [HttpDelete]
        [Route("DeleteDetailFabricante/{cmpy}/{consec}/{codusu}/{codprofab}")]
        public int DeleteDetailFabricante(string cmpy, string consec, string codusu, string codprofab)
        {
            return _rCheDetalleQueries.DeleteDetailFabricante(cmpy, consec, codusu, codprofab);
        }

        [HttpDelete]
        [Route("DeleteDetail/{cmpy}/{consec}/{codusu}")]
        public int DeleteDetail(string cmpy, string consec, string codusu)
        {
            return _rCheDetalleQueries.DeleteDetail(cmpy, consec, codusu);
        }

        [HttpDelete]
        [Route("Delete/{cmpy}/{consec}/{codusu}")]
        public int Delete(string cmpy, string consec, string codusu)
        {
            return _rCheDetalleQueries.Delete(cmpy, consec, codusu);
        }

    }
}
