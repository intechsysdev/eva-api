using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
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
    public interface ICuposCompromisoQueries
    {
        Task<List<mob_cupocompromisoE>> GetByClient(string cmpy, string teri, string reg, string cust, string ship, string num);
        int Insert(mob_cupocompromisoE cupocompromiso);
        int Update(mob_cupocompromisoE item);
        int Delete(string cmpy, string num, string reg, string teri);
    }

    public class CuposCompromisoQueries : ICuposCompromisoQueries
    {
        private readonly ApplicationDbContext _context;
        public CuposCompromisoQueries(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_cupocompromisoE>> GetByClient(string cmpy, string teri, string reg, string cust, string ship, string num)
        {
            List<mob_cupocompromisoE> lstCuposCompromiso = null;
            try
            {
                // Se consulta la vista view_mob_cupocompromiso que conserva las columnas
                // de mob_cupocompromiso y agrega el resultado de la gestión de Cartera en IBES.
                // mcc_valor_cupo_aprobado se protege con COALESCE porque en la vista puede venir
                // NULL (solo aplica a tiposolicitud 2/3) y la propiedad es decimal no nullable.
                string sqlQuery = "Select mcc_cmpy, mcc_reg, mcc_teri, mcc_consecutivo, mcc_tiposolicitud, mcc_compromisopago, mcc_cust, mcc_ship, mcc_fechaeva, mcc_horaeva, mcc_valorcompromiso, mcc_fechacompromiso, mcc_valornotacredito, mcc_fechanotacredito, mcc_observacioncompromisonc, mcc_tipocupo, mcc_valorcupo, mcc_observacioncupo, mcc_marca, mcc_fechaibes, mcc_estado_compromiso, mcc_fecha_apro_compromiso, mcc_obs_cartera_compromiso, mcc_estado_cupo, mcc_fecha_apro_cupo, COALESCE(mcc_valor_cupo_aprobado, 0) AS mcc_valor_cupo_aprobado, mcc_obs_cartera_cupo, mcc_estado_final " +
                    "FROM view_mob_cupocompromiso WHERE mcc_cmpy ='" + cmpy + "' AND mcc_teri ='" + teri + "' AND mcc_reg ='" + reg + "' AND mcc_cust ='" + cust + "' AND mcc_ship ='" + ship + "'";


                // num = "0" (o vacío) => consulta TODOS los compromisos del cliente (online a demanda).
                // Un consecutivo específico (> 0) mantiene el filtro por compatibilidad con el flujo existente.
                if (!string.IsNullOrWhiteSpace(num) && num != "0")
                {
                    sqlQuery += " AND mcc_consecutivo ='" + num + "'";
                }


                sqlQuery += " ORDER BY mcc_consecutivo DESC";

                lstCuposCompromiso = await _context.mob_cupocompromiso.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstCuposCompromiso), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstCuposCompromiso = JsonSerializer.Deserialize<List<mob_cupocompromisoE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstCuposCompromiso;
        }

        public int Insert(mob_cupocompromisoE cupocompromiso)
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
                            command.CommandText = "Select * FROM mob_cupocompromiso WHERE mcc_cmpy = @p_cmpy AND mcc_teri = @p_teri AND mcc_reg = @p_reg AND mcc_consecutivo = @p_consecutivo";
                            command.Parameters.Add(new DB2Parameter("p_cmpy", cupocompromiso.mcc_cmpy));
                            command.Parameters.Add(new DB2Parameter("p_teri", cupocompromiso.mcc_teri));
                            command.Parameters.Add(new DB2Parameter("p_reg", cupocompromiso.mcc_reg));
                            command.Parameters.Add(new DB2Parameter("p_consecutivo", cupocompromiso.mcc_consecutivo));

                            using (var resultCommand = command.ExecuteReader())
                            {
                                bool hasRows = resultCommand.Read();
                                resultCommand.Close();

                                if (!hasRows)
                                {
                                    using (var insertCommand = connection.CreateCommand())
                                    {
                                        insertCommand.Transaction = transaction;
                                        insertCommand.CommandText = "Insert into mob_cupocompromiso(mcc_cmpy, mcc_reg, mcc_teri, mcc_consecutivo, mcc_tiposolicitud, mcc_compromisopago, mcc_cust, mcc_ship, mcc_fechaeva, mcc_horaeva, mcc_valorcompromiso, mcc_fechacompromiso, mcc_valornotacredito, mcc_fechanotacredito, mcc_observacioncompromisonc, mcc_tipocupo, mcc_valorcupo, mcc_observacioncupo, mcc_marca) values(@mcc_cmpy, @mcc_reg, @mcc_teri, @mcc_consecutivo, @mcc_tiposolicitud, @mcc_compromisopago, @mcc_cust, @mcc_ship, @mcc_fechaeva, @mcc_horaeva, @mcc_valorcompromiso, @mcc_fechacompromiso, @mcc_valornotacredito, @mcc_fechanotacredito, @mcc_observacioncompromisonc, @mcc_tipocupo, @mcc_valorcupo, @mcc_observacioncupo, @mcc_marca)";
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_cmpy", cupocompromiso.mcc_cmpy));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_reg", cupocompromiso.mcc_reg));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_teri", cupocompromiso.mcc_teri));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_consecutivo", cupocompromiso.mcc_consecutivo));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_tiposolicitud", cupocompromiso.mcc_tiposolicitud));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_compromisopago", cupocompromiso.mcc_compromisopago));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_cust", cupocompromiso.mcc_cust));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_ship", cupocompromiso.mcc_ship));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_fechaeva", cupocompromiso.mcc_fechaeva));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_horaeva", cupocompromiso.mcc_horaeva));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_valorcompromiso", cupocompromiso.mcc_valorcompromiso));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_fechacompromiso", cupocompromiso.mcc_fechacompromiso));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_valornotacredito", cupocompromiso.mcc_valornotacredito));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_fechanotacredito", cupocompromiso.mcc_fechanotacredito));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_observacioncompromisonc", cupocompromiso.mcc_observacioncompromisonc));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_tipocupo", cupocompromiso.mcc_tipocupo));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_valorcupo", cupocompromiso.mcc_valorcupo));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_observacioncupo", cupocompromiso.mcc_observacioncupo));
                                        insertCommand.Parameters.Add(new DB2Parameter("mcc_marca", cupocompromiso.mcc_marca));
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

        public int Update(mob_cupocompromisoE item)
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
                            command.CommandText = "SELECT * FROM mob_cupocompromiso WHERE mcc_cmpy = @p_cmpy AND mcc_reg = @p_reg AND mcc_teri = @p_teri AND mcc_consecutivo = @p_consecutivo AND mcc_fechaeva = @p_fechaeva";
                            command.Parameters.Add(new DB2Parameter("p_cmpy", item.mcc_cmpy));
                            command.Parameters.Add(new DB2Parameter("p_reg", item.mcc_reg));
                            command.Parameters.Add(new DB2Parameter("p_teri", item.mcc_teri));
                            command.Parameters.Add(new DB2Parameter("p_consecutivo", item.mcc_consecutivo));
                            command.Parameters.Add(new DB2Parameter("p_fechaeva", item.mcc_fechaeva));

                            using (var resultCommand = command.ExecuteReader())
                            {
                                bool hasRows = resultCommand.Read();
                                resultCommand.Close();

                                if (hasRows)
                                {
                                    using (var updateCommand = connection.CreateCommand())
                                    {
                                        updateCommand.Transaction = transaction;
                                        updateCommand.CommandText = "UPDATE mob_cupocompromiso SET mcc_tiposolicitud = @mcc_tiposolicitud, mcc_compromisopago = @mcc_compromisopago, mcc_horaeva = @mcc_horaeva, mcc_valorcompromiso = @mcc_valorcompromiso, mcc_fechacompromiso = @mcc_fechacompromiso, mcc_valornotacredito = @mcc_valornotacredito, mcc_fechanotacredito = @mcc_fechanotacredito, mcc_observacioncompromisonc = @mcc_observacioncompromisonc, mcc_tipocupo = @mcc_tipocupo, mcc_valorcupo = @mcc_valorcupo, mcc_observacioncupo = @mcc_observacioncupo WHERE mcc_cmpy = @mcc_cmpy AND mcc_reg = @mcc_reg AND mcc_teri = @mcc_teri AND mcc_consecutivo = @mcc_consecutivo AND mcc_fechaeva = @mcc_fechaeva";
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_tiposolicitud", item.mcc_tiposolicitud));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_compromisopago", item.mcc_compromisopago));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_horaeva", item.mcc_horaeva));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_valorcompromiso", item.mcc_valorcompromiso));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_fechacompromiso", item.mcc_fechacompromiso));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_valornotacredito", item.mcc_valornotacredito));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_fechanotacredito", item.mcc_fechanotacredito));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_observacioncompromisonc", item.mcc_observacioncompromisonc));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_tipocupo", item.mcc_tipocupo));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_valorcupo", item.mcc_valorcupo));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_observacioncupo", item.mcc_observacioncupo));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_cmpy", item.mcc_cmpy));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_reg", item.mcc_reg));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_teri", item.mcc_teri));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_consecutivo", item.mcc_consecutivo));
                                        updateCommand.Parameters.Add(new DB2Parameter("mcc_fechaeva", item.mcc_fechaeva));
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
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "SELECT * FROM mob_cupocompromiso WHERE mcc_cmpy = @p_cmpy AND mcc_consecutivo = @p_num AND mcc_reg = @p_reg AND mcc_teri = @p_teri";
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
                                        deleteCommand.CommandText = "DELETE FROM mob_cupocompromiso WHERE mcc_cmpy = @mcc_cmpy AND mcc_consecutivo = @mcc_consecutivo AND mcc_reg = @mcc_reg AND mcc_teri = @mcc_teri";
                                        deleteCommand.Parameters.Add(new DB2Parameter("mcc_cmpy", cmpy));
                                        deleteCommand.Parameters.Add(new DB2Parameter("mcc_consecutivo", num));
                                        deleteCommand.Parameters.Add(new DB2Parameter("mcc_reg", reg));
                                        deleteCommand.Parameters.Add(new DB2Parameter("mcc_teri", teri));
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
    }
}
