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
    public interface IMercadosQueries
    {
        Task<List<mob_mercadoE>> GetList(string cmpy);
        Task<List<mob_submercadoE>> GetListSubmercados(string cmpy);

    }
    public class MercadosQueries : IMercadosQueries
    {
        private readonly ApplicationDbContext _context;
        public MercadosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<mob_mercadoE>> GetList(string cmpy)
        {
            List<mob_mercadoE> lstMercados = null;
            try
            {

                lstMercados = await _context.mob_mercado.Where(x => x.mer_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstMercados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstMercados = JsonSerializer.Deserialize<List<mob_mercadoE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstMercados;
        }

        public async Task<List<mob_submercadoE>> GetListSubmercados(string cmpy)
        {
            List<mob_submercadoE> lstSubmercados = null;
            try
            {

                lstSubmercados = await _context.mob_submercado.Where(x => x.sbm_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstSubmercados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstSubmercados = JsonSerializer.Deserialize<List<mob_submercadoE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstSubmercados;
        }
    }
}
