using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Models.Command;
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
    public interface IGruposCanalQueries
    {
        Task<List<grupo_canalE>> GetList(string cmpy);
        Task<List<grupo_canalE>> GetListByIdentity(string code);
    }
    public class GruposCanalQueries : IGruposCanalQueries
    {
        private readonly ApplicationDbContext _context;
        public GruposCanalQueries(
            ApplicationDbContext context)
        { 
            _context = context; 
        }

        public async Task<List<grupo_canalE>> GetList(string cmpy)
        {
            List<grupo_canalE> lstGruposCanal = null;
            try {
                lstGruposCanal = await _context.grupo_canal.Where(x => x.gc_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstGruposCanal), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstGruposCanal = JsonSerializer.Deserialize<List<grupo_canalE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstGruposCanal;
        }

        public async Task<List<grupo_canalE>> GetListByIdentity(string code)
        {
            List<grupo_canalE> lstGruposCanal = null;
            try
            {
                string sqlQuery = "SELECT UNIQUE gc_cmpy,gc_grupo,gc_nombre " +
                " FROM ases_custship ases_custship LEFT JOIN mob_rcustmain mob_rcustmain ON ases_custship.ascs_cust = mob_rcustmain.rctt_cust AND ases_custship.ascs_ship = mob_rcustmain.rctt_succli LEFT JOIN custtype custtype ON mob_rcustmain.rctt_type = custtype.cu_type AND mob_rcustmain.rctt_cmpy = custtype.cu_cmpy LEFT JOIN grupo_canal grupo_canal ON custtype.cu_comm = grupo_canal.gc_grupo AND custtype.cu_cmpy = grupo_canal.gc_cmpy " +
                  " WHERE ases_custship.ascs_code ='" + code + "'";
                lstGruposCanal = await _context.grupo_canal.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstGruposCanal), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstGruposCanal = JsonSerializer.Deserialize<List<grupo_canalE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstGruposCanal;
        }

    }
}
