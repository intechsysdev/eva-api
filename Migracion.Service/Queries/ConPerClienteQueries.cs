using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Models.Command;
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
    public interface IConPerClienteQueries
    {
        Task<List<mob_rconperclienteE>> GetList(string rconperc_cmpy, string rctt_codusu);
        int Insert(mob_rconperclienteCommand contacto);
        int Delete(string cmpy, string cus, string succli);
        int DeleteByContacto(string cmpy, string cus, string succli, string codcon);
        int DeleteByPerfil(string cmpy, string cus, string codcon, string codper);
    }
    public class ConPerClienteQueries : IConPerClienteQueries
    {
        private readonly ApplicationDbContext _context;
        public ConPerClienteQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<mob_rconperclienteE>> GetList(string rconperc_cmpy, string rctt_codusu)
        {
            List<mob_rconperclienteE> lstAsociados = null;
            try
            {
                string sqlQuery = "Select rconperc_cmpy, rconperc_cus, rconperc_succli, rconperc_codcon, rconperc_codper, rconperc_date, rconperc_time, rconperc_estadoupd " +
                                  " FROM mob_rconpercliente rconpercliente, mob_rcustrans rcustrans " +
                                  "WHERE rconpercliente.rconperc_cmpy = rcustrans.rctt_cmpy AND rconpercliente.rconperc_cus = rcustrans.rctt_cust AND rconpercliente.rconperc_succli = rcustrans.rctt_succli AND rconperc_cmpy ='" + rconperc_cmpy + "'" + "AND rctt_codusu = '" + rctt_codusu + "'";
                lstAsociados = await _context.mob_rconpercliente.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rconperclienteE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public int Insert(mob_rconperclienteCommand contacto)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText =
                "SELECT * FROM mob_rconpercliente " +
                "WHERE rconperc_cmpy='" + contacto.rconperc_cmpy + "' AND rconperc_codcon = '" + contacto.rconperc_codcon + "' AND rconperc_succli = '" + contacto.rconperc_succli + "' AND rconperc_cus = '" + contacto.rconperc_cus + "' AND rconperc_codper = '" + contacto.rconperc_codper + "' ";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rconpercliente (rconperc_cmpy,rconperc_cus,rconperc_succli,rconperc_codcon,rconperc_codper,rconperc_enter,rconperc_date,rconperc_time,rconperc_estadoupd) values (@rconperc_cmpy,@rconperc_cus,@rconperc_succli,@rconperc_codcon,@rconperc_codper,@rconperc_enter,@rconperc_date,@rconperc_time,@rconperc_estadoupd)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rconperc_cmpy", contacto.rconperc_cmpy),
                                 new DB2Parameter("rconperc_cus", contacto.rconperc_cus),
                                 new DB2Parameter("rconperc_succli", contacto.rconperc_succli),
                                 new DB2Parameter("rconperc_codcon", contacto.rconperc_codcon),
                                 new DB2Parameter("rconperc_codper", contacto.rconperc_codper),
                                 new DB2Parameter("rconperc_enter", contacto.rconperc_enter),
                                 new DB2Parameter("rconperc_date", contacto.rconperc_date),
                                 new DB2Parameter("rconperc_time", contacto.rconperc_time),
                                 new DB2Parameter("rconperc_estadoupd", contacto.rconperc_estadoupd)

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

        public int Delete(string cmpy, string cus, string succli)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rconpercliente WHERE rconperc_cmpy = '"+ cmpy + "' AND rconperc_cus = '"+ cus + "' AND rconperc_succli = '"+ succli + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rconpercliente WHERE rconperc_cmpy = '" + cmpy + "' AND rconperc_cus = '" + cus + "' AND rconperc_succli = '" + succli + "'";

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

        public int DeleteByContacto(string cmpy, string cus, string succli, string codcon)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rconpercliente WHERE rconperc_cmpy = '"+ cmpy + "' AND rconperc_cus = '"+ cus + "' AND rconperc_succli = '"+ succli + "' AND rconperc_codcon = '"+ codcon + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rconpercliente WHERE rconperc_cmpy = '" + cmpy + "' AND rconperc_cus = '" + cus + "' AND rconperc_succli = '" + succli + "' AND rconperc_codcon = '" + codcon + "'";

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

        public int DeleteByPerfil(string cmpy, string cus, string codcon, string codper)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rconpercliente WHERE rconperc_cmpy = '" + cmpy + "' AND rconperc_cus = '" + cus + "' AND rconperc_codcon = '" + codcon + "' AND  rconperc_codper = '" + codper + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rconpercliente WHERE rconperc_cmpy = '" + cmpy + "' AND rconperc_cus = '" + cus + "' AND rconperc_codcon = '" + codcon + "' AND  rconperc_codper = '" + codper + "'";

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
