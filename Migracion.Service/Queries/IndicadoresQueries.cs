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
    public interface IIndicadoresQueries {
        Task<List<mob_indicadoresE>> GetList(string indcust_cmpy, string indcust_codusu);
    }
    public class IndicadoresQueries: IIndicadoresQueries
    {
        private readonly ApplicationDbContext _context;
        public IndicadoresQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_indicadoresE>> GetList(string indcust_cmpy, string indcust_codusu)
        {
            List<mob_indicadoresE> lstIndicadores = new List<mob_indicadoresE>();
            
            try
            {

                var resultIndicaCliente = await _context.mob_indicacliente.Where(x => x.indcust_cmpy == indcust_cmpy && x.indcust_codusu == indcust_codusu)
                                         .Select(s => new
                                         {
                                             s.indcust_cmpy,
                                             s.indcust_codusu,
                                             s.indcust_concepto,
                                             s.indcust_descrip
                                         }).Distinct().ToListAsync();


                foreach (var item in resultIndicaCliente)
                {
                    lstIndicadores.Add(new mob_indicadoresE
                    {
                        indcust_cmpy = item.indcust_cmpy,
                        indcust_codusu = item.indcust_codusu,
                        indcust_concepto = item.indcust_concepto,
                        indcust_descrip = item.indcust_descrip
                    });
                }


                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstIndicadores), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstIndicadores = JsonSerializer.Deserialize<List<mob_indicadoresE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstIndicadores;
        }
    }
}
