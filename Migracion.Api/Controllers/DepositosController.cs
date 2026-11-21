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
    public class DepositosController : ControllerBase
    {
        private readonly IDepositosQueries _depositosQueries;

        public DepositosController(IDepositosQueries depositosQueries)
        {
            _depositosQueries = depositosQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_depositosE>> GetList(string cmpy)
        {
            return await _depositosQueries.GetList(cmpy);
            
        }
    }
}
