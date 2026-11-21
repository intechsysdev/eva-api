using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Models.Dtos;
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
    public class InventariosController : ControllerBase
    {
        private readonly IInventariosQueries _inventariosQueries;

        public InventariosController(IInventariosQueries inventariosQueries)
        {
            _inventariosQueries = inventariosQueries;
        }

        [HttpGet]
        [Route("GetListByConsec/{cmpy}/{cust}/{succli}/{consec}")]
        public async Task<List<mob_rcheqinventariosE>> GetListByConsec(string cmpy, string cust, string succli, int consec)
        {
            return await _inventariosQueries.GetListByConsec(cmpy, cust, succli, consec);
            
        }

        [HttpGet]
        [Route("GetListByCliente/{cmpy}/{cust}/{succli}")]
        public async Task<List<mob_rcheqinventariosE>> GetListByCliente(string cmpy, string cust, string succli)
        {
            return await _inventariosQueries.GetListByCliente(cmpy, cust, succli);
        }

        [HttpGet]
        [Route("GetListByCodUsu/{cmpy}/{codusu}/{consec}/{cust}/{succli}")]
        public async Task<List<mob_rcheqinventariosE>> GetListByCodUsu(string cmpy, string codusu, int consec, string cust, string succli)
        {
            return await _inventariosQueries.GetListByCodUsu(cmpy, codusu, consec, cust, succli);
            
        }

        [HttpGet]
        [Route("GetDetail/{cmpy}/{codusu}/{consec}")]
        public async Task<List<mob_rcheqinvdetalleE>> GetDetail(string cmpy, string codusu, int consec)
        {
            return await _inventariosQueries.GetDetail(cmpy, codusu, consec);
            
        }

        [HttpGet]
        [Route("GetDetailById/{cmpy}/{codusu}/{consec}/{id}")]
        public async Task<List<mob_rcheqinvdetalleE>> GetDetailById(string cmpy, string codusu, int consec, string id)
        {
            return await _inventariosQueries.GetDetailById(cmpy, codusu, consec, id);
            
        }

        [HttpGet]
        [Route("GetListByCompany/{cmpy}")]
        public async Task<List<InventarioStockDto>> GetListByCompany(string cmpy)
        {
            return await _inventariosQueries.GetListByCompany(cmpy);
        }

        [HttpGet]
        [Route("GetByProducto/{cmpy}/{codProducto}")]
        public async Task<InventarioStockDto> GetByProducto(string cmpy, string codProducto)
        {
            return await _inventariosQueries.GetByProducto(cmpy, codProducto);
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rcheqinventariosE inventario)
        {
            return _inventariosQueries.Insert(inventario);
        }

        [HttpPost]
        [Route("InsertDetail")]
        public int InsertDetail(mob_rcheqinvdetalleE detalle)
        {
            return _inventariosQueries.InsertDetail(detalle);
        }

        [HttpPut]
        [Route("Update")]
        public int Update(mob_rcheqinventariosE item)
        {
            return _inventariosQueries.Update(item);
        }

        [HttpPut]
        [Route("UpdateDetail")]
        public int UpdateDetail(mob_rcheqinvdetalleE item)
        {
            return _inventariosQueries.UpdateDetail(item);
        }

        [HttpDelete]
        [Route("Delete/{cmpy}/{consec}/{codUsu}")]
        public int Delete(string cmpy, string consec, string codUsu)
        {
            return _inventariosQueries.Delete(cmpy, consec, codUsu);
        }

        [HttpDelete]
        [Route("DeleteDetail/{cmpy}/{consec}/{codUsu}")]
        public int DeleteDetail(string cmpy, string consec, string codUsu)
        {
            return _inventariosQueries.DeleteDetail(cmpy, consec, codUsu);
        }

        [HttpDelete]
        [Route("DeleteDetailByProd/{cmpy}/{consec}/{codUsu}/{id}")]
        public int DeleteDetailByProd(string cmpy, string consec, string codUsu, string id)
        {
            return _inventariosQueries.DeleteDetailByProd(cmpy, consec, codUsu, id);
        }

    }
}
