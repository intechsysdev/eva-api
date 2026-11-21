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
    public interface ISubLineasQueries
    {
        Task<List<mob_rsublineasE>> GetList(string cmpy);
        Task<List<mob_rsublineasE>> GetListByLinea(string cmpy, string linea);
    }
    public class SubLineasQueries : ISubLineasQueries
    {
        private readonly ApplicationDbContext _context;
        public SubLineasQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rsublineasE>> GetList(string cmpy)
        {
            List<mob_rsublineasE> lstSubLineas = null;
            try
            {

                lstSubLineas = await _context.mob_rsublineas.Where(x => x.rsb_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstSubLineas), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstSubLineas = JsonSerializer.Deserialize<List<mob_rsublineasE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstSubLineas;
        }

        public async Task<List<mob_rsublineasE>> GetListByLinea(string cmpy, string linea)
        {
            List<mob_rsublineasE> lstSubLineas = null;
            try
            {

                lstSubLineas = await _context.mob_rsublineas.Where(x => x.rsb_cmpy == cmpy && x.rsb_linea == linea).ToListAsync();
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstSubLineas), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstSubLineas = JsonSerializer.Deserialize<List<mob_rsublineasE>>(responseJson);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstSubLineas;
        }
    }
}
