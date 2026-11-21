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
    public interface IMensajesQueries
    {
        Task<List<mob_rmensajesE>> GetList(string cmpy, string codusu, string consec, string fecha);
        int Insert(mob_rmensajesE mensaje);
    }
    public class MensajesQueries : IMensajesQueries
    {
        private readonly ApplicationDbContext _context;
        public MensajesQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<mob_rmensajesE>> GetList(string cmpy, string codusu, string consec, string fecha)
        {
            List<mob_rmensajesE> lstMensajes = null;
            try
            {

                lstMensajes = await _context.mob_rmensajes.Where(x => x.mens_cmpy == cmpy && x.mens_codusu == codusu && x.mens_consec == consec && x.mens_fecha == fecha).ToListAsync();
                lstMensajes.ForEach(x => x.mens_fecha = Convert.ToDateTime(x.mens_fecha).ToString("MM/dd/yyyy"));

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstMensajes), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstMensajes = JsonSerializer.Deserialize<List<mob_rmensajesE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstMensajes;
        }
        public int Insert(mob_rmensajesE mensaje)
        {
            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * FROM mob_rmensajes WHERE mens_cmpy = '" + mensaje.mens_cmpy + "' AND mens_codusu = '" + mensaje.mens_codusu + "' AND mens_consec = '" + mensaje.mens_consec + "' AND mens_fecha = '" + mensaje.mens_fecha + "'";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rmensajes (mens_cmpy,mens_consec,mens_codusu,mens_tipo,mens_cust,mens_succli,mens_coddep,mens_fecha,mens_hora,mens_mensaje,mens_urlimagen,mens_mercado,mens_submercado) values ( @mens_cmpy, @mens_consec, @mens_codusu, @mens_tipo, @mens_cust, @mens_succli, @mens_coddep, @mens_fecha, @mens_hora, @mens_mensaje, @mens_urlimagen, @mens_mercado, @mens_submercado)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("mens_cmpy", mensaje.mens_cmpy),
                                 new DB2Parameter("mens_consec", mensaje.mens_consec),
                                 new DB2Parameter("mens_codusu", mensaje.mens_codusu),
                                 new DB2Parameter("mens_tipo", mensaje.mens_tipo),
                                 new DB2Parameter("mens_cust", mensaje.mens_cust),
                                 new DB2Parameter("mens_succli", mensaje.mens_succli),
                                 new DB2Parameter("mens_coddep", mensaje.mens_coddep),
                                 new DB2Parameter("mens_fecha", mensaje.mens_fecha),
                                 new DB2Parameter("mens_hora", mensaje.mens_hora),
                                 new DB2Parameter("mens_mensaje", mensaje.mens_mensaje),
                                 new DB2Parameter("mens_urlimagen", mensaje.mens_urlimagen),
                                 new DB2Parameter("mens_mercado", mensaje.mens_mercado),
                                 new DB2Parameter("mens_submercado", mensaje.mens_submercado)

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
