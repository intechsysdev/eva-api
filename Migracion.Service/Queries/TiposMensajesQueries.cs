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
    public interface ITiposMensajesQueries
    {
        Task<List<mob_tipomensajesE>> GetList(string cmpy);
    }
    public class TiposMensajesQueries : ITiposMensajesQueries
    {
        private readonly ApplicationDbContext _context;
        public TiposMensajesQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<mob_tipomensajesE>> GetList(string cmpy)
        {
            List<mob_tipomensajesE> lstTipoMensajes = null;
            try
            {
                lstTipoMensajes = await _context.mob_tipomensajes.Where(x => x.tmen_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstTipoMensajes), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstTipoMensajes = JsonSerializer.Deserialize<List<mob_tipomensajesE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstTipoMensajes;
        }
    }
}
