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
    public interface IDistrictQueries {
        Task<List<mob_districtE>> GetList(string dis_cmpy);
    }
    public class DistrictQueries : IDistrictQueries
    {
        private readonly ApplicationDbContext _context;
        public DistrictQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_districtE>> GetList(string dis_cmpy)
        {
            List<mob_districtE> lstDistrict = null;
            try
            {

                lstDistrict = await _context.mob_district.Where(x=> x.dis_cmpy == dis_cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstDistrict), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstDistrict = JsonSerializer.Deserialize<List<mob_districtE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstDistrict;
        }
    }
}
