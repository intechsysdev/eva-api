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
    public interface IActualizacionQueries
    {
        Task<List<mob_ractualiza>> GetList();
    }
    public class ActualizacionQueries : IActualizacionQueries
    {
        private readonly ApplicationDbContext _context;
        public ActualizacionQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_ractualiza>> GetList()
        {
            List<mob_ractualiza> lstAbonos = null;
            try
            {

                lstAbonos = await _context.mob_ractualiza.ToListAsync();
                lstAbonos.ForEach(x =>
                {
                    x.ract_fecha = Convert.ToDateTime(x.ract_fecha).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAbonos), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAbonos = JsonSerializer.Deserialize<List<mob_ractualiza>>(responseJson);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAbonos;
        }
    }
}
