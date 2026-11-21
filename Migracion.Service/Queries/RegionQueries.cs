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
    public interface IRegionQueries {
        Task<List<RegionE>> GetList();
    }
    public class RegionQueries : IRegionQueries
    {
        private readonly ApplicationDbContext _context;
        public RegionQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RegionE>> GetList()
        {
            List<RegionE> regiones = null;
            try
            {

                string sqlQuery = "SELECT   region.reg_cmpy,region.reg_regi,region.reg_desc " +
                "FROM region region WHERE region.reg_cmpy IN(SELECT rusu_cmpy FROM mob_rusuarios GROUP BY rusu_cmpy) AND region.reg_regi IN(SELECT rusu_reg FROM mob_rusuarios GROUP BY rusu_reg)";
                regiones = await _context.region.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(regiones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                regiones = JsonSerializer.Deserialize<List<RegionE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return regiones;
        }
    }
}
