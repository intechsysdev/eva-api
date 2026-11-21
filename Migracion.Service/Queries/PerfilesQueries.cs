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
    public interface IPerfilesQueries {
        Task<List<mob_rperfilcontactoE>> GetListContacts(string cmpy);
        Task<List<mob_rperfilesE>> GetList();
    }
    public class PerfilesQueries : IPerfilesQueries
    {
        private readonly ApplicationDbContext _context;
        public PerfilesQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rperfilcontactoE>> GetListContacts(string cmpy)
        {
            List<mob_rperfilcontactoE> lstAcciones = null;
            try
            {

                lstAcciones = await _context.mob_rperfilcontacto.Where(x => x.rpcont_cmpy == cmpy).ToListAsync();
                lstAcciones.ForEach(x =>
                {
                    x.rpcont_date = Convert.ToDateTime(x.rpcont_date).ToString("MM/dd/yyyy");

                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rperfilcontactoE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

        public async Task<List<mob_rperfilesE>> GetList()
        {
            List<mob_rperfilesE> lstAcciones = null;
            try
            {

                lstAcciones = await _context.mob_rperfiles.ToListAsync();
                lstAcciones.ForEach(x =>
                {
                    x.rper_date = Convert.ToDateTime(x.rper_date).ToString("MM/dd/yyyy");
                    
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rperfilesE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }
    }
}
