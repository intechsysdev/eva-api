using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Persistence.DataBase;

namespace Migracion.Service.Queries
{
    public interface IEmpleadosQueries
    {
        Task<List<EmpleadosE>> GetList(string empleado);
    }

    public class EmpleadosQueries : IEmpleadosQueries
    {
        private readonly ApplicationDbContext _context;
        public EmpleadosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmpleadosE>> GetList(string empleado)
        {
            List<EmpleadosE> lstEmpleados = null;
            try
            {
                string sqlQuery = "SELECT UNIQUE ases_custship.ascs_code empleado,nombre,rctt_reg idregion,reg_desc region " +
                " FROM  ases_custship ases_custship LEFT JOIN empleado1 empleado1 ON ases_custship.ascs_code = empleado1.empleado  LEFT JOIN mob_rcustmain mob_rcustmain ON ases_custship.ascs_cust = mob_rcustmain.rctt_cust AND ases_custship.ascs_ship = mob_rcustmain.rctt_succli LEFT JOIN region ON mob_rcustmain.rctt_reg = region.reg_regi AND mob_rcustmain.rctt_cmpy = region.reg_cmpy " +
                  " WHERE ases_custship.ascs_code ='" + empleado + "'";
                lstEmpleados = await _context.empleado1.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstEmpleados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstEmpleados = JsonSerializer.Deserialize<List<EmpleadosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstEmpleados;
        }
    }
}
