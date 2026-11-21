using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Models.Command;
using Migracion.Models.Dtos;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Migracion.Service.Queries
{
    public interface IClientesQueries
    {
        Task<List<mob_rcusdetalleE>> GetDetail(string cmpy, string codUsu);
        Task<List<mob_rcustmainDto>> GetListByCodUsu(string cmpy, string codUsu);
        Task<List<mob_rcustmainDto>> GetListByRuta(string cmpy, string codUsu, string codRuta);
        Task<List<mob_rcustmainDto>> GetListByClient(string cmpy, string resal, string tipoDoc);
        Task<List<mob_rcustshipDto>> GetListVerifyByClient(string cmpy, string cust, string tipoDoc);
        Task<List<mob_rcustmainDto>> GetList(string cmpy, string codusu);
        Task<List<mob_rcustmainDto>> GetListByClientCodPer( string cmpy, string perfil, string codusu);
        Task<List<mob_rcustmainDto>> GetDetailDataClient(string perfil, string cmpy, string cus, string succli, string tipoDoc);
        Task<List<mob_rcustmainDto>> GetListByIdentity(string code);
        Task<List<mob_rcustmainDto>> GetListIdentityByGc(string code,int gc);
        Task<List<EstadoClienteE>> GetListStatus(string cmpy);
        Task<mob_rcustmainDto> GetbyCust(string cmpy, string cust);

        int InsertDetail(mob_rcusdetalleCommand client);
        int InsertNuevos(mob_rcustshipCommand client);
        int InsertVerificado(mob_rcustshipCommand client);
        int InsertActualizados(mob_rcustshipCommand client);

        int UpdateDetail(mob_rcusdetalleCommand client);

        int DeleteDetail(string cmpy, string cus, string succli);
        int Delete(string cmpy, string cust, string succli, string codusu);
    }


    public class ClientesQueries : IClientesQueries
    {
        private readonly ApplicationDbContext _context;
        
       
        public ClientesQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rcusdetalleE>> GetDetail(string cmpy, string codUsu)
        {

            List<mob_rcusdetalleE> lstAsociados = null;
            try
            {
                string sqlQuery = "Select rcusde_cmpy, rcusde_cus, rcusde_succli, rcusde_coddet, rcusde_desdet, rcusde_valdet, rcusde_date, rcusde_time, rcusde_estadoupd " +
                                  "FROM  mob_rcusdetalle rcusdetalle,mob_rcustrans rcustrans " +
                                  "WHERE rcusdetalle.rcusde_cmpy=rcustrans.rctt_cmpy AND rcusdetalle.rcusde_cus=rcustrans.rctt_cust AND rcusdetalle.rcusde_succli=rcustrans.rctt_succli " +
                                  "AND rcusde_cmpy='" + cmpy + "' AND rctt_codusu='" + codUsu + "'";
                lstAsociados = await _context.mob_rcusdetalle.FromSqlRaw(sqlQuery).ToListAsync();
                lstAsociados.ForEach(x =>
                {
                    x.rcusde_date = Convert.ToDateTime(x.rcusde_date).ToString("MM/dd/yyyy");   
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcusdetalleE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rcustmainDto>> GetListByCodUsu(string cmpy, string codUsu)
        {
            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                List<mob_rcustmainDto> lstAsociados = null;
                try
                {
                    string sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd, '' as resumendatos " +
                                      "FROM mob_rcustmain " +
                                      " WHERE rctt_cmpy ='" + cmpy + "'  AND rctt_codusu ='" + codUsu + "'";
                    lstAsociados = await _context.mob_rcustmainDtos.FromSqlRaw(sqlQuery).ToListAsync();
                    lstAsociados.ForEach(x =>
                    {
                        x.rctt_level = x.rctt_level.Trim();
                        x.rctt_date = Convert.ToDateTime(x.rctt_date).ToString("MM/dd/yyyy");
                        x.rctt_feccum = Convert.ToDateTime(x.rctt_feccum).ToString("MM/dd/yyyy");
                    });

                    var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                    lstAsociados = JsonSerializer.Deserialize<List<mob_rcustmainDto>>(responseJson);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                return lstAsociados;
            }
        }

        public async Task<mob_rcustmainDto> GetbyCust(string cmpy, string cust)
        {

            List<mob_rcustmainDto> lstAsociados = null;
            try
            {
                string sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd, '' as resumendatos " +
                                  "FROM mob_rcustmain " +
                                  " WHERE rctt_cmpy ='" + cmpy + "'  AND rctt_cust ='" + cust + "'";
                lstAsociados = await _context.mob_rcustmainDtos.FromSqlRaw(sqlQuery).ToListAsync();
                lstAsociados.ForEach(x =>
                {
                    x.rctt_level = x.rctt_level.Trim();
                    x.rctt_date = Convert.ToDateTime(x.rctt_date).ToString("MM/dd/yyyy");
                    x.rctt_feccum = Convert.ToDateTime(x.rctt_feccum).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcustmainDto>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados.FirstOrDefault();
        }

        public async Task<List<mob_rcustmainDto>> GetListByRuta(string cmpy, string codUsu, string codRuta)
        {

            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                List<mob_rcustmainDto> lstAsociados = null;
                try
                {
                    string sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd,'' as resumendatos " +
                                       "FROM mob_rcustmain " +
                                       "WHERE rctt_cmpy='" + cmpy + "' AND rctt_codusu='" + codUsu + "' AND rctt_ruta='" + codRuta + "'";
                    lstAsociados = await _context.mob_rcustmainDtos.FromSqlRaw(sqlQuery).ToListAsync();

                    lstAsociados.ForEach(x =>
                    {
                        x.rctt_level = x.rctt_level.Trim();
                        x.rctt_date = Convert.ToDateTime(x.rctt_date).ToString("MM/dd/yyyy");
                        x.rctt_feccum = Convert.ToDateTime(x.rctt_feccum).ToString("MM/dd/yyyy");
                    });

                    var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                    lstAsociados = JsonSerializer.Deserialize<List<mob_rcustmainDto>>(responseJson);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                return lstAsociados;
            }

        }

        public async Task<List<mob_rcustmainDto>> GetListByClient(string cmpy, string resal, string tipoDoc)
        {

            List<mob_rcustmainDto> lstAsociados = null;
            try
            {
                string sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd,'' as resumendatos " +
                                  "FROM mob_rcustmain " +
                                  "WHERE rctt_cmpy='" + cmpy + "'  AND rctt_resal='" + resal + "' AND rctt_tipdoc='" + tipoDoc + "'";
                lstAsociados = await _context.mob_rcustmainDtos.FromSqlRaw(sqlQuery).ToListAsync();

                lstAsociados.ForEach(x =>
                {
                    x.rctt_level = x.rctt_level.Trim();
                    x.rctt_date = Convert.ToDateTime(x.rctt_date).ToString("MM/dd/yyyy");
                    x.rctt_feccum = Convert.ToDateTime(x.rctt_feccum).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcustmainDto>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rcustshipDto>> GetListVerifyByClient(string cmpy, string cust, string tipoDoc)
        {

            List<mob_rcustshipDto> lstAsociados = null;
            try
            {
                string sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd,'' as resumendatos " +
                                  "FROM mob_rcustship " +
                                  "WHERE rctt_cmpy='" + cmpy + "'  AND rctt_cust='" + cust + "' AND rctt_tipdoc='" + tipoDoc + "'";
                lstAsociados = await _context.mob_rcustshipDtos.FromSqlRaw(sqlQuery).ToListAsync();
                lstAsociados.ForEach(x =>
                {
                    x.rctt_date = Convert.ToDateTime(x.rctt_date).ToString("MM/dd/yyyy");
                });
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcustshipDto>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rcustmainDto>> GetListByClientCodPer(string cmpy, string perfil, string codUsu)
        {
            List<mob_rcustmainDto> lstAsociados = null;
            try
            {
                string sqlQuery = "";

                if (perfil == "1")
                {
                    sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd, CONCAT(CONCAT(TRIM(rctt_razsoc),' | '),TRIM(rctt_addr)) as resumendatos " +
                                      "FROM mob_rcustrans WHERE rctt_cmpy ='" + cmpy + "'  AND rctt_codusu ='" + codUsu + "'";
                }
                else if (perfil == "2")
                {
                    sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd, CONCAT(CONCAT(TRIM(rctt_nomcli),' | '),TRIM(rctt_addr)) as resumendatos " +
                                      "FROM mob_rcustmain WHERE rctt_cmpy ='" + cmpy + "'  AND rctt_codusu ='" + codUsu + "'";
                }
                else if (perfil == "3")
                {
                    sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd, CONCAT(CONCAT(TRIM(rctt_nomcli),' | '),TRIM(rctt_addr)) as resumendatos " +
                                      "FROM mob_rcustases WHERE rctt_cmpy ='" + cmpy + "'  AND rctt_codusu ='" + codUsu + "'";

                }
                lstAsociados =  await _context.mob_rcustmainDtos.FromSqlRaw(sqlQuery).ToListAsync();
                lstAsociados.ForEach(x =>
                {
                    //x.rctt_level = x.rctt_level.Trim();
                    x.rctt_date = Convert.ToDateTime(x.rctt_date).ToString("MM/dd/yyyy");
                    x.rctt_feccum = Convert.ToDateTime(x.rctt_feccum).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcustmainDto>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rcustmainDto>> GetListByIdentity(string code)
        {

            List<mob_rcustmainDto> lstAsociados = null;
            try
            {
                string sqlQuery = "SELECT rctt_cmpy, ases_custship.ascs_code,rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd, '' as resumendatos" +
                    " FROM ases_custship ases_custship LEFT JOIN mob_rcustmain mob_rcustmain ON ases_custship.ascs_cust = mob_rcustmain.rctt_cust AND ases_custship.ascs_ship = mob_rcustmain.rctt_succli" +
                                  " WHERE ases_custship.ascs_code ='" + code + "'";
                lstAsociados = await _context.mob_rcustmainDtos.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcustmainDto>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rcustmainDto>> GetListIdentityByGc(string code, int gc)
        {

            List<mob_rcustmainDto> lstAsociados = null;
            try
            {
                string sqlQuery = "SELECT rctt_cmpy, ases_custship.ascs_code,rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd, '' as resumendatos" +
                    " FROM ases_custship ases_custship LEFT JOIN mob_rcustmain mob_rcustmain ON ases_custship.ascs_cust = mob_rcustmain.rctt_cust AND ases_custship.ascs_ship = mob_rcustmain.rctt_succli LEFT JOIN custtype custtype ON mob_rcustmain.rctt_type = custtype.cu_type AND mob_rcustmain.rctt_cmpy = custtype.cu_cmpy LEFT JOIN grupo_canal grupo_canal ON custtype.cu_comm = grupo_canal.gc_grupo AND custtype.cu_cmpy = grupo_canal.gc_cmpy" +
                    " WHERE ases_custship.ascs_code ='" + code + "' AND gc_grupo =" + gc;
                lstAsociados = await _context.mob_rcustmainDtos.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcustmainDto>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rcustmainDto>> GetDetailDataClient(string perfil, string cmpy, string cus, string succli, string tipoDoc)
        {
            List<mob_rcustmainDto> lstAsociados = null;
            try
            {
                string sqlQuery = "";

                if (perfil == "1")
                {
                    sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd, '' as resumendatos " +
                                      "FROM mob_rcustrans WHERE rctt_cmpy ='" + cmpy + "'  AND rctt_cust ='" + cus + "'  AND rctt_succli ='" + succli + "'  AND rctt_tipdoc ='" + tipoDoc + "'";
                }
                else if (perfil == "2")
                {
                    sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd, '' as resumendatos " +
                                      "FROM mob_rcustmain WHERE rctt_cmpy ='" + cmpy + "'  AND rctt_cust ='" + cus + "'  AND rctt_succli ='" + succli + "'  AND rctt_tipdoc ='" + tipoDoc + "'";
                }
                else if (perfil == "3")
                {
                    sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd, '' as resumendatos " +
                                      "FROM mob_rcustases WHERE rctt_cmpy ='" + cmpy + "'  AND rctt_cust ='" + cus + "'  AND rctt_succli ='" + succli + "'  AND rctt_tipdoc ='" + tipoDoc + "'";

                }
                lstAsociados = await _context.mob_rcustmainDtos.FromSqlRaw(sqlQuery).ToListAsync();
                lstAsociados.ForEach(x =>
                {
                    //x.rctt_level = x.rctt_level.Trim();
                    x.rctt_date = Convert.ToDateTime(x.rctt_date).ToString("MM/dd/yyyy");
                    x.rctt_feccum = Convert.ToDateTime(x.rctt_feccum).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcustmainDto>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<EstadoClienteE>> GetListStatus(string cmpy)
        {
            List<EstadoClienteE> estados = null;
            try
            {
                estados = await _context.estadocliente.Where(x => x.est_cmpy == cmpy).ToListAsync();
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(estados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                estados = JsonSerializer.Deserialize<List<EstadoClienteE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return estados;
        }

        public int InsertDetail(mob_rcusdetalleCommand client)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "Select * FROM mob_rcusdetalle WHERE rcusde_cmpy = @p_cmpy AND rcusde_coddet = @p_coddet AND rcusde_cus = @p_cus AND rcusde_succli = @p_succli";
                            command.Parameters.Add(new DB2Parameter("p_cmpy", client.rcusde_cmpy));
                            command.Parameters.Add(new DB2Parameter("p_coddet", client.rcusde_coddet));
                            command.Parameters.Add(new DB2Parameter("p_cus", client.rcusde_cus));
                            command.Parameters.Add(new DB2Parameter("p_succli", client.rcusde_succli));

                            using (var resultCommand = command.ExecuteReader())
                            {
                                bool hasRows = resultCommand.Read();
                                resultCommand.Close();

                                if (!hasRows)
                                {
                                    using (var insertCommand = connection.CreateCommand())
                                    {
                                        insertCommand.Transaction = transaction;
                                        insertCommand.CommandText = "Insert into mob_rcusdetalle (rcusde_cmpy,rcusde_cus, rcusde_succli, rcusde_coddet, rcusde_desdet, rcusde_valdet, rcusde_enter, rcusde_date, rcusde_time, rcusde_estadoupd) values (@rcusde_cmpy,@rcusde_cus,@rcusde_succli,@rcusde_coddet,@rcusde_desdet,@rcusde_valdet,@rcusde_enter,@rcusde_date,@rcusde_time,@rcusde_estadoupd)";
                                        insertCommand.Parameters.Add(new DB2Parameter("rcusde_cmpy", client.rcusde_cmpy));
                                        insertCommand.Parameters.Add(new DB2Parameter("rcusde_cus", client.rcusde_cus));
                                        insertCommand.Parameters.Add(new DB2Parameter("rcusde_succli", client.rcusde_succli));
                                        insertCommand.Parameters.Add(new DB2Parameter("rcusde_coddet", client.rcusde_coddet));
                                        insertCommand.Parameters.Add(new DB2Parameter("rcusde_desdet", client.rcusde_desdet));
                                        insertCommand.Parameters.Add(new DB2Parameter("rcusde_valdet", client.rcusde_valdet));
                                        insertCommand.Parameters.Add(new DB2Parameter("rcusde_enter", client.rcusde_enter));
                                        insertCommand.Parameters.Add(new DB2Parameter("rcusde_date", client.rcusde_date));
                                        insertCommand.Parameters.Add(new DB2Parameter("rcusde_time", client.rcusde_time));
                                        insertCommand.Parameters.Add(new DB2Parameter("rcusde_estadoupd", client.rcusde_estadoupd));
                                        result = insertCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int InsertNuevos(mob_rcustshipCommand client)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var insertCommand = connection.CreateCommand())
                        {
                            insertCommand.Transaction = transaction;
                            insertCommand.CommandText = "Insert into mob_rcustship (rctt_cmpy,rctt_reg,rctt_cust,rctt_tipdoc,rctt_resal,rctt_succli,rctt_razsoc,rctt_nomcli,rctt_addr,rctt_tele,rctt_celular,rctt_fax,rctt_dist,rctt_sic,rctt_codusu,rctt_ruta,rctt_sec,rctt_clinvo,rctt_date,rctt_estrato,rctt_latitud,rctt_longitud,rctt_frevisita,rctt_horsugvis,rctt_codabc,rctt_type,rctt_zip,rctt_zip4,rctt_vot,rctt_limit,rctt_level,rctt_bal,rctt_cont,rctt_feccum,rctt_pedmini,rctt_diasent,rctt_tax,rctt_nomcan,rctt_schrg,rctt_diavisita,rctt_mail,rctt_mppago,rctt_vpmin,rctt_zone,rctt_entdt,rctt_time,rctt_estadoupd,rctt_urlimagen) values (@rctt_cmpy,@rctt_reg,@rctt_cust,@rctt_tipdoc,@rctt_resal,@rctt_succli,@rctt_razsoc,@rctt_nomcli,@rctt_addr,@rctt_tele,@rctt_celular,@rctt_fax,@rctt_dist,@rctt_sic,@rctt_codusu,@rctt_ruta,@rctt_sec,@rctt_clinvo,@rctt_date,@rctt_estrato,@rctt_latitud,@rctt_longitud,@rctt_frevisita,@rctt_horsugvis,@rctt_codabc,@rctt_type,@rctt_zip,@rctt_zip4,@rctt_vot,@rctt_limit,@rctt_level,@rctt_bal,@rctt_cont,@rctt_feccum,@rctt_pedmini,@rctt_diasent,@rctt_tax,@rctt_nomcan,@rctt_schrg,@rctt_diavisita,@rctt_mail,@rctt_mppago,@rctt_vpmin,@rctt_zone,@rctt_entdt,@rctt_time,@rctt_estadoupd,@rctt_urlimagen)";
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_cmpy", client.rctt_cmpy));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_reg", client.rctt_reg));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_cust", client.rctt_cust));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_tipdoc", client.rctt_tipdoc));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_resal", client.rctt_resal));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_succli", client.rctt_succli));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_razsoc", client.rctt_razsoc));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_nomcli", client.rctt_nomcli));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_addr", client.rctt_addr));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_tele", client.rctt_tele));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_celular", client.rctt_celular));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_fax", client.rctt_fax));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_dist", client.rctt_dist));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_sic", client.rctt_sic));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_codusu", client.rctt_codusu));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_ruta", client.rctt_ruta));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_sec", client.rctt_sec));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_clinvo", client.rctt_clinvo));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_date", client.rctt_date));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_estrato", client.rctt_estrato));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_latitud", client.rctt_latitud));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_longitud", client.rctt_longitud));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_frevisita", client.rctt_frevisita));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_horsugvis", client.rctt_horsugvis));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_codabc", client.rctt_codabc));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_type", client.rctt_type));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_zip", client.rctt_zip));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_zip4", client.rctt_zip4));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_vot", client.rctt_vot));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_limit", client.rctt_limit));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_level", client.rctt_level));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_bal", client.rctt_bal));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_cont", client.rctt_cont));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_feccum", client.rctt_feccum));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_pedmini", client.rctt_pedmini));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_diasent", client.rctt_diasent));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_tax", client.rctt_tax));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_nomcan", client.rctt_nomcan));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_schrg", client.rctt_schrg));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_diavisita", client.rctt_diavisita));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_mail", client.rctt_mail));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_mppago", client.rctt_mppago));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_vpmin", client.rctt_vpmin));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_zone", client.rctt_zone));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_entdt", client.rctt_entdt));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_time", client.rctt_time));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_estadoupd", client.rctt_estadoupd));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_urlimagen", client.rctt_urlimagen));
                            result = insertCommand.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int InsertVerificado(mob_rcustshipCommand client)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "SELECT * FROM mob_rcustship WHERE rctt_cmpy = @p_cmpy AND rctt_cust = @p_cust";
                            command.Parameters.Add(new DB2Parameter("p_cmpy", client.rctt_cmpy));
                            command.Parameters.Add(new DB2Parameter("p_cust", client.rctt_cust));

                            using (var resultCommand = command.ExecuteReader())
                            {
                                bool hasRows = resultCommand.Read();
                                resultCommand.Close();

                                if (!hasRows)
                                {
                                    using (var insertCommand = connection.CreateCommand())
                                    {
                                        insertCommand.Transaction = transaction;
                                        insertCommand.CommandText = "Insert into mob_rcustship(rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc, rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg, rctt_diavisita, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd, rctt_urlimagen) values(@rctt_cmpy, @rctt_reg, @rctt_cust, @rctt_tipdoc, @rctt_resal, @rctt_succli, @rctt_razsoc, @rctt_nomcli, @rctt_addr, @rctt_tele, @rctt_celular, @rctt_fax, @rctt_dist, @rctt_sic, @rctt_codusu, @rctt_ruta, @rctt_sec, @rctt_clinvo, @rctt_date, @rctt_estrato, @rctt_latitud, @rctt_longitud, @rctt_frevisita, @rctt_horsugvis, @rctt_codabc, @rctt_type, @rctt_zip, @rctt_zip4, @rctt_vot, @rctt_limit, @rctt_level, @rctt_bal, @rctt_cont, @rctt_feccum, @rctt_pedmini, @rctt_diasent, @rctt_tax, @rctt_nomcan, @rctt_schrg, @rctt_diavisita, @rctt_mail, @rctt_mppago, @rctt_vpmin, @rctt_zone, @rctt_entdt, @rctt_time, @rctt_estadoupd, @rctt_urlimagen)";
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_cmpy", client.rctt_cmpy));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_reg", client.rctt_reg));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_cust", client.rctt_cust));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_tipdoc", client.rctt_tipdoc));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_resal", client.rctt_resal));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_succli", client.rctt_succli));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_razsoc", client.rctt_razsoc));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_nomcli", client.rctt_nomcli));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_addr", client.rctt_addr));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_tele", client.rctt_tele));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_celular", client.rctt_celular));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_fax", client.rctt_fax));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_dist", client.rctt_dist));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_sic", client.rctt_sic));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_codusu", client.rctt_codusu));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_ruta", client.rctt_ruta));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_sec", client.rctt_sec));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_clinvo", client.rctt_clinvo));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_date", client.rctt_date));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_estrato", client.rctt_estrato));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_latitud", client.rctt_latitud));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_longitud", client.rctt_longitud));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_frevisita", client.rctt_frevisita));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_horsugvis", client.rctt_horsugvis));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_codabc", client.rctt_codabc));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_type", client.rctt_type));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_zip", client.rctt_zip));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_zip4", client.rctt_zip4));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_vot", client.rctt_vot));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_limit", client.rctt_limit));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_level", client.rctt_level));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_bal", client.rctt_bal));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_cont", client.rctt_cont));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_feccum", client.rctt_feccum));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_pedmini", client.rctt_pedmini));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_diasent", client.rctt_diasent));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_tax", client.rctt_tax));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_nomcan", client.rctt_nomcan));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_schrg", client.rctt_schrg));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_diavisita", client.rctt_diavisita));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_mail", client.rctt_mail));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_mppago", client.rctt_mppago));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_vpmin", client.rctt_vpmin));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_zone", client.rctt_zone));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_entdt", client.rctt_entdt));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_time", client.rctt_time));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_estadoupd", client.rctt_estadoupd));
                                        insertCommand.Parameters.Add(new DB2Parameter("rctt_urlimagen", client.rctt_urlimagen));
                                        result = insertCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int InsertActualizados(mob_rcustshipCommand client)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var insertCommand = connection.CreateCommand())
                        {
                            insertCommand.Transaction = transaction;
                            insertCommand.CommandText = "Insert into mob_rcustship (rctt_cmpy,rctt_reg,rctt_cust,rctt_tipdoc,rctt_resal,rctt_succli,rctt_razsoc,rctt_nomcli,rctt_addr,rctt_tele,rctt_celular,rctt_fax,rctt_dist,rctt_sic,rctt_codusu,rctt_ruta,rctt_sec,rctt_clinvo,rctt_date,rctt_estrato,rctt_latitud,rctt_longitud,rctt_frevisita,rctt_horsugvis,rctt_codabc,rctt_type,rctt_zip,rctt_zip4,rctt_vot,rctt_limit,rctt_level,rctt_bal,rctt_cont,rctt_feccum,rctt_pedmini,rctt_diasent,rctt_tax,rctt_nomcan,rctt_schrg,rctt_diavisita,rctt_mail,rctt_mppago,rctt_vpmin,rctt_zone,rctt_entdt,rctt_time,rctt_estadoupd,rctt_urlimagen) values (@rctt_cmpy,@rctt_reg,@rctt_cust,@rctt_tipdoc,@rctt_resal,@rctt_succli,@rctt_razsoc,@rctt_nomcli,@rctt_addr,@rctt_tele,@rctt_celular,@rctt_fax,@rctt_dist,@rctt_sic,@rctt_codusu,@rctt_ruta,@rctt_sec,@rctt_clinvo,@rctt_date,@rctt_estrato,@rctt_latitud,@rctt_longitud,@rctt_frevisita,@rctt_horsugvis,@rctt_codabc,@rctt_type,@rctt_zip,@rctt_zip4,@rctt_vot,@rctt_limit,@rctt_level,@rctt_bal,@rctt_cont,@rctt_feccum,@rctt_pedmini,@rctt_diasent,@rctt_tax,@rctt_nomcan,@rctt_schrg,@rctt_diavisita,@rctt_mail,@rctt_mppago,@rctt_vpmin,@rctt_zone,@rctt_entdt,@rctt_time,@rctt_estadoupd,@rctt_urlimagen)";
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_cmpy", client.rctt_cmpy));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_reg", client.rctt_reg));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_cust", client.rctt_cust));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_tipdoc", client.rctt_tipdoc));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_resal", client.rctt_resal));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_succli", client.rctt_succli));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_razsoc", client.rctt_razsoc));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_nomcli", client.rctt_nomcli));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_addr", client.rctt_addr));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_tele", client.rctt_tele));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_celular", client.rctt_celular));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_fax", client.rctt_fax));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_dist", client.rctt_dist));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_sic", client.rctt_sic));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_codusu", client.rctt_codusu));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_ruta", client.rctt_ruta));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_sec", client.rctt_sec));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_clinvo", client.rctt_clinvo));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_date", client.rctt_date));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_estrato", client.rctt_estrato));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_latitud", client.rctt_latitud));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_longitud", client.rctt_longitud));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_frevisita", client.rctt_frevisita));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_horsugvis", client.rctt_horsugvis));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_codabc", client.rctt_codabc));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_type", client.rctt_type));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_zip", client.rctt_zip));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_zip4", client.rctt_zip4));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_vot", client.rctt_vot));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_limit", client.rctt_limit));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_level", client.rctt_level));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_bal", client.rctt_bal));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_cont", client.rctt_cont));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_feccum", client.rctt_feccum));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_pedmini", client.rctt_pedmini));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_diasent", client.rctt_diasent));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_tax", client.rctt_tax));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_nomcan", client.rctt_nomcan));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_schrg", client.rctt_schrg));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_diavisita", client.rctt_diavisita));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_mail", client.rctt_mail));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_mppago", client.rctt_mppago));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_vpmin", client.rctt_vpmin));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_zone", client.rctt_zone));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_entdt", client.rctt_entdt));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_time", client.rctt_time));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_estadoupd", client.rctt_estadoupd));
                            insertCommand.Parameters.Add(new DB2Parameter("rctt_urlimagen", client.rctt_urlimagen));
                            result = insertCommand.ExecuteNonQuery(); 
                        }

                        using (var updateCoordCommand = connection.CreateCommand())
                        {
                            updateCoordCommand.Transaction = transaction;
                            updateCoordCommand.CommandText = "UPDATE ajb.xcustship1 SET xcs1_x = @xcs1_x, xcs1_y = @xcs1_y WHERE xcs1_cmpy = @xcs1_cmpy AND xcs1_cust = @xcs1_cust AND xcs1_ship = @xcs1_ship";
                            updateCoordCommand.Parameters.Add(new DB2Parameter("xcs1_x", client.rctt_longitud));
                            updateCoordCommand.Parameters.Add(new DB2Parameter("xcs1_y", client.rctt_latitud));
                            updateCoordCommand.Parameters.Add(new DB2Parameter("xcs1_cmpy", client.rctt_cmpy));
                            updateCoordCommand.Parameters.Add(new DB2Parameter("xcs1_cust", client.rctt_cust));
                            updateCoordCommand.Parameters.Add(new DB2Parameter("xcs1_ship", client.rctt_succli));
                            updateCoordCommand.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int UpdateDetail(mob_rcusdetalleCommand client)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "SELECT * FROM mob_rcusdetalle WHERE rcusde_cmpy = @p_cmpy AND rcusde_coddet = @p_coddet AND rcusde_cus = @p_cus AND rcusde_succli = @p_succli";
                            command.Parameters.Add(new DB2Parameter("p_cmpy", client.rcusde_cmpy));
                            command.Parameters.Add(new DB2Parameter("p_coddet", client.rcusde_coddet));
                            command.Parameters.Add(new DB2Parameter("p_cus", client.rcusde_cus));
                            command.Parameters.Add(new DB2Parameter("p_succli", client.rcusde_succli));

                            using (var resultCommand = command.ExecuteReader())
                            {
                                bool hasRows = resultCommand.Read();
                                resultCommand.Close();

                                if (hasRows)
                                {
                                    using (var updateCommand = connection.CreateCommand())
                                    {
                                        updateCommand.Transaction = transaction;
                                        updateCommand.CommandText = "UPDATE mob_rcusdetalle SET rcusde_valdet = @rcusde_valdet, rcusde_enter = @rcusde_enter, rcusde_date = @rcusde_date, rcusde_time = @rcusde_time, rcusde_estadoupd = @rcusde_estadoupd WHERE rcusde_cmpy = @rcusde_cmpy AND rcusde_coddet = @rcusde_coddet AND rcusde_cus = @rcusde_cus AND rcusde_succli = @rcusde_succli";
                                        updateCommand.Parameters.Add(new DB2Parameter("rcusde_valdet", client.rcusde_valdet));
                                        updateCommand.Parameters.Add(new DB2Parameter("rcusde_enter", client.rcusde_enter));
                                        updateCommand.Parameters.Add(new DB2Parameter("rcusde_date", client.rcusde_date));
                                        updateCommand.Parameters.Add(new DB2Parameter("rcusde_time", client.rcusde_time));
                                        updateCommand.Parameters.Add(new DB2Parameter("rcusde_estadoupd", client.rcusde_estadoupd));
                                        updateCommand.Parameters.Add(new DB2Parameter("rcusde_cmpy", client.rcusde_cmpy));
                                        updateCommand.Parameters.Add(new DB2Parameter("rcusde_coddet", client.rcusde_coddet));
                                        updateCommand.Parameters.Add(new DB2Parameter("rcusde_cus", client.rcusde_cus));
                                        updateCommand.Parameters.Add(new DB2Parameter("rcusde_succli", client.rcusde_succli));
                                        result = updateCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int DeleteDetail(string cmpy, string cus, string succli)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "SELECT * FROM mob_rcusdetalle WHERE rcusde_cmpy = @p_cmpy AND rcusde_cus = @p_cus AND rcusde_succli = @p_succli";
                            command.Parameters.Add(new DB2Parameter("p_cmpy", cmpy));
                            command.Parameters.Add(new DB2Parameter("p_cus", cus));
                            command.Parameters.Add(new DB2Parameter("p_succli", succli));

                            using (var resultCommand = command.ExecuteReader())
                            {
                                bool hasRows = resultCommand.Read();
                                resultCommand.Close();

                                if (hasRows)
                                {
                                    using (var deleteCommand = connection.CreateCommand())
                                    {
                                        deleteCommand.Transaction = transaction;
                                        deleteCommand.CommandText = "DELETE FROM mob_rcusdetalle WHERE rcusde_cmpy = @rcusde_cmpy AND rcusde_cus = @rcusde_cus AND rcusde_succli = @rcusde_succli";
                                        deleteCommand.Parameters.Add(new DB2Parameter("rcusde_cmpy", cmpy));
                                        deleteCommand.Parameters.Add(new DB2Parameter("rcusde_cus", cus));
                                        deleteCommand.Parameters.Add(new DB2Parameter("rcusde_succli", succli));
                                        result = deleteCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int Delete(string cmpy, string cust, string succli, string codusu)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "SELECT * FROM mob_rcustship WHERE rctt_cmpy = @p_cmpy AND rctt_cust = @p_cust AND rctt_succli = @p_succli AND rctt_codusu = @p_codusu";
                            command.Parameters.Add(new DB2Parameter("p_cmpy", cmpy));
                            command.Parameters.Add(new DB2Parameter("p_cust", cust));
                            command.Parameters.Add(new DB2Parameter("p_succli", succli));
                            command.Parameters.Add(new DB2Parameter("p_codusu", codusu));

                            using (var resultCommand = command.ExecuteReader())
                            {
                                bool hasRows = resultCommand.Read();
                                resultCommand.Close();

                                if (hasRows)
                                {
                                    using (var deleteCommand = connection.CreateCommand())
                                    {
                                        deleteCommand.Transaction = transaction;
                                        deleteCommand.CommandText = "DELETE FROM mob_rcustship WHERE rctt_cmpy = @rctt_cmpy AND rctt_cust = @rctt_cust AND rctt_succli = @rctt_succli AND rctt_codusu = @rctt_codusu";
                                        deleteCommand.Parameters.Add(new DB2Parameter("rctt_cmpy", cmpy));
                                        deleteCommand.Parameters.Add(new DB2Parameter("rctt_cust", cust));
                                        deleteCommand.Parameters.Add(new DB2Parameter("rctt_succli", succli));
                                        deleteCommand.Parameters.Add(new DB2Parameter("rctt_codusu", codusu));
                                        result = deleteCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public async Task<List<mob_rcustmainDto>> GetList(string cmpy, string codusu)
        {

            List<mob_rcustmainDto> lstAsociados = null;
            try
            {
                string sqlQuery = "";
                if (codusu != "")
                {
                    sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd, '' as resumendatos " +
                                  "FROM mob_rcustmain " +
                                  " WHERE rctt_cmpy ='" + cmpy + "' and rctt_codusu in (" + codusu + ")";
                }
                else {
                    sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, rctt_entdt, rctt_time, rctt_estadoupd, '' as resumendatos " +
                                    "FROM mob_rcustmain " +
                                    " WHERE rctt_cmpy ='" + cmpy + "' and rctt_codusu  = 'NA'";
                }
                 
                lstAsociados = await _context.mob_rcustmainDtos.FromSqlRaw(sqlQuery).ToListAsync();
                lstAsociados.ForEach(x =>
                {
                    x.rctt_level = x.rctt_level.Trim();
                    x.rctt_date = Convert.ToDateTime(x.rctt_date).ToString("MM/dd/yyyy");
                    x.rctt_feccum = Convert.ToDateTime(x.rctt_feccum).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcustmainDto>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

    }
}
