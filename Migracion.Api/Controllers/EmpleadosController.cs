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
    public class EmpleadosController : ControllerBase
    {
        private readonly IEmpleadosQueries _empleadosQueries;

        public EmpleadosController(IEmpleadosQueries empleadosQueries)
        {
            _empleadosQueries = empleadosQueries;
        }

        [HttpGet]
        [Route("GetList/{empleado}")]
        public async Task<List<EmpleadosE>> GetList(string empleado)
        {
            return await _empleadosQueries.GetList(empleado);
            
        }
    }
}
