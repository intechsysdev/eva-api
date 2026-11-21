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
    public interface ICanalesQueries {
        Task<List<mob_rcusttypeE>> GetList(string cmpy);
    }
    public class CanalesQueries : ICanalesQueries
    {
        private readonly ApplicationDbContext _context;
        public CanalesQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rcusttypeE>> GetList(string cmpy)
        {
            List<mob_rcusttypeE> lstAcciones = null;
            try
            {

                lstAcciones = await _context.mob_rcusttype.Where(x => x.cu_cmpy == cmpy).ToListAsync();
                lstAcciones.ForEach(x => {
                    x.cu_type = x.cu_type.Trim();
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rcusttypeE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }
    }
}
