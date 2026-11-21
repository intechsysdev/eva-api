using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Persistence.DataBase;
using Migracion.Service.Queries;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ConsignacionesController : ControllerBase
    {
        private readonly IConsignacionesQueries _consignacionesQueries;

        public ConsignacionesController(IConsignacionesQueries consignacionesQueries)
        {
            _consignacionesQueries = consignacionesQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{reg}/{consec}/{teri}")]
        public async Task<List<mob_rconsignacionE>> GetList(string cmpy, string reg, int consec, string teri)
        {
            return await _consignacionesQueries.GetList(cmpy, reg, consec, teri);
            
        }

        [HttpGet]
        [Route("GetListByFec/{cmpy}/{reg}/{consec}/{teri}/{feceva}")]
        public async Task<List<mob_rconsignacionE>> GetListByFec(string cmpy, string reg, int consec, string teri, string feceva)
        {
            return await _consignacionesQueries.GetListByFec(cmpy, reg, consec, teri, feceva);
            
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rconsignacionE consignacion)
        {
            return _consignacionesQueries.Insert(consignacion);
        }

        [HttpPut]
        [Route("Update")]
        public int Update(mob_rconsignacionE consignacion)
        {
            return _consignacionesQueries.Update(consignacion);
        }

        [HttpDelete]
        [Route("Delete/{cmpy}/{consec}/{reg}/{teri}")]
        public int Delete(string cmpy, string consec, string reg, string teri)
        {
            return _consignacionesQueries.Delete(cmpy, consec, reg, teri);
        }
    }
}
