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
    public interface IProductosIvaQueries
    {
        Task<List<mob_rprodvendedorDto>> GetListByVendedor(string cmpy, string codUsu);

    }
    public class ProductosIvaQueries : IProductosIvaQueries
    {
        private readonly ApplicationDbContext _context;
        public ProductosIvaQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rprodvendedorDto>> GetListByVendedor(string cmpy, string codUsu)
        {
            List<mob_rprodvendedorDto> lstProductos = null;
            try
            {
                string sqlQuery = "SELECT rprodven_cmpy, rprodven_id, rprodven_codusu, rprodven_tax " +
                    "FROM mob_rprodvendedor WHERE rprodven_cmpy = '"+ cmpy + "' AND rprodven_codusu ='"+ codUsu + "'";
                lstProductos = await _context.mob_rprodvendedorDtos.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstProductos), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstProductos = JsonSerializer.Deserialize<List<mob_rprodvendedorDto>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstProductos;
        }
    }
}
