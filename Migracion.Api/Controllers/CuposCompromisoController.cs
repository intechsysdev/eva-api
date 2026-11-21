using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Models.Dtos;
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
    public class CuposCompromisoController : ControllerBase
    {
        private readonly ICuposCompromisoQueries _cuposCompromisoQueries;

        public CuposCompromisoController(ICuposCompromisoQueries cuposCompromisoQueries)
        {
            _cuposCompromisoQueries = cuposCompromisoQueries;
        }

        [HttpGet]
        [Route("GetByClient/{cmpy}/{teri}/{reg}/{cust}/{ship}/{num}")]
        public async Task<List<mob_cupocompromisoE>> GetByClient(string cmpy, string teri, string reg, string cust, string ship, string num)
        {

            return await _cuposCompromisoQueries.GetByClient(cmpy, teri, reg, cust, ship, num);
            
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_cupocompromisoE cupocompromiso)
        {
            return _cuposCompromisoQueries.Insert(cupocompromiso);
        }

        [HttpPut]
        [Route("Update")]
        public int Update(mob_cupocompromisoE item)
        {
            return _cuposCompromisoQueries.Update(item);
        }

        [HttpDelete]
        [Route("Delete/{cmpy}/{num}/{reg}/{teri}")]
        public int Delete(string cmpy, string num, string reg, string teri)
        {
            return _cuposCompromisoQueries.Delete(cmpy, num, reg, teri);
        }




    }
}
