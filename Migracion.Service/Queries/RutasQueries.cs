using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Migracion.Models;
using Migracion.Models.Command;
using Migracion.Models.Dtos;
using Migracion.Persistence.DataBase;
using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Migracion.Service.Queries
{
    public interface IRutasQueries
    {
        Task<List<mob_rrutasE>> GetList(string cmpy, string codUsu);
        Task<List<mob_rrutasDto>> GetDetailDataCliente(string cmpy, string[] codUsu,string[] codRuta,  string fecRuta);
        Task<List<mob_rrutasE>> GetListByFecha(string cmpy, string fecRuta, string codUsu);
        List<int> GetListDistinct(string cmpy);
        int Insert(mob_rrutasE ruta);
        int Delete(string cmpy, string cus, string succli, string codUsu);
        
    }
    public class RutasQueries : IRutasQueries
    {
        private readonly ApplicationDbContext _context;
        public RutasQueries(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<List<mob_rrutasE>> GetList(string cmpy, string codUsu)
        {
            List<mob_rrutasE> rutas = null;
            try
            {

                rutas = await _context.mob_rrutas.Where(x => x.rruta_cmpy == cmpy && x.rruta_codusu == codUsu).ToListAsync();
                rutas.ForEach(x => x.rruta_fecha = Convert.ToDateTime(x.rruta_fecha).ToString("MM/dd/yyyy"));

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(rutas), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                rutas = JsonSerializer.Deserialize<List<mob_rrutasE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rutas;
        }

        public async Task <List<mob_rrutasDto>> GetDetailDataCliente(string cmpy, string[] codUsu, string[] codRuta, string fecRuta)
        {
            List<mob_rrutasDto> rutas = null;
            try
            {
                string stringJoin = "";
                string stringRutas = "";
                if (codUsu.Length > 0)
                {
                    foreach (var item in codUsu)
                    {
                        stringJoin += $"'{item}',";
                    }
                    stringJoin = stringJoin.Substring(0, stringJoin.Length - 1);
                }

                if (codRuta.Length > 0)
                {
                    foreach (var item in codRuta)
                    {
                        stringRutas += $"'{item}',";
                    }

                    stringRutas = stringRutas.Substring(0, stringRutas.Length - 1);
                }

                string sqlQuery = "";

                if (stringRutas == "") {
                    sqlQuery = "SELECT rruta_cmpy,rruta_codusu,mob_rusuarios.rusu_nombre,mob_rusuarios.rusu_codper,rruta_cus,rruta_succli,rctt_razsoc,rctt_nomcli,rruta_ruta,rruta_sec,rruta_fecha,rruta_date,rruta_time,rruta_estadoupd,rctt_reg,region.reg_desc,rctt_tipdoc,rctt_tele,rctt_celular,rctt_dist,mob_district.dis_desc,rctt_ruta,rctt_sec, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_type,mob_rcusttype.cu_desc" +
                       " FROM mob_rrutas mob_rrutas INNER JOIN mob_rcustmain mob_rcustmain ON mob_rrutas.rruta_cus = mob_rcustmain.rctt_cust AND mob_rrutas.rruta_succli = mob_rcustmain.rctt_succli" +
                       " LEFT JOIN region ON region.reg_regi = mob_rcustmain.rctt_reg AND region.reg_cmpy = mob_rcustmain.rctt_cmpy" +
                       " LEFT JOIN mob_district mob_district ON mob_rcustmain.rctt_dist=mob_district.dis_dist" +
                       " LEFT JOIN mob_rusuarios mob_rusuarios ON mob_rrutas.rruta_codusu=mob_rusuarios.rusu_codusu" +
                       " LEFT JOIN mob_rcusttype mob_rcusttype ON mob_rcustmain.rctt_type=mob_rcusttype.cu_type  " +
                       " WHERE rruta_cmpy = '" + cmpy + "' AND rruta_codusu  in (" + stringJoin + ")  AND rruta_fecha = '" + fecRuta + "'" +
                       "UNION ALL " +
                       "SELECT rruta_cmpy,rruta_codusu,mob_rusuarios.rusu_nombre,mob_rusuarios.rusu_codper,rruta_cus,rruta_succli,rctt_razsoc,rctt_nomcli,rruta_ruta,rruta_sec,rruta_fecha,rruta_date,rruta_time,rruta_estadoupd,rctt_reg,region.reg_desc,rctt_tipdoc,rctt_tele,rctt_celular,rctt_dist,mob_district.dis_desc,rctt_ruta,rctt_sec, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_type,mob_rcusttype.cu_desc" +
                       " FROM mob_rrutas mob_rrutas INNER JOIN mob_rcustrans mob_rcustmain ON mob_rrutas.rruta_cus = mob_rcustmain.rctt_cust AND mob_rrutas.rruta_succli = mob_rcustmain.rctt_succli" +
                       " LEFT JOIN region ON region.reg_regi = mob_rcustmain.rctt_reg AND region.reg_cmpy = mob_rcustmain.rctt_cmpy" +
                       " LEFT JOIN mob_district mob_district ON mob_rcustmain.rctt_dist=mob_district.dis_dist" +
                       " LEFT JOIN mob_rusuarios mob_rusuarios ON mob_rrutas.rruta_codusu=mob_rusuarios.rusu_codusu" +
                       " LEFT JOIN mob_rcusttype mob_rcusttype ON mob_rcustmain.rctt_type=mob_rcusttype.cu_type  " +
                       " WHERE rruta_cmpy = '" + cmpy + "' AND rruta_codusu  in (" + stringJoin + ")  AND rruta_fecha = '" + fecRuta + "'" +
                       "UNION ALL " +
                       "SELECT rruta_cmpy,rruta_codusu,mob_rusuarios.rusu_nombre,mob_rusuarios.rusu_codper,rruta_cus,rruta_succli,rctt_razsoc,rctt_nomcli,rruta_ruta,rruta_sec,rruta_fecha,rruta_date,rruta_time,rruta_estadoupd,rctt_reg,region.reg_desc,rctt_tipdoc,rctt_tele,rctt_celular,rctt_dist,mob_district.dis_desc,rctt_ruta,rctt_sec, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_type,mob_rcusttype.cu_desc" +
                       " FROM mob_rrutas mob_rrutas INNER JOIN mob_rcustases mob_rcustmain ON mob_rrutas.rruta_cus = mob_rcustmain.rctt_cust AND mob_rrutas.rruta_succli = mob_rcustmain.rctt_succli" +
                       " LEFT JOIN region ON region.reg_regi = mob_rcustmain.rctt_reg AND region.reg_cmpy = mob_rcustmain.rctt_cmpy" +
                       " LEFT JOIN mob_district mob_district ON mob_rcustmain.rctt_dist=mob_district.dis_dist" +
                       " LEFT JOIN mob_rusuarios mob_rusuarios ON mob_rrutas.rruta_codusu=mob_rusuarios.rusu_codusu" +
                       " LEFT JOIN mob_rcusttype mob_rcusttype ON mob_rcustmain.rctt_type=mob_rcusttype.cu_type  " +
                       " WHERE rruta_cmpy = '" + cmpy + "' AND rruta_codusu  in (" + stringJoin + ")  AND rruta_fecha = '" + fecRuta + "'" +

                       " ORDER BY rruta_fecha,rruta_ruta,rruta_sec";
                }
                else
                {
                    sqlQuery = "SELECT rruta_cmpy,rruta_codusu,mob_rusuarios.rusu_nombre,mob_rusuarios.rusu_codper,rruta_cus,rruta_succli,rctt_razsoc,rctt_nomcli,rruta_ruta,rruta_sec,rruta_fecha,rruta_date,rruta_time,rruta_estadoupd,rctt_reg,region.reg_desc,rctt_tipdoc,rctt_tele,rctt_celular,rctt_dist,mob_district.dis_desc,rctt_ruta,rctt_sec, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_type,mob_rcusttype.cu_desc" +
                       " FROM mob_rrutas mob_rrutas INNER JOIN mob_rcustmain mob_rcustmain ON mob_rrutas.rruta_cus = mob_rcustmain.rctt_cust AND mob_rrutas.rruta_succli = mob_rcustmain.rctt_succli" +
                       " LEFT JOIN region ON region.reg_regi = mob_rcustmain.rctt_reg AND region.reg_cmpy = mob_rcustmain.rctt_cmpy" +
                       " LEFT JOIN mob_district mob_district ON mob_rcustmain.rctt_dist=mob_district.dis_dist" +
                       " LEFT JOIN mob_rusuarios mob_rusuarios ON mob_rrutas.rruta_codusu=mob_rusuarios.rusu_codusu" +
                       " LEFT JOIN mob_rcusttype mob_rcusttype ON mob_rcustmain.rctt_type=mob_rcusttype.cu_type  " +
                       " WHERE rruta_cmpy = '" + cmpy + "' AND rruta_codusu  in (" + stringJoin + ")  AND rruta_fecha = '" + fecRuta + "' AND rruta_ruta  in (" + stringRutas + ")" +
                       "UNION ALL " +
                        "SELECT rruta_cmpy,rruta_codusu,mob_rusuarios.rusu_nombre,mob_rusuarios.rusu_codper,rruta_cus,rruta_succli,rctt_razsoc,rctt_nomcli,rruta_ruta,rruta_sec,rruta_fecha,rruta_date,rruta_time,rruta_estadoupd,rctt_reg,region.reg_desc,rctt_tipdoc,rctt_tele,rctt_celular,rctt_dist,mob_district.dis_desc,rctt_ruta,rctt_sec, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_type,mob_rcusttype.cu_desc" +
                       " FROM mob_rrutas mob_rrutas INNER JOIN mob_rcustrans mob_rcustmain ON mob_rrutas.rruta_cus = mob_rcustmain.rctt_cust AND mob_rrutas.rruta_succli = mob_rcustmain.rctt_succli" +
                       " LEFT JOIN region ON region.reg_regi = mob_rcustmain.rctt_reg AND region.reg_cmpy = mob_rcustmain.rctt_cmpy" +
                       " LEFT JOIN mob_district mob_district ON mob_rcustmain.rctt_dist=mob_district.dis_dist" +
                       " LEFT JOIN mob_rusuarios mob_rusuarios ON mob_rrutas.rruta_codusu=mob_rusuarios.rusu_codusu" +
                       " LEFT JOIN mob_rcusttype mob_rcusttype ON mob_rcustmain.rctt_type=mob_rcusttype.cu_type  " +
                       " WHERE rruta_cmpy = '" + cmpy + "' AND rruta_codusu  in (" + stringJoin + ")  AND rruta_fecha = '" + fecRuta + "' AND rruta_ruta  in (" + stringRutas + ")" +
                       "UNION ALL " +
                        "SELECT rruta_cmpy,rruta_codusu,mob_rusuarios.rusu_nombre,mob_rusuarios.rusu_codper,rruta_cus,rruta_succli,rctt_razsoc,rctt_nomcli,rruta_ruta,rruta_sec,rruta_fecha,rruta_date,rruta_time,rruta_estadoupd,rctt_reg,region.reg_desc,rctt_tipdoc,rctt_tele,rctt_celular,rctt_dist,mob_district.dis_desc,rctt_ruta,rctt_sec, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_type,mob_rcusttype.cu_desc" +
                       " FROM mob_rrutas mob_rrutas INNER JOIN mob_rcustases mob_rcustmain ON mob_rrutas.rruta_cus = mob_rcustmain.rctt_cust AND mob_rrutas.rruta_succli = mob_rcustmain.rctt_succli" +
                       " LEFT JOIN region ON region.reg_regi = mob_rcustmain.rctt_reg AND region.reg_cmpy = mob_rcustmain.rctt_cmpy" +
                       " LEFT JOIN mob_district mob_district ON mob_rcustmain.rctt_dist=mob_district.dis_dist" +
                       " LEFT JOIN mob_rusuarios mob_rusuarios ON mob_rrutas.rruta_codusu=mob_rusuarios.rusu_codusu" +
                       " LEFT JOIN mob_rcusttype mob_rcusttype ON mob_rcustmain.rctt_type=mob_rcusttype.cu_type  " +
                        " WHERE rruta_cmpy = '" + cmpy + "' AND rruta_codusu  in (" + stringJoin + ")  AND rruta_fecha = '" + fecRuta + "' AND rruta_ruta  in (" + stringRutas + ")" +
                       " ORDER BY rruta_fecha,rruta_ruta,rruta_sec";
                }

                

                rutas = await _context.mob_rrutasDto.FromSqlRaw(sqlQuery).ToListAsync();

                rutas.ForEach(x =>
                {
                    x.rruta_fecha = Convert.ToDateTime(x.rruta_fecha).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(rutas), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                rutas = JsonSerializer.Deserialize<List<mob_rrutasDto>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rutas;
        }


        public List<int>  GetListDistinct(string cmpy)
        {
            List<int> rutas = new List<int>();
            try
            {
                List<string> LstDistinct = _context.mob_rrutas.Where(x => x.rruta_cmpy == cmpy).GroupBy(item => item.rruta_ruta).Select(s =>s.Key).AsEnumerable().ToList();
                LstDistinct.ForEach(x => {
                    rutas.Add(Convert.ToInt32(x));

                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rutas.OrderBy(x=>x).ToList();
        }

        public async Task<List<mob_rrutasE>> GetListByFecha(string cmpy, string fecRuta, string codUsu)
        {
            List<mob_rrutasE> rutas = null;
            try
            {

                rutas = await _context.mob_rrutas.Where(x => x.rruta_cmpy == cmpy && x.rruta_fecha == fecRuta && x.rruta_codusu == codUsu).ToListAsync();
                rutas.ForEach(x => x.rruta_fecha = Convert.ToDateTime(x.rruta_fecha).ToString("MM/dd/yyyy"));

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(rutas), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                rutas = JsonSerializer.Deserialize<List<mob_rrutasE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rutas;
        }

        public int Insert(mob_rrutasE ruta)
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
                            command.CommandText = "SELECT * FROM mob_rrutas WHERE rruta_cmpy = @p_cmpy AND rruta_cus = @p_cus AND rruta_succli = @p_succli AND rruta_ruta = @p_ruta";
                            command.Parameters.Add(new DB2Parameter("p_cmpy", ruta.rruta_cmpy));
                            command.Parameters.Add(new DB2Parameter("p_cus", ruta.rruta_cus));
                            command.Parameters.Add(new DB2Parameter("p_succli", ruta.rruta_succli));
                            command.Parameters.Add(new DB2Parameter("p_ruta", ruta.rruta_ruta));

                            using (var resultCommand = command.ExecuteReader())
                            {
                                bool hasRows = resultCommand.Read();
                                resultCommand.Close();

                                if (!hasRows)
                                {
                                    using (var insertCommand = connection.CreateCommand())
                                    {
                                        insertCommand.Transaction = transaction;
                                        insertCommand.CommandText = "INSERT INTO mob_rrutas(rruta_cmpy, rruta_codusu, rruta_cus, rruta_succli, rruta_ruta, rruta_sec, rruta_fecha) VALUES(@rruta_cmpy, @rruta_codusu, @rruta_cus, @rruta_succli, @rruta_ruta, @rruta_sec, @rruta_fecha)";
                                        insertCommand.Parameters.Add(new DB2Parameter("rruta_cmpy", ruta.rruta_cmpy));
                                        insertCommand.Parameters.Add(new DB2Parameter("rruta_codusu", ruta.rruta_codusu));
                                        insertCommand.Parameters.Add(new DB2Parameter("rruta_cus", ruta.rruta_cus));
                                        insertCommand.Parameters.Add(new DB2Parameter("rruta_succli", ruta.rruta_succli));
                                        insertCommand.Parameters.Add(new DB2Parameter("rruta_ruta", ruta.rruta_ruta));
                                        insertCommand.Parameters.Add(new DB2Parameter("rruta_sec", ruta.rruta_sec));
                                        insertCommand.Parameters.Add(new DB2Parameter("rruta_fecha", ruta.rruta_fecha));
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

        public int Delete(string cmpy, string cus, string succli, string codUsu)
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
                            command.CommandText = "SELECT * FROM mob_rrutas WHERE rruta_cmpy = @p_cmpy AND rruta_cus = @p_cus AND rruta_succli = @p_succli AND rruta_codusu = @p_codusu";
                            command.Parameters.Add(new DB2Parameter("p_cmpy", cmpy));
                            command.Parameters.Add(new DB2Parameter("p_cus", cus));
                            command.Parameters.Add(new DB2Parameter("p_succli", succli));
                            command.Parameters.Add(new DB2Parameter("p_codusu", codUsu));

                            using (var resultCommand = command.ExecuteReader())
                            {
                                bool hasRows = resultCommand.Read();
                                resultCommand.Close();

                                if (hasRows)
                                {
                                    using (var deleteCommand = connection.CreateCommand())
                                    {
                                        deleteCommand.Transaction = transaction;
                                        deleteCommand.CommandText = "DELETE FROM mob_rrutas WHERE rruta_cmpy = @rruta_cmpy AND rruta_cus = @rruta_cus AND rruta_succli = @rruta_succli AND rruta_codusu = @rruta_codusu";
                                        deleteCommand.Parameters.Add(new DB2Parameter("rruta_cmpy", cmpy));
                                        deleteCommand.Parameters.Add(new DB2Parameter("rruta_cus", cus));
                                        deleteCommand.Parameters.Add(new DB2Parameter("rruta_succli", succli));
                                        deleteCommand.Parameters.Add(new DB2Parameter("rruta_codusu", codUsu));
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
    }
}
