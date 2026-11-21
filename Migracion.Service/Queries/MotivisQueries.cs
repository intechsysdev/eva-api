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
    public interface IMotivisQueries
    {
        Task<List<mob_motivisE>> GetList(string cmpy);
    }
    public class MotivisQueries : IMotivisQueries
    {
        private readonly ApplicationDbContext _context;
        public MotivisQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_motivisE>> GetList(string cmpy)
        {
            List<mob_motivisE> lstMotivis = null;
            try
            {
                lstMotivis = await _context.mob_motivis.Where(x => x.cmpy == cmpy && x.mot_estadoupd == "A").ToListAsync();

                // Convertir valores a mayúsculas
                lstMotivis.ForEach(x =>
                {
                    x.cmpy = x.cmpy?.ToUpper();
                    x.motcod = x.motcod?.ToUpper();
                    x.motdes = x.motdes?.ToUpper();
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstMotivis), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstMotivis = JsonSerializer.Deserialize<List<mob_motivisE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstMotivis;
        }

    }
}
