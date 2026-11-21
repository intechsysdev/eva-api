using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Migracion.Service.Queries
{
    public interface IOptionConsentQueries
    {
        TResult Insert(option_consent item);
        Task<List<option_consent>> GetByReg(string cmpy, string reg);
        Task<List<option_consent>> GetByCelular(string cmpy, string celular);
        TResult UpdateStatusByCelular(string status, params string[] celulares);


    }

    public class OptionConsentQueries : IOptionConsentQueries
    {
        /// <summary>
        /// Indicativos de país de las compañías (CO=57, PE=51, EC=593, KR=1); se usan
        /// para derivar el número local cuando el callback trae el número internacional.
        /// </summary>
        private static readonly string[] IndicativosPais = { "593", "57", "51", "1" };

        private readonly ApplicationDbContext _context;

        public OptionConsentQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public TResult Insert(option_consent item)
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

                            // Idempotencia: si ya existe consentimiento para el celular, no se duplica.
                            using (var existsCommand = connection.CreateCommand())
                            {
                                existsCommand.Transaction = transaction;
                                existsCommand.CommandText = "SELECT FIRST 1 woc_id FROM wa_optin_consent WHERE woc_cmpy = @woc_cmpy AND woc_celular = @woc_celular";
                                existsCommand.Parameters.Add(new DB2Parameter("woc_cmpy", item.woc_cmpy));
                                existsCommand.Parameters.Add(new DB2Parameter("woc_celular", item.woc_celular));

                                var existingId = existsCommand.ExecuteScalar();
                                if (existingId != null && existingId != DBNull.Value)
                                {
                                    transaction.Commit();
                                    return new TResult()
                                    {
                                        IsSuccess = true,
                                        Message = "Ya existe consentimiento para el celular",
                                        ExpireDate = DateTime.Now
                                    };
                                }
                            }

                            using (var insertCommand = connection.CreateCommand())
                            {
                                insertCommand.Transaction = transaction;
                                insertCommand.CommandText =
                                    "INSERT INTO wa_optin_consent(woc_cmpy, woc_reg, woc_cust, woc_succli, woc_celular, woc_status, woc_channel, woc_source_app, woc_source_version, woc_ref_waevalog, woc_ip_address, woc_device_info, woc_consent_text, woc_consent_version, woc_optin_dt, woc_codusu) " +
                                    "VALUES (@woc_cmpy, @woc_reg, @woc_cust, @woc_succli, @woc_celular, @woc_status, @woc_channel, @woc_source_app, @woc_source_version, @woc_ref_waevalog, @woc_ip_address, @woc_device_info, @woc_consent_text, @woc_consent_version, @woc_optin_dt, @woc_codusu)";

                                insertCommand.Parameters.Add(new DB2Parameter("woc_cmpy", item.woc_cmpy));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_reg", item.woc_reg));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_cust", item.woc_cust));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_succli", (object?)item.woc_succli ?? DBNull.Value));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_celular", item.woc_celular));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_status", string.IsNullOrEmpty(item.woc_status) ? "A" : item.woc_status));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_channel", item.woc_channel));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_source_app", (object?)item.woc_source_app ?? DBNull.Value));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_source_version", (object?)item.woc_source_version ?? DBNull.Value));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_ref_waevalog", (object?)item.woc_ref_waevalog ?? DBNull.Value));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_ip_address", (object?)item.woc_ip_address ?? DBNull.Value));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_device_info", (object?)item.woc_device_info ?? DBNull.Value));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_consent_text", item.woc_consent_text));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_consent_version", item.woc_consent_version));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_optin_dt", item.woc_optin_dt ?? DateTime.Now));
                                insertCommand.Parameters.Add(new DB2Parameter("woc_codusu", (object?)item.woc_codusu ?? DBNull.Value));

                                int result = insertCommand.ExecuteNonQuery();

                                tresult = new TResult()
                                {
                                    IsSuccess = result > 0,
                                    Message = result > 0 ? result.ToString() : "No data return",
                                    ExpireDate = DateTime.Now
                                };
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

        public Task<List<option_consent>> GetByReg(string cmpy, string reg)
        {
            return Task.FromResult(Query("woc_reg", cmpy, reg));
        }

        public Task<List<option_consent>> GetByCelular(string cmpy, string celular)
        {
            return Task.FromResult(Query("woc_celular", cmpy, celular));
        }

        /// <summary>
        /// Consulta wa_optin_consent con SQL directo (sin EF) seleccionando solo las
        /// columnas que el Insert maneja, para no depender de columnas opcionales
        /// (woc_optout_dt, woc_entdt, woc_updt) que pueden no existir en la tabla.
        /// </summary>
        private List<option_consent> Query(string filterColumn, string cmpy, string value)
        {
            var consents = new List<option_consent>();
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT woc_id, woc_cmpy, woc_reg, woc_cust, woc_succli, woc_celular, woc_status, woc_channel, " +
                        "woc_source_app, woc_source_version, woc_ref_waevalog, woc_ip_address, woc_device_info, " +
                        "woc_consent_text, woc_consent_version, woc_optin_dt, woc_codusu " +
                        "FROM wa_optin_consent " +
                        "WHERE woc_cmpy = @woc_cmpy AND " + filterColumn + " = @value " +
                        "ORDER BY woc_id DESC";
                    command.Parameters.Add(new DB2Parameter("woc_cmpy", cmpy));
                    command.Parameters.Add(new DB2Parameter("value", value));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            consents.Add(new option_consent
                            {
                                woc_id = Convert.ToInt32(reader.GetValue(0)),
                                woc_cmpy = GetNullableString(reader, 1),
                                woc_reg = GetNullableString(reader, 2),
                                woc_cust = GetNullableString(reader, 3),
                                woc_succli = GetNullableString(reader, 4),
                                woc_celular = GetNullableString(reader, 5),
                                woc_status = GetNullableString(reader, 6),
                                woc_channel = GetNullableString(reader, 7),
                                woc_source_app = GetNullableString(reader, 8),
                                woc_source_version = GetNullableString(reader, 9),
                                woc_ref_waevalog = reader.IsDBNull(10) ? (long?)null : Convert.ToInt64(reader.GetValue(10)),
                                woc_ip_address = GetNullableString(reader, 11),
                                woc_device_info = GetNullableString(reader, 12),
                                woc_consent_text = GetNullableString(reader, 13),
                                woc_consent_version = GetNullableString(reader, 14),
                                woc_optin_dt = reader.IsDBNull(15) ? (DateTime?)null : Convert.ToDateTime(reader.GetValue(15)),
                                woc_codusu = GetNullableString(reader, 16)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return consents;
        }

        private static string GetNullableString(IDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal)?.TrimEnd();
        }

        /// <summary>
        /// Columnas existentes de wa_optin_consent según el catálogo de Informix.
        /// Permite armar UPDATEs que no fallen cuando las columnas opcionales
        /// (woc_optout_dt, woc_updt) no existen en la tabla.
        /// </summary>
        private static HashSet<string> GetColumnasTabla(
            System.Data.Common.DbConnection connection,
            System.Data.Common.DbTransaction transaction)
        {
            var columnas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText =
                    "SELECT c.colname FROM syscolumns c " +
                    "INNER JOIN systables t ON t.tabid = c.tabid " +
                    "WHERE t.tabname = 'wa_optin_consent'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        columnas.Add(reader.GetString(0).Trim());
                }
            }
            return columnas;
        }

        /// <summary>
        /// Actualiza el estado del consentimiento (A = autoriza, R = rechaza) para los
        /// celulares indicados. Para 'R' registra woc_optout_dt; para 'A' refresca woc_optin_dt.
        /// </summary>
        public TResult UpdateStatusByCelular(string status, params string[] celulares)
        {
            TResult tresult = new TResult()
            {
                IsSuccess = false,
                Message = "",
                ExpireDate = DateTime.Now
            };

            // Solo se aceptan los celulares no vacíos, normalizados a solo dígitos
            // (la app guarda woc_celular sin '+', espacios ni otros caracteres).
            var celularesNormalizados = (celulares ?? Array.Empty<string>())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => System.Text.RegularExpressions.Regex.Replace(c, @"\D", ""))
                .Where(c => c.Length > 0)
                .Distinct()
                .ToList();

            // El callback de WhatsApp entrega el número internacional completo
            // (ej. 573001234567) pero woc_celular se guarda sin el indicativo de
            // país (ej. 3001234567). Se agregan las variantes sin indicativo para
            // que el IN coincida en cualquiera de los dos formatos.
            var celularesValidos = celularesNormalizados
                .SelectMany(cel =>
                {
                    var variantes = new List<string> { cel };
                    foreach (var indicativo in IndicativosPais)
                    {
                        if (cel.StartsWith(indicativo) && cel.Length - indicativo.Length >= 7)
                            variantes.Add(cel.Substring(indicativo.Length));
                    }
                    return variantes;
                })
                .Distinct()
                .ToArray();

            if (celularesValidos.Length == 0)
            {
                tresult.Message = "No se recibió un número de celular válido";
                return tresult;
            }

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
                            using (var lockCommand = connection.CreateCommand())
                            {
                                lockCommand.Transaction = transaction;
                                lockCommand.CommandText = "SET LOCK MODE TO WAIT 5";
                                lockCommand.ExecuteNonQuery();
                            }

                            using (var updateCommand = connection.CreateCommand())
                            {
                                updateCommand.Transaction = transaction;

                                // El proveedor DB2/Informix enlaza los parámetros por posición:
                                // deben agregarse a la colección en el mismo orden en que los
                                // marcadores aparecen en el SQL (@woc_status primero, luego @celN).
                                // El nombre del DB2Parameter va SIN '@', igual que en Insert/Query.
                                updateCommand.Parameters.Add(new DB2Parameter("woc_status", status));

                                var inParams = new List<string>();
                                for (int i = 0; i < celularesValidos.Length; i++)
                                {
                                    inParams.Add("@cel" + i);
                                    updateCommand.Parameters.Add(new DB2Parameter("cel" + i, celularesValidos[i]));
                                }

                                // Las columnas de fechas son opcionales (pueden no existir en la
                                // tabla, ver comentario de Query); se incluyen en el SET solo si
                                // existen para que el UPDATE no falle por una columna desconocida.
                                var columnas = GetColumnasTabla(connection, transaction);
                                var setClauses = new List<string> { "woc_status = @woc_status" };
                                if (status == "A")
                                {
                                    // Para 'A' se refresca el opt-in y se limpia el opt-out.
                                    if (columnas.Contains("woc_optin_dt"))
                                        setClauses.Add("woc_optin_dt = CURRENT YEAR TO SECOND");
                                    if (columnas.Contains("woc_optout_dt"))
                                        setClauses.Add("woc_optout_dt = NULL");
                                }
                                else if (columnas.Contains("woc_optout_dt"))
                                {
                                    // Para 'R' se marca la fecha de opt-out.
                                    setClauses.Add("woc_optout_dt = CURRENT YEAR TO SECOND");
                                }
                                if (columnas.Contains("woc_updt"))
                                    setClauses.Add("woc_updt = CURRENT YEAR TO SECOND");

                                // TRIM sobre la columna para que espacios almacenados en
                                // woc_celular no impidan el match (los valores de los
                                // parámetros ya vienen normalizados a solo dígitos).
                                updateCommand.CommandText =
                                    "UPDATE wa_optin_consent SET " + string.Join(", ", setClauses) +
                                    " WHERE TRIM(woc_celular) IN (" + string.Join(", ", inParams) + ")";

                                int result = updateCommand.ExecuteNonQuery();

                                tresult = new TResult()
                                {
                                    IsSuccess = result > 0,
                                    Message = result > 0 ? result.ToString() : "No se encontró consentimiento para el celular",
                                    ExpireDate = DateTime.Now
                                };
                            }

                            transaction.Commit();
                            return tresult;
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
