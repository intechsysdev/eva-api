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
    public interface ITransferencistasQueries
    {
        Task<List<mob_rcustransDto>> GetListByCodUsu(string cmpy, string codUsu);
        Task<List<mob_rcustransDto>> GetListByRuta(string cmpy, string codUsu, string codRuta);
        Task<List<mob_rcustransDto>> GetListByTipoDoc(string cmpy, string cust, string succli, string tipoDoc);

        int Insert(mob_rcustransCommand client);

        int Update(mob_rcustransCommand client);

        int Delete(string cmpy, string cust, string succli, string codusu);
    }
    public class TransferencistasQueries : ITransferencistasQueries
    {
        private readonly ApplicationDbContext _context;
        public TransferencistasQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rcustransDto>> GetListByCodUsu(string cmpy, string codUsu)
        {

            List<mob_rcustransDto> lstAsociados = null;
            try
            {
                string sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_enter, rctt_entdt, rctt_time, rctt_estadoupd " +
                                  "FROM mob_rcustrans " +
                                  "WHERE rctt_cmpy='" + cmpy + "' AND rctt_codusu='" + codUsu + "'";
                lstAsociados = await _context.mob_rcustransDtos.FromSqlRaw(sqlQuery).ToListAsync();

                lstAsociados.ForEach(x =>
                {
                    x.rctt_date = Convert.ToDateTime(x.rctt_date).ToString("MM/dd/yyyy");
                    x.rctt_feccum = Convert.ToDateTime(x.rctt_feccum).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcustransDto>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rcustransDto>> GetListByRuta(string cmpy, string codUsu, string codRuta)
        {

            List<mob_rcustransDto> lstAsociados = null;
            try
            {
                string sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_enter, rctt_entdt, rctt_time, rctt_estadoupd " +
                                  "FROM mob_rcustrans " +
                                  "WHERE rctt_cmpy='" + cmpy + "' AND rctt_codusu='" + codUsu + "' AND rctt_ruta='" + codRuta + "'";
                lstAsociados = await _context.mob_rcustransDtos.FromSqlRaw(sqlQuery).ToListAsync(); 
                lstAsociados.ForEach(x =>
                {
                    x.rctt_date = Convert.ToDateTime(x.rctt_date).ToString("MM/dd/yyyy");
                    x.rctt_feccum = Convert.ToDateTime(x.rctt_feccum).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcustransDto>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rcustransDto>> GetListByTipoDoc(string cmpy, string cust, string succli, string tipoDoc)
        {

            List<mob_rcustransDto> lstAsociados = null;
            try
            {
                string sqlQuery = "Select rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc,rctt_type, rctt_zip, rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg,rctt_diavisita, rctt_imagen, rctt_urlimagen, rctt_enter, rctt_entdt, rctt_time, rctt_estadoupd " +
                                  "FROM mob_rcustrans " +
                                  "WHERE rctt_cmpy='" + cmpy + "' AND rctt_cust='" + cust + "' AND rctt_succli='" + succli + "' AND rctt_tipdoc='" + tipoDoc + "'";
                lstAsociados = await _context.mob_rcustransDtos.FromSqlRaw(sqlQuery).ToListAsync();
                lstAsociados.ForEach(x =>
                {
                    x.rctt_date = Convert.ToDateTime(x.rctt_date).ToString("MM/dd/yyyy");
                    x.rctt_feccum = Convert.ToDateTime(x.rctt_feccum).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcustransDto>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public int Insert(mob_rcustransCommand client)
        {

            int result = 0;
            try
            {

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    //command.CommandText = "Select * FROM mob_rcustrans " +
                    //                      "WHERE rctt_cust = '" + client.rctt_cust + "' AND rctt_tipdoc = '" + client.rctt_tipdoc + "' AND rctt_succli = '" + client.rctt_succli + "'";

                    command.CommandText = "Select * FROM mob_rcustrans " +
                                          "WHERE rctt_cust = '" + client.rctt_cust + "' AND rctt_succli = '" + client.rctt_succli + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rcustrans (rctt_cmpy,rctt_reg,rctt_cust,rctt_tipdoc,rctt_resal,rctt_succli,rctt_razsoc,rctt_nomcli,rctt_addr,rctt_tele,rctt_celular,rctt_fax,rctt_dist,rctt_sic,rctt_codusu,rctt_ruta,rctt_sec,rctt_clinvo,rctt_date,rctt_estrato,rctt_latitud,rctt_longitud,rctt_frevisita,rctt_horsugvis,rctt_codabc,rctt_type,rctt_zip,rctt_zip4,rctt_vot,rctt_limit,rctt_level,rctt_bal,rctt_cont,rctt_feccum,rctt_pedmini,rctt_diasent,rctt_tax,rctt_nomcan,rctt_schrg,rctt_diavisita,rctt_enter,rctt_entdt,rctt_time,rctt_estadoupd,rctt_urlimagen) values (@rctt_cmpy,@rctt_reg,@rctt_cust,@rctt_tipdoc,@rctt_resal,@rctt_succli,@rctt_razsoc,@rctt_nomcli,@rctt_addr,@rctt_tele,@rctt_celular,@rctt_fax,@rctt_dist,@rctt_sic,@rctt_codusu,@rctt_ruta,@rctt_sec,@rctt_clinvo,@rctt_date,@rctt_estrato,@rctt_latitud,@rctt_longitud,@rctt_frevisita,@rctt_horsugvis,@rctt_codabc,@rctt_type,@rctt_zip,@rctt_zip4,@rctt_vot,@rctt_limit,@rctt_level,@rctt_bal,@rctt_cont,@rctt_feccum,@rctt_pedmini,@rctt_diasent,@rctt_tax,@rctt_nomcan,@rctt_schrg,@rctt_diavisita,@rctt_enter,@rctt_entdt,@rctt_time,@rctt_estadoupd,@rctt_urlimagen)";
                            result = _context.Database.ExecuteSqlRaw(commandText,
                                new DB2Parameter("rctt_cmpy", client.rctt_cmpy),
                                new DB2Parameter("rctt_reg", client.rctt_reg),
                                new DB2Parameter("rctt_cust", client.rctt_cust),
                                new DB2Parameter("rctt_tipdoc", client.rctt_tipdoc),
                                new DB2Parameter("rctt_resal", client.rctt_resal),
                                new DB2Parameter("rctt_succli", client.rctt_succli),
                                new DB2Parameter("rctt_razsoc", client.rctt_razsoc),
                                new DB2Parameter("rctt_nomcli", client.rctt_nomcli),
                                new DB2Parameter("rctt_addr", client.rctt_addr),
                                new DB2Parameter("rctt_tele", client.rctt_tele),
                                new DB2Parameter("rctt_celular", client.rctt_celular),
                                new DB2Parameter("rctt_fax", client.rctt_fax),
                                new DB2Parameter("rctt_dist", client.rctt_dist),
                                new DB2Parameter("rctt_sic", client.rctt_sic),
                                new DB2Parameter("rctt_codusu", client.rctt_codusu),
                                new DB2Parameter("rctt_ruta", client.rctt_ruta),
                                new DB2Parameter("rctt_sec", client.rctt_sec),
                                new DB2Parameter("rctt_clinvo", client.rctt_clinvo),
                                new DB2Parameter("rctt_date", client.rctt_date),
                                new DB2Parameter("rctt_estrato", client.rctt_estrato),
                                new DB2Parameter("rctt_latitud", client.rctt_latitud),
                                new DB2Parameter("rctt_longitud", client.rctt_longitud),
                                new DB2Parameter("rctt_frevisita", client.rctt_frevisita),
                                new DB2Parameter("rctt_horsugvis", client.rctt_horsugvis),
                                new DB2Parameter("rctt_codabc", client.rctt_codabc),
                                new DB2Parameter("rctt_type", client.rctt_type),
                                new DB2Parameter("rctt_zip", client.rctt_zip),
                                new DB2Parameter("rctt_zip4", client.rctt_zip4),
                                new DB2Parameter("rctt_vot", client.rctt_vot),
                                new DB2Parameter("rctt_limit", client.rctt_limit),
                                new DB2Parameter("rctt_level", client.rctt_level),
                                new DB2Parameter("rctt_bal", client.rctt_bal),
                                new DB2Parameter("rctt_cont", client.rctt_cont),
                                new DB2Parameter("rctt_feccum", client.rctt_feccum),
                                new DB2Parameter("rctt_pedmini", client.rctt_pedmini),
                                new DB2Parameter("rctt_diasent", client.rctt_diasent),
                                new DB2Parameter("rctt_tax", client.rctt_tax),
                                new DB2Parameter("rctt_nomcan", client.rctt_nomcan),
                                new DB2Parameter("rctt_schrg", client.rctt_schrg),
                                new DB2Parameter("rctt_diavisita", client.rctt_diavisita),
                                new DB2Parameter("rctt_enter", client.rctt_enter),
                                new DB2Parameter("rctt_entdt", client.rctt_entdt),
                                new DB2Parameter("rctt_time", client.rctt_time),
                                new DB2Parameter("rctt_estadoupd", client.rctt_estadoupd),
                                new DB2Parameter("rctt_urlimagen", client.rctt_urlimagen)
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

        public int Update(mob_rcustransCommand client)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcustrans WHERE rctt_cmpy = '" + client.rctt_cmpy + "' AND rctt_cust = '" + client.rctt_cust + "' AND rctt_succli = '" + client.rctt_succli + "'";

                    _context.Database.OpenConnection();
                     using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "UPDATE mob_rcustrans SET rctt_razsoc = @rctt_razsoc, rctt_nomcli = @rctt_nomcli, rctt_codabc = @rctt_codabc, rctt_type = @rctt_type, " +
                                "rctt_vot = @rctt_vot, rctt_dist = @rctt_dist, rctt_addr = @rctt_addr, rctt_zip = @rctt_zip, rctt_tele = @rctt_tele, rctt_celular = @rctt_celular, " +
                                "rctt_fax = @rctt_fax, rctt_estrato = @rctt_estrato, rctt_latitud = @rctt_latitud, rctt_longitud = @rctt_longitud, rctt_ruta = @rctt_ruta, rctt_sec = @rctt_sec, " +
                                "rctt_frevisita = @rctt_frevisita, rctt_diavisita = @rctt_diavisita, rctt_horsugvis = @rctt_horsugvis, rctt_date = @rctt_date, rctt_time = @rctt_time, " +
                                "rctt_estadoupd= @rctt_estadoupd, rctt_urlimagen = @rctt_urlimagen WHERE rctt_cmpy = @rctt_cmpy AND rctt_cust = @rctt_cust AND rctt_succli = @rctt_succli";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rctt_razsoc", client.rctt_razsoc),
                                 new DB2Parameter("rctt_nomcli", client.rctt_nomcli),
                                 new DB2Parameter("rctt_codabc", client.rctt_codabc),
                                 new DB2Parameter("rctt_type", client.rctt_type),
                                 new DB2Parameter("rctt_vot", client.rctt_vot),
                                 new DB2Parameter("rctt_dist", client.rctt_dist),
                                 new DB2Parameter("rctt_addr", client.rctt_addr),
                                 new DB2Parameter("rctt_zip", client.rctt_zip),
                                 new DB2Parameter("rctt_tele", client.rctt_tele),
                                 new DB2Parameter("rctt_celular", client.rctt_celular),
                                 new DB2Parameter("rctt_fax", client.rctt_fax),
                                 new DB2Parameter("rctt_estrato", client.rctt_estrato),
                                 new DB2Parameter("rctt_latitud", client.rctt_latitud),
                                 new DB2Parameter("rctt_longitud", client.rctt_longitud),
                                 new DB2Parameter("rctt_ruta", client.rctt_ruta),
                                 new DB2Parameter("rctt_sec", client.rctt_sec),
                                 new DB2Parameter("rctt_frevisita", client.rctt_frevisita),
                                 new DB2Parameter("rctt_diavisita", client.rctt_diavisita),
                                 new DB2Parameter("rctt_horsugvis", client.rctt_horsugvis),
                                 new DB2Parameter("rctt_date", client.rctt_date),
                                 new DB2Parameter("rctt_time", client.rctt_time),
                                 new DB2Parameter("rctt_estadoupd", client.rctt_estadoupd),
                                 new DB2Parameter("rctt_urlimagen", client.rctt_urlimagen),
                                 new DB2Parameter("rctt_cmpy", client.rctt_cmpy),
                                 new DB2Parameter("rctt_cust", client.rctt_cust),
                                 new DB2Parameter("rctt_succli", client.rctt_succli)
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

        public int Delete(string cmpy, string cust, string succli, string codusu)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                     command.CommandText = "SELECT * FROM mob_rcustrans WHERE rctt_cmpy = '"+ cmpy + "' AND rctt_cust = '"+ cust + "' AND rctt_succli = '"+ succli + "' AND rctt_codusu = '"+ codusu + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                     {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rcustrans WHERE rctt_cmpy = '" + cmpy + "' AND rctt_cust = '" + cust + "' AND rctt_succli = '" + succli + "' AND rctt_codusu = '" + codusu + "'";
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
