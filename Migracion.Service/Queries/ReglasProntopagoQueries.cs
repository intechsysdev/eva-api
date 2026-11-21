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
    public interface IReglasProntopagoQueries
    {
        Task<List<mob_reglasprontopagoE>> GetList(string cmpy);

    }
    public class ReglasProntopagoQueries : IReglasProntopagoQueries
    {
        private readonly ApplicationDbContext _context;
        public ReglasProntopagoQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_reglasprontopagoE>> GetList(string cmpy)
        {
            List<mob_reglasprontopagoE> lstReglas = null;
            try
            {

                lstReglas = await _context.mob_reglasprontopago.Where(x => x.mrp_cmpy == cmpy).ToListAsync();
                lstReglas.ForEach(x => {
                    x.mrp_type = x.mrp_type.Trim();
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstReglas), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstReglas = JsonSerializer.Deserialize<List<mob_reglasprontopagoE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstReglas;
        }
    }
}
