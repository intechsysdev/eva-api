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
    public interface IMediosVisitasQueries {
        Task<List<mob_rmediovisitaE>> GetList(string cmpy);
    }
    public class MediosVisitasQueries : IMediosVisitasQueries
    {
        private readonly ApplicationDbContext _context;
        public MediosVisitasQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rmediovisitaE>> GetList(string cmpy)
        {
            List<mob_rmediovisitaE> lstAcciones = null;
            try
            {
                lstAcciones = await _context.mob_rmediovisita.Where(x => x.rmedio_cmpy == cmpy).ToListAsync();
                
                // Convertir valores a mayúsculas
                lstAcciones.ForEach(x =>
                {
                    x.rmedio_cmpy = x.rmedio_cmpy?.ToUpper();
                    x.rmedio_codmedvis = x.rmedio_codmedvis?.ToUpper();
                    x.rmedio_desmedvis = x.rmedio_desmedvis?.ToUpper();
                    x.rmedio_time = x.rmedio_time?.ToUpper();
                    x.rmedio_estadoupd = x.rmedio_estadoupd?.ToUpper();
                    // La app compara contra 'S' exacto: se entrega sin relleno y en
                    // mayúscula, y vacío se trata como el default de la columna.
                    x.rmedio_reqgeo = string.IsNullOrWhiteSpace(x.rmedio_reqgeo)
                        ? "N"
                        : x.rmedio_reqgeo.Trim().ToUpper();
                });
                
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rmediovisitaE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }
    }
}
