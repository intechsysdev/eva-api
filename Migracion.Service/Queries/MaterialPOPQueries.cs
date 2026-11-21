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
    public interface IMaterialPOPQueries
    {
        Task<List<mob_rmaterialpopE>> GetList(string cmpy);
        Task<List<MovimientoMaterialDto>> GetMovimientosList(string cmpy, string fecIni, string fecFin);
    }
    public class MaterialPOPQueries : IMaterialPOPQueries
    {
        private readonly ApplicationDbContext _context;
        public MaterialPOPQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rmaterialpopE>> GetList(string cmpy)
        {
            List<mob_rmaterialpopE> lststatus = null;
            try
            {
                lststatus = await _context.mob_rmaterialpop.Where(x => x.rmaterialpop_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lststatus), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lststatus = JsonSerializer.Deserialize<List<mob_rmaterialpopE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lststatus;
        }

        public async Task<List<MovimientoMaterialDto>> GetMovimientosList(string cmpy, string fecIni, string fecFin)
        {
            List<MovimientoMaterialDto> lstAcciones = null;
            try
            {

                string sqlQuery = "SELECT " +
                    "mob_rpop.rpop_idpop," +
                    "mob_rdetallepop.rdetallepop_cmpy," +
                    "mob_rdetallepop.rdetallepop_idpop," +
                    "mob_rdetallepop.rdetallepop_idmarca," +
                    "mob_rdetallepop.rdetallepop_idmaterial," +
                    "mob_rdetallepop.rdetallepop_idfotosnap," +
                    "mob_rdetallepop.rdetallepop_idfotoeva," +
                    "mob_rdetallepop.rdetallepop_image," +
                    "mob_rdetallepop.rdetallepop_date," +
                    "mob_rdetallepop.rdetallepop_time," +
                    "mob_rpop.rpop_latitud," +
                    "mob_rpop.rpop_longitud," +
                    "mob_rusuarios.rusu_nombre," +
                    "mob_rusuarios.rusu_reg," +
                    "region.reg_desc," +
                    "mob_rusuarios.rusu_teri," +
                    "territor.ter_desc," +
                    "mob_rpop.rpop_codper," +
                    "mob_rperfiles.rper_desper," +
                    "mob_rpop.rpop_codusu ," +
                    "mob_rmarcaspop.rmarcapop_marca," +
                    "mob_rmarcaspop.rmarcapop_activo," +
                    "mob_rpop.rpop_observacion," +
                    "mob_rpop.rpop_createbyemail," +
                    "mob_rpop.rpop_createbydevice," +
                    "mob_rpop.rpop_updatebyuser," +
                    "mob_rpop.rpop_date," +
                    "mob_rpop.rpop_time, " +
                    "mob_rcustmain.rctt_razsoc, " +
                    "mob_rcustmain.rctt_nomcli, " +
                    "mob_rcustmain.rctt_addr, " +
                    "mob_rcustmain.rctt_latitud, " +
                    "mob_rcustmain.rctt_longitud, " +
                    "mob_rmateriapop.rmaterialpop_des " +
                    "FROM mob_rpop mob_rpop " +
                    "INNER JOIN  mob_rdetallepop mob_rdetallepop ON mob_rdetallepop.rdetallepop_idpop = mob_rpop.rpop_idpop " +
                    "INNER JOIN mob_rmarcaspop mob_rmarcaspop ON mob_rmarcaspop.rmarcapop_idmarca = mob_rdetallepop.rdetallepop_idmarca " +
                    "INNER JOIN mob_rmaterialpop mob_rmateriapop ON mob_rmateriapop.rmaterialpop_idmaterial = mob_rdetallepop.rdetallepop_idmaterial " +
                    "LEFT JOIN mob_rusuarios mob_rusuarios ON mob_rpop.rpop_iduser = mob_rusuarios.rusu_cedula " +
                    "LEFT JOIN region ON region.reg_regi = mob_rusuarios.rusu_reg AND region.reg_cmpy = mob_rusuarios.rusu_cmpy " +
                    "LEFT JOIN territor ON territor.ter_teri = mob_rusuarios.rusu_teri AND territor.ter_cmpy = mob_rusuarios.rusu_cmpy " +
                    "LEFT JOIN mob_rperfiles mob_rperfiles ON mob_rpop.rpop_codper = mob_rperfiles.rper_codper " +
                    "INNER JOIN mob_rcustmain mob_rcustmain ON mob_rpop.rpop_cust = mob_rcustmain.rctt_cust AND mob_rpop.rpop_succli = mob_rcustmain.rctt_succli " +
                    "WHERE rpop_date  BETWEEN  '" + fecIni + "' AND '" + fecFin + "' " +
                    "UNION ALL " +
                    "SELECT " +
                    "mob_rpop.rpop_idpop, " +
                    "mob_rdetallepop.rdetallepop_cmpy, " +
                    "mob_rdetallepop.rdetallepop_idpop, " +
                    "mob_rdetallepop.rdetallepop_idmarca, " +
                    "mob_rdetallepop.rdetallepop_idmaterial, " +
                    "mob_rdetallepop.rdetallepop_idfotosnap, " +
                    "mob_rdetallepop.rdetallepop_idfotoeva, " +
                    "mob_rdetallepop.rdetallepop_image, " +
                    "mob_rdetallepop.rdetallepop_date, " +
                    "mob_rdetallepop.rdetallepop_time, " +
                    "mob_rpop.rpop_latitud, " +
                    "mob_rpop.rpop_longitud, " +
                    "mob_rusuarios.rusu_nombre, " +
                    "mob_rusuarios.rusu_reg, " +
                    "region.reg_desc, " +
                    "mob_rusuarios.rusu_teri, " +
                    "territor.ter_desc, " +
                    "mob_rpop.rpop_codper, " +
                    "mob_rperfiles.rper_desper, " +
                    "mob_rpop.rpop_codusu , " +
                    "mob_rmarcaspop.rmarcapop_marca, " +
                    "mob_rmarcaspop.rmarcapop_activo, " +
                    "mob_rpop.rpop_observacion, " +
                    "mob_rpop.rpop_createbyemail, " +
                    "mob_rpop.rpop_createbydevice, " +
                    "mob_rpop.rpop_updatebyuser, " +
                    "mob_rpop.rpop_date, " +
                    "mob_rpop.rpop_time, " +
                    "mob_rcustmain.rctt_razsoc, " +
                    "mob_rcustmain.rctt_nomcli, " +
                    "mob_rcustmain.rctt_addr, " +
                    "mob_rcustmain.rctt_latitud, " +
                    "mob_rcustmain.rctt_longitud, " +
                    "mob_rmateriapop.rmaterialpop_des " +
                    "FROM mob_rpop mob_rpop " +
                    "INNER JOIN  mob_rdetallepop mob_rdetallepop ON mob_rdetallepop.rdetallepop_idpop = mob_rpop.rpop_idpop " +
                    "INNER JOIN mob_rmarcaspop mob_rmarcaspop ON mob_rmarcaspop.rmarcapop_idmarca = mob_rdetallepop.rdetallepop_idmarca " +
                    "INNER JOIN mob_rmaterialpop mob_rmateriapop ON mob_rmateriapop.rmaterialpop_idmaterial = mob_rdetallepop.rdetallepop_idmaterial " +
                    "LEFT JOIN mob_rusuarios mob_rusuarios ON mob_rpop.rpop_iduser = mob_rusuarios.rusu_cedula " +
                    "LEFT JOIN region ON region.reg_regi = mob_rusuarios.rusu_reg AND region.reg_cmpy = mob_rusuarios.rusu_cmpy " +
                    "LEFT JOIN territor ON territor.ter_teri = mob_rusuarios.rusu_teri AND territor.ter_cmpy = mob_rusuarios.rusu_cmpy " +
                    "LEFT JOIN mob_rperfiles mob_rperfiles ON mob_rpop.rpop_codper = mob_rperfiles.rper_codper " +
                    "INNER JOIN mob_rcustrans mob_rcustmain ON mob_rpop.rpop_cust = mob_rcustmain.rctt_cust AND mob_rpop.rpop_succli = mob_rcustmain.rctt_succli " +
                    "WHERE rpop_date  BETWEEN  '" + fecIni + "' AND '" + fecFin + "' ";

                lstAcciones = await _context.MovimientoMaterialDtoEs.FromSqlRaw(sqlQuery).ToListAsync();
                lstAcciones.ForEach(x =>
                {
                    x.rpop_date = Convert.ToDateTime(x.rpop_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<MovimientoMaterialDto>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }
    }
}
