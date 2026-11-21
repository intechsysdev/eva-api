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
    public interface IReferenciasQueries {
       Task<List<mob_rinvclassE>> GetList(string cmpy);
    }
    public class ReferenciasQueries : IReferenciasQueries
    {
        private readonly ApplicationDbContext _context;
        public ReferenciasQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rinvclassE>> GetList(string cmpy)
        {
            List<mob_rinvclassE> lstAcciones = null;
            try
            {

                lstAcciones = await _context.mob_rinvclass.Where(x => x.ric_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rinvclassE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }
    }
}
