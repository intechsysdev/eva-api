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
    public class ChequePostfechadoController : ControllerBase
    {
        private readonly IChequePosfechadoQueries _chequePosfechadoQueries;

        public ChequePostfechadoController(IChequePosfechadoQueries chequePosfechadoQueries)
        {
            _chequePosfechadoQueries = chequePosfechadoQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{reg}/{teri}/{num}/{entdt}")]
        public async Task<List<mob_rrcheckposE>> GetList(string cmpy, string reg, string teri, string num, string entdt)
        {
            return await _chequePosfechadoQueries.GetList(cmpy, reg, teri, num, entdt);
            
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rrcheckposE cheque)
        {
            return _chequePosfechadoQueries.Insert(cheque);
        }
    }
}
