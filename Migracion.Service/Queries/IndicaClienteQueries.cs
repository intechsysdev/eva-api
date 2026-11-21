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
    public interface IIndicaClienteQueries {
        Task<List<mob_indicaclienteE>> GetList(string indcust_cmpy, string indcust_codusu, string indcust_concepto);

    }
    public class IndicaClienteQueries: IIndicaClienteQueries
    {
        private readonly ApplicationDbContext _context;
        public IndicaClienteQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_indicaclienteE>> GetList(string indcust_cmpy, string indcust_codusu, string indcust_concepto)
        {
            List<mob_indicaclienteE> lstIndicaClientes = null;
            try
            {

                lstIndicaClientes = await _context.mob_indicacliente.Where(x=> x.indcust_cmpy == indcust_cmpy && x.indcust_codusu == indcust_codusu
                && x.indcust_concepto == indcust_concepto).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstIndicaClientes), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstIndicaClientes = JsonSerializer.Deserialize<List<mob_indicaclienteE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstIndicaClientes;
        }
    }
}
