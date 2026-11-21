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
    public class MercadosController : ControllerBase
    {
        private readonly IMercadosQueries _mercadosQueries;

        public MercadosController(IMercadosQueries mercadosQueries)
        {
            _mercadosQueries = mercadosQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_mercadoE>> GetList(string cmpy)
        {
            return await _mercadosQueries.GetList(cmpy);
            
        }

        [HttpGet]
        [Route("GetListSubmercados/{cmpy}")]
        public async Task<List<mob_submercadoE>> GetListSubmercados(string cmpy)
        {
            return await _mercadosQueries.GetListSubmercados(cmpy);
            
        }
    }
}
