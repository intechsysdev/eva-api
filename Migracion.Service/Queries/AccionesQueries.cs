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
    public interface IAccionesQueries {
        Task<List<mob_accionesE>> GetList(string cmpy);
    }
    public class AccionesQueries : IAccionesQueries
    {
        private readonly ApplicationDbContext _context;
        public AccionesQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_accionesE>> GetList(string cmpy)
        {
            List<mob_accionesE> lstAcciones = null;
            try
            {
                lstAcciones = await _context.mob_acciones.Where(x => x.cmpy == cmpy && x.acc_estadoupd =="A").ToListAsync();
                
                // Convertir valores a mayúsculas
                lstAcciones.ForEach(x =>
                {
                    x.cmpy = x.cmpy?.ToUpper();
                    x.acctip = x.acctip?.ToUpper();
                    x.accdesc = x.accdesc?.ToUpper();
                });
                
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_accionesE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }
    }
}
