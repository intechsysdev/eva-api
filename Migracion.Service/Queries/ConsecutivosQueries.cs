using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Migracion.Models;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Migracion.Service.Queries
{
    public interface IConsecutivosQueries
    {
        Task<List<mob_rconsecutivosE>> GetList(string cmpy, string codusu);
        int Update(mob_rconsecutivosE consecutivo);
        Task<string> GetLastConsecutive(string codDoc, string codUsu, string reg, string teri);
    }
    public class ConsecutivosQueries : IConsecutivosQueries
    {
        private readonly ApplicationDbContext _context;
        private readonly IUsuariosQueries _usuariosQueries;
        public ConsecutivosQueries(
          ApplicationDbContext context,
          IUsuariosQueries usuariosQueries)
        {
            _context = context;
            _usuariosQueries = usuariosQueries;
        }

        public async Task<List<mob_rconsecutivosE>> GetList(string cmpy, string codusu)
        {
            List<mob_rconsecutivosE> lstAcciones = null;
            try
            {
                lstAcciones = await _context.mob_rconsecutivos.Where(x => x.rcons_cmpy == cmpy && x.rcons_codusu == codusu).ToListAsync();
                lstAcciones.ForEach(x =>
                {
                    x.rcons_date = Convert.ToDateTime(x.rcons_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rconsecutivosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            lstAcciones.ForEach(x =>
            {
                x.rcons_coddoc = x.rcons_coddoc.Trim();
            });

            // Obtener reg y teri del vendedor para validar consecutivos de P y C
            var lstUsuarios = await _usuariosQueries.GetListCmpy(cmpy, codusu, "2");
            var usuario = lstUsuarios?.FirstOrDefault();

            string reg = usuario?.rusu_reg?.Trim() ?? "";
            string teri = usuario?.rusu_teri?.Trim() ?? "";

            // Validar y corregir consecutivos de V, P y SC
            var codDocsToValidate = new[] { "V", "P", "SC" };
            foreach (var consecutivo in lstAcciones.Where(x => codDocsToValidate.Contains(x.rcons_coddoc!.Trim().ToUpper())))
            {
                string codDoc = consecutivo.rcons_coddoc!.Trim().ToUpper();
                string lastConsecutive = await GetLastConsecutive(codDoc, codusu, reg, teri);

                if (!string.IsNullOrEmpty(lastConsecutive))
                {
                    int lastConsecValue = int.TryParse(lastConsecutive.Trim(), out var parsed) ? parsed : 0;
                    int currentConsecValue = int.TryParse(consecutivo.rcons_consec?.Trim(), out var parsedCurrent) ? parsedCurrent : 0;

                    if (lastConsecValue > currentConsecValue)
                    {
                        consecutivo.rcons_consec = lastConsecValue.ToString();
                        Update(consecutivo);
                    }
                }
            }

            return lstAcciones;
        }

        public int Update(mob_rconsecutivosE consecutivo)
        {
            int result = 0;
            int maxRetries = 3;
            int retryDelayMs = 200;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var connection = _context.Database.GetDbConnection();
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Establecer LOCK MODE WAIT para esperar hasta 5 segundos por el bloqueo
                            using (var lockCommand = connection.CreateCommand())
                            {
                                lockCommand.Transaction = transaction;
                                lockCommand.CommandText = "SET LOCK MODE TO WAIT 5";
                                lockCommand.ExecuteNonQuery();
                            }

                            // Verificar si existe el registro
                            bool exists = false;
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = "SELECT COUNT(*) FROM mob_rconsecutivos WHERE rcons_cmpy = ? AND rcons_codusu = ? AND rcons_coddoc = ?";
                                command.Parameters.Add(new DB2Parameter("p_cmpy", consecutivo.rcons_cmpy));
                                command.Parameters.Add(new DB2Parameter("p_codusu", consecutivo.rcons_codusu));
                                command.Parameters.Add(new DB2Parameter("p_coddoc", consecutivo.rcons_coddoc));
                                var countResult = command.ExecuteScalar();
                                exists = Convert.ToInt32(countResult) > 0;
                            }

                            if (exists)
                            {
                                // Ejecutar UPDATE dentro de la misma transacción
                                using (var updateCommand = connection.CreateCommand())
                                {
                                    updateCommand.Transaction = transaction;
                                    updateCommand.CommandText = @"UPDATE mob_rconsecutivos 
                                SET rcons_consec = ?, 
                                    rcons_date = ?, 
                                    rcons_time = ?, 
                                    rcons_enter = ? 
                                WHERE rcons_cmpy = ? 
                                  AND rcons_codusu = ? 
                                  AND rcons_coddoc = ?";

                                    updateCommand.Parameters.Add(new DB2Parameter("rcons_consec", consecutivo.rcons_consec));
                                    updateCommand.Parameters.Add(new DB2Parameter("rcons_date", consecutivo.rcons_date));
                                    updateCommand.Parameters.Add(new DB2Parameter("rcons_time", consecutivo.rcons_time));
                                    updateCommand.Parameters.Add(new DB2Parameter("rcons_enter", consecutivo.rcons_enter));
                                    updateCommand.Parameters.Add(new DB2Parameter("rcons_cmpy", consecutivo.rcons_cmpy));
                                    updateCommand.Parameters.Add(new DB2Parameter("rcons_codusu", consecutivo.rcons_codusu));
                                    updateCommand.Parameters.Add(new DB2Parameter("rcons_coddoc", consecutivo.rcons_coddoc));

                                    result = updateCommand.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            return result; // Éxito, salir del bucle de reintentos
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception ex) when (ex.Message.Contains("record is locked") && attempt < maxRetries - 1)
                {
                    // Esperar antes de reintentar con backoff exponencial
                    System.Threading.Thread.Sleep(retryDelayMs * (attempt + 1));
                    continue;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error actualizando consecutivo después de {attempt + 1} intentos: {ex.Message}", ex);
                }
            }

            return result;
        }

        public async Task<string> GetLastConsecutive(string codDoc, string codUsu, string reg, string teri)
        {
            string result = "";
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    switch (codDoc.ToUpper())
                    {
                        case "V":
                            command.CommandText = "SELECT MAX(rvis_consec) FROM mob_rvisita WHERE rvis_enter = ?";
                            command.Parameters.Add(new DB2Parameter("p_codusu", codUsu));
                            break;
                        case "P":
                            command.CommandText = "SELECT MAX(orh_num) FROM mob_rordhead WHERE orh_reg = ? AND orh_teri = ?";
                            command.Parameters.Add(new DB2Parameter("p_reg", reg));
                            command.Parameters.Add(new DB2Parameter("p_teri", teri));
                            break;
                        case "SC":
                            command.CommandText = "SELECT MAX(mcc_consecutivo) FROM MOB_CUPOCOMPROMISO WHERE mcc_reg = ? AND mcc_teri = ?";
                            command.Parameters.Add(new DB2Parameter("p_reg", reg));
                            command.Parameters.Add(new DB2Parameter("p_teri", teri));
                            break;
                        default:
                            return result;
                    }

                    var queryResult = await command.ExecuteScalarAsync();
                    result = queryResult != null && queryResult != DBNull.Value ? queryResult.ToString()! : "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo último consecutivo: {ex.Message}", ex);
            }

            return result;
        }
    }
}
