using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Models.Dtos;
using Migracion.Persistence.DataBase;
using Migracion.Service.Queries;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidosQueries _pedidosQueries;
        private readonly Migracion.Service.Bonificados.IBonificadosPedidoService _bonificados;
        private readonly ILogger<PedidosController> _logger;

        public PedidosController(IPedidosQueries pedidosQueries,
            Migracion.Service.Bonificados.IBonificadosPedidoService bonificados,
            ILogger<PedidosController> logger)
        {
            _pedidosQueries = pedidosQueries;
            _bonificados = bonificados;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetByUser/{cmpy}/{codUsu}/{reg}/{teri}")]
        public async Task<List<mob_rcustmainSimp>> GetByUser(string cmpy, string codUsu, string reg, string teri)
        {
            return await _pedidosQueries.GetByUser(cmpy, codUsu, reg, teri);
            
        }

        [HttpGet]
        [Route("GetByClient/{cmpy}/{teri}/{reg}/{cust}/{ship}/{num}")]
        public async Task<List<mob_rordheadE>> GetByClient(string cmpy, string teri, string reg, string cust, string ship, string num)
        {
            return await _pedidosQueries.GetByClient(cmpy, teri, reg, cust, ship, num);
            
        }

        [HttpGet]
        [Route("GetByFactIbes/{cmpy}/{teri}/{reg}/{cust}/{ship}/{num}")]
        public async Task<List<mob_fact_pedido_eva_Dto>> GetByFactIbes(string cmpy, string teri, string reg, string cust, string ship, string num)
        {
            return await _pedidosQueries.GetByFactIbes(cmpy, teri, reg, cust, ship, num);
        }

        [HttpGet]
        [Route("GetByDate/{cmpy}/{reg}/{teri}/{num}/{entdt}")]
        public async Task<List<mob_rordheadE>> GetByDate(string cmpy, string reg, string teri, string num, string entdt)
        {
            return await _pedidosQueries.GetByDate(cmpy, reg, teri, num, entdt);
            
        }

        [HttpGet]
        [Route("GetByReg/{cmpy}/{reg}/{teri}/{num}")]
        public async Task<List<mob_rordheadE>> GetByReg(string cmpy, string reg, string teri, string num)
        {
            return await _pedidosQueries.GetByReg(cmpy, reg, teri, num);
            
        }

        [HttpGet]
        [Route("GetDetail/{cmpy}/{reg}/{teri}/{num}")]
        public async Task<List<mob_rordlineE>> GetDetail(string cmpy, string reg, string teri, string num)
        {
            return await _pedidosQueries.GetDetail(cmpy, reg, teri, num);
            
        }

        [HttpGet]
        [Route("GetDetailByProduct/{cmpy}/{reg}/{teri}/{num}/{part}/{level}/{endtdt}")]
        public async Task<List<mob_rordlineE>> GetDetailByProduct(string cmpy, string reg, string teri, string num, string part, string level, string endtdt)
        {
            return await _pedidosQueries.GetDetailByProduct(cmpy, reg, teri, num, part, level, endtdt);
            
        }

        [HttpGet]
        [Route("GetDetailByPart/{cmpy}/{reg}/{teri}/{num}/{part}")]
        public async Task<List<mob_rordlineE>> GetDetailByPart(string cmpy, string reg, string teri, string num, string part)
        {
            return await _pedidosQueries.GetDetailByPart(cmpy, reg, teri, num, part);
            
        }

        [HttpGet]
        [Route("GetHis/{numero}/{cmpy}/{reg}/{teri}/{cust}/{ship}")]
        public async Task<List<mob_rordhead_hisE>> GetHis(int numero, string cmpy, string reg, string teri, string cust, string ship)
        {
            return await _pedidosQueries.GetHis(numero,cmpy,reg,teri,cust,ship);
        }

        [HttpGet]
        [Route("GetDetailHis/{cmpy}/{reg}/{teri}/{num}")]
        public async Task<List<mob_rordline_hisE>> GetDetailHis(string cmpy, string reg, string teri, string num)
        {
            return await _pedidosQueries.GetDetailHis(cmpy, reg, teri, num);
        }

        [HttpGet]
        [Route("GetFullHis/{numero}/{cmpy}/{reg}/{teri}/{cust}/{ship}")]
        public async Task<List<mob_pedido_full_Dto>> GetFullHis(int numero, string cmpy, string reg, string teri, string cust, string ship)
        {
            return await _pedidosQueries.GetFull(numero, cmpy, reg, teri, cust, ship);
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rordheadE pedido)
        {
            return _pedidosQueries.Insert(pedido);
        }

        [HttpPost]
        [Route("InsertDetail")]
        public int InsertDetail(mob_rordlineE detalle)
        {
            return _pedidosQueries.InsertDetail(detalle);
        }

        [HttpPost]
        [Route("InsertFull")]
        public async Task<int> InsertFull(mob_rordDto item)
        {
            var resultado = _pedidosQueries.InsertFull(item);

            // Bonificados: solo las versiones de la app que mandan el bloque
            // (una lista vacía quita los del envío anterior). Va después del
            // pedido y en su propia transacción: si falla, el pedido ya quedó
            // guardado y la app no lo reintenta por esto.
            if (item.bonificacion != null && item.encabezado != null)
            {
                try
                {
                    var revision = await _bonificados.RegistrarAsync(item.encabezado, item.bonificacion);
                    if (revision is { Valido: false })
                    {
                        _logger.LogWarning("Pedido {Num} ({Cmpy}): bonificados rechazados {Codigos}.",
                            item.encabezado.orh_num, item.encabezado.orh_cmpy,
                            string.Join(", ", revision.Violaciones.Select(v => $"{v.Regla} {v.Codigo}")));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "No se registraron los bonificados del pedido {Num} ({Cmpy}).",
                        item.encabezado.orh_num, item.encabezado.orh_cmpy);
                }
            }

            return resultado;
        }




        [HttpPut]
        [Route("Update")]
        public int Update(mob_rordheadE item)
        {
            return _pedidosQueries.Update(item);
        }

        [HttpPut]
        [Route("UpdateDetail")]
        public int UpdateDetail(mob_rordlineE item)
        {
            return _pedidosQueries.UpdateDetail(item);
        }

        [HttpDelete]
        [Route("Delete/{cmpy}/{num}/{reg}/{teri}")]
        public int Delete(string cmpy, string num, string reg, string teri)
        {
            return _pedidosQueries.Delete(cmpy, num, reg, teri);
        }

        [HttpDelete]
        [Route("DeleteByReg/{cmpy}/{num}/{reg}/{teri}")]
        public int DeleteByReg(string cmpy, string num, string reg, string teri)
        {
            return _pedidosQueries.DeleteByReg(cmpy, num, reg, teri);
        }

        [HttpDelete]
        [Route("DeleteDetailByProd/{cmpy}/{num}/{reg}/{teri}/{part}")]
        public int DeleteDetailByProd(string cmpy, string num, string reg, string teri, string part)
        {
            return _pedidosQueries.DeleteDetailByProd(cmpy, num, reg, teri, part);
        }

        [HttpDelete]
        [Route("DeleteDetail/{cmpy}/{num}/{reg}/{teri}")]
        public int DeleteDetail(string cmpy, string num, string reg, string teri)
        {
            return _pedidosQueries.DeleteDetail(cmpy, num, reg, teri);
        }

        [HttpDelete]
        [Route("DeleteFull/{cmpy}/{num}/{reg}/{teri}")]
        public int DeleteFull(string cmpy, string num, string reg, string teri)
        {
            return _pedidosQueries.DeleteFull(cmpy, num, reg, teri);
        }
    }
}
