using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Persistence.DataBase;
using Migracion.Service.Queries;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RecibosCajasController : ControllerBase
    {
        private readonly IRecibosCajasQueries _recibosCajasQueries;

        public RecibosCajasController(IRecibosCajasQueries recibosCajasQueries)
        {
            _recibosCajasQueries = recibosCajasQueries;
        }

        [HttpGet]
        [Route("GetListProntoPago/{cmpy}/{reg}/{teri}/{num}/{entdt}")]
        public async Task<List<mob_rcashrecpE>> GetListProntoPago(string cmpy, string reg, string teri, string num, string entdt)
        {
            return await _recibosCajasQueries.GetListProntoPago(cmpy, reg, teri, num, entdt);
            
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{reg}/{teri}/{num}/{inv}/{entdt}")]
        public async Task<List<mob_rcashapplE>> GetList(string cmpy, string reg, string teri, string num, string inv, string entdt)
        {
            return await _recibosCajasQueries.GetList(cmpy, reg, teri, num, inv, entdt);
            
        }

        [HttpPost]
        [Route("InsertProntoPago")]
        public int InsertProntoPago(mob_rcashrecpE prontoPago)
        {
            return _recibosCajasQueries.InsertProntoPago(prontoPago);
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rcashapplE recibo)
        {
            return _recibosCajasQueries.Insert(recibo);
        }

        [HttpGet]
        [Route("GenerarReciboCajaPdf/{cmpy}/{reg}/{teri}/{num}/{cust}/{succli}")]
        public async Task<IActionResult> GenerarReciboCajaPdf(string cmpy, string reg, string teri, string num, string cust, string succli)
        {
            try
            {
                var pdfBytes = await _recibosCajasQueries.GenerarReciboCajaPdf(cmpy, reg, teri, num, cust, succli);
                return File(pdfBytes, "application/pdf", $"ReciboCaja_{cmpy}_{num}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
