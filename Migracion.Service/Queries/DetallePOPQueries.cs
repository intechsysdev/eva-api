using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Service.Queries
{
    public interface IDetallePOPQueries
    {
        Task<List<mob_rdetallepopE>> GetList(string cmpy);
    }
    public class DetallePOPQueries : IDetallePOPQueries
    {
        private readonly ApplicationDbContext _context;
        public DetallePOPQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rdetallepopE>> GetList(string cmpy)
        {
            List<mob_rdetallepopE> lststatus = null;
            try
            {
                lststatus =await _context.mob_rdetallepop.Where(x => x.rdetallepop_cmpy == cmpy).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lststatus;
        }

    }
}
