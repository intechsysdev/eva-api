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

    public interface IMenuPpalQueries
    {
        Task<List<mob_rmenuppalE>> GetList(string cmpy, string codper, string idioma);
    }
    public class MenuPpalQueries : IMenuPpalQueries
    {
        private readonly ApplicationDbContext _context;
        public MenuPpalQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rmenuppalE>> GetList(string cmpy, string codper, string idioma)
        {

            List<mob_rmenuppalE> lstAsociados = null;
            try
            {
                string sqlQuery = $"Select DISTINCT rmen_cmpy, rmen_codmen, rmen_desmen, rmen_cabecera, rmen_aplicacion, rmen_date, rmen_time, rmen_estadoupd " +
                                  "FROM mob_rmenuppal rmenuppal,mob_rperfilmenuppal rperfilmenuppal " +
                                  "WHERE rmenuppal.rmen_cmpy=rperfilmenuppal.rpfm_cmpy and  rmenuppal.rmen_codmen=rperfilmenuppal.rpfm_codmen " +
                                  "AND rmen_cmpy= '" + cmpy + "' AND rpfm_codper='" + codper + "' AND rmen_idioma='" + idioma + "'";
                lstAsociados = await _context.mob_rmenuppal.FromSqlRaw(sqlQuery).ToListAsync();
                lstAsociados.ForEach(x => x.rmen_date = Convert.ToDateTime(x.rmen_date).ToString("MM/dd/yyyy"));

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rmenuppalE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }


    }
}
