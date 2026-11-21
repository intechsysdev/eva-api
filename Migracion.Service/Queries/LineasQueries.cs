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
    public interface ILineasQueries
    {
        Task<List<mob_rlineaE>> GetList(string cmpy);
    }
    public class LineasQueries : ILineasQueries
    {
        private readonly ApplicationDbContext _context;
        public LineasQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rlineaE>> GetList(string cmpy)
        {
            List<mob_rlineaE> lstLineas = null;
            try
            {

                lstLineas = await _context.mob_rlinea.Where(x => x.rln_cmpy == cmpy).ToListAsync();
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstLineas), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstLineas = JsonSerializer.Deserialize<List<mob_rlineaE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstLineas;
        }
    }
}
