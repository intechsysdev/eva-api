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
    public interface IPresupuestosQueries {
        Task<List<mob_restzonaE>> GetListByCodUsu(string cmpy, string codUsu);
    }
    public class PresupuestosQueries : IPresupuestosQueries
    {
        private readonly ApplicationDbContext _context;
        public PresupuestosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_restzonaE>> GetListByCodUsu(string cmpy, string codUsu)
        {

            List<mob_restzonaE> lstAsociados = null;
            try
            {
                string sqlQuery = "Select DISTINCT rez_cmpy, rez_codusu, rez_id, rez_urrv, rez_ur " +
                                  "FROM  mob_restzona restzona,mob_rprodvendedor rprodvendedor " +
                                  "WHERE  restzona.rez_id=rprodvendedor.rprodven_id AND rez_cmpy='" + cmpy + "' AND rez_codusu='" + codUsu +"'";
                lstAsociados = await _context.mob_restzona.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_restzonaE>>(responseJson);



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }
    }
}
