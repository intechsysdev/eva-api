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
    public interface IAbonosQueries
    {
        Task<List<mob_rabonosE>> GetList(string cmpy, string teri,string reg);
        Task<List<mob_rexcepcionabonosE>> GetExceptionList(string cmpy, string codUsu);
    }
    public class AbonosQueries : IAbonosQueries
    {
        private readonly ApplicationDbContext _context;
        public AbonosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<mob_rabonosE>> GetList(string cmpy, string teri, string reg)
        {
            List<mob_rabonosE> lstAbonos = null;
            try
            {
                lstAbonos = await _context.mob_rabonos.Where(x => x.mabo_cmpy == cmpy && x.mabo_teri == teri && x.mabo_reg == reg).ToListAsync();
                lstAbonos.ForEach(x => x.mabo_fecrc = Convert.ToDateTime(x.mabo_fecrc).ToString("MM/dd/yyyy"));

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAbonos), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAbonos = JsonSerializer.Deserialize<List<mob_rabonosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAbonos;
        }

        public async Task<List<mob_rexcepcionabonosE>> GetExceptionList(string cmpy, string codUsu)
        {
            List<mob_rexcepcionabonosE> lstAbonos = null;
            try
            {
                lstAbonos = await _context.mob_rexcepcionabonos.Where(x => x.exa_cmpy == cmpy && x.exa_codusu == codUsu).ToListAsync();
                

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAbonos), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAbonos = JsonSerializer.Deserialize<List<mob_rexcepcionabonosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAbonos;
        }
    }
}
