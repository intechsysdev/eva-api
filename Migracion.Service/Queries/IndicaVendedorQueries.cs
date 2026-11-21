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
    public interface IIndicaVendedorQueries {
        Task<List<mob_indicavendedorE>> GetList(string indven_cmpy, string indven_codusu);
    }
    public class IndicaVendedorQueries : IIndicaVendedorQueries
    {
        private readonly ApplicationDbContext _context;
        public IndicaVendedorQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_indicavendedorE>> GetList(string indven_cmpy, string indven_codusu)
        {
            List<mob_indicavendedorE> lstIndicaVendedores = null;
            try
            {
                lstIndicaVendedores = await _context.mob_indicavendedor.Where(x=>x.indven_cmpy == indven_cmpy && x.indven_codusu == indven_codusu).ToListAsync();
                
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstIndicaVendedores), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstIndicaVendedores = JsonSerializer.Deserialize<List<mob_indicavendedorE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstIndicaVendedores;
        }
    }
}
