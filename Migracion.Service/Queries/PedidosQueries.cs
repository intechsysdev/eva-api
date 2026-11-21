using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Migracion.Models;
using Migracion.Models.Dtos;
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
    public interface IPedidosQueries
    {

        Task<List<mob_rcustmainSimp>> GetByUser(string cmpy, string codUsu, string reg, string teri);
        Task<List<mob_rordheadE>> GetByClient(string cmpy, string teri, string reg, string cust, string ship, string num);
        Task<List<mob_fact_pedido_eva_Dto>> GetByFactIbes(string cmpy, string teri, string reg, string cust, string ship, string num);
        Task<List<mob_rordheadE>> GetByDate(string cmpy, string reg, string teri, string num, string entdt);
        Task<List<mob_rordheadE>> GetByReg(string cmpy, string reg, string teri, string num);
        Task<List<mob_rordlineE>> GetDetail(string cmpy, string reg, string teri, string num);
        Task<List<mob_rordlineE>> GetDetailByProduct(string cmpy, string reg, string teri, string num, string part, string level, string endtdt);
        Task<List<mob_rordlineE>> GetDetailByPart(string cmpy, string reg, string teri, string num, string part);
        Task<List<mob_rordhead_hisE>> GetHis(int numero, string cmpy, string reg, string teri, string cust, string ship);
        Task<List<mob_rordline_hisE>> GetDetailHis(string cmpy, string reg, string teri, string num);
        Task<List<mob_pedido_full_Dto>> GetFull(int numero, string cmpy, string reg, string teri, string cust, string ship);
        Task<List<mob_rordheadE>> GetByNum(string cmpy, string num, string orh_reg, string orh_teri);
        int Insert(mob_rordheadE pedido);
        int InsertDetail(mob_rordlineE detalle);
        int InsertFull(mob_rordDto item);
        int Update(mob_rordheadE item);
        int UpdateDetail(mob_rordlineE item);

        int Delete(string cmpy, string num, string reg, string teri);
        int DeleteByReg(string cmpy, string num, string reg, string teri);
        int DeleteDetailByProd(string cmpy, string num, string reg, string teri, string part);
        int DeleteDetail(string cmpy, string num, string reg, string teri);
        int DeleteFull(string cmpy, string num, string reg, string teri);
    }
    public class PedidosQueries : IPedidosQueries
    {
        private readonly ApplicationDbContext _context;
        public PedidosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rcustmainSimp>> GetByUser(string cmpy, string codUsu, string reg, string teri)
        {
            List<mob_rcustmainSimp> lstAsociados = null;
            int maxRetries = 3;
            int retryDelayMs = 200;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var connection = _context.Database.GetDbConnection();
                    if (connection.State != ConnectionState.Open)
                        await connection.OpenAsync();

                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Establecer LOCK MODE WAIT
                            using (var lockCommand = connection.CreateCommand())
                            {
                                lockCommand.Transaction = transaction.GetDbTransaction();
                                lockCommand.CommandText = "SET LOCK MODE TO WAIT 5";
                                await lockCommand.ExecuteNonQueryAsync();
                            }

                            string sqlQuery = "SELECT rctt_cmpy, rctt_cust, rctt_succli " +
                                "FROM mob_rcustmain, mob_rordhead " +
                                "WHERE rctt_cmpy = {0} AND rctt_cust = orh_cust AND rctt_succli = orh_ship " +
                                "AND rctt_codusu = {1} AND orh_reg = {2} AND orh_teri = {3} " +
                                "GROUP BY rctt_cmpy, rctt_cust, rctt_succli";

                            lstAsociados = await _context.mob_rcustmainSimps
                                .FromSqlRaw(sqlQuery, cmpy, codUsu, reg, teri)
                                .ToListAsync();

                            var responseJson = Regex.Replace(
                                Regex.Replace(JsonSerializer.Serialize(lstAsociados),
                                    Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN),
                                Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS);

                            lstAsociados = JsonSerializer.Deserialize<List<mob_rcustmainSimp>>(responseJson);

                            await transaction.CommitAsync();
                            return lstAsociados;
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
                catch (Exception ex) when (ex.Message.Contains("record is locked") && attempt < maxRetries - 1)
                {
                    await Task.Delay(retryDelayMs * (attempt + 1));
                    continue;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error obteniendo pedidos por usuario después de {attempt + 1} intentos: {ex.Message}", ex);
                }
            }

            return lstAsociados;
        }


        public async Task<List<mob_rordheadE>> GetByClient(string cmpy, string teri, string reg, string cust, string ship, string num)
        {
            List<mob_rordheadE> lstAsociados = null;
            int maxRetries = 5;
            int retryDelayMs = 500;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var connection = _context.Database.GetDbConnection();
                    if (connection.State != ConnectionState.Open)
                        await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // CLAVE: Enrollar EF Core en la transacción manual
                            _context.Database.UseTransaction(transaction);

                            // Establecer LOCK MODE WAIT para esperar hasta 15 segundos por el bloqueo
                            using (var lockCommand = connection.CreateCommand())
                            {
                                lockCommand.Transaction = transaction;
                                lockCommand.CommandText = "SET LOCK MODE TO WAIT 15";
                                lockCommand.ExecuteNonQuery();
                            }

                            string sqlQuery = "Select orh_cmpy, orh_reg, orh_teri, orh_num, orh_entdt, orh_time, orh_com, orh_shpdt, orh_cust, orh_ship, orh_po, orh_num2, orh_marca, orh_date " +
                                "FROM mob_rordhead WHERE orh_cmpy ='" + cmpy + "' AND orh_teri ='" + teri + "' AND orh_reg ='" + reg + "' AND orh_cust ='" + cust + "' AND orh_ship ='" + ship + "' AND orh_num ='" + num + "'";
                            /*string sqlQuery = @"SELECT orh_cmpy, orh_reg, orh_teri, orh_num, orh_entdt, 
                                orh_time, orh_com, orh_shpdt, orh_cust, orh_ship, orh_po, 
                                orh_num2, orh_marca, orh_date 
                                FROM mob_rordhead 
                                WHERE orh_cmpy = {0} AND orh_teri = {1} AND orh_reg = {2} 
                                AND orh_cust = {3} AND orh_ship = {4} AND orh_num = {5}";*/

                            lstAsociados = await _context.mob_rordhead.FromSqlRaw(sqlQuery).ToListAsync();
                            lstAsociados.ForEach(x =>
                            {
                                x.orh_date = Convert.ToDateTime(x.orh_date).ToString("MM/dd/yyyy");
                                x.orh_entdt = Convert.ToDateTime(x.orh_entdt).ToString("MM/dd/yyyy");
                                x.orh_shpdt = Convert.ToDateTime(x.orh_shpdt).ToString("MM/dd/yyyy");
                            });

                            var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                            lstAsociados = JsonSerializer.Deserialize<List<mob_rordheadE>>(responseJson);

                            transaction.Commit();
                            return lstAsociados; // Éxito, salir del bucle de reintentos
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                        finally
                        {
                            // Limpiar la transacción del contexto EF al terminar
                            _context.Database.UseTransaction(null);
                        }
                    }
                }
                // Filtro corregido para capturar los mensajes reales de Informix
                catch (Exception ex) when (attempt < maxRetries - 1 && IsLockException(ex))
                {
                    int delay = retryDelayMs * (int)Math.Pow(2, attempt); // backoff exponencial
                    await Task.Delay(delay);
                    // continúa al siguiente intento
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"Error obteniendo pedidos por cliente después de {attempt + 1} intentos: {ex.Message}", ex);
                }
            }

            return lstAsociados;
        }

        // Método auxiliar que cubre todos los mensajes de lock de Informix
        private bool IsLockException(Exception ex)
        {
            var msg = ex.Message ?? string.Empty;
            var innerMsg = ex.InnerException?.Message ?? string.Empty;

            return msg.Contains("Lock Timeout Expired", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("record is locked", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("-154", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("ISAM error", StringComparison.OrdinalIgnoreCase)
                || innerMsg.Contains("Lock Timeout Expired", StringComparison.OrdinalIgnoreCase)
                || innerMsg.Contains("-154", StringComparison.OrdinalIgnoreCase);
        }
        public async Task<List<mob_fact_pedido_eva_Dto>> GetByFactIbes(string cmpy, string teri, string reg, string cust, string ship, string num)
        {
            List<mob_fact_pedido_eva_Dto> lstResultado = null;
            int maxRetries = 5;
            int retryDelayMs = 500;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var connection = _context.Database.GetDbConnection();
                    if (connection.State != ConnectionState.Open)
                        await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            _context.Database.UseTransaction(transaction);

                            using (var lockCommand = connection.CreateCommand())
                            {
                                lockCommand.Transaction = transaction;
                                lockCommand.CommandText = "SET LOCK MODE TO WAIT 15";
                                lockCommand.ExecuteNonQuery();
                            }

                            string sqlQuery = @"SELECT 
                                orh.orh_cmpy,
                                orh.orh_reg,
                                orh.orh_teri,
                                orh.orh_date AS fecha,
                                orh.orh_num AS num_pedido_eva,
                                orh.orh_ship AS cliente_cod,
                                NVL(cm.cm_name, 'Cliente no encontrado') AS nombre_cliente,
                                orh.orh_marca AS marca,
                                DECODE(orh.orh_marca, 'S', orh.orh_num2, 0) AS ped_ibes,
                                DECODE(orh.orh_marca, 'S', oh.or_inv, 0) AS fact_ibes
                                FROM mob_rordhead orh
                                LEFT JOIN custmain cm
                                    ON cm.cm_cmpy = orh.orh_cmpy
                                    AND cm.cm_cust = orh.orh_cust
                                LEFT JOIN ordhead oh
                                    ON oh.or_cmpy = orh.orh_cmpy
                                    AND oh.or_num = orh.orh_num2
                                WHERE orh.orh_cmpy = '" + cmpy + "' AND orh.orh_teri = '" + teri + "' AND orh.orh_reg = '" + reg + "' AND orh.orh_cust = '" + cust + "' AND orh.orh_ship = '" + ship + "' AND orh.orh_num = '" + num + "'";

                            lstResultado = await _context.mob_fact_pedido_eva.FromSqlRaw(sqlQuery).ToListAsync();

                            lstResultado.ForEach(x =>
                            {
                                if (!string.IsNullOrEmpty(x.fecha))
                                    x.fecha = Convert.ToDateTime(x.fecha).ToString("MM/dd/yyyy");
                            });

                            var responseJson = Regex.Replace(
                                Regex.Replace(JsonSerializer.Serialize(lstResultado), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN),
                                Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS);
                            lstResultado = JsonSerializer.Deserialize<List<mob_fact_pedido_eva_Dto>>(responseJson);

                            transaction.Commit();
                            return lstResultado;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                        finally
                        {
                            _context.Database.UseTransaction(null);
                        }
                    }
                }
                catch (Exception ex) when (attempt < maxRetries - 1 && IsLockException(ex))
                {
                    int delay = retryDelayMs * (int)Math.Pow(2, attempt);
                    await Task.Delay(delay);
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"Error obteniendo factura pedido EVA después de {attempt + 1} intentos: {ex.Message}", ex);
                }
            }

            return lstResultado;
        }

        public async Task<List<mob_rordheadE>> GetByNum(string cmpy, string num, string orh_reg, string orh_teri)
        {

            List<mob_rordheadE> lstAsociados = null;
            int maxRetries = 3;
            int retryDelayMs = 200;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var connection = _context.Database.GetDbConnection();
                    if (connection.State != ConnectionState.Open)
                        await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // CLAVE: Enrollar EF Core en la transacción manual
                            _context.Database.UseTransaction(transaction);

                            // Establecer LOCK MODE WAIT para esperar hasta 5 segundos por el bloqueo
                            using (var lockCommand = connection.CreateCommand())
                            {
                                lockCommand.Transaction = transaction;
                                lockCommand.CommandText = "SET LOCK MODE TO WAIT 5";
                                lockCommand.ExecuteNonQuery();
                            }

                            string sqlQuery = "Select orh_cmpy, orh_reg, orh_teri, orh_num, orh_entdt, orh_time, orh_com, orh_shpdt, orh_cust, orh_ship, orh_po, orh_num2, orh_marca, orh_date " +
                                "FROM mob_rordhead WHERE orh_cmpy = {0} AND orh_num = {1} AND orh_reg = {2} AND orh_teri = {3} AND orh_marca = 'S' ";

                            lstAsociados = await _context.mob_rordhead.FromSqlRaw(sqlQuery, cmpy, num, orh_reg, orh_teri).ToListAsync();
                            lstAsociados.ForEach(x =>
                            {
                                x.orh_date = Convert.ToDateTime(x.orh_date).ToString("MM/dd/yyyy");
                                x.orh_entdt = Convert.ToDateTime(x.orh_entdt).ToString("MM/dd/yyyy");
                                x.orh_shpdt = Convert.ToDateTime(x.orh_shpdt).ToString("MM/dd/yyyy");
                            });

                            var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                            lstAsociados = JsonSerializer.Deserialize<List<mob_rordheadE>>(responseJson);

                            transaction.Commit();
                            return lstAsociados;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                        finally
                        {
                            _context.Database.UseTransaction(null);
                        }
                    }
                }
                catch (Exception ex) when ((ex.Message.Contains("record is locked") || ex.Message.Contains("key value locked")) && attempt < maxRetries - 1)
                {
                    await Task.Delay(retryDelayMs * (attempt + 1));
                    continue;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error obteniendo pedidos por número después de {attempt + 1} intentos: {ex.Message}", ex);
                }
            }

            return lstAsociados;
        }


        public async Task<List<mob_rordheadE>> GetByDate(string cmpy, string reg, string teri, string num, string entdt)
        {

            List<mob_rordheadE> lstAsociados = null;
            try
            {
                lstAsociados = await _context.mob_rordhead.Where(x => x.orh_cmpy == cmpy && x.orh_reg == reg &&
                x.orh_teri == teri && x.orh_num == num && x.orh_entdt == entdt).ToListAsync();

                lstAsociados.ForEach(x =>
                {
                    x.orh_date = Convert.ToDateTime(x.orh_date).ToString("MM/dd/yyyy");
                    x.orh_entdt = Convert.ToDateTime(x.orh_entdt).ToString("MM/dd/yyyy");
                    x.orh_shpdt = Convert.ToDateTime(x.orh_shpdt).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rordheadE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rordheadE>> GetByReg(string cmpy, string reg, string teri, string num)
        {

            List<mob_rordheadE> lstAsociados = null;
            try
            {
                lstAsociados = await _context.mob_rordhead.Where(x => x.orh_cmpy == cmpy && x.orh_reg == reg &&
                x.orh_teri == teri && x.orh_num == num).ToListAsync();

                lstAsociados.ForEach(x =>
                {
                    x.orh_date = Convert.ToDateTime(x.orh_date).ToString("MM/dd/yyyy");
                    x.orh_entdt = Convert.ToDateTime(x.orh_entdt).ToString("MM/dd/yyyy");
                    x.orh_shpdt = Convert.ToDateTime(x.orh_shpdt).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAsociados), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAsociados = JsonSerializer.Deserialize<List<mob_rordheadE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAsociados;
        }

        public async Task<List<mob_rordlineE>> GetDetail(string cmpy, string reg, string teri, string num)
        {

            List<mob_rordlineE> lstDetalles = null;
            try
            {
                lstDetalles =await _context.mob_rordline.Where(x => x.orl_cmpy == cmpy && x.orl_reg == reg &&
                x.orl_teri == teri && x.orl_num == num).ToListAsync();

                lstDetalles.ForEach(x =>
                {
                    x.orl_date = Convert.ToDateTime(x.orl_date).ToString("MM/dd/yyyy");
                    x.orl_entdt = Convert.ToDateTime(x.orl_entdt).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstDetalles), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstDetalles = JsonSerializer.Deserialize<List<mob_rordlineE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstDetalles;
        }

        public async Task<List<mob_rordlineE>> GetDetailByProduct(string cmpy, string reg, string teri, string num, string part, string level, string endtdt)
        {

            List<mob_rordlineE> lstDetalles = null;
            try
            {
                lstDetalles = await _context.mob_rordline.Where(x => x.orl_cmpy == cmpy && x.orl_reg == reg &&
                x.orl_teri == teri && x.orl_num == num && x.orl_part == part && x.orl_level == level && x.orl_entdt == endtdt).ToListAsync();

                lstDetalles.ForEach(x =>
                {
                    x.orl_date = Convert.ToDateTime(x.orl_date).ToString("MM/dd/yyyy");
                    x.orl_entdt = Convert.ToDateTime(x.orl_entdt).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstDetalles), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstDetalles = JsonSerializer.Deserialize<List<mob_rordlineE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstDetalles;
        }

        public async Task<List<mob_rordlineE>> GetDetailByPart(string cmpy, string reg, string teri, string num, string part)
        {

            List<mob_rordlineE> lstDetalles = null;
            try
            {
                lstDetalles = await _context.mob_rordline.Where(x => x.orl_cmpy == cmpy && x.orl_reg == reg &&
                x.orl_teri == teri && x.orl_num == num && x.orl_part == part).ToListAsync();

                lstDetalles.ForEach(x =>
                {
                    x.orl_date = Convert.ToDateTime(x.orl_date).ToString("MM/dd/yyyy");
                    x.orl_entdt = Convert.ToDateTime(x.orl_entdt).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstDetalles), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstDetalles = JsonSerializer.Deserialize<List<mob_rordlineE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstDetalles;
        }

        public async Task<List<mob_rordhead_hisE>> GetHis(int numero, string cmpy, string reg, string teri, string cust, string ship)
        {

            List<mob_rordhead_hisE> lstHis = null;
            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    string sqlQuery = "Select first " + numero + "orh_cmpy, orh_reg, orh_teri, orh_num, orh_entdt, orh_time, orh_com, " +
                        "orh_shpdt, orh_cust, orh_ship, orh_po, orh_num2, orh_marca, orh_date " +
                        "FROM mob_rordhead " +
                        "WHERE orh_cmpy = '" + cmpy + "' AND orh_reg ='" + reg + "' AND orh_teri = '" + teri + "'AND orh_cust ='" + cust + "' AND orh_ship = '" + ship + "'" + " ORDER BY orh_entdt DESC";

                    lstHis = await _context.mob_rordhead_his.FromSqlRaw(sqlQuery).ToListAsync();
                    lstHis.ForEach(x =>
                    {
                        x.orh_date = Convert.ToDateTime(x.orh_date).ToString("MM/dd/yyyy");
                        x.orh_entdt = Convert.ToDateTime(x.orh_entdt).ToString("MM/dd/yyyy");
                        x.orh_shpdt = Convert.ToDateTime(x.orh_shpdt).ToString("MM/dd/yyyy");
                    });

                    var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstHis), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));

                    lstHis = JsonSerializer.Deserialize<List<mob_rordhead_hisE>>(responseJson);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                return lstHis;
            }
        }

        public async Task<List<mob_pedido_full_Dto>> GetFull(int numero, string cmpy, string reg, string teri, string cust, string ship)
        {

            List<mob_rordhead_hisE> lstHis = null;
            List<mob_pedido_full_Dto> result_list = new List<mob_pedido_full_Dto>();

            if (numero == 0)
            {
                return result_list;
            }
            
            try
            {
                lstHis = await GetHis( numero,  cmpy,  reg,  teri,  cust,  ship);
                // Se serializa con foreach + await en lugar de List.ForEach(async ...),
                // que generaba async void (fire-and-forget): se retornaba antes de tiempo
                // y se disparaban consultas concurrentes sobre la misma conexion DB2,
                // provocando bloqueos en Informix y timeout del endpoint.
                foreach (var x in lstHis)
                {
                    List<mob_rordline_hisE> lst_linea = await GetDetailHis(cmpy, reg, teri, x.orh_num);
                    if(lst_linea.Count > 0)
                    {
                        mob_pedido_full_Dto item_full = new mob_pedido_full_Dto()
                        {
                            encabezado = x,
                            list_detail = lst_linea
                        };
                        result_list.Add(item_full);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result_list;
        }

        public async Task<List<mob_rordline_hisE>> GetDetailHis(string cmpy, string reg, string teri, string num)
        {

            List<mob_rordline_hisE> lstDetallesHis = null;
            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    string sqlQuery = "Select orl_cmpy, orl_reg, orl_teri, orl_num, orl_entdt, orl_time, orl_part, orl_qord, orl_txbl, orl_level, orl_marca, orl_date " +
                        "FROM mob_rordline " +
                        "WHERE orl_cmpy = '" + cmpy + "' AND orl_reg = '" + reg + "' AND orl_teri = '" + teri + "' AND orl_num ='" + num + "'";

                    lstDetallesHis = await _context.mob_rordline_his.FromSqlRaw(sqlQuery).ToListAsync();
                    lstDetallesHis.ForEach(x =>
                    {
                        x.orl_date = Convert.ToDateTime(x.orl_date).ToString("MM/dd/yyyy");
                        x.orl_entdt = Convert.ToDateTime(x.orl_entdt).ToString("MM/dd/yyyy");
                    });

                    var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstDetallesHis), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                    lstDetallesHis = JsonSerializer.Deserialize<List<mob_rordline_hisE>>(responseJson);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                return lstDetallesHis;
            }
        }

        public int Insert(mob_rordheadE pedido)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "Select * FROM mob_rordhead WHERE orh_cmpy = @p_cmpy AND orh_teri = @p_teri AND orh_num = @p_num AND orh_entdt = @p_entdt";
                            command.Parameters.Add(new DB2Parameter("p_cmpy", pedido.orh_cmpy));
                            command.Parameters.Add(new DB2Parameter("p_teri", pedido.orh_teri));
                            command.Parameters.Add(new DB2Parameter("p_num", pedido.orh_num));
                            command.Parameters.Add(new DB2Parameter("p_entdt", pedido.orh_entdt));

                            using (var resultCommand = command.ExecuteReader())
                            {
                                bool hasRows = resultCommand.Read();
                                resultCommand.Close();

                                if (!hasRows)
                                {
                                    using (var insertCommand = connection.CreateCommand())
                                    {
                                        insertCommand.Transaction = transaction;
                                        insertCommand.CommandText = "Insert into mob_rordhead(orh_cmpy, orh_reg, orh_teri, orh_num, orh_entdt, orh_time, orh_com, orh_shpdt, orh_cust, orh_ship, orh_po, orh_num2, orh_marca, orh_date) values(@orh_cmpy, @orh_reg, @orh_teri, @orh_num, @orh_entdt, @orh_time, @orh_com, @orh_shpdt, @orh_cust, @orh_ship, @orh_po, @orh_num2, @orh_marca, @orh_date)";
                                        insertCommand.Parameters.Add(new DB2Parameter("orh_cmpy", pedido.orh_cmpy));
                                        insertCommand.Parameters.Add(new DB2Parameter("orh_reg", pedido.orh_reg));
                                        insertCommand.Parameters.Add(new DB2Parameter("orh_teri", pedido.orh_teri));
                                        insertCommand.Parameters.Add(new DB2Parameter("orh_num", pedido.orh_num));
                                        insertCommand.Parameters.Add(new DB2Parameter("orh_entdt", pedido.orh_entdt));
                                        insertCommand.Parameters.Add(new DB2Parameter("orh_time", pedido.orh_time));
                                        insertCommand.Parameters.Add(new DB2Parameter("orh_com", pedido.orh_com));
                                        insertCommand.Parameters.Add(new DB2Parameter("orh_shpdt", pedido.orh_shpdt));
                                        insertCommand.Parameters.Add(new DB2Parameter("orh_cust", pedido.orh_cust));
                                        insertCommand.Parameters.Add(new DB2Parameter("orh_ship", pedido.orh_ship));
                                        insertCommand.Parameters.Add(new DB2Parameter("orh_po", pedido.orh_po));
                                        insertCommand.Parameters.Add(new DB2Parameter("orh_num2", pedido.orh_num2));
                                        insertCommand.Parameters.Add(new DB2Parameter("orh_marca", pedido.orh_marca));
                                        insertCommand.Parameters.Add(new DB2Parameter("orh_date", pedido.orh_date));
                                        result = insertCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int InsertFull(mob_rordDto pedido)
        {
            int result = 0;
            int maxRetries = 5;
            int retryDelayMs = 500;
            var random = new Random();

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var connection = _context.Database.GetDbConnection();

                    // Cerrar y reabrir la conexión en reintentos para limpiar estado
                    if (attempt > 0 && connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Establecer LOCK MODE WAIT para esperar hasta 15 segundos por el bloqueo
                            using (var lockCommand = connection.CreateCommand())
                            {
                                lockCommand.Transaction = transaction;
                                lockCommand.CommandText = "SET LOCK MODE TO WAIT 15";
                                lockCommand.ExecuteNonQuery();
                            }

                            // 1. Consultar el pedido solo con los campos necesarios, sin filtro de marca
                            bool existePedido = false;
                            string marcaExistente = null;

                            using (var checkCommand = connection.CreateCommand())
                            {
                                checkCommand.Transaction = transaction;
                                checkCommand.CommandText = "SELECT orh_marca FROM mob_rordhead WHERE orh_cmpy = @p_cmpy AND orh_num = @p_num AND orh_reg = @p_reg AND orh_teri = @p_teri";
                                checkCommand.Parameters.Add(new DB2Parameter("p_cmpy", pedido.encabezado.orh_cmpy));
                                checkCommand.Parameters.Add(new DB2Parameter("p_num", pedido.encabezado.orh_num));
                                checkCommand.Parameters.Add(new DB2Parameter("p_reg", pedido.encabezado.orh_reg));
                                checkCommand.Parameters.Add(new DB2Parameter("p_teri", pedido.encabezado.orh_teri));

                                using (var reader = checkCommand.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        existePedido = true;
                                        marcaExistente = reader["orh_marca"]?.ToString()?.Trim();
                                    }
                                    reader.Close();
                                }
                            }

                            if (existePedido)
                            {
                                // 2. Si tiene marca S, retornar 1 porque no se permite hacer cambios
                                if (marcaExistente == "S")
                                {
                                    transaction.Commit();
                                    return 1;
                                }

                                // 3. Si no tiene marca S, eliminar encabezado existente e insertar lo nuevo
                                DeleteInternal(connection, transaction, pedido.encabezado.orh_cmpy, pedido.encabezado.orh_num, pedido.encabezado.orh_reg, pedido.encabezado.orh_teri);
                            }

                            // Eliminar SIEMPRE los detalles antes de insertarlos, incluso si el
                            // encabezado no existía: evita filas huérfanas de intentos previos
                            // fallidos y violaciones de clave duplicada, y habilita la inserción
                            // directa de detalles sin un SELECT de verificación por fila.
                            DeleteDetailInternal(connection, transaction, pedido.encabezado.orh_cmpy, pedido.encabezado.orh_num, pedido.encabezado.orh_reg, pedido.encabezado.orh_teri);

                            // Insertar nuevo encabezado (aplica tanto si no existía como si se eliminó)
                            using (var insertCommand = connection.CreateCommand())
                            {
                                insertCommand.Transaction = transaction;
                                insertCommand.CommandText = "Insert into mob_rordhead(orh_cmpy, orh_reg, orh_teri, orh_num, orh_entdt, orh_time, orh_com, orh_shpdt, orh_cust, orh_ship, orh_po, orh_num2, orh_marca, orh_date) values(@orh_cmpy, @orh_reg, @orh_teri, @orh_num, @orh_entdt, @orh_time, @orh_com, @orh_shpdt, @orh_cust, @orh_ship, @orh_po, @orh_num2, @orh_marca, @orh_date)";
                                insertCommand.Parameters.Add(new DB2Parameter("orh_cmpy", pedido.encabezado.orh_cmpy));
                                insertCommand.Parameters.Add(new DB2Parameter("orh_reg", pedido.encabezado.orh_reg));
                                insertCommand.Parameters.Add(new DB2Parameter("orh_teri", pedido.encabezado.orh_teri));
                                insertCommand.Parameters.Add(new DB2Parameter("orh_num", pedido.encabezado.orh_num));
                                insertCommand.Parameters.Add(new DB2Parameter("orh_entdt", pedido.encabezado.orh_entdt));
                                insertCommand.Parameters.Add(new DB2Parameter("orh_time", pedido.encabezado.orh_time));
                                insertCommand.Parameters.Add(new DB2Parameter("orh_com", pedido.encabezado.orh_com));
                                insertCommand.Parameters.Add(new DB2Parameter("orh_shpdt", pedido.encabezado.orh_shpdt));
                                insertCommand.Parameters.Add(new DB2Parameter("orh_cust", pedido.encabezado.orh_cust));
                                insertCommand.Parameters.Add(new DB2Parameter("orh_ship", pedido.encabezado.orh_ship));
                                insertCommand.Parameters.Add(new DB2Parameter("orh_po", pedido.encabezado.orh_po));
                                insertCommand.Parameters.Add(new DB2Parameter("orh_num2", pedido.encabezado.orh_num2));
                                insertCommand.Parameters.Add(new DB2Parameter("orh_marca", pedido.encabezado.orh_marca));
                                insertCommand.Parameters.Add(new DB2Parameter("orh_date", pedido.encabezado.orh_date));
                                result = insertCommand.ExecuteNonQuery();
                            }

                            // Insertar detalles dentro de la misma transacción.
                            // Se usa inserción directa (sin SELECT de verificación previo):
                            // en este punto los detalles del pedido ya fueron borrados (si existía)
                            // o el pedido es nuevo, por lo que la verificación es redundante y, al
                            // hacer un barrido secuencial de la tabla bajo LOCK MODE WAIT, provocaba
                            // deadlocks entre inserciones concurrentes (-143 ISAM deadlock detected).
                            foreach (var detalle in pedido.detalle)
                            {
                                InsertDetailDirect(connection, transaction, detalle);
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
                catch (Exception ex) when (
                    (ex.Message.Contains("record is locked") ||
                    ex.Message.Contains("Lock Timeout") ||
                    ex.Message.Contains("ISAM error") ||
                    ex.Message.Contains("Could not position") ||
                    ex.Message.Contains("key value locked"))
                    && attempt < maxRetries - 1)
                {
                    // Esperar antes de reintentar con backoff exponencial + jitter aleatorio
                    int delay = retryDelayMs * (attempt + 1) + random.Next(0, 300);
                    System.Threading.Thread.Sleep(delay);
                    continue;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error insertando pedido completo después de {attempt + 1} intentos: {ex.Message}", ex);
                }
            }

            return result;
        }

        public int InsertDetail(mob_rordlineE detalle)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        result = InsertDetailInternal(connection, transaction, detalle);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        private int InsertDetailInternal(DbConnection connection, DbTransaction transaction, mob_rordlineE detalle)
        {
            int result = 0;
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "Select * FROM mob_rordline WHERE orl_cmpy = @p_cmpy AND orl_teri = @p_teri AND orl_num = @p_num AND orl_part = @p_part AND orl_level = @p_level AND orl_entdt = @p_entdt";
                command.Parameters.Add(new DB2Parameter("p_cmpy", detalle.orl_cmpy));
                command.Parameters.Add(new DB2Parameter("p_teri", detalle.orl_teri));
                command.Parameters.Add(new DB2Parameter("p_num", detalle.orl_num));
                command.Parameters.Add(new DB2Parameter("p_part", detalle.orl_part));
                command.Parameters.Add(new DB2Parameter("p_level", detalle.orl_level));
                command.Parameters.Add(new DB2Parameter("p_entdt", detalle.orl_entdt));

                using (var resultCommand = command.ExecuteReader())
                {
                    bool hasRows = resultCommand.Read();
                    resultCommand.Close();

                    if (!hasRows)
                    {
                        using (var insertCommand = connection.CreateCommand())
                        {
                            insertCommand.Transaction = transaction;
                            insertCommand.CommandText = "Insert into mob_rordline (orl_cmpy,orl_reg,orl_teri,orl_num,orl_entdt,orl_time,orl_part,orl_qord,orl_txbl,orl_level,orl_marca,orl_date) values ( @orl_cmpy, @orl_reg, @orl_teri, @orl_num, @orl_entdt, @orl_time, @orl_part, @orl_qord, @orl_txbl, @orl_level, @orl_marca, @orl_date)";
                            insertCommand.Parameters.Add(new DB2Parameter("orl_cmpy", detalle.orl_cmpy));
                            insertCommand.Parameters.Add(new DB2Parameter("orl_reg", detalle.orl_reg));
                            insertCommand.Parameters.Add(new DB2Parameter("orl_teri", detalle.orl_teri));
                            insertCommand.Parameters.Add(new DB2Parameter("orl_num", detalle.orl_num));
                            insertCommand.Parameters.Add(new DB2Parameter("orl_entdt", detalle.orl_entdt));
                            insertCommand.Parameters.Add(new DB2Parameter("orl_time", detalle.orl_time));
                            insertCommand.Parameters.Add(new DB2Parameter("orl_part", detalle.orl_part));
                            insertCommand.Parameters.Add(new DB2Parameter("orl_qord", detalle.orl_qord));
                            insertCommand.Parameters.Add(new DB2Parameter("orl_txbl", detalle.orl_txbl));
                            insertCommand.Parameters.Add(new DB2Parameter("orl_level", detalle.orl_level));
                            insertCommand.Parameters.Add(new DB2Parameter("orl_marca", detalle.orl_marca));
                            insertCommand.Parameters.Add(new DB2Parameter("orl_date", detalle.orl_date));
                            result = insertCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            return result;
        }

        // Inserta un detalle directamente, sin el SELECT de verificación de existencia.
        // Pensado para el flujo de InsertFull, donde los detalles previos ya fueron eliminados
        // dentro de la misma transacción. Evita el barrido secuencial que causaba deadlocks.
        private int InsertDetailDirect(DbConnection connection, DbTransaction transaction, mob_rordlineE detalle)
        {
            using (var insertCommand = connection.CreateCommand())
            {
                insertCommand.Transaction = transaction;
                insertCommand.CommandText = "Insert into mob_rordline (orl_cmpy,orl_reg,orl_teri,orl_num,orl_entdt,orl_time,orl_part,orl_qord,orl_txbl,orl_level,orl_marca,orl_date) values ( @orl_cmpy, @orl_reg, @orl_teri, @orl_num, @orl_entdt, @orl_time, @orl_part, @orl_qord, @orl_txbl, @orl_level, @orl_marca, @orl_date)";
                insertCommand.Parameters.Add(new DB2Parameter("orl_cmpy", detalle.orl_cmpy));
                insertCommand.Parameters.Add(new DB2Parameter("orl_reg", detalle.orl_reg));
                insertCommand.Parameters.Add(new DB2Parameter("orl_teri", detalle.orl_teri));
                insertCommand.Parameters.Add(new DB2Parameter("orl_num", detalle.orl_num));
                insertCommand.Parameters.Add(new DB2Parameter("orl_entdt", detalle.orl_entdt));
                insertCommand.Parameters.Add(new DB2Parameter("orl_time", detalle.orl_time));
                insertCommand.Parameters.Add(new DB2Parameter("orl_part", detalle.orl_part));
                insertCommand.Parameters.Add(new DB2Parameter("orl_qord", detalle.orl_qord));
                insertCommand.Parameters.Add(new DB2Parameter("orl_txbl", detalle.orl_txbl));
                insertCommand.Parameters.Add(new DB2Parameter("orl_level", detalle.orl_level));
                insertCommand.Parameters.Add(new DB2Parameter("orl_marca", detalle.orl_marca));
                insertCommand.Parameters.Add(new DB2Parameter("orl_date", detalle.orl_date));
                return insertCommand.ExecuteNonQuery();
            }
        }

        public int Update(mob_rordheadE item)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "SELECT * FROM mob_rordhead WHERE orh_cmpy = @p_cmpy AND orh_reg = @p_reg AND orh_teri = @p_teri AND orh_num = @p_num AND orh_entdt = @p_entdt";
                            command.Parameters.Add(new DB2Parameter("p_cmpy", item.orh_cmpy));
                            command.Parameters.Add(new DB2Parameter("p_reg", item.orh_reg));
                            command.Parameters.Add(new DB2Parameter("p_teri", item.orh_teri));
                            command.Parameters.Add(new DB2Parameter("p_num", item.orh_num));
                            command.Parameters.Add(new DB2Parameter("p_entdt", item.orh_entdt));

                            using (var resultCommand = command.ExecuteReader())
                            {
                                bool hasRows = resultCommand.Read();
                                resultCommand.Close();

                                if (hasRows)
                                {
                                    using (var updateCommand = connection.CreateCommand())
                                    {
                                        updateCommand.Transaction = transaction;
                                        updateCommand.CommandText = "UPDATE mob_rordhead SET orh_com = @orh_com, orh_shpdt = @orh_shpdt, orh_po = @orh_po, orh_date = @orh_date, orh_num2 = @orh_num2 WHERE orh_cmpy = @orh_cmpy AND orh_reg = @orh_reg AND orh_teri = @orh_teri AND orh_num = @orh_num AND orh_entdt = @orh_entdt";
                                        updateCommand.Parameters.Add(new DB2Parameter("orh_com", item.orh_com));
                                        updateCommand.Parameters.Add(new DB2Parameter("orh_shpdt", item.orh_shpdt));
                                        updateCommand.Parameters.Add(new DB2Parameter("orh_po", item.orh_po));
                                        updateCommand.Parameters.Add(new DB2Parameter("orh_date", item.orh_date));
                                        updateCommand.Parameters.Add(new DB2Parameter("orh_num2", item.orh_num2));
                                        updateCommand.Parameters.Add(new DB2Parameter("orh_cmpy", item.orh_cmpy));
                                        updateCommand.Parameters.Add(new DB2Parameter("orh_reg", item.orh_reg));
                                        updateCommand.Parameters.Add(new DB2Parameter("orh_teri", item.orh_teri));
                                        updateCommand.Parameters.Add(new DB2Parameter("orh_num", item.orh_num));
                                        updateCommand.Parameters.Add(new DB2Parameter("orh_entdt", item.orh_entdt));
                                        result = updateCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int UpdateDetail(mob_rordlineE item)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "SELECT * FROM mob_rordline WHERE orl_cmpy = @p_cmpy AND orl_reg = @p_reg AND orl_teri = @p_teri AND orl_num = @p_num AND orl_part = @p_part AND orl_level = @p_level AND orl_entdt = @p_entdt";
                            command.Parameters.Add(new DB2Parameter("p_cmpy", item.orl_cmpy));
                            command.Parameters.Add(new DB2Parameter("p_reg", item.orl_reg));
                            command.Parameters.Add(new DB2Parameter("p_teri", item.orl_teri));
                            command.Parameters.Add(new DB2Parameter("p_num", item.orl_num));
                            command.Parameters.Add(new DB2Parameter("p_part", item.orl_part));
                            command.Parameters.Add(new DB2Parameter("p_level", item.orl_level));
                            command.Parameters.Add(new DB2Parameter("p_entdt", item.orl_entdt));

                            using (var resultCommand = command.ExecuteReader())
                            {
                                bool hasRows = resultCommand.Read();
                                resultCommand.Close();

                                if (hasRows)
                                {
                                    using (var updateCommand = connection.CreateCommand())
                                    {
                                        updateCommand.Transaction = transaction;
                                        updateCommand.CommandText = "UPDATE mob_rordline SET orl_entdt = @orl_entdt, orl_time = @orl_time, orl_qord = @orl_qord, orl_txbl = @orl_txbl, orl_level = @orl_level, orl_marca = @orl_marca, orl_date = @orl_date WHERE orl_cmpy = @orl_cmpy AND orl_reg = @orl_reg AND orl_teri = @orl_teri AND orl_num = @orl_num AND orl_part = @orl_part AND orl_level = @orl_level2 AND orl_entdt = @orl_entdt2";
                                        updateCommand.Parameters.Add(new DB2Parameter("orl_entdt", item.orl_entdt));
                                        updateCommand.Parameters.Add(new DB2Parameter("orl_time", item.orl_time));
                                        updateCommand.Parameters.Add(new DB2Parameter("orl_qord", item.orl_qord));
                                        updateCommand.Parameters.Add(new DB2Parameter("orl_txbl", item.orl_txbl));
                                        updateCommand.Parameters.Add(new DB2Parameter("orl_level", item.orl_level));
                                        updateCommand.Parameters.Add(new DB2Parameter("orl_marca", item.orl_marca));
                                        updateCommand.Parameters.Add(new DB2Parameter("orl_date", item.orl_date));
                                        updateCommand.Parameters.Add(new DB2Parameter("orl_cmpy", item.orl_cmpy));
                                        updateCommand.Parameters.Add(new DB2Parameter("orl_reg", item.orl_reg));
                                        updateCommand.Parameters.Add(new DB2Parameter("orl_teri", item.orl_teri));
                                        updateCommand.Parameters.Add(new DB2Parameter("orl_num", item.orl_num));
                                        updateCommand.Parameters.Add(new DB2Parameter("orl_part", item.orl_part));
                                        updateCommand.Parameters.Add(new DB2Parameter("orl_level2", item.orl_level));
                                        updateCommand.Parameters.Add(new DB2Parameter("orl_entdt2", item.orl_entdt));
                                        result = updateCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int Delete(string cmpy, string num, string reg, string teri)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        result = DeleteInternal(connection, transaction, cmpy, num, reg, teri);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        private int DeleteInternal(DbConnection connection, DbTransaction transaction, string cmpy, string num, string reg, string teri)
        {
            int result = 0;
            using (var deleteCommand = connection.CreateCommand())
            {
                deleteCommand.Transaction = transaction;
                deleteCommand.CommandText = "DELETE FROM mob_rordhead WHERE orh_cmpy = @orh_cmpy AND orh_num = @orh_num AND orh_reg = @orh_reg AND orh_teri = @orh_teri";
                deleteCommand.Parameters.Add(new DB2Parameter("orh_cmpy", cmpy));
                deleteCommand.Parameters.Add(new DB2Parameter("orh_num", num));
                deleteCommand.Parameters.Add(new DB2Parameter("orh_reg", reg));
                deleteCommand.Parameters.Add(new DB2Parameter("orh_teri", teri));
                result = deleteCommand.ExecuteNonQuery();
            }
            return result;
        }

        public int DeleteFull(string cmpy, string num, string reg, string teri)
        {
            int result_enc = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        result_enc = DeleteFullInternal(connection, transaction, cmpy, num, reg, teri);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result_enc;
        }

        private int DeleteFullInternal(DbConnection connection, DbTransaction transaction, string cmpy, string num, string reg, string teri)
        {
            int result_enc = DeleteInternal(connection, transaction, cmpy, num, reg, teri);
            if (result_enc != 0)
            {
                DeleteDetailInternal(connection, transaction, cmpy, num, reg, teri);
            }
            return result_enc;
        }

        public int DeleteByReg(string cmpy, string num, string reg, string teri)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "SELECT * FROM mob_rordhead WHERE orh_cmpy = @p_cmpy AND orh_num = @p_num AND orh_reg = @p_reg AND orh_teri = @p_teri";
                            command.Parameters.Add(new DB2Parameter("p_cmpy", cmpy));
                            command.Parameters.Add(new DB2Parameter("p_num", num));
                            command.Parameters.Add(new DB2Parameter("p_reg", reg));
                            command.Parameters.Add(new DB2Parameter("p_teri", teri));

                            using (var resultCommand = command.ExecuteReader())
                            {
                                bool hasRows = resultCommand.Read();
                                resultCommand.Close();

                                if (hasRows)
                                {
                                    using (var deleteCommand = connection.CreateCommand())
                                    {
                                        deleteCommand.Transaction = transaction;
                                        deleteCommand.CommandText = "DELETE FROM mob_rordhead WHERE orh_cmpy = @orh_cmpy AND orh_num = @orh_num AND orh_reg = @orh_reg AND orh_teri = @orh_teri";
                                        deleteCommand.Parameters.Add(new DB2Parameter("orh_cmpy", cmpy));
                                        deleteCommand.Parameters.Add(new DB2Parameter("orh_num", num));
                                        deleteCommand.Parameters.Add(new DB2Parameter("orh_reg", reg));
                                        deleteCommand.Parameters.Add(new DB2Parameter("orh_teri", teri));
                                        result = deleteCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int DeleteDetailByProd(string cmpy, string num, string reg, string teri, string part)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "SELECT * FROM mob_rordline WHERE orl_cmpy = @p_cmpy AND orl_num = @p_num AND orl_reg = @p_reg AND orl_teri = @p_teri AND orl_part = @p_part";
                            command.Parameters.Add(new DB2Parameter("p_cmpy", cmpy));
                            command.Parameters.Add(new DB2Parameter("p_num", num));
                            command.Parameters.Add(new DB2Parameter("p_reg", reg));
                            command.Parameters.Add(new DB2Parameter("p_teri", teri));
                            command.Parameters.Add(new DB2Parameter("p_part", part));

                            using (var resultCommand = command.ExecuteReader())
                            {
                                bool hasRows = resultCommand.Read();
                                resultCommand.Close();

                                if (hasRows)
                                {
                                    using (var deleteCommand = connection.CreateCommand())
                                    {
                                        deleteCommand.Transaction = transaction;
                                        deleteCommand.CommandText = "DELETE FROM mob_rordline WHERE orl_cmpy = @orl_cmpy AND orl_num = @orl_num AND orl_reg = @orl_reg AND orl_teri = @orl_teri AND orl_part = @orl_part";
                                        deleteCommand.Parameters.Add(new DB2Parameter("orl_cmpy", cmpy));
                                        deleteCommand.Parameters.Add(new DB2Parameter("orl_num", num));
                                        deleteCommand.Parameters.Add(new DB2Parameter("orl_reg", reg));
                                        deleteCommand.Parameters.Add(new DB2Parameter("orl_teri", teri));
                                        deleteCommand.Parameters.Add(new DB2Parameter("orl_part", part));
                                        result = deleteCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int DeleteDetail(string cmpy, string num, string reg, string teri)
        {
            int result = 0;
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        result = DeleteDetailInternal(connection, transaction, cmpy, num, reg, teri);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        private int DeleteDetailInternal(DbConnection connection, DbTransaction transaction, string cmpy, string num, string reg, string teri)
        {
            int result = 0;
            using (var deleteCommand = connection.CreateCommand())
            {
                deleteCommand.Transaction = transaction;
                deleteCommand.CommandText = "DELETE FROM mob_rordline WHERE orl_cmpy = @orl_cmpy AND orl_num = @orl_num AND orl_reg = @orl_reg AND orl_teri = @orl_teri";
                deleteCommand.Parameters.Add(new DB2Parameter("orl_cmpy", cmpy));
                deleteCommand.Parameters.Add(new DB2Parameter("orl_num", num));
                deleteCommand.Parameters.Add(new DB2Parameter("orl_reg", reg));
                deleteCommand.Parameters.Add(new DB2Parameter("orl_teri", teri));
                result = deleteCommand.ExecuteNonQuery();
            }
            return result;
        }
    }
}
