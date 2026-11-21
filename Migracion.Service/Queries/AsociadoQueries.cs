using IBM.Data.Db2;
using Microsoft.Data.SqlClient;
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
    public interface IAsociadoQueries
    {
        Task<List<mob_rasociadoE>> GetListByCodUsu(string cmpy, string codUsu);
        Task<List<mob_rasociadoE>> GetListByReg(string cmpy, string reg);

        int InsertAsociado(mob_rasociadoE asociado);
        int Update(mob_rasociadoE asociado);
        int Delete(string cmpy, string cust, string succli);
        int DeleteContact(string cmpy, string custd, string succli, string codcon);
        int DeleteDepositContact(string cmpy, string custd, string codcon);

    }
    public class AsociadoQueries : IAsociadoQueries
    {
        private readonly ApplicationDbContext _context;
        public AsociadoQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rasociadoE>> GetListByCodUsu(string cmpy, string codUsu)
        {
            List<mob_rasociadoE> lstAsociados = null;
            try
            {
                string sqlQuery = "Select rasoc_cmpy, rasoc_codcon, rasoc_cust, rasoc_succli, rasoc_coddeposito, rasoc_estado, rasoc_desestado, rasoc_date, rasoc_time, rasoc_estadoupd, rasoc_custd,rasoc_enter " +
                    " FROM mob_rasociado rasociado,mob_rcustrans rcustrans " +
                    " WHERE rasociado.rasoc_cmpy=rcustrans.rctt_cmpy AND rasociado.rasoc_cust=rcustrans.rctt_cust AND rasociado.rasoc_succli=rcustrans.rctt_succli  AND rctt_cmpy='" + cmpy + "' AND rctt_codusu= '" + codUsu + "'";
                lstAsociados = await _context.mob_rasociado.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rasociadoE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rasociadoE>> GetListByReg(string cmpy, string reg)
        {
            List<mob_rasociadoE> lstAsociados = null;
            try
            {
                string sqlQuery = "Select rasoc_cmpy, rasoc_codcon, rasoc_cust, rasoc_succli, rasoc_coddeposito, rasoc_estado, rasoc_desestado, rasoc_date, rasoc_time, rasoc_estadoupd, rasoc_custd,rasoc_enter" +
                    " FROM mob_rasociado rasociado,mob_rcustrans rcustrans " +
                    " WHERE rasociado.rasoc_cmpy=rcustrans.rctt_cmpy AND rasociado.rasoc_cust=rcustrans.rctt_cust AND rasociado.rasoc_succli=rcustrans.rctt_succli  AND rctt_cmpy='" + cmpy + "' AND rctt_reg= '" + reg + "'";
                lstAsociados = await _context.mob_rasociado.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rasociadoE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public int InsertAsociado(mob_rasociadoE asociado)
        {

            int result = 0;
            try
            {
                List<mob_rasociadoE> exist = _context.mob_rasociado.Where(x => x.rasoc_custd == asociado.rasoc_custd &&
                x.rasoc_codcon == asociado.rasoc_codcon && x.rasoc_cust == asociado.rasoc_cust && x.rasoc_succli == asociado.rasoc_succli
                && x.rasoc_coddeposito == asociado.rasoc_coddeposito
                ).ToList();

                if (exist.Count == 0)
                {
                    var commandText = "Insert into mob_rasociado (rasoc_cmpy,rasoc_custd,rasoc_codcon,rasoc_cust,rasoc_succli,rasoc_coddeposito,rasoc_estado,rasoc_desestado,rasoc_enter,rasoc_date,rasoc_time,rasoc_estadoupd) " +
                                      "values(@rasoc_cmpy,@rasoc_custd,@rasoc_codcon,@rasoc_cust,@rasoc_succli,@rasoc_coddeposito,@rasoc_estado,@rasoc_desestado,@rasoc_enter,@rasoc_date,@rasoc_time,@rasoc_estadoupd )";

                    result = _context.Database.ExecuteSqlRaw(commandText,
                         new DB2Parameter("rasoc_cmpy", asociado.rasoc_cmpy),
                         new DB2Parameter("rasoc_custd", asociado.rasoc_custd),
                         new DB2Parameter("rasoc_codcon", asociado.rasoc_codcon),
                         new DB2Parameter("rasoc_cust", asociado.rasoc_cust),
                         new DB2Parameter("rasoc_succli", asociado.rasoc_succli),
                         new DB2Parameter("rasoc_coddeposito", asociado.rasoc_coddeposito),
                         new DB2Parameter("rasoc_estado", asociado.rasoc_estado),
                         new DB2Parameter("rasoc_desestado", asociado.rasoc_desestado),
                         new DB2Parameter("rasoc_enter", asociado.rasoc_enter),
                         new DB2Parameter("rasoc_date", asociado.rasoc_date),
                         new DB2Parameter("rasoc_time", asociado.rasoc_time),
                         new DB2Parameter("rasoc_estadoupd", asociado.rasoc_estadoupd));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int Update(mob_rasociadoE asociado)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                   command.CommandText = "Select * FROM mob_rasociado WHERE rasoc_cmpy = '"+ asociado.rasoc_cmpy + "' AND rasoc_codcon = '"+ asociado.rasoc_codcon + "' AND rasoc_cust = '"+ asociado.rasoc_cust + "' AND rasoc_succli = '"+ asociado.rasoc_succli + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "UPDATE mob_rasociado SET rasoc_coddeposito = @rasoc_coddeposito, rasoc_enter = @rasoc_enter, rasoc_date = @rasoc_date, " +
                                "rasoc_time = @rasoc_time, rasoc_estadoupd = @rasoc_estadoupd " +
                                "WHERE rasoc_cmpy =  @rasoc_cmpy AND rasoc_codcon =  @rasoc_codcon AND rasoc_cust =  @rasoc_cust AND rasoc_succli =  @rasoc_succli";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rasoc_coddeposito", asociado.rasoc_coddeposito),
                                 new DB2Parameter("rasoc_enter", asociado.rasoc_enter),
                                 new DB2Parameter("rasoc_date", asociado.rasoc_date),
                                 new DB2Parameter("rasoc_time", asociado.rasoc_time),
                                 new DB2Parameter("rasoc_estadoupd", asociado.rasoc_estadoupd),
                                 new DB2Parameter("rasoc_cmpy", asociado.rasoc_cmpy),
                                 new DB2Parameter("rasoc_codcon", asociado.rasoc_codcon),
                                 new DB2Parameter("rasoc_cust", asociado.rasoc_cust),
                                 new DB2Parameter("rasoc_succli", asociado.rasoc_succli)
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

        public int Delete(string cmpy, string cust, string succli)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rasociado WHERE rasoc_cmpy = '"+ cmpy + "' AND rasoc_cust = '"+cust+"' AND rasoc_succli = '"+succli+"'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rasociado WHERE rasoc_cmpy = '" + cmpy + "' AND rasoc_cust = '" + cust + "' AND rasoc_succli = '" + succli + "'";

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

        public int DeleteContact(string cmpy, string custd, string succli, string codcon)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rasociado WHERE rasoc_cmpy = '"+ cmpy + "' AND rasoc_custd = '"+ custd + "' AND rasoc_succli = '"+ succli + "' AND rasoc_codcon = '"+ codcon + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rasociado WHERE rasoc_cmpy = '" + cmpy + "' AND rasoc_custd = '" + custd + "' AND rasoc_succli = '" + succli + "' AND rasoc_codcon = '" + codcon + "'";

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

        public int DeleteDepositContact(string cmpy, string custd, string codcon)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rasociado WHERE rasoc_cmpy = '"+ cmpy + "' AND rasoc_custd = '"+ custd + "' AND rasoc_codcon = '"+ codcon + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rasociado WHERE rasoc_cmpy = '" + cmpy + "' AND rasoc_custd = '" + custd + "' AND rasoc_codcon = '" + codcon + "'";

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
