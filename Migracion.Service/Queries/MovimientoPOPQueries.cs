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
    public interface IMovimientoPOPQueries
    {
        Task<List<mob_rpope>> GetList(string cmpy);
    }
    public class MovimientoPOPQueries : IMovimientoPOPQueries
    {
        private readonly ApplicationDbContext _context;
        public MovimientoPOPQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rpope>> GetList(string cmpy)
        {
            List<mob_rpope> lststatus = null;
            try
            {
                lststatus = await _context.mob_rpop.Where(x => x.rpop_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lststatus), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lststatus = JsonSerializer.Deserialize<List<mob_rpope>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lststatus;
        }
    }
}
