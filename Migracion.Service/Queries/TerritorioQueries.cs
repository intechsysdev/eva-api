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
    public interface ITerritorioQueries
    {
        Task<List<TerritorioE>> GetList();
    }
    public class TerritorioQueries : ITerritorioQueries
    {
        private readonly ApplicationDbContext _context;
        public TerritorioQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TerritorioE>> GetList()
        {
            List<TerritorioE> regiones = null;
            try
            {

                string sqlQuery = "SELECT   territor.ter_cmpy,territor.ter_teri,territor.ter_desc " +
                "FROM territor territor WHERE territor.ter_cmpy IN(SELECT rusu_cmpy FROM mob_rusuarios GROUP BY rusu_cmpy) AND territor.ter_teri IN(SELECT rusu_teri FROM mob_rusuarios GROUP BY rusu_teri)";
                regiones = await _context.territorio.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(regiones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                regiones = JsonSerializer.Deserialize<List<TerritorioE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return regiones;
        }

    }
}
