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
    public interface IContactImageQueries
    {
        Task<List<mob_rcontacimageE>> GetList(string rcont_cmpy, string rctt_codusu);
        int Insert(mob_rcontacimageCommand contactImage);
        int Delete(string cmpy, string codtip, string codcon);
    }
    public class ContactImageQueries : IContactImageQueries
    {
        private readonly ApplicationDbContext _context;
        public ContactImageQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<mob_rcontacimageE>> GetList(string rcont_cmpy, string rctt_codusu)
        {
            List<mob_rcontacimageE> lstAsociados = null;
            try
            {
                string sqlQuery = "Select DISTINCT rcont_cmpy, rcont_codcon, rcont_codtip " +
                "FROM mob_rcontacimage rcontacimage,mob_rcontactocliente rcontactocliente, mob_rcustrans rcustrans " +
                "WHERE  rcontacimage.rcont_codcon = rcontactocliente.rconcli_codcon AND rcontactocliente.rconcli_cus = rcustrans.rctt_cust " +
                "AND rcontactocliente.rconcli_succli = rcustrans.rctt_succli AND rcont_cmpy ='" + rcont_cmpy + "' AND rctt_codusu ='" + rctt_codusu + "'";
                lstAsociados = await _context.mob_rcontacimage.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcontacimageE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public int Insert(mob_rcontacimageCommand contactImage)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcontacimage " +
                    "WHERE rcont_cmpy = '" + contactImage.rcont_cmpy +"' AND rcont_codcon = '" + contactImage.rcont_codcon+"' AND rcont_codtip = '" + contactImage.rcont_codtip +"'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rcontacimage (rcont_cmpy,rcont_codcon,rcont_codtip,rcont_enter,rcont_date,rcont_time,rcont_estadoupd) values (@rcont_cmpy,@rcont_codcon,@rcont_codtip,@rcont_enter,@rcont_date,@rcont_time,@rcont_estadoupd)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rcont_cmpy", contactImage.rcont_cmpy),
                                 new DB2Parameter("rcont_codcon", contactImage.rcont_codcon),
                                 new DB2Parameter("rcont_codtip", contactImage.rcont_codtip),
                                 new DB2Parameter("rcont_enter", contactImage.rcont_enter),
                                 new DB2Parameter("rcont_date", contactImage.rcont_date),
                                 new DB2Parameter("rcont_time", contactImage.rcont_time),
                                 new DB2Parameter("rcont_estadoupd", contactImage.rcont_estadoupd)
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

        public int Delete(string cmpy, string codtip, string codcon)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcontacimage WHERE rcont_cmpy = '"+ cmpy + "' AND rcont_codtip = '"+ codtip + "' AND rcont_codcon = '"+ codcon + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rcontacimage WHERE rcont_cmpy = '" + cmpy + "' AND rcont_codtip = '" + codtip + "' AND rcont_codcon = '" + codcon + "'";

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
