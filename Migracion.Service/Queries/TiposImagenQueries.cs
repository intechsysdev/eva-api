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
    public interface ITiposImagenQueries {
        Task<List<mob_rtipoimagenE>> GetList(string cmpy);
    }
    public class TiposImagenQueries : ITiposImagenQueries
    {
        private readonly ApplicationDbContext _context;
        public TiposImagenQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rtipoimagenE>> GetList(string cmpy)
        {
            List<mob_rtipoimagenE> lstAcciones = null;
            try
            {

                lstAcciones = await _context.mob_rtipoimagen.Where(x => x.rtipimg_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rtipoimagenE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

    }
}
