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
    public interface IBancoQueries
    {
        Task<List<mob_rbancosE>> GetList(string cmpy);
        Task<List<mob_bankacctE>> GetListConsignaciones(string cmpy);
    }
    public class BancoQueries : IBancoQueries
    {
        private readonly ApplicationDbContext _context;
        public BancoQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rbancosE>> GetList(string cmpy)
        {
            List<mob_rbancosE> lstBancos = null;
            try
            {

                lstBancos =  await _context.mob_rbancos.Where(x => x.rbn_cmpy == cmpy).ToListAsync();
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstBancos), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstBancos = JsonSerializer.Deserialize<List<mob_rbancosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstBancos;
        }

        public async Task<List<mob_bankacctE>> GetListConsignaciones(string cmpy)
        {
            List<mob_bankacctE> lstBancosConsig = null;
            try
            {

                lstBancosConsig = await _context.mob_bankacct.Where(x => x.mba_cmpy == cmpy).ToListAsync();
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstBancosConsig), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstBancosConsig = JsonSerializer.Deserialize<List<mob_bankacctE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstBancosConsig;
        }
    }
}
