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
    public interface IMarcasQueries
    {
        Task<List<mob_marcaE>> GetList(string cmpy);
    }
    public class MarcasQueries : IMarcasQueries
    {
        private readonly ApplicationDbContext _context;
        public MarcasQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<mob_marcaE>> GetList(string cmpy)
        {
            List<mob_marcaE> lstMarcas = null;
            try
            {

                lstMarcas = await _context.mob_marca.Where(x => x.mar_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstMarcas), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstMarcas = JsonSerializer.Deserialize<List<mob_marcaE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstMarcas;
        }
    }
}
