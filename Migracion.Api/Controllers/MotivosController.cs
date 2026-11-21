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
    public class MotivosController : ControllerBase
    {
        private readonly IMotivosQueries _motivosQueries;

        public MotivosController(IMotivosQueries motivosQueries)
        {
            _motivosQueries = motivosQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_motivosE>> GetList(string cmpy)
        {
            return await _motivosQueries.GetList(cmpy);
            
        }
    }
}
