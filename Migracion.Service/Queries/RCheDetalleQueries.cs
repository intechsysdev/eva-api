using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Models.Command;
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
    public interface IRCheDetalleQueries {
        Task<List<mob_rcheqdetalleE>> GetList(string cmpy, string codusu,string consec);
        Task<List<mob_rcheqdetalleE>> GetListDetailByDate(string cmpy, string codusu,  string fechaIni, string fechaFin);
        Task<List<mob_rchequeoE>> GetListChequeos(string rche_cmpy, string rche_cust, string rche_succli, string rche_consec);
        Task<List<mob_rchequeoE>> GetListByDate(string rche_cmpy, string rche_codusu, string fechaIni, string fechaFin);

        int InsertDetail(mob_rcheqdetalleCommand chedetalle);
        int Insert(mob_rchequeoCommand chequeo);

        int UpdateDetail(mob_rcheqdetalleCommand detalle, string codUsu, string cmpy);
        int Update(mob_rchequeoCommand chequeo);

        int DeleteDetailFabricante(string cmpy, string consec, string codusu, string codprofab);
        int DeleteDetail(string cmpy, string consec, string codusu);
        int Delete(string cmpy, string consec, string codusu);
    }
    public class RCheDetalleQueries : IRCheDetalleQueries
    {
        private readonly ApplicationDbContext _context;
        public RCheDetalleQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rcheqdetalleE>> GetList(string cmpy, string codusu, string consec)
        {
            List<mob_rcheqdetalleE> lstRCheque = null;
            try
            {
                string sqlQuery = "SELECT rcheqdet_cmpy,rcheqdet_consec,rcheqdet_codusu,rcheqdet_codfab,rcheqdet_codprofab,rcheqdet_preche,rcheqdet_novedad,rcheqdet_obsproche,rcheqdet_marca,rcheqdet_date,rcheqdet_time, " +
                                 "rpropre_desc,rpropre_urlimagen,rpropre_imagen, mob_rfabricante.rfab_desfab rpr_desfab " +
                                 "FROM mob_rcheqdetalle,mob_rprodprecios,mob_rfabricante " +
                                 "WHERE mob_rcheqdetalle.rcheqdet_cmpy ='" + cmpy +"' AND mob_rcheqdetalle.rcheqdet_codusu = '" + codusu +"' AND " +
                                 "mob_rcheqdetalle.rcheqdet_consec = '" + consec + "' AND " +
                                 "mob_rcheqdetalle.rcheqdet_codprofab = mob_rprodprecios.rpropre_codigo AND mob_rprodprecios.rpropre_codfab=mob_rfabricante.rfab_codfab";
                lstRCheque = await _context.mob_rcheqdetalle.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstRCheque), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstRCheque = JsonSerializer.Deserialize<List<mob_rcheqdetalleE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstRCheque;
        }

        public async Task<List<mob_rcheqdetalleE>> GetListDetailByDate(string cmpy, string codusu, string fechaIni, string fechaFin)
        {
            List<mob_rcheqdetalleE> lstRCheque = null;
            try
            {

                string sqlQuery = "SELECT rcheqdet_cmpy,rcheqdet_consec, rcheqdet_codusu, rcheqdet_codfab,rcheqdet_codprofab,rcheqdet_preche,rcheqdet_novedad,rcheqdet_obsproche,rcheqdet_marca,rcheqdet_date,rcheqdet_time,rpropre_desc,rpropre_urlimagen," +
                    "rpropre_imagen, mob_rfabricante.rfab_desfab rpr_desfab " +
                    "FROM mob_rchequeo, mob_rcheqdetalle,mob_rprodprecios,mob_rfabricante " +
                    "WHERE mob_rchequeo.rche_cmpy = '" + cmpy + "' AND mob_rchequeo.rche_codusu = '" + codusu + "' AND mob_rchequeo.rche_fecche >= '" + fechaIni + "' " +
                    "AND mob_rchequeo.rche_fecche <= '" + fechaFin + "' AND mob_rchequeo.rche_consec = mob_rcheqdetalle.rcheqdet_consec " +
                    "AND mob_rchequeo.rche_codusu = mob_rcheqdetalle.rcheqdet_codusu AND mob_rcheqdetalle.rcheqdet_codprofab = mob_rprodprecios.rpropre_codigo " +
                    "AND mob_rprodprecios.rpropre_codfab=mob_rfabricante.rfab_codfab";
                lstRCheque = await _context.mob_rcheqdetalle.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstRCheque), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstRCheque = JsonSerializer.Deserialize<List<mob_rcheqdetalleE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstRCheque;
        }


        public async Task<List<mob_rchequeoE>> GetListChequeos(string rche_cmpy,string rche_cust,
            string rche_succli,string rche_consec)
        {
            List<mob_rchequeoE> lstRCheque = null;
            try
            {

                lstRCheque = await _context.mob_rchequeo.Where(x=>x.rche_cmpy == rche_cmpy &&
                x.rche_cust == rche_cust && x.rche_succli == rche_succli && x.rche_consec == rche_consec).ToListAsync();

                lstRCheque.ForEach(x => x.rche_fecche = Convert.ToDateTime(x.rche_fecche).ToString("MM/dd/yyyy"));

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstRCheque), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstRCheque = JsonSerializer.Deserialize<List<mob_rchequeoE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstRCheque;
        }

        public async Task<List<mob_rchequeoE>> GetListByDate(string rche_cmpy, string rche_codusu,string fechaIni, string fechaFin)
        {
            List<mob_rchequeoE> lstRCheque = null;
            try
            {

                string sqlQuery = "Select * FROM mob_rchequeo " +
                  "WHERE rche_cmpy='" + rche_cmpy + "' AND rche_codusu = '" + rche_codusu + "' AND rche_fecche BETWEEN  '" + fechaIni + "' AND '" + fechaFin + "'";
                lstRCheque = await _context.mob_rchequeo.FromSqlRaw(sqlQuery).ToListAsync();

                lstRCheque.ForEach(x => x.rche_fecche = Convert.ToDateTime(x.rche_fecche).ToString("MM/dd/yyyy"));

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstRCheque), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstRCheque = JsonSerializer.Deserialize<List<mob_rchequeoE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstRCheque;
        }

        public int InsertDetail(mob_rcheqdetalleCommand chedetalle)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT rcheqdet_cmpy,rcheqdet_consec,rcheqdet_codusu,rcheqdet_codfab,rcheqdet_codprofab,rcheqdet_preche,rcheqdet_novedad,rcheqdet_obsproche,rcheqdet_marca,rcheqdet_date,rcheqdet_time " +
                                 "FROM mob_rcheqdetalle " +
                                 "WHERE mob_rcheqdetalle.rcheqdet_cmpy ='" + chedetalle.rcheqdet_cmpy + "' AND mob_rcheqdetalle.rcheqdet_codusu = '" + chedetalle.rcheqdet_codusu + "' AND " +
                                 "mob_rcheqdetalle.rcheqdet_consec = '" + chedetalle.rcheqdet_consec + "' AND " +
                                 "mob_rcheqdetalle.rcheqdet_codprofab = '" + chedetalle.rcheqdet_codprofab + "' AND mob_rcheqdetalle.rcheqdet_codfab='" + chedetalle.rcheqdet_codfab + "'";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows) {
                            var commandText = "Insert into mob_rcheqdetalle (rcheqdet_cmpy,rcheqdet_consec,rcheqdet_codusu,rcheqdet_codfab,rcheqdet_codprofab,rcheqdet_preche,rcheqdet_novedad,rcheqdet_obsproche,rcheqdet_marca,rcheqdet_date,rcheqdet_time) " +
"values (@rcheqdet_cmpy,@rcheqdet_consec,@rcheqdet_codusu,@rcheqdet_codfab,@rcheqdet_codprofab,@rcheqdet_preche,@rcheqdet_novedad,@rcheqdet_obsproche,@rcheqdet_marca,@rcheqdet_date,@rcheqdet_time)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rcheqdet_cmpy", chedetalle.rcheqdet_cmpy),
                                 new DB2Parameter("rcheqdet_consec", chedetalle.rcheqdet_consec),
                                 new DB2Parameter("rcheqdet_codusu", chedetalle.rcheqdet_codusu),
                                 new DB2Parameter("rcheqdet_codfab", chedetalle.rcheqdet_codfab),
                                 new DB2Parameter("rcheqdet_codprofab", chedetalle.rcheqdet_codprofab),
                                 new DB2Parameter("rcheqdet_preche", Convert.ToDecimal(chedetalle.rcheqdet_preche)),
                                 new DB2Parameter("rcheqdet_novedad", chedetalle.rcheqdet_novedad),
                                 new DB2Parameter("rcheqdet_obsproche", chedetalle.rcheqdet_obsproche),
                                 new DB2Parameter("rcheqdet_marca", chedetalle.rcheqdet_marca),
                                 new DB2Parameter("rcheqdet_date", chedetalle.rcheqdet_date),
                                 new DB2Parameter("rcheqdet_time", chedetalle.rcheqdet_time));
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

        public int Insert(mob_rchequeoCommand chequeo)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rchequeo " +
                    "WHERE rche_cmpy ='" + chequeo.rche_cmpy + "'  AND rche_codusu = '" + chequeo.rche_codusu +"' AND rche_consec ='"+ chequeo.rche_consec+"'";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rchequeo(rche_cmpy, rche_consec, rche_cust, rche_succli, rche_codusu, rche_custdep, rche_fecche, rche_horiniche, rche_horfinche, rche_obsche, rche_marca, rche_date, rche_time) values(@rche_cmpy, @rche_consec, @rche_cust, @rche_succli, @rche_codusu, @rche_custdep, @rche_fecche, @rche_horiniche, @rche_horfinche, @rche_obsche, @rche_marca, @rche_date, @rche_time)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rche_cmpy", chequeo.rche_cmpy),
                                 new DB2Parameter("rche_consec", chequeo.rche_consec),
                                 new DB2Parameter("rche_cust", chequeo.rche_cust),
                                 new DB2Parameter("rche_succli", chequeo.rche_succli),
                                 new DB2Parameter("rche_codusu", chequeo.rche_codusu),
                                 new DB2Parameter("rche_custdep", chequeo.rche_custdep),
                                 new DB2Parameter("rche_fecche", chequeo.rche_fecche),
                                 new DB2Parameter("rche_horiniche", chequeo.rche_horiniche),
                                 new DB2Parameter("rche_horfinche", chequeo.rche_horfinche),
                                 new DB2Parameter("rche_obsche", chequeo.rche_obsche),
                                 new DB2Parameter("rche_marca", chequeo.rche_marca),
                                 new DB2Parameter("rche_date", chequeo.rche_date),
                                 new DB2Parameter("rche_time", chequeo.rche_time)

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

        public int UpdateDetail(mob_rcheqdetalleCommand detalle, string codusu, string cmpy)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcheqdetalle WHERE rcheqdet_cmpy = '" + cmpy + "' AND rcheqdet_codusu = '" + codusu + "' AND rcheqdet_consec = '" + detalle.rcheqdet_consec + "' AND rcheqdet_codprofab = '" + detalle.rcheqdet_codprofab + "' AND rcheqdet_codfab = '" + detalle.rcheqdet_codfab + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                      if (resultCommand.HasRows)
                        {
                            var commandText = "UPDATE mob_rcheqdetalle SET rcheqdet_preche = @rcheqdet_preche, rcheqdet_novedad = @rcheqdet_novedad, rcheqdet_obsproche = @rcheqdet_obsproche, " +
                                "rcheqdet_marca = @rcheqdet_marca, rcheqdet_date = @rcheqdet_date, rcheqdet_time = @rcheqdet_time " +
                                "WHERE rcheqdet_cmpy = @rcheqdet_cmpy AND rcheqdet_codusu = @rcheqdet_codusu AND rcheqdet_consec = @rcheqdet_consec AND rcheqdet_codprofab = @rcheqdet_codprofab AND " +
                                "rcheqdet_codfab = @rcheqdet_codfab";


                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rcheqdet_preche", detalle.rcheqdet_preche),
                                 new DB2Parameter("rcheqdet_novedad", detalle.rcheqdet_novedad),
                                 new DB2Parameter("rcheqdet_obsproche", detalle.rcheqdet_obsproche),
                                 new DB2Parameter("rcheqdet_marca", detalle.rcheqdet_marca),
                                 new DB2Parameter("rcheqdet_date", detalle.rcheqdet_date),
                                 new DB2Parameter("rcheqdet_time", detalle.rcheqdet_time),
                                 new DB2Parameter("rcheqdet_cmpy", cmpy),
                                 new DB2Parameter("rcheqdet_codusu", codusu),
                                 new DB2Parameter("rcheqdet_consec", detalle.rcheqdet_consec),
                                 new DB2Parameter("rcheqdet_codprofab", detalle.rcheqdet_codprofab),
                                 new DB2Parameter("rcheqdet_codfab", detalle.rcheqdet_codfab)
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

        public int Update(mob_rchequeoCommand chequeo)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * FROM mob_rchequeo WHERE rche_cmpy = '"+ chequeo.rche_cmpy + "' AND rche_codusu = '"+ chequeo.rche_codusu + "' AND rche_consec = '"+ chequeo.rche_consec + "' AND rche_cust = '"+ chequeo.rche_cust + "' AND rche_succli = '"+ chequeo.rche_succli + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "UPDATE mob_rchequeo SET rche_obsche = @rche_obsche, rche_fecche = @rche_fecche WHERE rche_cmpy = @rche_cmpy AND rche_codusu = @rche_codusu AND " +
                                "rche_consec = @rche_consec AND rche_cust = @rche_cust AND rche_succli = @rche_succli";


                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rche_obsche", chequeo.rche_obsche),
                                 new DB2Parameter("rche_fecche", chequeo.rche_fecche),
                                 new DB2Parameter("rche_cmpy", chequeo.rche_cmpy),
                                 new DB2Parameter("rche_codusu", chequeo.rche_codusu),
                                 new DB2Parameter("rche_consec", chequeo.rche_consec),
                                 new DB2Parameter("rche_cust", chequeo.rche_cust),
                                 new DB2Parameter("rche_succli", chequeo.rche_succli)
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

        public int DeleteDetailFabricante(string cmpy, string consec, string codusu, string codprofab)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcheqdetalle WHERE rcheqdet_cmpy = '"+ cmpy + "' AND rcheqdet_consec = '"+ consec + "' AND rcheqdet_codusu = '"+ codusu + "' AND rcheqdet_codprofab = '"+ codprofab + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rcheqdetalle WHERE rcheqdet_cmpy = '" + cmpy + "' AND rcheqdet_consec = '" + consec + "' AND rcheqdet_codusu = '" + codusu + "' AND rcheqdet_codprofab = '" + codprofab + "'";

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

        public int DeleteDetail(string cmpy, string consec, string codusu)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcheqdetalle WHERE rcheqdet_cmpy = '"+ cmpy + "' AND rcheqdet_consec = '"+ consec + "' AND rcheqdet_codusu = '"+ codusu + "'";
                        

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rcheqdetalle WHERE rcheqdet_cmpy = '" + cmpy + "' AND rcheqdet_consec = '" + consec + "' AND rcheqdet_codusu = '" + codusu + "'";

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

        public int Delete(string cmpy, string consec, string codusu)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rchequeo WHERE rche_cmpy = '"+ cmpy + "' AND rche_consec = '"+ consec + "' AND rche_codusu = '"+ codusu + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rchequeo WHERE rche_cmpy = '" + cmpy + "' AND rche_consec = '" + consec + "' AND rche_codusu = '" + codusu + "'";

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
