using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Api.Models;
using Migracion.Models;
using Migracion.Models.Command;
using Migracion.Models.Dtos;
using Migracion.Persistence.DataBase;
using Migracion.Service.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IClientesQueries _clientesQueries;

        public ClientesController(IClientesQueries clientesQueries)
        {
            _clientesQueries = clientesQueries;
        }

        [HttpGet]
        [Route("GetDetail/{cmpy}/{codUsu}")]
        public async Task<List<mob_rcusdetalleE>> GetDetail(string cmpy, string codUsu)
        {
            return await _clientesQueries.GetDetail(cmpy, codUsu);
          
        }

        [HttpGet]
        [Route("GetListByCodUsu/{cmpy}/{codUsu}")]
        public async Task<List<mob_rcustmainDto>> GetListByCodUsu(string cmpy, string codUsu)
        {
            return await _clientesQueries.GetListByCodUsu(cmpy, codUsu);
            
        }

        [HttpGet]
        [Route("GetListByRuta/{cmpy}/{codUsu}/{ruta}")]
        public async Task<List<mob_rcustmainDto>> GetListByRuta(string cmpy, string codUsu, string ruta)
        {
            return await _clientesQueries.GetListByRuta(cmpy, codUsu, ruta);
            
        }

        [HttpGet]
        [Route("GetListByClient/{cmpy}/{resal}/{tipoDoc}")]
        public async Task<List<mob_rcustmainDto>> GetListByClient(string cmpy, string resal, string tipoDoc)
        {
            return await _clientesQueries.GetListByClient(cmpy, resal, tipoDoc);
            
        }

        [HttpGet]
        [Route("GetListVerifyByClient/{cmpy}/{cust}/{tipoDoc}")]
        public async Task<List<mob_rcustshipDto>> GetListVerifyByClient(string cmpy, string cust, string tipoDoc)
        {
            return await _clientesQueries.GetListVerifyByClient(cmpy, cust, tipoDoc);
            
        }

        [HttpGet]
        [Route("GetListByClientCodPer/{cmpy}/{perfil}/{codUsu}")]
        public async Task<List<mob_rcustmainDto>> GetListByClientCodPer( string cmpy, string perfil, string codUsu)
        {
            return await _clientesQueries.GetListByClientCodPer(cmpy, perfil, codUsu);
            
        }

        [HttpGet]
        [Route("GetListByIdentity/{code}")]
        public async Task<List<mob_rcustmainDto>> GetListByIdentity(string code)
        {
            return await _clientesQueries.GetListByIdentity(code);
            
        }

        [HttpGet]
        [Route("GetListIdentityByGc/{code}/{gc}")]
        public async Task<List<mob_rcustmainDto>> GetListIdentityByGc(string code,int gc)
        {
            return await _clientesQueries.GetListIdentityByGc(code, gc);
            
        }


        [HttpGet]
        [Route("GetDetailDataClient/{perfil}/{cmpy}/{cus}/{succli}/{tipoDoc}")]
        public async Task<List<mob_rcustmainDto>> GetDetailDataClient(string perfil, string cmpy, string cus, string succli, string tipoDoc)
        {
            return await _clientesQueries.GetDetailDataClient(perfil, cmpy, cus, succli, tipoDoc);
            
        }

        [HttpGet]
        [Route("GetListStatus/{cmpy}")]
        public async Task<List<EstadoClienteE>> GetListStatus(string cmpy)
        {
            return await _clientesQueries.GetListStatus(cmpy);

        }

        [HttpPost]
        [Route("InsertDetail")]
        public int Insert(mob_rcusdetalleCommand item)
        {
            return _clientesQueries.InsertDetail(item);
        }

        [HttpPost]
        [Route("InsertNuevos")]
        public int InsertNuevos(mob_rcustshipCommand item)
        {
            return _clientesQueries.InsertNuevos(item);
        }

        [HttpPost]
        [Route("InsertVerificado")]
        public int InsertVerificado(mob_rcustshipCommand item)
        {
            return _clientesQueries.InsertVerificado(item);
        }

        [HttpPost]
        [Route("InsertActualizados")]
        public int InsertActualizados(mob_rcustshipCommand item)
        {
            return _clientesQueries.InsertActualizados(item);
        }

        [HttpPut]
        [Route("UpdateDetail")]
        public int UpdateDetail(mob_rcusdetalleCommand client)
        {
            return _clientesQueries.UpdateDetail(client);
        }

        [HttpDelete]
        [Route("DeleteDetail/{cmpy}/{cus}/{succli}")]
        public int DeleteDetalle(string cmpy, string cus, string succli)
        {
            return _clientesQueries.DeleteDetail(cmpy, cus, succli);
        }

        [HttpDelete]
        [Route("Delete/{cmpy}/{cust}/{succli}/{codusu}")]
        public int Delete(string cmpy, string cust, string succli, string codusu)
        {
            return _clientesQueries.Delete(cmpy, cust, succli,codusu);
        }

        [HttpPost]
        [Route("GetList")]
        public async Task<List<mob_rcustmainDto>> GetList(FilterClienteDto filters)
        {
            string codusu = "";
            List<string> codUsu = new List<string>();
            if (filters.usuarios.Length > 0)
            {
                string result = "";
                foreach (var item in filters.usuarios)
                {
                    result = "'" + item + "'";
                    codUsu.Add(result);
                }
                codusu = String.Join(",", codUsu);
            }
            
            List<mob_rcustmainDto> lstResultFilter = await _clientesQueries.GetList(filters.cmpy, codusu);


            if (filters.regiones.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.regiones.Contains(x.rctt_reg)).ToList();
            }
            if (filters.ciudades.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.ciudades.Contains(x.rctt_dist)).ToList();
            }
            
            if (filters.canales.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.canales.Contains(x.rctt_type)).ToList();
            }

            return lstResultFilter;
        }
    }
}
