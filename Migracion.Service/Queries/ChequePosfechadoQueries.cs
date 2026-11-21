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

    public interface IChequePosfechadoQueries
    {
        Task<List<mob_rrcheckposE>> GetList(string cmpy, string reg, string teri, string num, string entdt);
        int Insert(mob_rrcheckposE cheque);
    }

    public class ChequePosfechadoQueries : IChequePosfechadoQueries
    {
        private readonly ApplicationDbContext _context;
        public ChequePosfechadoQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rrcheckposE>> GetList(string cmpy, string reg, string teri, string num, string entdt)
        {
            List<mob_rrcheckposE> lstCheques = null;
            try
            {
                lstCheques = await _context.mob_rrcheckpos.Where(x => x.cph_cmpy == cmpy && x.cph_reg == reg && x.cph_teri == teri && x.cph_num == num && x.cph_entdt == entdt).ToListAsync();
                lstCheques.ForEach(x =>
                {
                    x.cph_entdt = Convert.ToDateTime(x.cph_entdt).ToString("MM/dd/yyyy");
                    x.cph_date = Convert.ToDateTime(x.cph_date).ToString("MM/dd/yyyy");
                    x.cph_consigna = Convert.ToDateTime(x.cph_consigna).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstCheques), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstCheques = JsonSerializer.Deserialize<List<mob_rrcheckposE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstCheques;
        }

        public int Insert(mob_rrcheckposE cheque)
        {
            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * FROM mob_rrcheckpos WHERE cph_cmpy = '" + cheque.cph_cmpy + "' AND cph_teri = '" + cheque.cph_teri + "' AND cph_num = '" + cheque.cph_num + "' AND cph_entdt = '" + cheque.cph_entdt + "'";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rrcheckpos (cph_cmpy,cph_num,cph_tele,cph_cust,cph_ship,cph_entdt,cph_amt,cph_card,cph_crdt,cph_com,cph_consigna,cph_reg,cph_teri,cph_marca,cph_date,cph_time) values ( @cph_cmpy, @cph_num, @cph_tele, @cph_cust, @cph_ship, @cph_entdt, @cph_amt, @cph_card, @cph_crdt, @cph_com, @cph_consigna, @cph_reg, @cph_teri, @cph_marca, @cph_date, @cph_time)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("cph_cmpy", cheque.cph_cmpy),
                                 new DB2Parameter("cph_num", cheque.cph_num),
                                 new DB2Parameter("cph_tele", cheque.cph_tele),
                                 new DB2Parameter("cph_cust", cheque.cph_cust),
                                 new DB2Parameter("cph_ship", cheque.cph_ship),
                                 new DB2Parameter("cph_entdt", cheque.cph_entdt),
                                 new DB2Parameter("cph_amt", cheque.cph_amt),
                                 new DB2Parameter("cph_card", cheque.cph_card),
                                 new DB2Parameter("cph_crdt", cheque.cph_crdt),
                                 new DB2Parameter("cph_com", cheque.cph_com),
                                 new DB2Parameter("cph_consigna", cheque.cph_consigna),
                                 new DB2Parameter("cph_reg", cheque.cph_reg),
                                 new DB2Parameter("cph_teri", cheque.cph_teri),
                                 new DB2Parameter("cph_marca", cheque.cph_marca),
                                 new DB2Parameter("cph_date", cheque.cph_date),
                                 new DB2Parameter("cph_time", cheque.cph_time)

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
