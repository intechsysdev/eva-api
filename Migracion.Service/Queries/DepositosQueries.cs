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
    public interface IDepositosQueries {
        Task<List<mob_depositosE>> GetList(string cm_cmpy);
    }
    public class DepositosQueries : IDepositosQueries
    {
        private readonly ApplicationDbContext _context;
        public DepositosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_depositosE>> GetList(string cm_cmpy)
        {
            List<mob_depositosE> lstDepositos = null;
            try
            {

                lstDepositos = await _context.mob_depositos.Where(x=> x.cm_cmpy == cm_cmpy).ToListAsync();
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstDepositos), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstDepositos = JsonSerializer.Deserialize<List<mob_depositosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstDepositos;
        }
    }
}
