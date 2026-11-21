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
    public interface IContactosQueries
    {
        Task<List<mob_rcontactosE>> GetListByCodUsu(string rcont_cmpy, string rctt_codusu);
        Task<List<mob_rcontactosE>> GetContact(string rcont_cmpy, string rcont_codcon, string rcont_tipdoc);
        Task<List<mob_rcontactoclienteE>> GetListByClient(string rcont_cmpy, string rctt_codusu);

        int InsertClients(mob_rcontactoclienteCommand contactClient);
        int Insert(mob_rcontactoCommand contact);

        int UpdateCliente(mob_rcontactoclienteCommand contacoCliente);
        int UpdateByTipoDoc(mob_rcontactoCommand contact);
        int Update(mob_rcontactoCommand contact);

        int Delete(string cmpy, string cus, string succli);
        int DeleteByContact(string cmpy, string cus, string succli, string codcon, string tipdoc);
    }
    public class ContactosQueries : IContactosQueries
    {
        private readonly ApplicationDbContext _context;
        public ContactosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rcontactosE>> GetListByCodUsu(string rcont_cmpy, string rctt_codusu)
        {
            List<mob_rcontactosE> lstAsociados = null;
            try
            {
                string sqlQuery = "Select rcont_cmpy, rcont_codcon, rcont_tipdoc, rcont_numdoc, rcont_nom1, rcont_nom2, rcont_ape1, rcont_ape2, rcont_feccum, rcont_correo, rcont_celular, rcont_date, rcont_time, rcont_estadoupd " +
                                  " FROM mob_rcustrans mob_rcustrans, mob_rcontactocliente mob_rcontactocliente, mob_rcontactos mob_rcontactos " +
                                  "WHERE  mob_rcustrans.rctt_cust = mob_rcontactocliente.rconcli_cus AND mob_rcustrans.rctt_succli = mob_rcontactocliente.rconcli_succli AND mob_rcontactos.rcont_codcon = mob_rcontactocliente.rconcli_codcon AND rcont_cmpy ='" + rcont_cmpy + "' AND rctt_codusu ='" + rctt_codusu + "'";
                lstAsociados = await _context.mob_rcontactos.FromSqlRaw(sqlQuery).ToListAsync();

                lstAsociados.ForEach(x =>
                {
                    x.rcont_tipdoc = x.rcont_tipdoc.Trim();
                    x.rcont_feccum = x.rcont_feccum != null ? Convert.ToDateTime(x.rcont_feccum).ToString("MM/dd/yyyy") : "";
                    x.rcont_date = x.rcont_date != null ? Convert.ToDateTime(x.rcont_date).ToString("MM/dd/yyyy") : "";
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcontactosE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rcontactosE>> GetContact(string rcont_cmpy, string rcont_codcon, string rcont_tipdoc)
        {
            List<mob_rcontactosE> lstAsociados = null;
            try
            {
                string sqlQuery = "Select rcont_cmpy, rcont_codcon, rcont_tipdoc, rcont_numdoc, rcont_nom1, rcont_nom2, rcont_ape1, rcont_ape2, rcont_feccum, rcont_correo, rcont_celular, rcont_date, rcont_time, rcont_estadoupd " +
                                  " FROM mob_rcustrans mob_rcustrans, mob_rcontactocliente mob_rcontactocliente, mob_rcontactos mob_rcontactos " +
                                  "WHERE  mob_rcustrans.rctt_cust = mob_rcontactocliente.rconcli_cus AND mob_rcustrans.rctt_succli = mob_rcontactocliente.rconcli_succli " +
                                  "AND mob_rcontactos.rcont_codcon = mob_rcontactocliente.rconcli_codcon AND rcont_cmpy ='" + rcont_cmpy + "' " +
                                  "AND rcont_codcon  ='" + rcont_codcon + "' AND rcont_tipdoc = '" + rcont_tipdoc + "'";
                lstAsociados = await _context.mob_rcontactos.FromSqlRaw(sqlQuery).ToListAsync();
                lstAsociados.ForEach(x =>
                {
                    x.rcont_tipdoc = x.rcont_tipdoc.Trim();
                    x.rcont_feccum = x.rcont_feccum != null ? Convert.ToDateTime(x.rcont_feccum).ToString("MM/dd/yyyy") : "";
                    x.rcont_date = x.rcont_date != null ? Convert.ToDateTime(x.rcont_date).ToString("MM/dd/yyyy") : "";
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcontactosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rcontactoclienteE>> GetListByClient(string cmpy, string rctt_codusu)
        {
            List<mob_rcontactoclienteE> lstAsociados = null;
            try
            {
                string sqlQuery = "Select rconcli_cmpy, rconcli_cus, rconcli_succli, rconcli_codcon, rconcli_telefono, rconcli_celular, rconcli_correo, rconcli_date, rconcli_time, " +
                    "rconcli_estadoupd,rcont_tipdoc, rcont_numdoc, rcont_nom1, rcont_nom2, rcont_ape1, rcont_ape2, rcont_feccum, rcont_correo, rcont_celular " +
                    "FROM mob_rcontactocliente rcontactocliente, mob_rcustrans rcustrans, mob_rcontactos rcontactos " +
                    "WHERE rcontactocliente.rconcli_cmpy = rcustrans.rctt_cmpy " +
                    "AND rcontactocliente.rconcli_cus = rcustrans.rctt_cust " +
                    "AND rcontactocliente.rconcli_succli = rcustrans.rctt_succli " +
                    "AND rcontactocliente.rconcli_codcon = rcontactos.rcont_codcon  " +
                    "AND rcontactocliente.rconcli_tipdoc = rcontactos.rcont_tipdoc " +
                    "AND rconcli_cmpy ='" + cmpy + "' AND rctt_codusu ='" + rctt_codusu + "'";
                lstAsociados = await _context.mob_rcontactocliente.FromSqlRaw(sqlQuery).ToListAsync();
                lstAsociados.ForEach(x =>
                {
                    x.rcont_tipdoc = x.rcont_tipdoc.Trim();
                    x.rcont_feccum = x.rcont_feccum != null ? Convert.ToDateTime(x.rcont_feccum).ToString("MM/dd/yyyy") : "";
                    x.rconcli_date = x.rconcli_date != null ? Convert.ToDateTime(x.rconcli_date).ToString("MM/dd/yyyy") : "";
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rcontactoclienteE>>(responseJson);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public int InsertClients(mob_rcontactoclienteCommand contactClient)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcontactocliente " +
                    "WHERE rconcli_cmpy='" + contactClient.rconcli_cmpy + "' AND rconcli_tipdoc = '" + contactClient.rconcli_tipdoc + "' AND rconcli_codcon = '" + contactClient.rconcli_codcon + "' AND rconcli_cus = '" + contactClient.rconcli_cus + "' AND rconcli_succli='" + contactClient.rconcli_succli + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rcontactocliente (rconcli_cmpy,rconcli_cus,rconcli_succli,rconcli_codcon,rconcli_tipdoc,rconcli_telefono,rconcli_celular,rconcli_correo,rconcli_enter,rconcli_date,rconcli_time,rconcli_estadoupd) values (@rconcli_cmpy,@rconcli_cus,@rconcli_succli,@rconcli_codcon,@rconcli_tipdoc,@rconcli_telefono,@rconcli_celular,@rconcli_correo,@rconcli_enter,@rconcli_date,@rconcli_time,@rconcli_estadoupd)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rconcli_cmpy", contactClient.rconcli_cmpy),
                                 new DB2Parameter("rconcli_cus", contactClient.rconcli_cus),
                                 new DB2Parameter("rconcli_succli", contactClient.rconcli_succli),
                                 new DB2Parameter("rconcli_codcon", contactClient.rconcli_codcon),
                                 new DB2Parameter("rconcli_tipdoc", contactClient.rconcli_tipdoc),
                                 new DB2Parameter("rconcli_telefono", contactClient.rconcli_telefono),
                                 new DB2Parameter("rconcli_celular", contactClient.rconcli_celular),
                                 new DB2Parameter("rconcli_correo", contactClient.rconcli_correo),
                                 new DB2Parameter("rconcli_enter", contactClient.rconcli_enter),
                                 new DB2Parameter("rconcli_date", contactClient.rconcli_date),
                                 new DB2Parameter("rconcli_time", contactClient.rconcli_time),
                                 new DB2Parameter("rconcli_estadoupd", contactClient.rconcli_estadoupd)
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

        public int Insert(mob_rcontactoCommand contactClient)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcontactos " +
                    "WHERE rcont_cmpy='" + contactClient.rcont_cmpy + "' AND rcont_tipdoc='" + contactClient.rcont_tipdoc + "' AND rcont_codcon='" + contactClient.rcont_codcon + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rcontactos (rcont_cmpy,rcont_codcon,rcont_tipdoc,rcont_numdoc,rcont_nom1,rcont_nom2,rcont_ape1,rcont_ape2,rcont_feccum,rcont_correo,rcont_celular,rcont_enter,rcont_date,rcont_time,rcont_estadoupd) values (@rcont_cmpy,@rcont_codcon,@rcont_tipdoc,@rcont_numdoc,@rcont_nom1,@rcont_nom2,@rcont_ape1,@rcont_ape2,@rcont_feccum,@rcont_correo,@rcont_celular,@rcont_enter,@rcont_date,@rcont_time,@rcont_estadoupd)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rcont_cmpy", contactClient.rcont_cmpy),
                                 new DB2Parameter("rcont_codcon", contactClient.rcont_codcon),
                                 new DB2Parameter("rcont_tipdoc", contactClient.rcont_tipdoc),
                                 new DB2Parameter("rcont_numdoc", contactClient.rcont_numdoc),
                                 new DB2Parameter("rcont_nom1", contactClient.rcont_nom1),
                                 new DB2Parameter("rcont_nom2", contactClient.rcont_nom2),
                                 new DB2Parameter("rcont_ape1", contactClient.rcont_ape1),
                                 new DB2Parameter("rcont_ape2", contactClient.rcont_ape2),
                                 new DB2Parameter("rcont_feccum", contactClient.rcont_feccum),
                                 new DB2Parameter("rcont_correo", contactClient.rcont_correo),
                                 new DB2Parameter("rcont_celular", contactClient.rcont_celular),
                                 new DB2Parameter("rcont_enter", contactClient.rcont_enter),
                                 new DB2Parameter("rcont_date", contactClient.rcont_date),
                                 new DB2Parameter("rcont_time", contactClient.rcont_time),
                                  new DB2Parameter("rcont_estadoupd", contactClient.rcont_estadoupd)
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

        public int UpdateCliente(mob_rcontactoclienteCommand contactoCliente)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * FROM mob_rcontactocliente WHERE rconcli_cmpy = '" + contactoCliente.rconcli_cmpy + "' AND rconcli_tipdoc = '" + contactoCliente.rconcli_tipdoc + "' AND " +
                        "rconcli_codcon = '" + contactoCliente.rconcli_codcon + "' AND rconcli_cus = '" + contactoCliente.rconcli_cus + "' AND rconcli_succli = '" + contactoCliente.rconcli_succli + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "UPDATE mob_rcontactocliente SET rconcli_telefono = @rconcli_telefono, rconcli_celular = @rconcli_celular, " +
                                "rconcli_correo = @rconcli_correo, rconcli_date = @rconcli_date, rconcli_time = @rconcli_time, rconcli_estadoupd = @rconcli_estadoupd " +
                                "WHERE  rconcli_cmpy=@rconcli_cmpy AND rconcli_tipdoc = @rconcli_tipdoc AND rconcli_codcon = @rconcli_codcon AND " +
                                "rconcli_cus= @rconcli_cus AND rconcli_succli = @rconcli_succli";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rconcli_telefono", contactoCliente.rconcli_telefono),
                                 new DB2Parameter("rconcli_celular", contactoCliente.rconcli_celular),
                                 new DB2Parameter("rconcli_correo", contactoCliente.rconcli_correo),
                                 new DB2Parameter("rconcli_date", contactoCliente.rconcli_date),
                                 new DB2Parameter("rconcli_time", contactoCliente.rconcli_time),
                                 new DB2Parameter("rconcli_estadoupd", contactoCliente.rconcli_estadoupd),
                                 new DB2Parameter("rconcli_cmpy", contactoCliente.rconcli_cmpy),
                                 new DB2Parameter("rconcli_tipdoc", contactoCliente.rconcli_tipdoc),
                                 new DB2Parameter("rconcli_codcon", contactoCliente.rconcli_codcon),
                                 new DB2Parameter("rconcli_cus", contactoCliente.rconcli_cus),
                                 new DB2Parameter("rconcli_succli", contactoCliente.rconcli_succli)
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

        public int UpdateByTipoDoc(mob_rcontactoCommand contacto)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcontactos WHERE rcont_cmpy = '" + contacto.rcont_cmpy + "' AND rcont_tipdoc = '" + contacto.rcont_tipdoc + "' and rcont_codcon = '" + contacto.rcont_codcon + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "UPDATE mob_rcontactos SET rcont_nom1 = @rcont_nom1, rcont_nom2 = @rcont_nom2, rcont_correo = @rcont_correo, " +
                                "rcont_celular = @rcont_celular, rcont_ape1 = @rcont_ape1, rcont_ape2 = @rcont_ape2, rcont_feccum = @rcont_feccum, " +
                                "rcont_date = @rcont_date, rcont_time = @rcont_time, rcont_estadoupd = @rcont_estadoupd " +
                                "WHERE rcont_cmpy=@rcont_cmpy AND rcont_tipdoc = @rcont_tipdoc and rcont_codcon = @rcont_codcon";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rcont_nom1", contacto.rcont_nom1),
                                 new DB2Parameter("rcont_nom2", contacto.rcont_nom2),
                                 new DB2Parameter("rcont_correo", contacto.rcont_correo),
                                 new DB2Parameter("rcont_celular", contacto.rcont_celular),
                                 new DB2Parameter("rcont_ape1", contacto.rcont_ape1),
                                 new DB2Parameter("rcont_ape2", contacto.rcont_ape2),
                                 new DB2Parameter("rcont_feccum", contacto.rcont_feccum),
                                 new DB2Parameter("rcont_date", contacto.rcont_date),
                                 new DB2Parameter("rcont_time", contacto.rcont_time),
                                 new DB2Parameter("rcont_estadoupd", contacto.rcont_estadoupd),
                                 new DB2Parameter("rcont_cmpy", contacto.rcont_cmpy),
                                 new DB2Parameter("rcont_tipdoc", contacto.rcont_tipdoc),
                                 new DB2Parameter("rcont_codcon", contacto.rcont_codcon)
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

        public int Update(mob_rcontactoCommand contacto)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcontactos WHERE rcont_cmpy = '" + contacto.rcont_cmpy + "' AND  rcont_codcon = '" + contacto.rcont_codcon + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "UPDATE mob_rcontactos SET rcont_nom1 = @rcont_nom1,rcont_nom2 = @rcont_nom2,rcont_correo = @rcont_correo,rcont_celular = @rcont_celular," +
                                "rcont_ape1 = @rcont_ape1,rcont_ape2 = @rcont_ape2,rcont_feccum = @rcont_feccum,rcont_date = @rcont_date,rcont_time = @rcont_time," +
                                "rcont_estadoupd = @rcont_estadoupd WHERE rcont_cmpy=@rcont_cmpy AND rcont_codcon = @rcont_codcon";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rcont_nom1", contacto.rcont_nom1),
                                 new DB2Parameter("rcont_nom2", contacto.rcont_nom2),
                                 new DB2Parameter("rcont_correo", contacto.rcont_correo),
                                 new DB2Parameter("rcont_celular", contacto.rcont_celular),
                                 new DB2Parameter("rcont_ape1", contacto.rcont_ape1),
                                 new DB2Parameter("rcont_ape2", contacto.rcont_ape2),
                                 new DB2Parameter("rcont_feccum", contacto.rcont_feccum),
                                 new DB2Parameter("rcont_date", contacto.rcont_date),
                                 new DB2Parameter("rcont_time", contacto.rcont_time),
                                 new DB2Parameter("rcont_estadoupd", contacto.rcont_estadoupd),
                                 new DB2Parameter("rcont_cmpy", contacto.rcont_cmpy),
                                 new DB2Parameter("rcont_codcon", contacto.rcont_codcon)
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

        public int Delete(string cmpy, string cus, string succli)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcontactocliente WHERE rconcli_cmpy = '" + cmpy + "' AND  rconcli_cus = '" + cus + "' AND rconcli_succli = '" + succli + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rcontactocliente WHERE rconcli_cmpy = '" + cmpy + "' AND  rconcli_cus = '" + cus + "' AND rconcli_succli = '" + succli + "'";

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

        public int DeleteByContact(string cmpy, string cus, string succli, string codcon, string tipdoc)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcontactocliente WHERE rconcli_cmpy = '" + cmpy + "' AND  rconcli_cus = '" + cus + "' AND rconcli_succli = '" + succli + "' AND " +
                        "rconcli_codcon = '" + codcon + "' AND rconcli_tipdoc = '" + tipdoc + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rcontactocliente WHERE rconcli_cmpy = '" + cmpy + "' AND  rconcli_cus = '" + cus + "' AND rconcli_succli = '" + succli + "' AND " +
                        "rconcli_codcon = '" + codcon + "' AND rconcli_tipdoc = '" + tipdoc + "'";

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
