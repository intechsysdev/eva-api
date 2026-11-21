using IBM.Data.Db2;
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
    public interface IDevolucionesQueries
    {
        Task<List<mob_rrmaheadE>> GetByReg(string cmpy, string teri, string reg, string cust, string ship, string num);
        Task<List<mob_rrmaheadE>> GetByDate(string cmpy, string teri, string reg, string num, string date);
        Task<List<mob_rrmaheadE>> GetByConsec(string cmpy, string teri, string reg, string num);
        Task<List<mob_rrmalineE>> GetDetail(string cmpy, string reg, string teri, string num, string part, string level, string entdt);
        Task<List<mob_rrmalineE>> GetDetailByProduct(string cmpy, string reg, string teri, string num, string part);

        int Insert(mob_rrmaheadE devolucion);
        int InsertDetail(mob_rrmalineE detalle);
        int Update(mob_rrmaheadE item);
        int UpdateDetail(mob_rrmalineE item);
        int Delete(string cmpy, string num, string reg, string teri);
        int DeleteDetailByProd(string cmpy, string num, string reg, string teri,string part);
        int DeleteDetail(string cmpy, string num, string reg, string teri);
    }
    public class DevolucionesQueries : IDevolucionesQueries
    {
        private readonly ApplicationDbContext _context;
        public DevolucionesQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rrmaheadE>> GetByReg(string cmpy, string teri, string reg, string cust, string ship, string num)
        {

            List<mob_rrmaheadE> lstAsociados = null;
            try
            {

                lstAsociados = await _context.mob_rrmahead.Where(x => x.rmh_cmpy == cmpy && x.rmh_teri == teri &&
                x.rmh_reg == reg && x.rmh_cust == cust && x.rmh_ship == ship && x.rmh_num == num
                ).ToListAsync();

                lstAsociados.ForEach(x =>
                {
                    x.rmh_entdt = Convert.ToDateTime(x.rmh_entdt).ToString("MM/dd/yyyy");
                    x.rmh_date = Convert.ToDateTime(x.rmh_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rrmaheadE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rrmaheadE>> GetByDate(string cmpy, string teri, string reg, string num, string date)
        {

            List<mob_rrmaheadE> lstAsociados = null;
            try
            {

                lstAsociados = await _context.mob_rrmahead.Where(x => x.rmh_cmpy == cmpy && x.rmh_teri == teri &&
                x.rmh_reg == reg && x.rmh_num == num && x.rmh_entdt == date
                ).ToListAsync();

                lstAsociados.ForEach(x =>
                {
                    x.rmh_entdt = Convert.ToDateTime(x.rmh_entdt).ToString("MM/dd/yyyy");
                    x.rmh_date = Convert.ToDateTime(x.rmh_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rrmaheadE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rrmaheadE>> GetByConsec(string cmpy, string teri, string reg, string num)
        {

            List<mob_rrmaheadE> lstAsociados = null;
            try
            {
                lstAsociados = await _context.mob_rrmahead.Where(x => x.rmh_cmpy == cmpy && x.rmh_teri == teri &&
                x.rmh_reg == reg && x.rmh_num == num
                ).ToListAsync();

                lstAsociados.ForEach(x =>
                {
                    x.rmh_entdt = Convert.ToDateTime(x.rmh_entdt).ToString("MM/dd/yyyy");
                    x.rmh_date = Convert.ToDateTime(x.rmh_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rrmaheadE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rrmalineE>> GetDetail(string cmpy, string reg, string teri, string num, string part, string level, string entdt)
        {

            List<mob_rrmalineE> lstAsociados = null;
            try
            {
                lstAsociados = await _context.mob_rrmaline.Where(x => x.rll_cmpy == cmpy && x.rll_reg == reg &&
                x.rll_teri == teri && x.rll_num == num && x.rll_part == part && x.rll_level == level && x.rll_entdt == entdt
                ).ToListAsync();

                lstAsociados.ForEach(x =>
                {
                    x.rll_entdt = Convert.ToDateTime(x.rll_entdt).ToString("MM/dd/yyyy");
                    x.rll_date = Convert.ToDateTime(x.rll_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rrmalineE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rrmalineE>> GetDetailByProduct(string cmpy, string reg, string teri, string num, string part)
        {

            List<mob_rrmalineE> lstAsociados = null;
            try
            {
                lstAsociados = await _context.mob_rrmaline.Where(x => x.rll_cmpy == cmpy && x.rll_reg == reg &&
                x.rll_teri == teri && x.rll_num == num && x.rll_part == part
                ).ToListAsync();

                lstAsociados.ForEach(x =>
                {
                    x.rll_entdt = Convert.ToDateTime(x.rll_entdt).ToString("MM/dd/yyyy");
                    x.rll_date = Convert.ToDateTime(x.rll_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rrmalineE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public int Insert(mob_rrmaheadE devolucion)
        {
            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * FROM mob_rrmahead WHERE rmh_cmpy = '" + devolucion.rmh_cmpy + "' AND rmh_teri = '" + devolucion.rmh_teri + "' AND rmh_num = '" + devolucion.rmh_num + "'  AND rmh_entdt = '" + devolucion.rmh_entdt + "'";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "insert into mob_rrmahead(rmh_cmpy, rmh_reg, rmh_teri, rmh_num, rmh_entdt, rmh_time, rmh_com, rmh_cust, rmh_ship, rmh_job, rmh_tipo, rmh_memo, rmh_num2, rmh_marca, rmh_date) values(@rmh_cmpy, @rmh_reg, @rmh_teri, @rmh_num, @rmh_entdt, @rmh_time, @rmh_com, @rmh_cust, @rmh_ship, @rmh_job, @rmh_tipo, @rmh_memo, @rmh_num2, @rmh_marca, @rmh_date)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rmh_cmpy", devolucion.rmh_cmpy),
                                 new DB2Parameter("rmh_reg", devolucion.rmh_reg),
                                 new DB2Parameter("rmh_teri", devolucion.rmh_teri),
                                 new DB2Parameter("rmh_num", devolucion.rmh_num),
                                 new DB2Parameter("rmh_entdt", devolucion.rmh_entdt),
                                 new DB2Parameter("rmh_time", devolucion.rmh_time),
                                 new DB2Parameter("rmh_com", devolucion.rmh_com),
                                 new DB2Parameter("rmh_cust", devolucion.rmh_cust),
                                 new DB2Parameter("rmh_ship", devolucion.rmh_ship),
                                 new DB2Parameter("rmh_job", devolucion.rmh_job),
                                 new DB2Parameter("rmh_tipo", devolucion.rmh_tipo),
                                 new DB2Parameter("rmh_memo", devolucion.rmh_memo),
                                 new DB2Parameter("rmh_num2", devolucion.rmh_num2),
                                 new DB2Parameter("rmh_marca", devolucion.rmh_marca),
                                 new DB2Parameter("rmh_date", devolucion.rmh_date)

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

        public int InsertDetail(mob_rrmalineE detalle)
        {
            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * FROM mob_rrmaline WHERE rll_cmpy = '" + detalle.rll_cmpy + "' AND rll_teri = '" + detalle.rll_teri + "' AND rll_num = '" + detalle.rll_num +
                        "' AND rll_part = '" + detalle.rll_part + "' AND rll_level = '" + detalle.rll_level + "' AND rll_entdt = '" + detalle.rll_entdt + "'";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "insert into mob_rrmaline (rll_cmpy,rll_reg,rll_teri,rll_num,rll_entdt,rll_time,rll_part,rll_qord,rll_txbl,rll_level,rll_marca,rll_date) values ( @rll_cmpy, @rll_reg, @rll_teri, @rll_num, @rll_entdt, @rll_time, @rll_part, @rll_qord, @rll_txbl, @rll_level, @rll_marca, @rll_date)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rll_cmpy", detalle.rll_cmpy),
                                 new DB2Parameter("rll_reg", detalle.rll_reg),
                                 new DB2Parameter("rll_teri", detalle.rll_teri),
                                 new DB2Parameter("rll_num", detalle.rll_num),
                                 new DB2Parameter("rll_entdt", detalle.rll_entdt),
                                 new DB2Parameter("rll_time", detalle.rll_time),
                                 new DB2Parameter("rll_part", detalle.rll_part),
                                 new DB2Parameter("rll_qord", detalle.rll_qord),
                                 new DB2Parameter("rll_txbl", detalle.rll_txbl),
                                 new DB2Parameter("rll_level", detalle.rll_level),
                                 new DB2Parameter("rll_marca", detalle.rll_marca),
                                 new DB2Parameter("rll_date", detalle.rll_date)

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

        public int Update(mob_rrmaheadE item)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rrmahead " +
                                          "WHERE rmh_cmpy = '" + item.rmh_cmpy + "' AND rmh_reg = '" + item.rmh_reg + "' AND rmh_teri = '" + item.rmh_teri + "' AND rmh_num = '" + item.rmh_num + "' AND rmh_entdt = '" + item.rmh_entdt + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "UPDATE mob_rrmahead SET rmh_com = @rmh_com, rmh_job  = @rmh_job, rmh_tipo = @rmh_tipo, rmh_memo = @rmh_memo, rmh_date = @rmh_date, rmh_num2 = @rmh_num2 " +
                                              "WHERE rmh_cmpy = @rmh_cmpy AND rmh_reg = @rmh_reg AND rmh_teri = @rmh_teri AND rmh_num = @rmh_num AND rmh_entdt = @rmh_entdt";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rmh_com", item.rmh_com),
                                 new DB2Parameter("rmh_job", item.rmh_job),
                                 new DB2Parameter("rmh_tipo", item.rmh_tipo),
                                 new DB2Parameter("rmh_memo", item.rmh_memo),
                                 new DB2Parameter("rmh_date", item.rmh_date),
                                 new DB2Parameter("rmh_num2", item.rmh_num2),
                                 new DB2Parameter("rmh_cmpy", item.rmh_cmpy),
                                 new DB2Parameter("rmh_reg", item.rmh_reg),
                                 new DB2Parameter("rmh_teri", item.rmh_teri),
                                 new DB2Parameter("rmh_num", item.rmh_num),
                                 new DB2Parameter("rmh_entdt", item.rmh_entdt)
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

        public int UpdateDetail(mob_rrmalineE item)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rrmaline " +
                                          "WHERE rll_cmpy = '" + item.rll_cmpy+ "' AND rll_num = '" + item.rll_num+ "' AND rll_reg = '" + item.rll_reg+ "' AND rll_teri = '" + item.rll_teri+ "' AND rll_part = '" + item.rll_part+ "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                         {
                            var commandText = "UPDATE mob_rrmaline SET rll_entdt = @rll_entdt,rll_time = @rll_time,rll_qord = @rll_qord,rll_txbl = @rll_txbl,rll_level = @rll_level,rll_marca = @rll_marca,rll_date = @rll_date " +
                                              "WHERE rll_cmpy = @rll_cmpy AND rll_reg = @rll_reg AND rll_teri = @rll_teri AND rll_num = @rll_num AND rll_part = @rll_part AND rll_level = @rll_level AND rll_entdt = @rll_entdt";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rll_entdt", item.rll_entdt),
                                 new DB2Parameter("rll_time", item.rll_time),
                                 new DB2Parameter("rll_qord", item.rll_qord),
                                 new DB2Parameter("rll_txbl", item.rll_txbl),
                                 new DB2Parameter("rll_level", item.rll_level),
                                 new DB2Parameter("rll_marca", item.rll_marca),
                                 new DB2Parameter("rll_date", item.rll_date),
                                 new DB2Parameter("rll_cmpy", item.rll_cmpy),
                                 new DB2Parameter("rll_reg", item.rll_reg),
                                 new DB2Parameter("rll_teri", item.rll_teri),
                                 new DB2Parameter("rll_num", item.rll_num),
                                 new DB2Parameter("rll_part", item.rll_part)
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

        public int Delete(string cmpy, string num, string reg, string teri)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rrmahead " +
                                          "WHERE rmh_cmpy = '" + cmpy + "' AND rmh_num = '" + num + "' AND rmh_reg = '" + reg + "' AND rmh_teri = '" + teri + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rrmahead " +
                                             "WHERE rmh_cmpy = '" + cmpy + "' AND rmh_num = '" + num + "' AND rmh_reg ='" + reg + "' AND rmh_teri = '" + teri + "'";

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

        public int DeleteDetailByProd(string cmpy, string num, string reg, string teri, string part)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rrmaline " +
                                          "WHERE rll_cmpy = '" +cmpy+ "' AND rll_num = '" +num+ "' AND rll_reg = '" +reg+ "' AND rll_teri = '" + teri+ "' AND rll_part = '" +part+ "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rrmaline " +
                                              "WHERE rll_cmpy = '" +cmpy+ "' AND rll_num = '" +num+ "' AND rll_reg = '" +reg+ "' AND rll_teri = '" +teri+ "' AND rll_part = '" + part+ "'";

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

        public int DeleteDetail(string cmpy, string num, string reg, string teri)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rrmaline " +
                                          "WHERE rll_cmpy = '" + cmpy + "' AND rll_num = '" + num + "' AND rll_reg = '" + reg + "' AND rll_teri = '" + teri + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rrmaline " +
                                              "WHERE rll_cmpy = '" + cmpy + "' AND rll_num = '" + num + "' AND rll_reg = '" + reg + "' AND rll_teri = '" + teri + "'";

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
