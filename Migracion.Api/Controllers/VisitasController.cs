using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Api.Models;
using Migracion.Models;
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
    public class VisitasController : ControllerBase
    {
        private readonly IVisitasQueries _visitasQueries;

        public VisitasController(IVisitasQueries visitasQueries)
        {
            _visitasQueries = visitasQueries;
        }

        [HttpGet]
        [Route("GetList/{codUsu}/{fecvis}/{fecvisFin}")]
        public async Task<List<mob_rvisitaE>> GetList(string codUsu,  string fecvis, string fecvisFin )
        {
            return await _visitasQueries.GetList(codUsu, fecvis, fecvisFin); 
            
        }

        [HttpPost]
        [Route("Insert")]
        public int Insert(mob_rvisitaE item)
        {
            return _visitasQueries.Insert(item);
        }

        [HttpPost]
        [Route("GetListCompuesto")]
        public async Task<List<mob_rvisitsellerDto>> GetListCompuesto([FromBody] FiltersDetalleDto filters)
        {
            List<mob_rvisitsellerDto> lstResultFilter = await _visitasQueries.GetListCompuesto(filters.codUsu, filters.codCust, filters.fecIni, filters.fecFin, filters.custFiltro, filters.succliFiltro);
            
            if (filters.Territorios.Length > 0)
            {
               lstResultFilter = lstResultFilter.Where(x => filters.Territorios.Contains(x.rusu_teri)).ToList();
            }
            if (filters.Regiones.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.Regiones.Contains(x.rctt_reg)).ToList();
            }
            if (filters.MedioVisita.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.MedioVisita.Contains(x.rvis_codmedvis)).ToList();
            }
            if (filters.MotivoVisita.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.MotivoVisita.Contains(x.motcod)).ToList();
            }
            if (filters.Acciones.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.Acciones.Contains(x.acctip)).ToList();
            }
            if (filters.noCompra.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.noCompra.Contains(x.movcod)).ToList();
            }
            if (filters.Tipomovimiento.Length > 0)
            {
               lstResultFilter = lstResultFilter.Where(x => filters.Tipomovimiento.Contains(x.rvis_coddoc)).ToList();
            }
            if ((filters.horaDesde != null || filters.horaHasta != null) && (filters.horaDesde != "" || filters.horaHasta != ""))
            {
                lstResultFilter = lstResultFilter.Where(x => TimeSpan.Parse(x.rvis_horinivis) >= TimeSpan.Parse(filters.horaDesde) && TimeSpan.Parse(x.rvis_horinivis) <= TimeSpan.Parse(filters.horaHasta)).ToList();
            }

            if (filters.duracionVisitaDesde != 0 || filters.duracionVisitaHasta != 0)
            {
                lstResultFilter = lstResultFilter.Where(x => Convert.ToInt32(TimeSpan.Parse(x.tiempoVis).Hours) >= filters.duracionVisitaDesde && Convert.ToInt32(TimeSpan.Parse(x.tiempoVis).Hours) <= filters.duracionVisitaHasta).ToList();

            }
            if (filters.Ciudades.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.Ciudades.Contains(x.dis_dist)).ToList();
            }
            
            return lstResultFilter;
        }

        [HttpPost]
        [Route("GetListCompuestoGroup")]
        public async Task<List<mob_rvisitsellerGroupDto>> GetListCompuestoGroup([FromBody] FiltersDetalleDto filters)
        {
            List<mob_rvisitsellerGroupDto> lstResultFilter = await _visitasQueries.GetListGroup(filters.codUsu, filters.codCust, filters.fecIni, filters.fecFin, filters.custFiltro, filters.succliFiltro);
            
            if (filters.Territorios.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.Territorios.Contains(x.rusu_teri)).ToList();
            }
           
            if ((filters.horaDesde != null || filters.horaHasta != null) && (filters.horaDesde != "" || filters.horaHasta != ""))
            {
                lstResultFilter = lstResultFilter.Where(x => TimeSpan.Parse(x.rvis_horinivis) >= TimeSpan.Parse(filters.horaDesde) && TimeSpan.Parse(x.rvis_horinivis) <= TimeSpan.Parse(filters.horaHasta)).ToList();
            }

            if (filters.duracionVisitaDesde != 0 || filters.duracionVisitaHasta != 0)
            {
                lstResultFilter = lstResultFilter.Where(x => Convert.ToInt32(TimeSpan.Parse(x.tiempoVis).Hours) >= filters.duracionVisitaDesde && Convert.ToInt32(TimeSpan.Parse(x.tiempoVis).Hours) <= filters.duracionVisitaHasta).ToList();

            }
            if (filters.Ciudades.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.Ciudades.Contains(x.dis_dist)).ToList();
            }

            return lstResultFilter;
        }

        [HttpPost]
        [Route("GetListCompuestoRutasGroup")]
        public async Task<List<mob_rvisitsellerGroupDto>> GetListCompuestoRutasGroup([FromBody] FiltersDetalleDto filters)
        {
            List<mob_rvisitsellerGroupDto> lstResultFilter = await _visitasQueries.GetListGroupRutas(filters.codUsu, filters.fecIni, filters.fecFin);
            
            if (filters.Territorios.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.Territorios.Contains(x.rusu_teri)).ToList();
            }

            if ((filters.horaDesde != null || filters.horaHasta != null) && (filters.horaDesde != "" || filters.horaHasta != ""))
            {
                lstResultFilter = lstResultFilter.Where(x => TimeSpan.Parse(x.rvis_horinivis) >= TimeSpan.Parse(filters.horaDesde) && TimeSpan.Parse(x.rvis_horinivis) <= TimeSpan.Parse(filters.horaHasta)).ToList();
            }

            if (filters.duracionVisitaDesde != 0 || filters.duracionVisitaHasta != 0)
            {
                lstResultFilter = lstResultFilter.Where(x => Convert.ToInt32(TimeSpan.Parse(x.tiempoVis).Hours) >= filters.duracionVisitaDesde && Convert.ToInt32(TimeSpan.Parse(x.tiempoVis).Hours) <= filters.duracionVisitaHasta).ToList();

            }
            if (filters.Ciudades.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.Ciudades.Contains(x.dis_dist)).ToList();
            }

            return lstResultFilter;
        }


        [HttpPost]
        [Route("GetListSummary")]
        public async Task<List<mob_rvisitSummaryDto>> GetListSummary([FromBody] FiltersDto filters)
        {
            List<mob_rvisitSummaryDto> lstResultFilter = await _visitasQueries.GetListSummary(filters.fecIni, filters.fecFin);
            

            if (filters.Territorios.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.Territorios.Contains(x.rusu_teri)).ToList();
            }
            if (filters.Regiones.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.Regiones.Contains(x.rusu_reg)).ToList();
            }
            if (filters.Perfiles.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.Perfiles.Contains(x.rusu_codper)).ToList();
            }
            if (filters.Vendedores.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filters.Vendedores.Contains(x.rvis_enter)).ToList();
            }
            if (filters.duracionVisitaDesde != 0 || filters.duracionVisitaHasta != 0)
            {
                lstResultFilter = lstResultFilter.Where(x => Convert.ToInt32(TimeSpan.Parse(x.tiempoVis).Hours) >= filters.duracionVisitaDesde && Convert.ToInt32(TimeSpan.Parse(x.tiempoVis).Hours) <= filters.duracionVisitaHasta).ToList();

            }

            if (filters.cantVisitaDesde != 0 || filters.cantVisitaHasta != 0)
            {
                lstResultFilter = lstResultFilter.Where(x => Convert.ToInt32(x.cantvisitas) >= filters.cantVisitaDesde && Convert.ToInt32(x.cantvisitas) <= filters.cantVisitaHasta).ToList();
            }

            if ((filters.horaDesde != null || filters.horaHasta != null) && (filters.horaDesde != "" || filters.horaHasta != ""))
            {
                lstResultFilter = lstResultFilter.Where(x => TimeSpan.Parse(x.minrvis_horinivis) >= TimeSpan.Parse(filters.horaDesde) && TimeSpan.Parse(x.maxrvis_horinivis) <= TimeSpan.Parse(filters.horaHasta)).ToList();
            }




            return lstResultFilter;
        }

       

    }
}
