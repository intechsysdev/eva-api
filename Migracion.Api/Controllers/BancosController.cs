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
    public class BancosController : ControllerBase
    {
        private readonly IBancoQueries _BancosQueries;

        public BancosController(IBancoQueries BancosQueries)
        {
            _BancosQueries = BancosQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_rbancosE>> GetList(string cmpy)
        {
            return await _BancosQueries.GetList(cmpy);
        }

        [HttpGet]
        [Route("GetListConsignaciones/{cmpy}")]
        public async Task<List<mob_bankacctE>> GetListConsignaciones(string cmpy)
        {
            return await _BancosQueries.GetListConsignaciones(cmpy);
            
        }
    }
}
