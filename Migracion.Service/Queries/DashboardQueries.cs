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
    public interface IDashboardQueries
    {
        Task<List<dash_E>> GetList(string cmpy, string codusu);
    }


    public class DashboardQueries : IDashboardQueries
    {
        private readonly ApplicationDbContext _context;
        public DashboardQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<dash_E>> GetList(string cmpy, string codusu)
        {

            List<dash_E> lstIndicadores = null;
            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    // Calcular los últimos 24 meses incluyendo el mes actual
                    DateTime now = DateTime.Now;
                    DateTime endDate = now; // Mes actual
                    DateTime startDate = now.AddMonths(-23); // 23 meses atrás desde el actual (total 24 meses)

                    int endYear = endDate.Year;
                    int endMonth = endDate.Month;
                    int startYear = startDate.Year;
                    int startMonth = startDate.Month;

                    // Crear valores combinados año*100+mes para comparación
                    int startPeriod = startYear * 100 + startMonth;
                    int endPeriod = endYear * 100 + endMonth;

                    lstIndicadores = await _context.dash.FromSqlInterpolated($@"
            SELECT 
                anio,
                mes,
                territorio,
              codigo,
              cuota,
               real,
               cumplimiento
            FROM
              view_mob_dashboard
            WHERE
               rc_cmpy = {cmpy}
               AND (anio * 100 + mes) BETWEEN {startPeriod} AND {endPeriod}
               AND codigo = {codusu}
            ORDER BY
               anio DESC, mes DESC
            LIMIT 24").ToListAsync();

                    var responseJson = Regex.Replace(
                        Regex.Replace(
                            JsonSerializer.Serialize(lstIndicadores),
                            Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN),
                        Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS);

                    lstIndicadores = JsonSerializer.Deserialize<List<dash_E>>(responseJson);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                return lstIndicadores;
            }
        }
    }

}
