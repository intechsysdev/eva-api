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
    public class NotasCreditosController : ControllerBase
    {
        private readonly INotasCreditosQueries _notasCreditosQueries;

        public NotasCreditosController(INotasCreditosQueries notasCreditosQueries)
        {
            _notasCreditosQueries = notasCreditosQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{reg}/{inv}/{num}/{teri}/{date}")]
        public async Task<List<mob_rcredapplE>> GetList(string cmpy, string reg, string inv, string num, string teri, string date)
        {
            return await _notasCreditosQueries.GetList(cmpy, reg, inv, num, teri, date);
            
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rcredapplE notaCredito)
        {
            return _notasCreditosQueries.Insert(notaCredito);
        }
    }
}
