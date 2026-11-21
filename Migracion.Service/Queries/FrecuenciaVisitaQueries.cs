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
    public interface IFrecuenciaVisitaQueries {
        Task<List<mob_rfrecuenciavisitaE>> GetList(string cmpy);
    }
    public class FrecuenciaVisitaQueries : IFrecuenciaVisitaQueries
    {
        private readonly ApplicationDbContext _context;
        public FrecuenciaVisitaQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rfrecuenciavisitaE>> GetList(string cmpy)
        {
            List<mob_rfrecuenciavisitaE> lstAcciones = null;
            try
            {

                lstAcciones = await _context.mob_rfrecuenciavisita.Where(x => x.rfrec_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rfrecuenciavisitaE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }
    }
}
