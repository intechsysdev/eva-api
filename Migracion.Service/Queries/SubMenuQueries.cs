using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Persistence.DataBase;

namespace Migracion.Service.Queries
{
    public interface ISubMenuQueries {
        Task<List<mob_rsubmenuE>> GetList(string cmpy, string codPer, string idioma);
    }
    public class SubMenuQueries : ISubMenuQueries
    {
        private readonly ApplicationDbContext _context;
        public SubMenuQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rsubmenuE>> GetList(string cmpy, string codPer, string idioma)
        {

            List<mob_rsubmenuE> lstAsociados = null;
            try
            {
                string sqlQuery = "Select DISTINCT rsubm_cmpy, rsubm_codmen, rsubm_codsubmen, rsubm_dessubmen, rsubm_cabecera, rsubm_aplicacion, rsubm_date, rsubm_time, rsubm_estadoupd, rsubm_icono_movil " +
                                  "FROM mob_rsubmenu rsubmenu, mob_rperfilmenuppal rperfilmenuppal " +
                                  "WHERE rsubmenu.rsubm_cmpy=rperfilmenuppal.rpfm_cmpy AND  rsubmenu.rsubm_codmen=rperfilmenuppal.rpfm_codmen AND rsubmenu.rsubm_codsubmen=rperfilmenuppal.rpfm_codsubmen AND rsubm_cmpy='" + cmpy + "' AND rpfm_codper='" + codPer +"' AND rsubm_idioma='" +  idioma +"'";
                lstAsociados = await _context.mob_rsubmenu.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rsubmenuE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }
    }
}
