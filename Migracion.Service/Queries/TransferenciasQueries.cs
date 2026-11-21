using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Models.Command;
using Migracion.Models.Dtos;
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
    public interface ITransferenciasQueries
    {

        Task<List<mob_rtransdetE>> GetListDetail(string cmpy, string codUsu, string tipo, string id, string num);
        Task<List<mob_rtransferenciaE>> GetList(string cmpy, string codUsu, string tipo, string num);
        Task<List<mob_rtransferencia_hisE>> GetListHis(string numero, string cmpy, string codUsu, string tipo, string cus, string succli, string custdep);
        Task<List<mob_rtransdet_hisE>> GetListDetailHis(string cmpy, string codUsu, string num, string tipo);
        Task<List<mob_rtransferenciahisE>> GetListHisDeposit(string codUsu);
        Task<List<mob_transferencia_full_dto>> GetFullHis(string numero, string cmpy, string codUsu, string tipo, string cus, string succli, string custdep);
        int InsertDetail(mob_rtransdetCommand detalle);
        int Insert(mob_rtransferenciaE transferencia);
        int UpdateDetail(mob_rtransdetCommand detalle);
        int Update(mob_rtransferenciaE transferencia);

        int DeleteDetail(string cmpy, string num, string tipo, string codusu);
        int DeleteDetailByProd(string cmpy, string num, string tipo, string codusu, string id);
        int Delete(string cmpy, string num, string tipo, string codusu);
    }

    public class TransferenciasQueries : ITransferenciasQueries
    {
        private readonly ApplicationDbContext _context;
        public TransferenciasQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<mob_rtransdetE>> GetListDetail(string cmpy, string codUsu, string tipo, string id, string num)
        {
            List<mob_rtransdetE> lstAcciones = null;
            try
            {

                lstAcciones = await _context.mob_rtransdet.Where(x => x.rtransd_cmpy == cmpy && x.rtransd_codusu == codUsu
                    && x.rtransd_tipo == tipo && x.rtransd_id == id && x.rtransd_num == num).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rtransdetE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

        public async Task<List<mob_rtransdet_hisE>> GetListDetailHis(string cmpy, string codUsu, string num, string tipo)
        {
            List<mob_rtransdet_hisE> lstAcciones = null;
            try
            {
                string sqlQuery = "Select rtransd_cmpy, rtransd_num, rtransd_codprodep, rtransd_id, rtransd_codusu, rtransd_linea, rtransd_cant, rtransd_vlrparcial, rtransd_dctogral, rtransd_vlrtotaliva, rtransd_vlrtotal, rtransd_tipo " +
                                  "FROM mob_rtransdet " +
                                  "WHERE rtransd_cmpy='" + cmpy + "' AND rtransd_codusu='" + codUsu + "' AND rtransd_num= '" + num + "' AND rtransd_tipo='" + tipo + "'";
                lstAcciones = await _context.mob_rtransdet_his.FromSqlRaw(sqlQuery).ToListAsync();

                lstAcciones.ForEach(x =>
                {
                    x.rtransd_tipo = x.rtransd_tipo.Trim();
                });


                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rtransdet_hisE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

        public async Task<List<mob_rtransferenciahisE>> GetListHisDeposit(string codUsu)
        {
            List<mob_rtransferenciahisE> lstAcciones = null;
            try
            {
                string sqlQuery = "Select rctt_cust, rctt_succli, rtrans_tipo, rtrans_custdep " +
                                  "FROM mob_rcustrans,mob_rtransferencia " +
                                  "WHERE  rctt_cust=rtrans_cus AND rctt_succli=rtrans_succli AND rtrans_codusu='" + codUsu+ "' " +
                                  "GROUP BY rctt_cust,rctt_succli,rtrans_tipo,rtrans_custdep ";
                lstAcciones = await _context.mob_rtransferenciahisEs.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rtransferenciahisE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

       

        public async Task<List<mob_rtransferenciaE>> GetList(string cmpy, string codUsu, string tipo, string num)
        {
            List<mob_rtransferenciaE> lstAcciones = null;
            try
            {

                lstAcciones = await _context.mob_rtransferencia.Where(x => x.rtrans_cmpy == cmpy && x.rtrans_codusu == codUsu
                    && x.rtrans_tipo == tipo && x.rtrans_num == num).ToListAsync();

                lstAcciones.ForEach(x =>
                {
                    x.rtrans_fectrans = Convert.ToDateTime(x.rtrans_fectrans).ToString("MM/dd/yyyy");
                    x.rtrans_fecenvio = Convert.ToDateTime(x.rtrans_fecenvio).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rtransferenciaE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

        public async Task<List<mob_rtransferencia_hisE>> GetListHis(string numero, string cmpy, string codUsu, string tipo, string cus, string succli, string dep)
        {
            List<mob_rtransferencia_hisE> lstTransferenciasHis = null;
            try
            {
                string sqlQuery = "Select first " + numero + " rtrans_cmpy, rtrans_num, rtrans_tipo, rtrans_cus, rtrans_succli, rtrans_codusu, rtrans_custdep, rtrans_coddeposito, rtrans_codcon, rtrans_fectrans, rtrans_horini, rtrans_horfin," +
                                  "rtrans_estenvio, rtrans_fecenvio, rtrans_vlrparcial, rtrans_dctogral, rtrans_totaliva, rtrans_total, rtrans_observacion, rtrans_marca " +
                                  "FROM mob_rtransferencia " +
                                  "WHERE rtrans_cmpy='" + cmpy + "' AND rtrans_codusu='" + codUsu + "' AND rtrans_tipo='" + tipo + "' AND rtrans_cus='" + cus + "' AND rtrans_succli='" + succli + "' AND rtrans_custdep='" + dep + "'" + "  ORDER BY rtrans_fectrans DESC";
                lstTransferenciasHis = await _context.mob_rtransferencia_his.FromSqlRaw(sqlQuery).ToListAsync();

                lstTransferenciasHis.ForEach(x =>
                {
                    x.rtrans_tipo = x.rtrans_tipo.Trim();
                    x.rtrans_fectrans = Convert.ToDateTime(x.rtrans_fectrans).ToString("MM/dd/yyyy");
                    x.rtrans_fecenvio = Convert.ToDateTime(x.rtrans_fecenvio).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstTransferenciasHis), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstTransferenciasHis = JsonSerializer.Deserialize<List<mob_rtransferencia_hisE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstTransferenciasHis;
        }

        public async Task<List<mob_transferencia_full_dto>> GetFullHis(string numero, string cmpy, string codUsu, string tipo, string cus, string succli, string custdep)
        {

            List<mob_rtransferencia_hisE> lstHis = null;
            List<mob_transferencia_full_dto> result_list = new List<mob_transferencia_full_dto>();
            try
            {

                lstHis = await GetListHis( numero,  cmpy,  codUsu,  tipo,  cus,  succli,  custdep);
                lstHis.ForEach(async x =>
                {
                    List<mob_rtransdet_hisE> lst_linea = await GetListDetailHis(cmpy, codUsu, x.rtrans_num,tipo);
                    mob_transferencia_full_dto item_full = new mob_transferencia_full_dto()
                    {
                        item = x,
                        detail = lst_linea
                    };
                    result_list.Add(item_full);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result_list;
        }

        public int InsertDetail(mob_rtransdetCommand detalle)
        {

            int result = 0;
            try
            {

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                command.CommandText = "SELECT * FROM mob_rtransdet " +
                    "WHERE rtransd_cmpy = '" + detalle.rtransd_cmpy + "' AND rtransd_codusu = '" + detalle.rtransd_codusu + "' AND rtransd_tipo = '" + detalle.rtransd_tipo + "' AND rtransd_id = '" + detalle.rtransd_id + "' AND rtransd_num = '" + detalle.rtransd_num + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                      if (!resultCommand.HasRows)
                       {
                            var commandText = "Insert into mob_rtransdet (rtransd_cmpy, rtransd_num, rtransd_tipo, rtransd_codprodep, rtransd_id, rtransd_codusu, rtransd_linea, rtransd_cant, rtransd_vlrparcial, rtransd_dctogral, rtransd_vlrtotaliva, rtransd_vlrtotal,rtransd_enter,rtransd_date,rtransd_time,rtransd_estadoupd) values (@rtransd_cmpy,@rtransd_num,@rtransd_tipo,@rtransd_codprodep,@rtransd_id,@rtransd_codusu,@rtransd_linea,@rtransd_cant,@rtransd_vlrparcial,@rtransd_dctogral,@rtransd_vlrtotaliva,@rtransd_vlrtotal,@rtransd_enter,@rtransd_date,@rtransd_time,@rtransd_estadoupd)";
                            result = _context.Database.ExecuteSqlRaw(commandText,
                                new DB2Parameter("rtransd_cmpy", detalle.rtransd_cmpy),
                                new DB2Parameter("rtransd_num", detalle.rtransd_num),
                                new DB2Parameter("rtransd_tipo", detalle.rtransd_tipo),
                                new DB2Parameter("rtransd_codprodep", detalle.rtransd_codprodep),
                                new DB2Parameter("rtransd_id", detalle.rtransd_id),
                                new DB2Parameter("rtransd_codusu", detalle.rtransd_codusu),
                                new DB2Parameter("rtransd_linea", detalle.rtransd_linea),
                                new DB2Parameter("rtransd_cant", detalle.rtransd_cant),
                                new DB2Parameter("rtransd_vlrparcial", detalle.rtransd_vlrparcial),
                                new DB2Parameter("rtransd_dctogral", detalle.rtransd_dctogral),
                                new DB2Parameter("rtransd_vlrtotaliva", detalle.rtransd_vlrtotaliva),
                                new DB2Parameter("rtransd_vlrtotal", detalle.rtransd_vlrtotal),
                                new DB2Parameter("rtransd_enter", detalle.rtransd_enter),
                                new DB2Parameter("rtransd_date", detalle.rtransd_date),
                                new DB2Parameter("rtransd_time", detalle.rtransd_time),
                                new DB2Parameter("rtransd_estadoupd", detalle.rtransd_estadoupd)
                             );
                       }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int Insert(mob_rtransferenciaE transferencia)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * FROM mob_rtransferencia " +
                                          " WHERE rtrans_cmpy = '" + transferencia.rtrans_cmpy + "' AND rtrans_codusu = '" + transferencia.rtrans_codusu + "' AND rtrans_tipo = '" + transferencia.rtrans_tipo + "' AND rtrans_num = '" + transferencia.rtrans_num + "'  AND rtrans_tipo = '" + transferencia.rtrans_tipo + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rtransferencia (rtrans_cmpy,rtrans_num,rtrans_tipo,rtrans_cus,rtrans_succli,rtrans_codusu,rtrans_custdep,rtrans_coddeposito,rtrans_codcon,rtrans_fectrans,rtrans_horini,rtrans_horfin,rtrans_estenvio,rtrans_fecenvio,rtrans_vlrparcial,rtrans_dctogral,rtrans_totaliva,rtrans_total,rtrans_observacion,rtrans_marca,rtrans_enter,rtrans_date,rtrans_time,rtrans_estadoupd,rtrans_latitud,rtrans_longitud) values (@rtrans_cmpy,@rtrans_num,@rtrans_tipo,@rtrans_cus,@rtrans_succli,@rtrans_codusu,@rtrans_custdep,@rtrans_coddeposito,@rtrans_codcon,@rtrans_fectrans,@rtrans_horini,@rtrans_horfin,@rtrans_estenvio,@rtrans_fecenvio,@rtrans_vlrparcial,@rtrans_dctogral,@rtrans_totaliva,@rtrans_total,@rtrans_observacion,@rtrans_marca,@rtrans_enter,@rtrans_date,@rtrans_time,@rtrans_estadoupd,@rtrans_latitud,@rtrans_longitud)";
                            result = _context.Database.ExecuteSqlRaw(commandText,
                                new DB2Parameter("rtrans_cmpy", transferencia.rtrans_cmpy),
                                new DB2Parameter("rtrans_num", transferencia.rtrans_num),
                                new DB2Parameter("rtrans_tipo", transferencia.rtrans_tipo),
                                new DB2Parameter("rtrans_cus", transferencia.rtrans_cus),
                                new DB2Parameter("rtrans_succli", transferencia.rtrans_succli),
                                new DB2Parameter("rtrans_codusu", transferencia.rtrans_codusu),
                                new DB2Parameter("rtrans_custdep", transferencia.rtrans_custdep),
                                new DB2Parameter("rtrans_coddeposito", transferencia.rtrans_coddeposito),
                                new DB2Parameter("rtrans_codcon", transferencia.rtrans_codcon),
                                new DB2Parameter("rtrans_fectrans", transferencia.rtrans_fectrans),
                                new DB2Parameter("rtrans_horini", transferencia.rtrans_horini),
                                new DB2Parameter("rtrans_horfin", transferencia.rtrans_horfin),
                                new DB2Parameter("rtrans_estenvio", transferencia.rtrans_estenvio),
                                new DB2Parameter("rtrans_fecenvio", transferencia.rtrans_fecenvio),
                                new DB2Parameter("rtrans_vlrparcial", transferencia.rtrans_vlrparcial),
                                new DB2Parameter("rtrans_dctogral", transferencia.rtrans_dctogral),
                                new DB2Parameter("rtrans_totaliva", transferencia.rtrans_totaliva),
                                new DB2Parameter("rtrans_total", transferencia.rtrans_total),
                                new DB2Parameter("rtrans_observacion", transferencia.rtrans_observacion),
                                new DB2Parameter("rtrans_marca", transferencia.rtrans_marca),
                                new DB2Parameter("rtrans_enter", transferencia.rtrans_enter),
                                new DB2Parameter("rtrans_date", transferencia.rtrans_date),
                                new DB2Parameter("rtrans_time", transferencia.rtrans_time),
                                new DB2Parameter("rtrans_estadoupd", transferencia.rtrans_estadoupd),
                                new DB2Parameter("rtrans_latitud", transferencia.rtrans_latitud),
                                new DB2Parameter("rtrans_longitud", transferencia.rtrans_longitud)
                             );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int UpdateDetail(mob_rtransdetCommand detalle)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rtransdet WHERE rtransd_cmpy = '"+ detalle.rtransd_cmpy + "' AND rtransd_codusu = '"+ detalle.rtransd_codusu + "' AND rtransd_tipo = '"+ detalle.rtransd_tipo + "' AND rtransd_id = '"+ detalle.rtransd_id + "' AND rtransd_num = '"+ detalle.rtransd_num + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "UPDATE mob_rtransdet SET rtransd_cant = @rtransd_cant, rtransd_vlrparcial = @rtransd_vlrparcial, rtransd_dctogral = @rtransd_dctogral, " +
                                "rtransd_vlrtotaliva =@rtransd_vlrtotaliva, rtransd_vlrtotal = @rtransd_vlrtotal, rtransd_date = @rtransd_date, rtransd_time = @rtransd_time, " +
                                "rtransd_estadoupd = @rtransd_estadoupd " +
                                "WHERE rtransd_cmpy =@rtransd_cmpy AND rtransd_codusu = @rtransd_codusu AND rtransd_tipo = @rtransd_tipo AND rtransd_id = @rtransd_id AND rtransd_num = @rtransd_num";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rtransd_cant", detalle.rtransd_cant),
                                 new DB2Parameter("rtransd_vlrparcial", detalle.rtransd_vlrparcial),
                                 new DB2Parameter("rtransd_dctogral", detalle.rtransd_dctogral),
                                 new DB2Parameter("rtransd_vlrtotaliva", detalle.rtransd_vlrtotaliva),
                                 new DB2Parameter("rtransd_vlrtotal", detalle.rtransd_vlrtotal),
                                 new DB2Parameter("rtransd_date", detalle.rtransd_date),
                                 new DB2Parameter("rtransd_time", detalle.rtransd_time),
                                 new DB2Parameter("rtransd_estadoupd", detalle.rtransd_estadoupd),
                                 new DB2Parameter("rtransd_cmpy", detalle.rtransd_cmpy),
                                 new DB2Parameter("rtransd_codusu", detalle.rtransd_codusu),
                                 new DB2Parameter("rtransd_tipo", detalle.rtransd_tipo),
                                 new DB2Parameter("rtransd_id", detalle.rtransd_id),
                                 new DB2Parameter("rtransd_num", detalle.rtransd_num)
                                 );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int Update(mob_rtransferenciaE transferencia)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rtransferencia WHERE rtrans_codusu = '"+ transferencia.rtrans_codusu + "' AND rtrans_tipo = '"+ transferencia.rtrans_tipo + "' AND rtrans_num = '"+ transferencia.rtrans_num + "' AND rtrans_cmpy = '"+ transferencia.rtrans_cmpy + "' AND rtrans_cus = '"+ transferencia.rtrans_cus + "' AND rtrans_succli = '"+ transferencia.rtrans_succli + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "UPDATE mob_rtransferencia SET rtrans_fectrans = @rtrans_fectrans, rtrans_fecenvio = @rtrans_fecenvio, rtrans_vlrparcial = @rtrans_vlrparcial, " +
                                "rtrans_dctogral = @rtrans_dctogral, rtrans_totaliva = @rtrans_totaliva, rtrans_total = @rtrans_total, rtrans_observacion = @rtrans_observacion " +
                                "WHERE rtrans_codusu = @rtrans_codusu AND rtrans_tipo = @rtrans_tipo AND rtrans_num = @rtrans_num AND rtrans_cmpy = @rtrans_cmpy AND " +
                                "rtrans_cus = @rtrans_cus AND rtrans_succli = @rtrans_succli";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rtrans_fectrans", transferencia.rtrans_fectrans),
                                 new DB2Parameter("rtrans_fecenvio", transferencia.rtrans_fecenvio),
                                 new DB2Parameter("rtrans_vlrparcial", transferencia.rtrans_vlrparcial),
                                 new DB2Parameter("rtrans_dctogral", transferencia.rtrans_dctogral),
                                 new DB2Parameter("rtrans_totaliva", transferencia.rtrans_totaliva),
                                 new DB2Parameter("rtrans_total", transferencia.rtrans_total),
                                 new DB2Parameter("rtrans_observacion", transferencia.rtrans_observacion),
                                 new DB2Parameter("rtrans_codusu", transferencia.rtrans_codusu),
                                 new DB2Parameter("rtrans_tipo", transferencia.rtrans_tipo),
                                 new DB2Parameter("rtrans_num", transferencia.rtrans_num),
                                 new DB2Parameter("rtrans_cmpy", transferencia.rtrans_cmpy),
                                 new DB2Parameter("rtrans_cus", transferencia.rtrans_cus),
                                 new DB2Parameter("rtrans_succli", transferencia.rtrans_succli)
                                 );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int  DeleteDetail(string cmpy, string num, string tipo, string codusu)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rtransdet " +
                                          "WHERE rtransd_cmpy = '" + cmpy + "' AND rtransd_num = '" + num + "' AND rtransd_tipo = '" + tipo + "' AND rtransd_codusu = '" + codusu + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rtransdet " +
                                              "WHERE rtransd_cmpy = '" + cmpy + "' AND rtransd_num = '" + num + "' AND rtransd_tipo = '" + tipo + "' AND rtransd_codusu = '" + codusu + "'";

                            result = _context.Database.ExecuteSqlRaw(commandText);
                       }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int DeleteDetailByProd(string cmpy, string num, string tipo, string codusu, string id)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rtransdet " +
                                          "WHERE rtransd_cmpy = '" + cmpy + "' AND rtransd_num = '" + num + "' AND rtransd_tipo = '" + tipo + "' AND rtransd_codusu = '" + codusu + "' AND rtransd_id = '" + id + "'";

                    _context.Database.OpenConnection();
                     using (var resultCommand = command.ExecuteReader())
                     {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rtransdet " +
                                              "WHERE rtransd_cmpy = '" + cmpy + "' AND rtransd_num = '" + num + "' AND rtransd_tipo = '" + tipo + "' AND rtransd_codusu = '" + codusu + "' AND rtransd_id = '" + id + "'";

                            result = _context.Database.ExecuteSqlRaw(commandText);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int Delete(string cmpy, string num, string tipo, string codusu)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                     command.CommandText = "SELECT * FROM mob_rtransferencia " +
                                          "WHERE rtrans_cmpy = '" + cmpy  + "' AND rtrans_num = '" + num + "' and rtrans_tipo ='" + tipo + "' AND rtrans_codusu = '" + codusu +"'";

                    _context.Database.OpenConnection();
                     using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rtransferencia " +
                                              "WHERE rtrans_cmpy = '" + cmpy +"' AND rtrans_num = '" +num  + "' and rtrans_tipo ='" + tipo + "' AND rtrans_codusu = '" + codusu +"'";

                            result = _context.Database.ExecuteSqlRaw(commandText);
                        }
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
