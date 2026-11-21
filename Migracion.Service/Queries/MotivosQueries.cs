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
    public interface IMotivosQueries {
        Task<List<mob_motivosE>> GetList(string cmpy);
    }
    public class MotivosQueries : IMotivosQueries
    {
        private readonly ApplicationDbContext _context;
        public MotivosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_motivosE>> GetList(string cmpy)
        {
            List<mob_motivosE> lstMotivos = null;
            try
            {
                lstMotivos = await _context.mob_motivos.Where(x => x.cmpy == cmpy && x.mov_estadoupd == "A").ToListAsync();

                // Convertir valores a mayúsculas
                lstMotivos.ForEach(x =>
                {
                    x.cmpy = x.cmpy?.ToUpper();
                    x.movcod = x.movcod?.ToUpper();
                    x.movdesc = x.movdesc?.ToUpper();
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstMotivos), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstMotivos = JsonSerializer.Deserialize<List<mob_motivosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstMotivos;
        }
    }
}
