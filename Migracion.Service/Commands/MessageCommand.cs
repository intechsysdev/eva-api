using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Models.Command;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Migracion.Service.Commands
{
    public interface IMessageCommand
    {
        TResult Insert(GupshupCallback item);
    }
    public class MessageCommand : IMessageCommand
    {
      

        private readonly ApplicationDbContext _context;
        public MessageCommand(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public TResult Insert(GupshupCallback item)
        {
            TResult tresult = new TResult()
            {
                IsSuccess = false,
                Message = "",
                ExpireDate = DateTime.Now
            };

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

                            var body_json = JsonSerializer.Serialize(item);

                            using (var insertCommand = connection.CreateCommand())
                            {
                                insertCommand.Transaction = transaction;
                                insertCommand.CommandText = "INSERT INTO waevalog(app_waevalog, version_waevalog, type_waevalog, waevalog_Json, waevalog_datetime) " +
                                                            "VALUES (@app_waevalog, @version_waevalog, @type_waevalog, @waevalog_Json::JSON, @waevalog_datetime)";
                                insertCommand.Parameters.Add(new DB2Parameter("app_waevalog", item.app));
                                insertCommand.Parameters.Add(new DB2Parameter("version_waevalog", item.version));
                                insertCommand.Parameters.Add(new DB2Parameter("type_waevalog", item.type));
                                insertCommand.Parameters.Add(new DB2Parameter("waevalog_Json", body_json));
                                insertCommand.Parameters.Add(new DB2Parameter("waevalog_datetime", DateTime.Now));

                                int result = insertCommand.ExecuteNonQuery();

                                if (result > 0)
                                {
                                    tresult = new TResult()
                                    {
                                        IsSuccess = true,
                                        Message = result.ToString(),
                                        ExpireDate = DateTime.Now
                                    };
                                }
                                else
                                {
                                    tresult = new TResult()
                                    {
                                        IsSuccess = true,
                                        Message = "No data return",
                                        ExpireDate = DateTime.Now
                                    };
                                }
                            }

                            transaction.Commit();
                            return tresult; // Éxito, salir del bucle de reintentos
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
                    tresult = new TResult()
                    {
                        IsSuccess = false,
                        Message = $"Error después de {attempt + 1} intentos: {ex.Message}",
                        ExpireDate = DateTime.Now
                    };
                    return tresult;
                }
            }

            return tresult;
        }
    }
}
