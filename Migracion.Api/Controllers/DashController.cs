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
    public class DashController : ControllerBase
    {
        private readonly IDashboardQueries _dashQueries;

        public DashController(IDashboardQueries dashQueries)
        {
            _dashQueries = dashQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/user/{codusu}")]
        public async Task<List<dash_E>> GetList(string cmpy,string codusu)
        {
            return await _dashQueries.GetList(cmpy, codusu);
        }

    }
}
