using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Service.Commands;
using Migracion.Service.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Migracion.Persistence.DataBase;
using Migracion.Api.Models;
using Migracion.Models.Dtos;

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ExhibicionesController : ControllerBase
    {
        private readonly IExhibicionCommand _exhibicionCommand;
        private readonly IExhibicionesQueries _exhibicionQueries;
        public ExhibicionesController(IExhibicionCommand exhibicionCommand, IExhibicionesQueries exhibicionQueries)
        {
            _exhibicionCommand = exhibicionCommand;
            _exhibicionQueries = exhibicionQueries;
        }

        [HttpPost]
        [Route("Insert/{cmpy}")]
        public TResult Insert(string cmpy, [FromBody] List<mob_rexhibicionE> lstExhibiciones)
        {
            return _exhibicionCommand.Insert(cmpy, lstExhibiciones);
        }

        [HttpPost]
        [Route("InsertPhoto/{cmpy}")]
        public TResult InsertPhoto(string cmpy, [FromBody] List<mob_rexhibicionfotoE> lstExhibicionesFotos)
        {
            return _exhibicionCommand.InsertPhoto(cmpy, lstExhibicionesFotos);
        }

        [HttpPost]
        [Route("InsertStatus/{cmpy}")]
        public int InsertStatus(string cmpy, List<mob_rstatusexhibicionE> lstExhibicionStatus)
        {
            return _exhibicionCommand.InsertStatus(cmpy, lstExhibicionStatus);
        }

        [HttpPost]
        [Route("InsertCategory/{cmpy}")]
        public int InsertCategory(string cmpy, List<mob_rcategoriaexhibicionE> lstExhibicionCategory)
        {
            return _exhibicionCommand.InsertCategory(cmpy, lstExhibicionCategory);
        }

        [HttpPost]
        [Route("InsertClass/{cmpy}")]
        public int InsertClass(string cmpy, List<mob_rclasexhibicionE> lstExhibicionClass)
        {
            return _exhibicionCommand.InsertClass(cmpy, lstExhibicionClass);
        }

        [HttpPost]
        [Route("InsertType/{cmpy}")]
        public int InsertType(string cmpy, List<mob_rtipoexhibicionE> lstExhibicionType)
        {
            return _exhibicionCommand.InsertType(cmpy, lstExhibicionType);
        }

        [HttpPost]
        [Route("InsertClassType/{cmpy}")]
        public int InsertClassType(string cmpy, List<mob_rclasetipoeexhibicionE> lstExhibicionClassType)
        {
            return _exhibicionCommand.InsertClassType(cmpy, lstExhibicionClassType);
        }

        //GET
        [HttpGet]
        [Route("GetStatusList/{cmpy}")]
        public async Task<List<mob_rstatusexhibicionE>> GetStatusList(string cmpy)
        {
            return await _exhibicionQueries.GetStatusList(cmpy);
            
        }

        [HttpGet]
        [Route("GetCategoriesList/{cmpy}")]
        public async Task<List<mob_rcategoriaexhibicionE>> GetCategoriesList(string cmpy)
        {
            return await _exhibicionQueries.GetCategoriesList(cmpy);
            
        }

        [HttpGet]
        [Route("GetClasessList/{cmpy}")]
        public async Task<List<mob_rclasexhibicionE>> GetClasessList(string cmpy)
        {
            return await _exhibicionQueries.GetClasessList(cmpy);
            
        }

        [HttpGet]
        [Route("GetTypeList/{cmpy}")]
        public async Task<List<mob_rtipoexhibicionE>> GetTypeList(string cmpy)
        {
            return await _exhibicionQueries.GetTypeList(cmpy);
            
        }

        [HttpGet]
        [Route("GetTypeClassList/{cmpy}")]
        public async Task<List<mob_rclasetipoeexhibicionE>> GetTypeClassList(string cmpy)
        {
            return await _exhibicionQueries.GetTypeClassList(cmpy);
            
        }

        [HttpGet]
        [Route("GetTypeClassCompList")]
        public async Task<List<mob_rclasetipoeexhibicionDto>> GetTypeClassCompList(int? classId)
        {
            List<mob_rclasetipoeexhibicionDto> responseJson = null;
            if (classId != null)
            {
                var list = await _exhibicionQueries.GetTypeClassCompList();
                responseJson = list.Where(x => x.rclastipoexh_idclase == Convert.ToInt32(classId)).ToList();
            }
            else
            {
                responseJson = await _exhibicionQueries.GetTypeClassCompList();
            }

            return responseJson;
        }

        //Delete
        [HttpDelete]
        [Route("DeleteStatus/{id}")]
        public bool DeleteStatus(string id)
        {
            return _exhibicionCommand.DeleteStatus(id);
        }

        [HttpDelete]
        [Route("DeleteCategory/{id}")]
        public bool DeleteCategory(string id)
        {
            return _exhibicionCommand.DeleteCategory(id);
        }

        [HttpDelete]
        [Route("DeleteClass/{id}")]
        public bool DeleteClass(string id)
        {
            return _exhibicionCommand.DeleteClass(id);
        }

        [HttpDelete]
        [Route("DeleteType/{id}")]
        public bool DeleteType(string id)
        {
            return _exhibicionCommand.DeleteType(id);
        }

        [HttpDelete]
        [Route("DeleteClassType/{idclass}/{idType}")]
        public bool DeleteClassType(int idclass, int idType)
        {
            return _exhibicionCommand.DeleteClassType(idclass, idType);
        }

        [HttpPut]
        [Route("UpdatetStatus")]
        public bool UpdatetStatus(mob_rstatusexhibicionE item)
        {
            return _exhibicionCommand.UpdatetStatus(item);
        }

        [HttpPut]
        [Route("UpdatetCategory")]
        public bool UpdatetCategory(mob_rcategoriaexhibicionE item)
        {
            return _exhibicionCommand.UpdatetCategory(item);
        }

        [HttpPut]
        [Route("UpdatetClass")]
        public bool UpdatetClass(mob_rclasexhibicionE item)
        {
            return _exhibicionCommand.UpdatetClass(item);
        }

        [HttpPut]
        [Route("UpdatetType")]
        public bool UpdatetType(mob_rtipoexhibicionE item)
        {
            return _exhibicionCommand.UpdatetType(item);
        }

        [HttpPut]
        [Route("UpdatetClassType")]
        public bool UpdatetClassType(mob_rclasetipoeexhibicionE item)
        {
            return _exhibicionCommand.UpdatetClassType(item);
        }

        [HttpGet]
        [Route("GetTypeClassListByidClass/{cmpy}/{idClase}")]
        public async Task<List<mob_rclasetipoeexhibicionE>> GetTypeClassListByidClass(string cmpy, int idClase)
        {
            List<mob_rclasetipoeexhibicionE> responseJson = null;
            var list = await _exhibicionQueries.GetTypeClassList(cmpy);
            responseJson = list.Where(x => x.rclastipoexh_idclase == idClase).ToList();
            return responseJson;
        }

        [HttpGet]
        [Route("GetList/{cmpy}/{fecIni}/{fecFin}")]
        public async Task<List<ExhibicionDtoE>> GetList(string cmpy,string fecIni, string fecFin)
        {
            return await _exhibicionQueries.GetList(cmpy, fecIni, fecFin);
            
        }

        [HttpPost]
        [Route("GetPhotosByCustList")]
        public async Task<List<mob_rexhibicionfotoE>> GetPhotosByCustList([FromBody] FilterExhibicionesFotosDto item)
        {
            return await _exhibicionQueries.GetPhotosByCustList(item);
            
        }

        [HttpGet]
        [Route("GetPhotosList/{cod_exh}")]
        public async Task<List<mob_rexhibicionfotoE>> GetPhotosList(string cod_exh)
        {
            return await _exhibicionQueries.GetPhotosList(cod_exh);
            
        }

        [HttpPut]
        [Route("UpdatetStatusExhibicion")]
        public bool UpdatetStatusExhibicion([FromBody] UpdateExhibicionStatus item)
        {
            return _exhibicionCommand.UpdateStatuExhibicion(item);
        }

        [HttpPost]
        [Route("GetFiltersList")]
        public async Task<List<ExhibicionDtoE>> GetFiltersList([FromBody] FilterExhibicionesDto filter)
        {
            List<ExhibicionDtoE> lstResultFilter = await _exhibicionQueries.GetList(filter.Cmpy, filter.FecIni, filter.FecFin);
            
            if (filter.Estados.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Estados.Contains(x.ID_ESTADO)).ToList();
            }

            if (filter.codUsu.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.codUsu.Contains(x.USUARIO)).ToList();
            }

            if (filter.Regiones.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Regiones.Contains(x.ID_REGION)).ToList();
            }

            if (filter.Territorios.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Territorios.Contains(x.ID_TERRITORIO)).ToList();
            }

            if (filter.Perfiles.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Perfiles.Contains(x.ID_PERFIL)).ToList();
            }

            if (filter.Clases.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Clases.Contains(x.ID_CLASE)).ToList();
            }

            if (filter.Tipos.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Tipos.Contains(x.ID_TIPO)).ToList();
            }

            if (filter.Canales.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Canales.Contains(x.ID_CANAL)).ToList();
            }

            if (filter.GrupoCanales.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.GrupoCanales.Contains(x.ID_GRUPOCANAL)).ToList();
            }

            if (filter.Ciudades.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Ciudades.Contains(x.ID_CIUDAD)).ToList();
            }

            if (filter.Lineas.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Lineas.Contains(x.ID_LINEA)).ToList();
            }

            if (filter.Sublineas.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Sublineas.Contains(x.ID_SUBLINEA)).ToList();
            }

            if (filter.Categorias.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.Categorias.Contains(x.ID_CATEGORIA)).ToList();
            }

            if (filter.UnaSolaVez.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.UnaSolaVez.Contains(x.UNASOLAVEZ)).ToList();
            }

            if (filter.TipoFotos.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.TipoFotos.Contains(x.TIPO_FOTO)).ToList();
            }

            if (filter.PremioDeseado.Length > 0)
            {
                lstResultFilter = lstResultFilter.Where(x => filter.PremioDeseado.Contains(x.PREMIO_DESEADO)).ToList();
            }

            return lstResultFilter;
        }

        [HttpGet]
        [Route("GetLastItem/{cmpy}")]
        public async Task<DateTime> GetLastItem(string cmpy)
        {
            mob_rexhibicionE item = await _exhibicionQueries.GetLastItem(cmpy);
            if(item != null)
            {
                return item.rexh_date;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        [HttpPost]
        [Route("GetListWithPhotos")]
        public async Task<List<ExhibicionesFotosAllDto>> GetListWithPhotos([FromBody] FilterExhibicionesDto filter)
        {
            var responseJson = await _exhibicionQueries.GetListAll(filter.Cmpy, filter.FecIni, filter.FecFin);
            
            if (filter.codUsu.Length > 0)
            {
                responseJson = responseJson.Where(x => filter.codUsu.Contains(x.USUARIO)).ToList();
            }
            return responseJson;
        }
    }
}
