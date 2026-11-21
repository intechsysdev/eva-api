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
    public interface INovedadesPreciosQueries
    {
        Task<List<mob_novedadpreciosE>> GetList(string cmpy);
    }
    public class NovedadesPreciosQueries : INovedadesPreciosQueries
    {
        private readonly ApplicationDbContext _context;
        public NovedadesPreciosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<mob_novedadpreciosE>> GetList(string cmpy)
        {
            List<mob_novedadpreciosE> lstNovedades = null;
            try
            {

                lstNovedades = await _context.mob_novedadprecios.Where(x => x.novp_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstNovedades), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstNovedades = JsonSerializer.Deserialize<List<mob_novedadpreciosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstNovedades;
        }
    }
}
