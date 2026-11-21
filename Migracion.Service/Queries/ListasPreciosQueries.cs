using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Models.Dtos;
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
    public interface IListasPreciosQueries {
        Task<List<mob_ritempricDto>> GetListByCodUsu(string cmpy, string codUsu);
        Task<List<mob_ritempricE>> GetListProducts(string cmpy, string lista, string codUsu);
    }
    public class ListasPreciosQueries : IListasPreciosQueries
    {
        private readonly ApplicationDbContext _context;
        public ListasPreciosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_ritempricDto>> GetListByCodUsu(string cmpy, string codUsu)
        {

            List<mob_ritempricDto> lstAsociados = null;
            try
            {
                string sqlQuery = "Select DISTINCT rpri_cmpy, rpri_lista " +
                                  "FROM mob_rprodvendedor rprodvendedor, mob_ritempric ritempric " +
                                  "WHERE rprodvendedor.rprodven_id=ritempric.rpri_id AND rpri_cmpy='" + cmpy + "' AND rprodven_codusu='" + codUsu + "'";
                lstAsociados = await _context.mob_ritempricDtos.FromSqlRaw(sqlQuery).ToListAsync();
                lstAsociados.ForEach(x =>
                {
                    x.rpri_lista = x.rpri_lista.Trim();
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_ritempricDto>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_ritempricE>> GetListProducts(string cmpy, string lista, string codUsu)
        {

            List<mob_ritempricE> lstAsociados = null;
            try
            {
                string sqlQuery = "Select rpri_cmpy, rpri_id, rpri_lista, rpri_price " +
                                  "FROM mob_rprodvendedor rprodvendedor, mob_ritempric ritempric  " +
                                  "WHERE  rprodvendedor.rprodven_id=ritempric.rpri_id AND rpri_cmpy='" + cmpy + "' AND rpri_lista='" + lista +"' AND rprodven_codusu='" + codUsu +"'";
                lstAsociados = await _context.mob_ritempric.FromSqlRaw(sqlQuery).ToListAsync();

                lstAsociados.ForEach(x =>
                {
                    x.rpri_lista = x.rpri_lista.Trim();
                });


                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_ritempricE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }
    }
}
