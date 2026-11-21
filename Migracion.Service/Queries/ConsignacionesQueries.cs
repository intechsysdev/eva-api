using IBM.Data.Db2;
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
    public interface IConsignacionesQueries
    {
        Task<List<mob_rconsignacionE>> GetList(string cmpy, string reg, int consec, string teri);
        Task<List<mob_rconsignacionE>> GetListByFec(string cmpy, string reg, int consec, string teri, string feceva);
        int Insert(mob_rconsignacionE consignacion);
        int Update(mob_rconsignacionE consignacion);
        int Delete(string cmpy, string consec, string reg, string teri);
    }

    public class ConsignacionesQueries : IConsignacionesQueries
    {
        private readonly ApplicationDbContext _context;
        public ConsignacionesQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rconsignacionE>> GetList(string cmpy, string reg, int consec, string teri)
        {
            List<mob_rconsignacionE> lstConsignaciones = null;
            try
            {
                lstConsignaciones = await _context.mob_rconsignacion.Where(x => x.mcn_cmpy == cmpy && x.mcn_reg == reg && x.mcn_consec == consec && x.mcn_teri == teri).ToListAsync();
                lstConsignaciones.ForEach(x =>
                {
                    x.mcn_fecmvto = Convert.ToDateTime(x.mcn_fecmvto).ToString("MM/dd/yyyy");
                    x.mcn_feccon = Convert.ToDateTime(x.mcn_feccon).ToString("MM/dd/yyyy");
                    x.mcn_feceva = Convert.ToDateTime(x.mcn_feceva).ToString("MM/dd/yyyy");
                    x.mcn_date = Convert.ToDateTime(x.mcn_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstConsignaciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstConsignaciones = JsonSerializer.Deserialize<List<mob_rconsignacionE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstConsignaciones;
        }

        public async Task<List<mob_rconsignacionE>> GetListByFec(string cmpy, string reg, int consec, string teri, string feceva)
        {
            List<mob_rconsignacionE> lstConsignaciones = null;
            try
            {
                lstConsignaciones = await _context.mob_rconsignacion.Where(x => x.mcn_cmpy == cmpy && x.mcn_reg == reg && x.mcn_consec == consec && x.mcn_teri == teri && x.mcn_feceva == feceva).ToListAsync();
                lstConsignaciones.ForEach(x =>
                {
                    x.mcn_fecmvto = Convert.ToDateTime(x.mcn_fecmvto).ToString("MM/dd/yyyy");
                    x.mcn_feccon = Convert.ToDateTime(x.mcn_feccon).ToString("MM/dd/yyyy");
                    x.mcn_feceva = Convert.ToDateTime(x.mcn_feceva).ToString("MM/dd/yyyy");
                    x.mcn_date = Convert.ToDateTime(x.mcn_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstConsignaciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstConsignaciones = JsonSerializer.Deserialize<List<mob_rconsignacionE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstConsignaciones;
        }

        public int Insert(mob_rconsignacionE consignacion)
        {
            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * FROM mob_rconsignacion WHERE mcn_cmpy = '" + consignacion.mcn_cmpy + "' AND mcn_consec = '" + consignacion.mcn_consec + "' AND mcn_teri = '" + consignacion.mcn_teri + "' AND mcn_feceva = '" + consignacion.mcn_feceva + "'";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rconsignacion (mcn_cmpy,mcn_consec,mcn_reg,mcn_teri,mcn_mbanum,mcn_numcom,mcn_fecmvto,mcn_feccon,mcn_efectivo,mcn_cheque,mcn_cantcheq,mcn_feceva,mcn_horaeva,mcn_marca,mcn_date) values ( @mcn_cmpy, @mcn_consec, @mcn_reg, @mcn_teri, @mcn_mbanum, @mcn_numcom, @mcn_fecmvto, @mcn_feccon, @mcn_efectivo, @mcn_cheque, @mcn_cantcheq, @mcn_feceva, @mcn_horaeva, @mcn_marca, @mcn_date)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("mcn_cmpy", consignacion.mcn_cmpy),
                                 new DB2Parameter("mcn_consec", consignacion.mcn_consec),
                                 new DB2Parameter("mcn_reg", consignacion.mcn_reg),
                                 new DB2Parameter("mcn_teri", consignacion.mcn_teri),
                                 new DB2Parameter("mcn_mbanum", consignacion.mcn_mbanum),
                                 new DB2Parameter("mcn_numcom", consignacion.mcn_numcom),
                                 new DB2Parameter("mcn_fecmvto", consignacion.mcn_fecmvto),
                                 new DB2Parameter("mcn_feccon", consignacion.mcn_feccon),
                                 new DB2Parameter("mcn_efectivo", consignacion.mcn_efectivo),
                                 new DB2Parameter("mcn_cheque", consignacion.mcn_cheque),
                                 new DB2Parameter("mcn_cantcheq", consignacion.mcn_cantcheq),
                                 new DB2Parameter("mcn_feceva", consignacion.mcn_feceva),
                                 new DB2Parameter("mcn_horaeva", consignacion.mcn_horaeva),
                                 new DB2Parameter("mcn_marca", consignacion.mcn_marca),
                                 new DB2Parameter("mcn_date", consignacion.mcn_date)

                                 );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int Update(mob_rconsignacionE consignacion)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rconsignacion WHERE mcn_cmpy = '"+ consignacion.mcn_cmpy + "' AND mcn_reg = '"+ consignacion.mcn_reg + "' AND mcn_teri = '"+ consignacion.mcn_teri + "' AND mcn_consec = '"+ consignacion.mcn_consec + "' AND mcn_feceva = '"+ consignacion.mcn_feceva + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            //var commandText = "UPDATE mob_rconsignacion SET mcn_mbanum = @mcn_mbanum, mcn_numcom  = @mcn_numcom, mcn_fecmvto = @mcn_fecmvto, " +
                            //    "mcn_feccon = @mcn_feccon, mcn_efectivo = @mcn_efectivo, mcn_cheque = @mcn_cheque, mcn_cantcheq = @mcn_cantcheq, mcn_feceva = @mcn_feceva, " +
                            //    "mcn_horaeva = @mcn_horaeva, mcn_marca = @mcn_marca WHERE mcn_cmpy = @mcn_cmpy AND mcn_reg = @mcn_reg AND mcn_teri = @mcn_teri AND mcn_consec = @mcn_consec AND mcn_feceva = @mcn_feceva";

                            var commandText = "UPDATE mob_rconsignacion SET mcn_mbanum = @mcn_mbanum, mcn_numcom  = @mcn_numcom, mcn_fecmvto = @mcn_fecmvto, " +
    "mcn_feccon = @mcn_feccon, mcn_efectivo = @mcn_efectivo, mcn_cheque = @mcn_cheque, mcn_cantcheq = @mcn_cantcheq, mcn_feceva = @mcn_feceva, " +
    "mcn_horaeva = @mcn_horaeva WHERE mcn_cmpy = @mcn_cmpy AND mcn_reg = @mcn_reg AND mcn_teri = @mcn_teri AND mcn_consec = @mcn_consec AND mcn_feceva = @mcn_feceva";


                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("mcn_mbanum", consignacion.mcn_mbanum),
                                 new DB2Parameter("mcn_numcom", consignacion.mcn_numcom),
                                 new DB2Parameter("mcn_fecmvto", consignacion.mcn_fecmvto),
                                 new DB2Parameter("mcn_feccon", consignacion.mcn_feccon),
                                 new DB2Parameter("mcn_efectivo", consignacion.mcn_efectivo),
                                 new DB2Parameter("mcn_cheque", consignacion.mcn_cheque),
                                 new DB2Parameter("mcn_cantcheq", consignacion.mcn_cantcheq),
                                 new DB2Parameter("mcn_feceva", consignacion.mcn_feceva),
                                 new DB2Parameter("mcn_horaeva", consignacion.mcn_horaeva),
                                 new DB2Parameter("mcn_cmpy", consignacion.mcn_cmpy),
                                 new DB2Parameter("mcn_reg", consignacion.mcn_reg),
                                 new DB2Parameter("mcn_teri", consignacion.mcn_teri),
                                 new DB2Parameter("mcn_consec", consignacion.mcn_consec),
                                 new DB2Parameter("mcn_date", consignacion.mcn_feceva)//,
                                 //new DB2Parameter("mcn_marca", consignacion.mcn_marca)
                                 );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int Delete(string cmpy, string consec, string reg, string teri)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rconsignacion " +
                                          "WHERE mcn_cmpy = '" +cmpy+ "' AND mcn_consec = '" +consec+ "' AND mcn_reg = '" +reg+ "' AND mcn_teri = '" + teri + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE mob_rconsignacion " +
                                              "WHERE mcn_cmpy = '" +cmpy+ "' AND mcn_consec = '" +consec+ "' AND mcn_reg = '" + reg + "' AND mcn_teri = '" +teri+ "'";

                            result = _context.Database.ExecuteSqlRaw(commandText);
                         }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}
