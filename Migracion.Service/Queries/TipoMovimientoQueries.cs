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
    public interface ITipoMovimientoQueries
    {
        Task<List<mob_docvisitaE>> GetList();
    }
    public class TipoMovimientoQueries : ITipoMovimientoQueries
    {
        private readonly ApplicationDbContext _context;
        public TipoMovimientoQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<mob_docvisitaE>> GetList()
        {
            List<mob_docvisitaE> lstAbonos = null;
            try
            {
                lstAbonos = await _context.mob_docvisita.ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAbonos), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAbonos = JsonSerializer.Deserialize<List<mob_docvisitaE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAbonos;
        }
    }
}
