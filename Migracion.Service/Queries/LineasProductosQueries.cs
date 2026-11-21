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
    public interface ILineasProductosQueries {
        Task<List<mob_rlineaE>> GetList(string cmpy);
        Task<List<mob_rlineaE>> GetListByCodUse(string cmpy, string codUsu);

    }
    public class LineasProductosQueries : ILineasProductosQueries
    {
        private readonly ApplicationDbContext _context;
        public LineasProductosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<mob_rlineaE>> GetList(string cmpy)
        {

            List<mob_rlineaE> lstLineas = null;
            try
            {

                lstLineas = await _context.mob_rlinea.Where(x => x.rln_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstLineas), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstLineas = JsonSerializer.Deserialize<List<mob_rlineaE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstLineas;
        }

        public async Task<List<mob_rlineaE>> GetListByCodUse(string cmpy, string codUsu)
        {

            List<mob_rlineaE> lstLineas = null;
            try
            {

                string sqlQuery = "Select DISTINCT rln_cmpy, rln_linea, rln_descr " +
                                  "FROM  mob_rlinea rlinea,mob_rproductos rproductos,mob_rprodvendedor rprodvendedor " +
                                  "WHERE rlinea.rln_linea=rproductos.rpr_linea AND rproductos.rpr_id=rprodvendedor.rprodven_id 	AND rln_cmpy='" + cmpy + "' AND rprodven_codusu='" + codUsu +"'";
                lstLineas = await _context.mob_rlinea.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstLineas), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstLineas = JsonSerializer.Deserialize<List<mob_rlineaE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstLineas;
        }

    }
}
