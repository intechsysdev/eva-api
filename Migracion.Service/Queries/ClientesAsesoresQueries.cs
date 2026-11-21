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
    public interface IClientesAsesoresQueries
    {
        Task<List<mob_rcustasesE>> GetList(string cmpy, string codusu, string ruta);
        int Insert(mob_rcustasesE asesor);
    }
    public class ClientesAsesoresQueries : IClientesAsesoresQueries
    {
        private readonly ApplicationDbContext _context;
        public ClientesAsesoresQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<mob_rcustasesE>> GetList(string cmpy, string codusu, string ruta)
        {
            List<mob_rcustasesE> lstAsesores = null;
            try
            {
                lstAsesores = await _context.mob_rcustases.Where(x => x.rctt_cmpy == cmpy && x.rctt_codusu == codusu && x.rctt_ruta == ruta).ToListAsync();
                lstAsesores.ForEach(x =>
                {
                    x.rctt_date = Convert.ToDateTime(x.rctt_date).ToString("MM/dd/yyyy");
                    x.rctt_feccum = Convert.ToDateTime(x.rctt_feccum).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsesores), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsesores = JsonSerializer.Deserialize<List<mob_rcustasesE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsesores;
        }

        public int Insert(mob_rcustasesE asesor)
        {
            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * FROM mob_rcustases WHERE rctt_cmpy = '" + asesor.rctt_cmpy + "' AND rctt_cust = '" + asesor.rctt_cust + "' AND rctt_tipdoc = '" + asesor.rctt_tipdoc + "' AND rctt_succli = '" + asesor.rctt_succli + "'";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "INSERT INTO mob_rcustases (rctt_cmpy, rctt_reg, rctt_cust, rctt_tipdoc, rctt_resal, rctt_succli, rctt_razsoc, rctt_nomcli, rctt_addr, rctt_tele, rctt_celular, rctt_fax, " +
                                "rctt_dist, rctt_sic, rctt_codusu, rctt_ruta, rctt_sec, rctt_clinvo, rctt_date, rctt_estrato, rctt_latitud, rctt_longitud, rctt_frevisita, rctt_horsugvis, rctt_codabc, rctt_type, rctt_zip, " +
                                "rctt_zip4, rctt_vot, rctt_limit, rctt_level, rctt_bal, rctt_cont, rctt_feccum, rctt_pedmini, rctt_diasent, rctt_tax, rctt_nomcan, rctt_schrg, rctt_diavisita, rctt_imagen, rctt_urlimagen, " +
                                "rctt_mail, rctt_mppago, rctt_vpmin, rctt_zone, estado_envio) VALUES ( @rctt_cmpy, @rctt_reg, @rctt_cust, @rctt_tipdoc, @rctt_resal, @rctt_succli, @rctt_razsoc, @rctt_nomcli, @rctt_addr, @rctt_tele, @rctt_celular, @rctt_fax, @rctt_dist, @rctt_sic, @rctt_codusu, @rctt_ruta, @rctt_sec, @rctt_clinvo, @rctt_date, @rctt_estrato, @rctt_latitud, @rctt_longitud, @rctt_frevisita, @rctt_horsugvis, @rctt_codabc, @rctt_type, @rctt_zip, @rctt_zip4, @rctt_vot, @rctt_limit, @rctt_level, @rctt_bal, @rctt_cont, @rctt_feccum, @rctt_pedmini, @rctt_diasent, @rctt_tax, @rctt_nomcan, @rctt_schrg, @rctt_diavisita, @rctt_imagen, @rctt_urlimagen, @rctt_mail, @rctt_mppago, @rctt_vpmin, @rctt_zone, @estado_envio)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rctt_cmpy", asesor.rctt_cmpy),
                                 new DB2Parameter("rctt_reg", asesor.rctt_reg),
                                 new DB2Parameter("rctt_cust", asesor.rctt_cust),
                                 new DB2Parameter("rctt_tipdoc", asesor.rctt_tipdoc),
                                 new DB2Parameter("rctt_resal", asesor.rctt_resal),
                                 new DB2Parameter("rctt_succli", asesor.rctt_succli),
                                 new DB2Parameter("rctt_razsoc", asesor.rctt_razsoc),
                                 new DB2Parameter("rctt_nomcli", asesor.rctt_nomcli),
                                 new DB2Parameter("rctt_addr", asesor.rctt_addr),
                                 new DB2Parameter("rctt_tele", asesor.rctt_tele),
                                 new DB2Parameter("rctt_celular", asesor.rctt_celular),
                                 new DB2Parameter("rctt_fax", asesor.rctt_fax),
                                 new DB2Parameter("rctt_dist", asesor.rctt_dist),
                                 new DB2Parameter("rctt_sic", asesor.rctt_sic),
                                 new DB2Parameter("rctt_codusu", asesor.rctt_codusu),
                                 new DB2Parameter("rctt_ruta", asesor.rctt_ruta),
                                 new DB2Parameter("rctt_sec", asesor.rctt_sec),
                                 new DB2Parameter("rctt_clinvo", asesor.rctt_clinvo),
                                 new DB2Parameter("rctt_date", asesor.rctt_date),
                                 new DB2Parameter("rctt_estrato", asesor.rctt_estrato),
                                 new DB2Parameter("rctt_latitud", asesor.rctt_latitud),
                                 new DB2Parameter("rctt_longitud", asesor.rctt_longitud),
                                 new DB2Parameter("rctt_frevisita", asesor.rctt_frevisita),
                                 new DB2Parameter("rctt_horsugvis", asesor.rctt_horsugvis),
                                 new DB2Parameter("rctt_codabc", asesor.rctt_codabc),
                                 new DB2Parameter("rctt_type", asesor.rctt_type),
                                 new DB2Parameter("rctt_zip", asesor.rctt_zip),
                                 new DB2Parameter("rctt_zip4", asesor.rctt_zip4),
                                 new DB2Parameter("rctt_vot", asesor.rctt_vot),
                                 new DB2Parameter("rctt_limit", asesor.rctt_limit),
                                 new DB2Parameter("rctt_level", asesor.rctt_level),
                                 new DB2Parameter("rctt_bal", asesor.rctt_bal),
                                 new DB2Parameter("rctt_cont", asesor.rctt_cont),
                                 new DB2Parameter("rctt_feccum", asesor.rctt_feccum),
                                 new DB2Parameter("rctt_pedmini", asesor.rctt_pedmini),
                                 new DB2Parameter("rctt_diasent", asesor.rctt_diasent),
                                 new DB2Parameter("rctt_tax", asesor.rctt_tax),
                                 new DB2Parameter("rctt_nomcan", asesor.rctt_nomcan),
                                 new DB2Parameter("rctt_schrg", asesor.rctt_schrg),
                                 new DB2Parameter("rctt_diavisita", asesor.rctt_diavisita),
                                 new DB2Parameter("rctt_imagen", asesor.rctt_imagen),
                                 new DB2Parameter("rctt_urlimagen", asesor.rctt_urlimagen),
                                 new DB2Parameter("rctt_mail", asesor.rctt_mail),
                                 new DB2Parameter("rctt_mppago", asesor.rctt_mppago),
                                 new DB2Parameter("rctt_vpmin", asesor.rctt_vpmin),
                                 new DB2Parameter("rctt_zone", asesor.rctt_zone)
                                 //new DB2Parameter("estado_envio", asesor.estado_envio)

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
    }
}
