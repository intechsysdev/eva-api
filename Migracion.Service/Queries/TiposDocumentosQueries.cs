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
    public interface ITiposDocumentosQueries {
        Task<List<mob_tipodocE>> GetList(string cmpy);
    }
    public class TiposDocumentosQueries : ITiposDocumentosQueries
    {
        private readonly ApplicationDbContext _context;
        public TiposDocumentosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_tipodocE>> GetList(string cmpy)
        {
            List<mob_tipodocE> lstAcciones = null;
            try
            {

                lstAcciones = await _context.mob_tipodoc.Where(x => x.rtipodoc_cmpy == cmpy && x.rtipodoc_estadoupd == "A").ToListAsync();

                lstAcciones.ForEach(x => { 
                    x.rtipodoc_cod = x.rtipodoc_cod.Trim();
                });
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_tipodocE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

    }
}
