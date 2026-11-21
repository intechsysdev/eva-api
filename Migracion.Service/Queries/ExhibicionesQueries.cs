using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Migracion.Service.Queries
{
    public interface IExhibicionesQueries
    {
        Task<List<mob_rstatusexhibicionE>> GetStatusList(string cmpy);
        Task<List<mob_rcategoriaexhibicionE>> GetCategoriesList(string cmpy);
        Task<List<mob_rclasexhibicionE>> GetClasessList(string cmpy);
        Task<List<mob_rtipoexhibicionE>> GetTypeList(string cmpy);
        Task<List<mob_rclasetipoeexhibicionE>> GetTypeClassList(string cmpy);
        Task<List<ExhibicionDtoE>> GetList(string cmpy, string fecIni, string fecFin);
        Task<List<mob_rexhibicionfotoE>> GetPhotosList(string cod_exh);
        Task<List<mob_rexhibicionfotoE>> GetPhotosByCustList(FilterExhibicionesFotosDto item);
        Task<List<mob_rclasetipoeexhibicionDto>> GetTypeClassCompList();
        Task<mob_rexhibicionE> GetLastItem(string cmpy);
        Task<List<ExhibicionesFotosAllDto>> GetListAll(string cmpy, string fecIni, string fecFin);
    }
    public class ExhibicionesQueries : IExhibicionesQueries
    {
        private readonly ApplicationDbContext _context;
        public ExhibicionesQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rstatusexhibicionE>> GetStatusList(string cmpy)
        {
            List<mob_rstatusexhibicionE> lststatus = null;
            try
            {
                lststatus = await _context.mob_rstatusexhibicion.Where(x => x.rstatusexh_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lststatus), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lststatus = JsonSerializer.Deserialize<List<mob_rstatusexhibicionE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lststatus;
        }

        public async Task<List<mob_rcategoriaexhibicionE>> GetCategoriesList(string cmpy)
        {
            List<mob_rcategoriaexhibicionE> lstCategories = null;
            try
            {
                lstCategories = await _context.mob_rcategoriaexhibicion.Where(x => x.rcateexh_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstCategories), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstCategories = JsonSerializer.Deserialize<List<mob_rcategoriaexhibicionE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstCategories;

        }

        public async Task<List<mob_rclasexhibicionE>> GetClasessList(string cmpy)
        {
            List<mob_rclasexhibicionE> lstClasses = null;
            try
            {
                lstClasses = await _context.mob_rclasexhibicion.Where(x => x.rclasexh_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstClasses), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstClasses = JsonSerializer.Deserialize<List<mob_rclasexhibicionE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstClasses;
        }

        public async Task<List<mob_rtipoexhibicionE>> GetTypeList(string cmpy)
        {
            List<mob_rtipoexhibicionE> lstTypes = null;
            try
            {
                lstTypes = await _context.mob_rtipoexhibicion.Where(x => x.rtipoexh_cmpy == cmpy).ToListAsync();
                
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstTypes), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstTypes = JsonSerializer.Deserialize<List<mob_rtipoexhibicionE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstTypes;
        }

        public async Task<List<mob_rclasetipoeexhibicionE>> GetTypeClassList(string cmpy)
        {
            List<mob_rclasetipoeexhibicionE> lstTypes = null;
            try
            {
                lstTypes = await _context.mob_rclasetipoeexhibicion.Where(x => x.rclastipoexh_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstTypes), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstTypes = JsonSerializer.Deserialize<List<mob_rclasetipoeexhibicionE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstTypes;
        }

        public async Task<List<ExhibicionDtoE>> GetList(string cmpy, string fecIni, string fecFin)
        {
            List<ExhibicionDtoE> lstAcciones = null;
            try
            {
                string sqlQuery = "SELECT rexh_cmpy, rexh_key CODIGO_EXHIBICION, rexh_idstatus ID_ESTADO,mob_rstatusexhibicion.rstatusexh_des ESTADO, mob_rstatusexhibicion.rstatusexh_image ICONO, rexh_iduser ID_USUARIO,mob_rusuarios.rusu_nombre NOMBRE, mob_rusuarios.rusu_reg ID_REGION, region.reg_desc REGION, mob_rusuarios.rusu_teri ID_TERRITORIO, territor.ter_desc TERRITORIO, rexh_codper ID_PERFIL,mob_rperfiles.rper_desper PERFIL, rexh_latitud LATITUD_EXHIBICION, rexh_longitud LONGITUD_EXHIBICION, mob_rusuarios.rusu_codusu USUARIO, rexh_idclase ID_CLASE, mob_rclasexhibicion.rclasexh_des CLASE, rexh_idtipo ID_TIPO,mob_rtipoexhibicion.rtipoexh_des TIPO, rexh_otrotipo OTRO_TIPO, rexh_cust ID_CLIENTE, rexh_succli ID_SUCURSAL,mob_rcustmain.rctt_razsoc RAZON_SOCIAL, mob_rcustmain.rctt_nomcli CLIENTE, mob_rcustmain.rctt_addr DIRECCION, mob_rcustmain.rctt_type ID_CANAL, mob_rcusttype.cu_desc CANAL, custtype.cu_comm ID_GRUPOCANAL, grupo_canal.gc_nombre GRUPO_CANAL, mob_rcustmain.rctt_dist ID_CIUDAD, mob_district.dis_desc CIUDAD, mob_rcustmain.rctt_latitud LATITUD, mob_rcustmain.rctt_longitud LONGITUD, rexh_linea ID_LINEA,mob_rlinea.rln_descr LINEA, rexh_subl ID_SUBLINEA,mob_rsublineas.rsb_descr SUB_LINEA, rexh_idcategoria ID_CATEGORIA,mob_rcategoriaexhibicion.rcateexh_des CATEGORIA, rexh_otracategoria OTRA_CATEGORIA, rexh_unasolavez UNASOLAVEZ, rexh_tipofoto TIPO_FOTO, rexh_observacion OBSERVACION, rexh_premiodeseado PREMIO_DESEADO, rexh_createbyemail CREADO_POR, rexh_createbydevice DISPOSITIVO, rexh_updatebyuser ACTUALIZADO_POR, rexh_dateupdate FECHA_ACTUALIZADO, rexh_date FECHA_CREADO, rexh_time HORA_CREADO, rexh_note NOTA FROM mob_rexhibicion LEFT JOIN mob_rstatusexhibicion mob_rstatusexhibicion ON mob_rexhibicion.rexh_idstatus = mob_rstatusexhibicion.rstatusexh_id LEFT JOIN mob_rusuarios mob_rusuarios ON mob_rexhibicion.rexh_iduser = mob_rusuarios.rusu_cedula LEFT JOIN mob_rperfiles mob_rperfiles ON mob_rexhibicion.rexh_codper = mob_rperfiles.rper_codper LEFT JOIN mob_rclasexhibicion mob_rclasexhibicion ON mob_rexhibicion.rexh_idclase = mob_rclasexhibicion.rclasexh_id LEFT JOIN mob_rtipoexhibicion mob_rtipoexhibicion ON mob_rexhibicion.rexh_idtipo = mob_rtipoexhibicion.rtipoexh_id LEFT JOIN mob_rlinea mob_rlinea ON mob_rexhibicion.rexh_linea = mob_rlinea.rln_linea LEFT JOIN mob_rsublineas mob_rsublineas ON mob_rexhibicion.rexh_linea = mob_rsublineas.rsb_linea AND mob_rexhibicion.rexh_subl = mob_rsublineas.rsb_subl LEFT JOIN mob_rcategoriaexhibicion mob_rcategoriaexhibicion ON mob_rexhibicion.rexh_idcategoria = mob_rcategoriaexhibicion.rcateexh_id INNER JOIN mob_rcustmain mob_rcustmain ON mob_rexhibicion.rexh_cust = mob_rcustmain.rctt_cust AND mob_rexhibicion.rexh_succli = mob_rcustmain.rctt_succli LEFT JOIN mob_rcusttype mob_rcusttype ON mob_rcustmain.rctt_type = mob_rcusttype.cu_type LEFT JOIN mob_district mob_district ON mob_rcustmain.rctt_dist = mob_district.dis_dist LEFT JOIN region ON region.reg_regi = mob_rusuarios.rusu_reg AND region.reg_cmpy = mob_rusuarios.rusu_cmpy LEFT JOIN territor ON territor.ter_teri = mob_rusuarios.rusu_teri AND territor.ter_cmpy = mob_rusuarios.rusu_cmpy LEFT JOIN custtype ON custtype.cu_type = mob_rcustmain.rctt_type AND custtype.cu_cmpy = mob_rcustmain.rctt_cmpy LEFT JOIN grupo_canal ON grupo_canal.gc_grupo = custtype.cu_comm AND grupo_canal.gc_cmpy = custtype.cu_cmpy " +
                 "WHERE  rexh_date BETWEEN  '" + fecIni + "' AND '" + fecFin + "'" +
                 " UNION ALL " +
                "SELECT rexh_cmpy, rexh_key CODIGO_EXHIBICION, rexh_idstatus ID_ESTADO,mob_rstatusexhibicion.rstatusexh_des ESTADO, mob_rstatusexhibicion.rstatusexh_image ICONO, rexh_iduser ID_USUARIO,mob_rusuarios.rusu_nombre NOMBRE, mob_rusuarios.rusu_reg ID_REGION, region.reg_desc REGION, mob_rusuarios.rusu_teri ID_TERRITORIO, territor.ter_desc TERRITORIO, rexh_codper ID_PERFIL,mob_rperfiles.rper_desper PERFIL, rexh_latitud LATITUD_EXHIBICION, rexh_longitud LONGITUD_EXHIBICION, mob_rusuarios.rusu_codusu USUARIO, rexh_idclase ID_CLASE, mob_rclasexhibicion.rclasexh_des CLASE, rexh_idtipo ID_TIPO,mob_rtipoexhibicion.rtipoexh_des TIPO, rexh_otrotipo OTRO_TIPO, rexh_cust ID_CLIENTE, rexh_succli ID_SUCURSAL,mob_rcustmain.rctt_razsoc RAZON_SOCIAL, mob_rcustmain.rctt_nomcli CLIENTE, mob_rcustmain.rctt_addr DIRECCION, mob_rcustmain.rctt_type ID_CANAL, mob_rcusttype.cu_desc CANAL, custtype.cu_comm ID_GRUPOCANAL, grupo_canal.gc_nombre GRUPO_CANAL, mob_rcustmain.rctt_dist ID_CIUDAD, mob_district.dis_desc CIUDAD, mob_rcustmain.rctt_latitud LATITUD, mob_rcustmain.rctt_longitud LONGITUD, rexh_linea ID_LINEA,mob_rlinea.rln_descr LINEA, rexh_subl ID_SUBLINEA,mob_rsublineas.rsb_descr SUB_LINEA, rexh_idcategoria ID_CATEGORIA,mob_rcategoriaexhibicion.rcateexh_des CATEGORIA, rexh_otracategoria OTRA_CATEGORIA, rexh_unasolavez UNASOLAVEZ, rexh_tipofoto TIPO_FOTO, rexh_observacion OBSERVACION, rexh_premiodeseado PREMIO_DESEADO, rexh_createbyemail CREADO_POR, rexh_createbydevice DISPOSITIVO, rexh_updatebyuser ACTUALIZADO_POR, rexh_dateupdate FECHA_ACTUALIZADO, rexh_date FECHA_CREADO, rexh_time HORA_CREADO, rexh_note NOTA FROM mob_rexhibicion LEFT JOIN mob_rstatusexhibicion mob_rstatusexhibicion ON mob_rexhibicion.rexh_idstatus = mob_rstatusexhibicion.rstatusexh_id LEFT JOIN mob_rusuarios mob_rusuarios ON mob_rexhibicion.rexh_iduser = mob_rusuarios.rusu_cedula LEFT JOIN mob_rperfiles mob_rperfiles ON mob_rexhibicion.rexh_codper = mob_rperfiles.rper_codper LEFT JOIN mob_rclasexhibicion mob_rclasexhibicion ON mob_rexhibicion.rexh_idclase = mob_rclasexhibicion.rclasexh_id LEFT JOIN mob_rtipoexhibicion mob_rtipoexhibicion ON mob_rexhibicion.rexh_idtipo = mob_rtipoexhibicion.rtipoexh_id LEFT JOIN mob_rlinea mob_rlinea ON mob_rexhibicion.rexh_linea = mob_rlinea.rln_linea LEFT JOIN mob_rsublineas mob_rsublineas ON mob_rexhibicion.rexh_linea = mob_rsublineas.rsb_linea AND mob_rexhibicion.rexh_subl = mob_rsublineas.rsb_subl LEFT JOIN mob_rcategoriaexhibicion mob_rcategoriaexhibicion ON mob_rexhibicion.rexh_idcategoria = mob_rcategoriaexhibicion.rcateexh_id INNER JOIN mob_rcustrans mob_rcustmain ON mob_rexhibicion.rexh_cust = mob_rcustmain.rctt_cust AND mob_rexhibicion.rexh_succli = mob_rcustmain.rctt_succli LEFT JOIN mob_rcusttype mob_rcusttype ON mob_rcustmain.rctt_type = mob_rcusttype.cu_type LEFT JOIN mob_district mob_district ON mob_rcustmain.rctt_dist = mob_district.dis_dist LEFT JOIN region ON region.reg_regi = mob_rusuarios.rusu_reg AND region.reg_cmpy = mob_rusuarios.rusu_cmpy LEFT JOIN territor ON territor.ter_teri = mob_rusuarios.rusu_teri AND territor.ter_cmpy = mob_rusuarios.rusu_cmpy LEFT JOIN custtype ON custtype.cu_type = mob_rcustmain.rctt_type AND custtype.cu_cmpy = mob_rcustmain.rctt_cmpy LEFT JOIN grupo_canal ON grupo_canal.gc_grupo = custtype.cu_comm AND grupo_canal.gc_cmpy = custtype.cu_cmpy " +
                "WHERE  rexh_date BETWEEN  '" + fecIni + "' AND '" + fecFin + "'" +
                " ORDER BY CODIGO_EXHIBICION,FECHA_CREADO,HORA_CREADO DESC";

                lstAcciones = await _context.ExhibicionDtoEs.FromSqlRaw(sqlQuery).ToListAsync();
                lstAcciones.ForEach(x =>
                {
                    x.FECHA_CREADO = Convert.ToDateTime(x.FECHA_CREADO).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<ExhibicionDtoE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

        public async Task<List<mob_rclasetipoeexhibicionDto>> GetTypeClassCompList()
        {
            List<mob_rclasetipoeexhibicionDto> lstAcciones = null;
            try
            {
                string sqlQuery = "SELECT mob_rclasetipoeexhibicion.rclastipoexh_idclase,mob_rclasetipoeexhibicion.rclastipoexh_idtipo,mob_rclasexhibicion.rclasexh_des,mob_rtipoexhibicion.rtipoexh_des " +
"FROM mob_rclasetipoeexhibicion mob_rclasetipoeexhibicion " +
"LEFT JOIN mob_rclasexhibicion mob_rclasexhibicion ON mob_rclasetipoeexhibicion.rclastipoexh_idclase = mob_rclasexhibicion.rclasexh_id " +
"LEFT JOIN mob_rtipoexhibicion mob_rtipoexhibicion  ON mob_rclasetipoeexhibicion.rclastipoexh_idtipo = mob_rtipoexhibicion.rtipoexh_id";

                lstAcciones = await _context.mob_rclasetipoeexhibicionDtos.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rclasetipoeexhibicionDto>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

        public async Task<List<mob_rexhibicionfotoE>> GetPhotosByCustList(FilterExhibicionesFotosDto item)
        {
            List<mob_rexhibicionfotoE> lstTypes = null;
            try
            {
                string sqlQuery = "SELECT * FROM mob_rexhibicionfotos " +
                "WHERE  rfotoexh_date  BETWEEN  '" + item.fecIni + "' AND '" + item.fecFin + "' AND rfotoexh_cust = '" + item.cust + "'  AND rfotoexh_succli = '" + item.succli + "' " + " AND rfotoexh_idclase  =" + item.idclase + " AND rfotoexh_idtipo =" + item.idtipo + " AND rfotoexh_linea  = '" + item.linea + "'" + " AND rfotoexh_subl   = '" + item.sublinea + "'";
                lstTypes = await _context.mob_rexhibicionfotos.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstTypes), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstTypes = JsonSerializer.Deserialize<List<mob_rexhibicionfotoE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstTypes;

        }

        public async Task<List<mob_rexhibicionfotoE>> GetPhotosList(string cod_exh)
        {
            List<mob_rexhibicionfotoE> lstTypes = null;
            try
            {
                lstTypes = await _context.mob_rexhibicionfotos.Where(x => x.rfotoexh_idsnap == cod_exh).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstTypes), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstTypes = JsonSerializer.Deserialize<List<mob_rexhibicionfotoE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstTypes;

        }

        public async Task<List<ExhibicionesFotosAllDto>> GetListAll(string cmpy, string fecIni, string fecFin)
        {
            List<ExhibicionesFotosAllDto> lstAcciones = null;
            try
            {
                string sqlQuery = "SELECT rexh_cmpy, rexh_key CODIGO_EXHIBICION, rexh_idstatus ID_ESTADO,mob_rstatusexhibicion.rstatusexh_des ESTADO, mob_rstatusexhibicion.rstatusexh_image ICONO, rexh_iduser ID_USUARIO,mob_rusuarios.rusu_nombre NOMBRE, mob_rusuarios.rusu_reg ID_REGION, region.reg_desc REGION, mob_rusuarios.rusu_teri ID_TERRITORIO, territor.ter_desc TERRITORIO, rexh_codper ID_PERFIL,mob_rperfiles.rper_desper PERFIL, rexh_latitud LATITUD_EXHIBICION, rexh_longitud LONGITUD_EXHIBICION, mob_rusuarios.rusu_codusu USUARIO, rexh_idclase ID_CLASE, mob_rclasexhibicion.rclasexh_des CLASE, rexh_idtipo ID_TIPO,mob_rtipoexhibicion.rtipoexh_des TIPO, rexh_otrotipo OTRO_TIPO, rexh_cust ID_CLIENTE, rexh_succli ID_SUCURSAL,mob_rcustmain.rctt_razsoc RAZON_SOCIAL, mob_rcustmain.rctt_nomcli CLIENTE, mob_rcustmain.rctt_addr DIRECCION, mob_rcustmain.rctt_type ID_CANAL, mob_rcusttype.cu_desc CANAL, custtype.cu_comm ID_GRUPOCANAL, grupo_canal.gc_nombre GRUPO_CANAL, mob_rcustmain.rctt_dist ID_CIUDAD, mob_district.dis_desc CIUDAD, mob_rcustmain.rctt_latitud LATITUD, mob_rcustmain.rctt_longitud LONGITUD, rexh_linea ID_LINEA,mob_rlinea.rln_descr LINEA, rexh_subl ID_SUBLINEA,mob_rsublineas.rsb_descr SUB_LINEA, rexh_idcategoria ID_CATEGORIA,mob_rcategoriaexhibicion.rcateexh_des CATEGORIA, rexh_otracategoria OTRA_CATEGORIA, rexh_unasolavez UNASOLAVEZ, rexh_tipofoto TIPO_FOTO, rexh_observacion OBSERVACION, rexh_premiodeseado PREMIO_DESEADO, rexh_createbyemail CREADO_POR, rexh_createbydevice DISPOSITIVO, rexh_updatebyuser ACTUALIZADO_POR, rexh_dateupdate FECHA_ACTUALIZADO, rexh_date FECHA_CREADO, rexh_time HORA_CREADO, rexh_note NOTA, mob_rexhibicionfotos.rfotoexh_image URLIMAGEN FROM mob_rexhibicion LEFT JOIN mob_rstatusexhibicion mob_rstatusexhibicion ON mob_rexhibicion.rexh_idstatus = mob_rstatusexhibicion.rstatusexh_id LEFT JOIN mob_rusuarios mob_rusuarios ON mob_rexhibicion.rexh_iduser = mob_rusuarios.rusu_cedula LEFT JOIN mob_rperfiles mob_rperfiles ON mob_rexhibicion.rexh_codper = mob_rperfiles.rper_codper LEFT JOIN mob_rclasexhibicion mob_rclasexhibicion ON mob_rexhibicion.rexh_idclase = mob_rclasexhibicion.rclasexh_id LEFT JOIN mob_rtipoexhibicion mob_rtipoexhibicion ON mob_rexhibicion.rexh_idtipo = mob_rtipoexhibicion.rtipoexh_id LEFT JOIN mob_rlinea mob_rlinea ON mob_rexhibicion.rexh_linea = mob_rlinea.rln_linea LEFT JOIN mob_rsublineas mob_rsublineas ON mob_rexhibicion.rexh_linea = mob_rsublineas.rsb_linea AND mob_rexhibicion.rexh_subl = mob_rsublineas.rsb_subl LEFT JOIN mob_rcategoriaexhibicion mob_rcategoriaexhibicion ON mob_rexhibicion.rexh_idcategoria = mob_rcategoriaexhibicion.rcateexh_id INNER JOIN mob_rcustmain mob_rcustmain ON mob_rexhibicion.rexh_cust = mob_rcustmain.rctt_cust AND mob_rexhibicion.rexh_succli = mob_rcustmain.rctt_succli LEFT JOIN mob_rcusttype mob_rcusttype ON mob_rcustmain.rctt_type = mob_rcusttype.cu_type LEFT JOIN mob_district mob_district ON mob_rcustmain.rctt_dist = mob_district.dis_dist LEFT JOIN region ON region.reg_regi = mob_rusuarios.rusu_reg AND region.reg_cmpy = mob_rusuarios.rusu_cmpy LEFT JOIN territor ON territor.ter_teri = mob_rusuarios.rusu_teri AND territor.ter_cmpy = mob_rusuarios.rusu_cmpy LEFT JOIN custtype ON custtype.cu_type = mob_rcustmain.rctt_type AND custtype.cu_cmpy = mob_rcustmain.rctt_cmpy LEFT JOIN grupo_canal ON grupo_canal.gc_grupo = custtype.cu_comm AND grupo_canal.gc_cmpy = custtype.cu_cmpy " +
"LEFT JOIN mob_rexhibicionfotos ON mob_rexhibicionfotos.rfotoexh_idsnap = mob_rexhibicion.rexh_key " +
"WHERE  rexh_date BETWEEN  '" + fecIni + "' AND '" + fecFin + "'" +
" UNION ALL " +
"SELECT rexh_cmpy, rexh_key CODIGO_EXHIBICION, rexh_idstatus ID_ESTADO,mob_rstatusexhibicion.rstatusexh_des ESTADO, mob_rstatusexhibicion.rstatusexh_image ICONO, rexh_iduser ID_USUARIO,mob_rusuarios.rusu_nombre NOMBRE, mob_rusuarios.rusu_reg ID_REGION, region.reg_desc REGION, mob_rusuarios.rusu_teri ID_TERRITORIO, territor.ter_desc TERRITORIO, rexh_codper ID_PERFIL,mob_rperfiles.rper_desper PERFIL, rexh_latitud LATITUD_EXHIBICION, rexh_longitud LONGITUD_EXHIBICION, mob_rusuarios.rusu_codusu USUARIO, rexh_idclase ID_CLASE, mob_rclasexhibicion.rclasexh_des CLASE, rexh_idtipo ID_TIPO,mob_rtipoexhibicion.rtipoexh_des TIPO, rexh_otrotipo OTRO_TIPO, rexh_cust ID_CLIENTE, rexh_succli ID_SUCURSAL,mob_rcustmain.rctt_razsoc RAZON_SOCIAL, mob_rcustmain.rctt_nomcli CLIENTE, mob_rcustmain.rctt_addr DIRECCION, mob_rcustmain.rctt_type ID_CANAL, mob_rcusttype.cu_desc CANAL, custtype.cu_comm ID_GRUPOCANAL, grupo_canal.gc_nombre GRUPO_CANAL, mob_rcustmain.rctt_dist ID_CIUDAD, mob_district.dis_desc CIUDAD, mob_rcustmain.rctt_latitud LATITUD, mob_rcustmain.rctt_longitud LONGITUD, rexh_linea ID_LINEA,mob_rlinea.rln_descr LINEA, rexh_subl ID_SUBLINEA,mob_rsublineas.rsb_descr SUB_LINEA, rexh_idcategoria ID_CATEGORIA,mob_rcategoriaexhibicion.rcateexh_des CATEGORIA, rexh_otracategoria OTRA_CATEGORIA, rexh_unasolavez UNASOLAVEZ, rexh_tipofoto TIPO_FOTO, rexh_observacion OBSERVACION, rexh_premiodeseado PREMIO_DESEADO, rexh_createbyemail CREADO_POR, rexh_createbydevice DISPOSITIVO, rexh_updatebyuser ACTUALIZADO_POR, rexh_dateupdate FECHA_ACTUALIZADO, rexh_date FECHA_CREADO, rexh_time HORA_CREADO, rexh_note NOTA,mob_rexhibicionfotos.rfotoexh_image URLIMAGEN  FROM mob_rexhibicion LEFT JOIN mob_rstatusexhibicion mob_rstatusexhibicion ON mob_rexhibicion.rexh_idstatus = mob_rstatusexhibicion.rstatusexh_id LEFT JOIN mob_rusuarios mob_rusuarios ON mob_rexhibicion.rexh_iduser = mob_rusuarios.rusu_cedula LEFT JOIN mob_rperfiles mob_rperfiles ON mob_rexhibicion.rexh_codper = mob_rperfiles.rper_codper LEFT JOIN mob_rclasexhibicion mob_rclasexhibicion ON mob_rexhibicion.rexh_idclase = mob_rclasexhibicion.rclasexh_id LEFT JOIN mob_rtipoexhibicion mob_rtipoexhibicion ON mob_rexhibicion.rexh_idtipo = mob_rtipoexhibicion.rtipoexh_id LEFT JOIN mob_rlinea mob_rlinea ON mob_rexhibicion.rexh_linea = mob_rlinea.rln_linea LEFT JOIN mob_rsublineas mob_rsublineas ON mob_rexhibicion.rexh_linea = mob_rsublineas.rsb_linea AND mob_rexhibicion.rexh_subl = mob_rsublineas.rsb_subl LEFT JOIN mob_rcategoriaexhibicion mob_rcategoriaexhibicion ON mob_rexhibicion.rexh_idcategoria = mob_rcategoriaexhibicion.rcateexh_id INNER JOIN mob_rcustrans mob_rcustmain ON mob_rexhibicion.rexh_cust = mob_rcustmain.rctt_cust AND mob_rexhibicion.rexh_succli = mob_rcustmain.rctt_succli LEFT JOIN mob_rcusttype mob_rcusttype ON mob_rcustmain.rctt_type = mob_rcusttype.cu_type LEFT JOIN mob_district mob_district ON mob_rcustmain.rctt_dist = mob_district.dis_dist LEFT JOIN region ON region.reg_regi = mob_rusuarios.rusu_reg AND region.reg_cmpy = mob_rusuarios.rusu_cmpy LEFT JOIN territor ON territor.ter_teri = mob_rusuarios.rusu_teri AND territor.ter_cmpy = mob_rusuarios.rusu_cmpy LEFT JOIN custtype ON custtype.cu_type = mob_rcustmain.rctt_type AND custtype.cu_cmpy = mob_rcustmain.rctt_cmpy LEFT JOIN grupo_canal ON grupo_canal.gc_grupo = custtype.cu_comm AND grupo_canal.gc_cmpy = custtype.cu_cmpy " +
"LEFT JOIN mob_rexhibicionfotos ON mob_rexhibicionfotos.rfotoexh_idsnap = mob_rexhibicion.rexh_key " +
"WHERE  rexh_date BETWEEN  '" + fecIni + "' AND '" + fecFin + "'" +
" ORDER BY CODIGO_EXHIBICION,FECHA_CREADO,HORA_CREADO DESC";

                lstAcciones = await _context.ExhibicionesFotosAllDtoEs.FromSqlRaw(sqlQuery).ToListAsync();
                lstAcciones.ForEach(x =>
                {
                    x.FECHA_CREADO = Convert.ToDateTime(x.FECHA_CREADO).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<ExhibicionesFotosAllDto>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

        public async Task<mob_rexhibicionE> GetLastItem(string cmpy)
        {
            List<mob_rexhibicionE> lst = await _context.mob_rexhibicion.Where(x => x.rexh_cmpy == cmpy).ToListAsync();

            var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lst), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
            lst = JsonSerializer.Deserialize<List<mob_rexhibicionE>>(responseJson);

            return lst.OrderByDescending(x=>x.rexh_date).FirstOrDefault();
        }
    }
}

