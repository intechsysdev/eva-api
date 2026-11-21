using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Persistence.DataBase;
using Migracion.Service.Queries;
using Migracion.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using Migracion.Api.Models;
using System.Linq;

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RutasController : ControllerBase
    {
        private readonly IRutasQueries _rutasQueries;

        public RutasController(IRutasQueries rutasQueries)
        {
            _rutasQueries = rutasQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{codUsu}")]
        public async Task<List<mob_rrutasE>> GetList(string cmpy, string codUsu)
        {
            return await _rutasQueries.GetList(cmpy, codUsu);
            
        }

        [HttpPost]
        [Route("GetDetailDataCliente")]
        public async Task<List<mob_rrutasDto>> GetDetailDataCliente([FromBody] FiltersRutaDto filters)
        {
            List<mob_rrutasDto> lresult = await _rutasQueries.GetDetailDataCliente(filters.cmpy, filters.codUsu, filters.rruta_ruta, filters.fecRuta);
            
            if (filters.codPer.Length > 0)
            {
                lresult = lresult.Where(x => filters.codPer.Contains(x.rusu_codper)).ToList();
            }
            return lresult;
        }

        [HttpGet]
        [Route("GetListByFecha/{cmpy}/{fecRuta}/{codUsu}")]
        public async Task<List<mob_rrutasE>> GetListByFecha(string cmpy, string fecRuta, string codUsu)
        {
            return await _rutasQueries.GetListByFecha(cmpy, fecRuta, codUsu);
            
        }

        [HttpGet]
        [Route("GetListDistinct/{cmpy}")]
        public List<int> GetListDistinct(string cmpy)
        {
            var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(_rutasQueries.GetListDistinct(cmpy)), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
            return JsonSerializer.Deserialize<List<int>>(responseJson);
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rrutasE ruta)
        {
            return _rutasQueries.Insert(ruta);
        }

        [HttpDelete]
        [Route("Delete/{cmpy}/{cus}/{succli}/{codUsu}")]
        public int Delete(string cmpy, string cus, string succli, string codUsu)
        {
            return _rutasQueries.Delete(cmpy, cus, succli, codUsu);
        }
    }
}
