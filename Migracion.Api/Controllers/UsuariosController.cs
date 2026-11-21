using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Persistence.DataBase;
using Migracion.Service.Queries;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosQueries _usuariosQueries;

        public UsuariosController(IUsuariosQueries usuariosQueries)
        {
            _usuariosQueries = usuariosQueries;
        }

        [HttpGet]
        [Route("GetList/{codusu}/{passwd}/{codper}")]
        public async Task<List<mob_rusuariosE>> GetList(string codusu, string passwd, string codper)
        {
            return await _usuariosQueries.GetList(codusu, passwd, codper);
            
        }

        [HttpGet]
        [Route("GetListCmpy/{cmpy}/{codusu}/{codper}")]
        public async Task<List<mob_rusuariosE>> GetListCmpy(string cmpy, string codusu, string codper)
        {
            return await _usuariosQueries.GetListCmpy(cmpy, codusu, codper);
            
        }

        [HttpPut]
        [Route("UpdateDetail")]
        public int UpdateDetail(mob_rusuariosE usuario)
        {
            return _usuariosQueries.Update(usuario);
        }

        [HttpGet]
        [Route("GetListAll/{cmpy}/{codper}")]
        public async Task<List<mob_rusuariosE>> GetListAll(string cmpy, string codper)
        {
            return await _usuariosQueries.GetListAll(cmpy, codper);
            
        }

        [HttpGet]
        [Route("GetCodUsuByCed/{cmpy}/{ced}")]
        public async Task<mob_rusuariosE> GetCodUsuByCed(string cmpy, string ced)
        {
            return await _usuariosQueries.GetCodUsuByCed(cmpy, ced);
        }
    }
}
