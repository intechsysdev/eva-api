using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Migracion.Api.Models;
using Migracion.Models;
using Migracion.Persistence.DataBase;
using Migracion.Service.Commands;
using Migracion.Service.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MaterialPOPController : ControllerBase
    {
        private readonly IMaterialPOPCommand _materialPOPCommand;
        private readonly IMaterialPOPQueries _materialPOPQueries;

        private readonly IMarcasPOPCommand _marcasPOPCommand;
        private readonly IMarcasPOPQueries _marcasPOPQueries;

        private readonly IMovimientoPOPCommand _movimientoPOPCommand;
        private readonly IMovimientoPOPQueries _movimientoPOPQueries;

        private readonly IDetallePOPCommand _detallePOPCommand;
        private readonly IDetallePOPQueries _detallePOPQueries;

        public MaterialPOPController(IMaterialPOPCommand materialPOPCommand, IMaterialPOPQueries materialPOPQueries,
            IMarcasPOPCommand marcasPOPCommand, IMarcasPOPQueries marcasPOPQueries, IMovimientoPOPCommand movimientoPOPCommand,
            IMovimientoPOPQueries movimientoPOPQueries, IDetallePOPCommand detallePOPCommand, IDetallePOPQueries detallePOPQueries)
        {
            _materialPOPCommand = materialPOPCommand;
            _materialPOPQueries = materialPOPQueries;
            _marcasPOPCommand = marcasPOPCommand;
            _marcasPOPQueries = marcasPOPQueries;
            _movimientoPOPCommand = movimientoPOPCommand;
            _movimientoPOPQueries = movimientoPOPQueries;
            _detallePOPCommand = detallePOPCommand;
            _detallePOPQueries = detallePOPQueries;
        }

        [HttpGet]
        [Route("GetList/{cmpy}")]
        public async Task<List<mob_rmaterialpopE>> GetList(string cmpy)
        {
            return await _materialPOPQueries.GetList(cmpy);
            
        }

        [HttpGet]
        [Route("GetListByStatus/{cmpy}/{status}")]
        public async Task<List<mob_rmaterialpopE>> GetListByStatus(string cmpy, string status)
        {
            List<mob_rmaterialpopE> lst = await _materialPOPQueries.GetList(cmpy);
            var responseJson = lst.Where(x => x.rmarcapop_activo == status).ToList();
            return responseJson;
        }

        [HttpPost]
        [Route("Insert")]
        public TResult Insert([FromBody] mob_rmaterialpopE item)
        {
            return _materialPOPCommand.Insert(item);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public TResult Delete(int id)
        {
            return _materialPOPCommand.Delete(id);
        }

        [HttpPut]
        [Route("Update")]
        public TResult Update([FromBody] mob_rmaterialpopE item)
        {
            return _materialPOPCommand.Update(item);
        }

        [HttpGet]
        [Route("GetListMarcas/{cmpy}")]
        public async Task<List<mob_rmarcaspopE>> GetListMarcas(string cmpy)
        {
            return await _marcasPOPQueries.GetList(cmpy);
            
        }


        [HttpGet]
        [Route("GetListMarcasByStatus/{cmpy}/{status}")]
        public async Task<List<mob_rmarcaspopE>> GetListMarcasByStatus(string cmpy, string status)
        {
            List<mob_rmarcaspopE> lst = await _marcasPOPQueries.GetList(cmpy);
            var responseJson = lst.Where(x => x.rmarcapop_activo == status).ToList();
            return responseJson;
        }


        [HttpPost]
        [Route("InsertMarca")]
        public TResult InsertMarca( [FromBody] mob_rmarcaspopE item)
        {
            return _marcasPOPCommand.Insert(item);
        }

        [HttpDelete]
        [Route("DeleteMarca/{id}")]
        public TResult DeleteMarca(int id)
        {
            return _marcasPOPCommand.Delete(id);
        }

        [HttpPut]
        [Route("UpdateMarca")]
        public TResult UpdateMarca([FromBody] mob_rmarcaspopE item)
        {
            return _marcasPOPCommand.Update(item);
        }

        //Movimientos
        [HttpGet]
        [Route("GetListMovimientos/{cmpy}")]
        public async Task<List<mob_rpope>> GetListMovimientos(string cmpy)
        {
            return await _movimientoPOPQueries.GetList(cmpy);
            
        }

        [HttpPost]
        [Route("InsertMovimiento")]
        public TResult InsertMovimiento([FromBody] mob_rpope item)
        {
            return _movimientoPOPCommand.Insert(item);
        }

        [HttpDelete]
        [Route("DeleteMovimiento/{cmpy}/{id}")]
        public TResult DeleteMovimiento(string cmpy, int id)
        {
            return _movimientoPOPCommand.Delete(cmpy, id);
        }

        [HttpPut]
        [Route("UpdateMovimiento")]
        public TResult UpdateMovimiento([FromBody] mob_rpope item)
        {
            return _movimientoPOPCommand.Update(item);
        }

        //Detalle
        [HttpGet]
        [Route("GetListDetalles/{cmpy}")]
        public async Task<List<mob_rdetallepopE>> GetListDetalles(string cmpy)
        {
            return await _detallePOPQueries.GetList(cmpy);
            
        }

        [HttpPost]
        [Route("InsertDetalle")]
        public TResult InsertDetalle([FromBody] mob_rdetallepopE item)
        {
            return _detallePOPCommand.Insert(item);
        }

        [HttpDelete]
        [Route("DeleteDetalle/{cmpy}/{id}/{idmaterial}")]
        public TResult DeleteDetalle(string cmpy, int id,int idmaterial)
        {
            return _detallePOPCommand.Delete(cmpy, id, idmaterial);
        }

        [HttpPost]
        [Route("GetFiltersList")]
        public async Task<List<MovimientoMaterialDto>> GetFiltersList([FromBody] FilterMaterialPOPDto filter)
        {
            List<MovimientoMaterialDto> lstResultFilter = await _materialPOPQueries.GetMovimientosList(filter.Cmpy, filter.FecIni, filter.FecFin);
            
            if (filter.codUsu.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.codUsu.Contains(x.rpop_codusu)).ToList();
            }

            if (filter.Regiones.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Regiones.Contains(x.rusu_reg)).ToList();
            }

            if (filter.Territorios.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Territorios.Contains(x.rusu_teri)).ToList();
            }

            if (filter.Perfiles.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Perfiles.Contains(x.rpop_codper)).ToList();
            }

            if (filter.Materiales.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Materiales.Contains(x.rdetallepop_idmaterial)).ToList();
            }

            if (filter.Marcas.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Marcas.Contains(x.rdetallepop_idmarca)).ToList();
            }

            return lstResultFilter;
        }
    }
}
