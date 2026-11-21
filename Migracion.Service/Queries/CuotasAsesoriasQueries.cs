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
    public interface ICuotasAsesoriasQueries
    {
        Task<List<mob_rcliproE>> GetList(string cmpy, string codusu);
    }
    public class CuotasAsesoriasQueries : ICuotasAsesoriasQueries
    {
        private readonly ApplicationDbContext _context;
        public CuotasAsesoriasQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<mob_rcliproE>> GetList(string cmpy, string codusu)
        {
            List<mob_rcliproE> lstCuota = null;
            try
            {

                lstCuota = await _context.mob_rclipro.Where(x => x.rcp_cmpy == cmpy && x.rcp_codusu == codusu).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstCuota), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstCuota = JsonSerializer.Deserialize<List<mob_rcliproE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstCuota;
        }
    }
}
