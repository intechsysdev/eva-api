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
    public class DevolucionesController : ControllerBase
    {

        private readonly IDevolucionesQueries _devolucionesQueries;

        public DevolucionesController(IDevolucionesQueries devolucionesQueries)
        {
            _devolucionesQueries = devolucionesQueries;
        }

        [HttpGet]
        [Route("GetByReg/{cmpy}/{teri}/{reg}/{cust}/{ship}/{num}")]
        public async Task<List<mob_rrmaheadE>> GetByReg(string cmpy, string teri, string reg, string cust, string ship, string num)
        {
            return await _devolucionesQueries.GetByReg(cmpy, teri, reg, cust, ship, num);
            
        }

        [HttpGet]
        [Route("GetByDate/{cmpy}/{teri}/{reg}/{num}/{date}")]
        public async Task<List<mob_rrmaheadE>> GetByDate(string cmpy, string teri, string reg, string num, string date)
        {
            return await _devolucionesQueries.GetByDate(cmpy, teri, reg, num, date);
            
        }

        [HttpGet]
        [Route("GetByConsec/{cmpy}/{teri}/{reg}/{num}")]
        public async Task<List<mob_rrmaheadE>> GetByConsec(string cmpy, string teri, string reg, string num)
        {
            return await _devolucionesQueries.GetByConsec(cmpy, teri, reg, num);
            
        }

        [HttpGet]
        [Route("GetDetail/{cmpy}/{reg}/{teri}/{num}/{part}/{level}/{entdt}")]
        public async Task<List<mob_rrmalineE>> GetDetail(string cmpy, string reg, string teri, string num, string part, string level, string entdt)
        {
            return await _devolucionesQueries.GetDetail(cmpy, reg, teri, num, part, level, entdt);
            
        }

        [HttpGet]
        [Route("GetDetailByProduct/{cmpy}/{reg}/{teri}/{num}/{part}")]
        public async Task<List<mob_rrmalineE>> GetDetailByProduct(string cmpy, string reg, string teri, string num, string part)
        {
            return await _devolucionesQueries.GetDetailByProduct(cmpy, reg, teri, num, part);
            
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rrmaheadE devolucion)
        {
            return _devolucionesQueries.Insert(devolucion);
        }

        [HttpPost]
        [Route("InsertDetail")]
        public int InsertDetail(mob_rrmalineE detalle)
        {
            return _devolucionesQueries.InsertDetail(detalle);
        }

        [HttpPut]
        [Route("Update")]
        public int Update(mob_rrmaheadE item)
        {
            return _devolucionesQueries.Update(item);
        }

        [HttpPut]
        [Route("UpdateDetail")]
        public int UpdateDetail(mob_rrmalineE item)
        {
            return _devolucionesQueries.UpdateDetail(item);
        }

        [HttpDelete]
        [Route("Delete/{cmpy}/{num}/{reg}/{teri}")]
        public int Delete(string cmpy, string num, string reg, string teri)
        {
            return _devolucionesQueries.Delete(cmpy, num, reg, teri);
        }

        [HttpDelete]
        [Route("DeleteDetailByProd/{cmpy}/{num}/{reg}/{teri}/{part}")]
        public int DeleteDetailByProd(string cmpy, string num, string reg, string teri, string part)
        {
            return _devolucionesQueries.DeleteDetailByProd(cmpy, num, reg, teri, part);
        }

        [HttpDelete]
        [Route("DeleteDetail/{cmpy}/{num}/{reg}/{teri}")]
        public int DeleteDetail(string cmpy, string num, string reg, string teri)
        {
            return _devolucionesQueries.DeleteDetail(cmpy, num, reg, teri);
        }

    }
}

