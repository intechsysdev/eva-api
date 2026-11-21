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
    public interface INotasCreditosQueries
    {
        Task<List<mob_rcredapplE>> GetList(string cmpy, string reg, string inv, string num, string teri, string date);

        int Insert(mob_rcredapplE notaCredito);
    }
    public class NotasCreditosQueries : INotasCreditosQueries
    {
        private readonly ApplicationDbContext _context;
        public NotasCreditosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rcredapplE>> GetList(string cmpy, string reg, string inv, string num, string teri, string date)
        {
            List<mob_rcredapplE> lstCheques = null;
            try
            {
                lstCheques = await _context.mob_rcredappl.Where(x => x.cda_cmpy == cmpy && x.cda_reg == reg && x.cda_inv == inv && x.cda_num == num && x.cda_teri == teri && x.cda_date == date).ToListAsync();
                lstCheques.ForEach(x => x.cda_date = Convert.ToDateTime(x.cda_date).ToString("MM/dd/yyyy"));

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstCheques), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstCheques = JsonSerializer.Deserialize<List<mob_rcredapplE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstCheques;
        }

        public int Insert(mob_rcredapplE notaCredito)
        {
            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcredappl WHERE cda_cmpy = '" + notaCredito.cda_cmpy + "' AND cda_inv = '" + notaCredito.cda_inv + "' AND cda_num = '" + notaCredito.cda_num + "' AND cda_teri = '" + notaCredito.cda_teri + "' AND cda_date = '" + notaCredito.cda_date + "'";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rcredappl (cda_cmpy,cda_inv,cda_num,cda_amt,cda_cust,cda_ship,cda_reg,cda_teri,cda_marca,cda_date,cda_time) values ( @cda_cmpy, @cda_inv, @cda_num, @cda_amt, @cda_cust, @cda_ship, @cda_reg, @cda_teri, @cda_marca, @cda_date, @cda_time)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("cda_cmpy", notaCredito.cda_cmpy),
                                 new DB2Parameter("cda_inv", notaCredito.cda_inv),
                                 new DB2Parameter("cda_num", notaCredito.cda_num),
                                 new DB2Parameter("cda_amt", notaCredito.cda_amt),
                                 new DB2Parameter("cda_cust", notaCredito.cda_cust),
                                 new DB2Parameter("cda_ship", notaCredito.cda_ship),
                                 new DB2Parameter("cda_reg", notaCredito.cda_reg),
                                 new DB2Parameter("cda_teri", notaCredito.cda_teri),
                                 new DB2Parameter("cda_marca", notaCredito.cda_marca),
                                 new DB2Parameter("cda_date", notaCredito.cda_date),
                                 new DB2Parameter("cda_time", notaCredito.cda_time)

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
    }
}
