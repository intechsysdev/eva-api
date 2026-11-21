using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Migracion.Models;
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
    public interface IInventariosQueries
    {
        Task<List<mob_rcheqinventariosE>> GetListByConsec(string cmpy, string cust, string succli, int consec);
        Task<List<mob_rcheqinventariosE>> GetListByCliente(string cmpy, string cust, string succli);
        Task<List<mob_rcheqinventariosE>> GetListByCodUsu(string cmpy, string codusu, int consec, string cust, string succli);
        Task<List<mob_rcheqinvdetalleE>> GetDetail(string cmpy, string codusu, int consec);
        Task<List<mob_rcheqinvdetalleE>> GetDetailById(string cmpy, string codusu, int consec, string id);
        Task<List<InventarioStockDto>> GetListByCompany(string cmpy);
        Task<InventarioStockDto> GetByProducto(string cmpy, string codProducto);
        int Insert(mob_rcheqinventariosE inventario);
        int InsertDetail(mob_rcheqinvdetalleE detalle);

        int Update(mob_rcheqinventariosE item);
        int UpdateDetail(mob_rcheqinvdetalleE item);
        int Delete(string cmpy, string consec, string codUsu);
        int DeleteDetailByProd(string cmpy, string consec, string codUsu,string id);
        int DeleteDetail(string cmpy, string consec, string codUsu);
    }

    public class InventariosQueries : IInventariosQueries
    {
        private readonly ApplicationDbContext _context;
        public InventariosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<InventarioStockDto>> GetListByCompany(string cmpy)
        {
            List<InventarioStockDto> lstInventarioStock = null;

            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    string sqlQuery = @"
                    SELECT
                       *
                    FROM view_mob_inventario
                    WHERE cmpy = '" + cmpy + @"'";

                    lstInventarioStock = await _context.inventarioStock.FromSqlRaw(sqlQuery).ToListAsync();

                    var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstInventarioStock), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                    lstInventarioStock = JsonSerializer.Deserialize<List<InventarioStockDto>>(responseJson);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                return lstInventarioStock;
            }
        }

        public async Task<InventarioStockDto> GetByProducto(string cmpy, string codProducto)
        {
            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                InventarioStockDto inventarioStock = null;
                try
                {
                    string sqlQuery = @"
                    SELECT
                       *
                    FROM view_mob_inventario
                    WHERE cmpy = '" + cmpy + @"'
                      AND cod_producto = '" + codProducto + @"'";

                    var lstInventarioStock = await _context.inventarioStock.FromSqlRaw(sqlQuery).ToListAsync();

                    var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstInventarioStock), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                    lstInventarioStock = JsonSerializer.Deserialize<List<InventarioStockDto>>(responseJson);

                    inventarioStock = lstInventarioStock?.FirstOrDefault();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }

                return inventarioStock;
            }
        }

        public async Task<List<mob_rcheqinventariosE>> GetListByConsec(string cmpy, string cust, string succli, int consec)
        {
            List<mob_rcheqinventariosE> lstInventarios = null;
            try
            {
                lstInventarios = await _context.mob_rcheqinventarios.Where(x => x.rcinv_cmpy == cmpy && x.rcinv_cust == cust && x.rcinv_succli == succli && x.rcinv_consec == consec).ToListAsync();
                lstInventarios.ForEach(x =>
                {
                    x.rcinv_date = Convert.ToDateTime(x.rcinv_date).ToString("MM/dd/yyyy");
                    x.rcinv_feccheq = Convert.ToDateTime(x.rcinv_feccheq).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstInventarios), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstInventarios = JsonSerializer.Deserialize<List<mob_rcheqinventariosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstInventarios;
        }

        public async Task<List<mob_rcheqinventariosE>> GetListByCliente(string cmpy, string cust, string succli)
        {
            List<mob_rcheqinventariosE> lstInventarios = null;
            try
            {
                lstInventarios = await _context.mob_rcheqinventarios.Where(x => x.rcinv_cmpy == cmpy && x.rcinv_cust == cust && x.rcinv_succli == succli).ToListAsync(); lstInventarios.ForEach(x =>
                {
                    x.rcinv_date = Convert.ToDateTime(x.rcinv_date).ToString("MM/dd/yyyy");
                    x.rcinv_feccheq = Convert.ToDateTime(x.rcinv_feccheq).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstInventarios), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstInventarios = JsonSerializer.Deserialize<List<mob_rcheqinventariosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstInventarios;
        }

        public async Task<List<mob_rcheqinventariosE>> GetListByCodUsu(string cmpy, string codusu, int consec, string cust, string succli)
        {
            List<mob_rcheqinventariosE> lstInventarios = null;
            try
            {
                lstInventarios = await _context.mob_rcheqinventarios.Where(x => x.rcinv_cmpy == cmpy && x.rcinv_codusu == codusu && x.rcinv_consec == consec && x.rcinv_cust == cust && x.rcinv_succli == succli).ToListAsync();
                lstInventarios.ForEach(x =>
                {
                    x.rcinv_date = Convert.ToDateTime(x.rcinv_date).ToString("MM/dd/yyyy");
                    x.rcinv_feccheq = Convert.ToDateTime(x.rcinv_feccheq).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstInventarios), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstInventarios = JsonSerializer.Deserialize<List<mob_rcheqinventariosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstInventarios;
        }

        public async Task<List<mob_rcheqinvdetalleE>> GetDetail(string cmpy, string codusu, int consec)
        {
            List<mob_rcheqinvdetalleE> lstInventariosDetalles = null;
            try
            {

                lstInventariosDetalles = await _context.mob_rcheqinvdetalle.Where(x => x.rcinvdet_cmpy == cmpy && x.rcinvdet_codusu == codusu && x.rcinvdet_consec == consec).ToListAsync();
                lstInventariosDetalles.ForEach(x =>
                {
                    x.rcinvdet_date = Convert.ToDateTime(x.rcinvdet_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstInventariosDetalles), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstInventariosDetalles = JsonSerializer.Deserialize<List<mob_rcheqinvdetalleE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstInventariosDetalles;
        }

        public async Task<List<mob_rcheqinvdetalleE>> GetDetailById(string cmpy, string codusu, int consec, string id)
        {
            List<mob_rcheqinvdetalleE> lstInventariosDetalles = null;
            try
            {

                lstInventariosDetalles = await _context.mob_rcheqinvdetalle.Where(x => x.rcinvdet_cmpy == cmpy && x.rcinvdet_codusu == codusu && x.rcinvdet_consec == consec && x.rcinvdet_id == id).ToListAsync();
                lstInventariosDetalles.ForEach(x =>
                {
                    x.rcinvdet_date = Convert.ToDateTime(x.rcinvdet_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstInventariosDetalles), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstInventariosDetalles = JsonSerializer.Deserialize<List<mob_rcheqinvdetalleE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstInventariosDetalles;
        }

        public int Insert(mob_rcheqinventariosE inventario)
        {
            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * FROM mob_rcheqinventarios WHERE rcinv_cmpy = '" + inventario.rcinv_cmpy + "' AND rcinv_codusu = '" + inventario.rcinv_codusu + "' AND rcinv_consec = '" + inventario.rcinv_consec + "' AND rcinv_feccheq = '" + inventario.rcinv_feccheq + "'";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rcheqinventarios (rcinv_cmpy,rcinv_consec,rcinv_cust,rcinv_succli,rcinv_codusu,rcinv_custdep,rcinv_feccheq,rcinv_horaini,rcinv_horafin,rcinv_observacion,rcinv_marca,rcinv_date,rcinv_time) values ( @rcinv_cmpy, @rcinv_consec, @rcinv_cust, @rcinv_succli, @rcinv_codusu, @rcinv_custdep, @rcinv_feccheq, @rcinv_horaini, @rcinv_horafin, @rcinv_observacion, @rcinv_marca, @rcinv_date, @rcinv_time)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rcinv_cmpy", inventario.rcinv_cmpy),
                                 new DB2Parameter("rcinv_consec", inventario.rcinv_consec),
                                 new DB2Parameter("rcinv_cust", inventario.rcinv_cust),
                                 new DB2Parameter("rcinv_succli", inventario.rcinv_succli),
                                 new DB2Parameter("rcinv_codusu", inventario.rcinv_codusu),
                                 new DB2Parameter("rcinv_custdep", inventario.rcinv_custdep),
                                 new DB2Parameter("rcinv_feccheq", inventario.rcinv_feccheq),
                                 new DB2Parameter("rcinv_horaini", inventario.rcinv_horaini),
                                 new DB2Parameter("rcinv_horafin", inventario.rcinv_horafin),
                                 new DB2Parameter("rcinv_observacion", inventario.rcinv_observacion),
                                 new DB2Parameter("rcinv_marca", inventario.rcinv_marca),
                                 new DB2Parameter("rcinv_date", inventario.rcinv_date),
                                 new DB2Parameter("rcinv_time", inventario.rcinv_time)

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

        public int InsertDetail(mob_rcheqinvdetalleE detalle)
        {
            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * FROM mob_rcheqinvdetalle WHERE rcinvdet_cmpy = '" + detalle.rcinvdet_cmpy + "' AND rcinvdet_consec = '" + detalle.rcinvdet_consec + "' AND rcinvdet_codusu = '" + detalle.rcinvdet_codusu + "' AND rcinvdet_id = '" + detalle.rcinvdet_id + "'";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rcheqinvdetalle (rcinvdet_cmpy,rcinvdet_consec,rcinvdet_codusu,rcinvdet_id,rcinvdet_inventlineal,rcinvdet_inventbodega,rcinvdet_observacion,rcinvdet_marca,rcinvdet_date,rcinvdet_time) values ( @rcinvdet_cmpy, @rcinvdet_consec, @rcinvdet_codusu, @rcinvdet_id, @rcinvdet_inventlineal, @rcinvdet_inventbodega, @rcinvdet_observacion, @rcinvdet_marca, @rcinvdet_date, @rcinvdet_time)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rcinvdet_cmpy", detalle.rcinvdet_cmpy),
                                 new DB2Parameter("rcinvdet_consec", detalle.rcinvdet_consec),
                                 new DB2Parameter("rcinvdet_codusu", detalle.rcinvdet_codusu),
                                 new DB2Parameter("rcinvdet_id", detalle.rcinvdet_id),
                                 new DB2Parameter("rcinvdet_inventlineal", detalle.rcinvdet_inventlineal),
                                 new DB2Parameter("rcinvdet_inventbodega", detalle.rcinvdet_inventbodega),
                                 new DB2Parameter("rcinvdet_observacion", detalle.rcinvdet_observacion),
                                 new DB2Parameter("rcinvdet_marca", detalle.rcinvdet_marca),
                                 new DB2Parameter("rcinvdet_date", detalle.rcinvdet_date),
                                 new DB2Parameter("rcinvdet_time", detalle.rcinvdet_time)

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

        public int Update(mob_rcheqinventariosE item)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcheqinventarios " +
                                          "WHERE rcinv_cmpy = '" + item.rcinv_cmpy + "' AND rcinv_codusu = '" + item.rcinv_codusu + "' AND rcinv_consec = '" + item.rcinv_consec + "' AND rcinv_cust = '" + item.rcinv_cust + "' AND rcinv_succli = '" + item.rcinv_succli + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                     {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "UPDATE mob_rcheqinventarios SET rcinv_observacion = @rcinv_observacion, rcinv_feccheq = @rcinv_feccheq " +
                                              "WHERE rcinv_cmpy = @rcinv_cmpy AND rcinv_codusu = @rcinv_codusu AND rcinv_consec = @rcinv_consec AND rcinv_cust = @rcinv_cust AND rcinv_succli = @rcinv_succli";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rcinv_observacion", item.rcinv_observacion),
                                 new DB2Parameter("rcinv_feccheq", item.rcinv_feccheq),
                                 new DB2Parameter("rcinv_cmpy", item.rcinv_cmpy),
                                 new DB2Parameter("rcinv_codusu", item.rcinv_codusu),
                                 new DB2Parameter("rcinv_consec", item.rcinv_consec),
                                 new DB2Parameter("rcinv_cust", item.rcinv_cust),
                                 new DB2Parameter("rcinv_succli", item.rcinv_succli)
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


        public int UpdateDetail(mob_rcheqinvdetalleE item)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcheqinvdetalle " +
                                          "WHERE cinvdet_cmpy = '" + item.rcinvdet_cmpy + "' AND rcinvdet_codusu = '" +item.rcinvdet_codusu+ "' AND rcinvdet_consec = '" +item.rcinvdet_consec+ "' AND rcinvdet_id = '" +item.rcinvdet_id+ "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "UPDATE mob_rcheqinvdetalle  SET  rcinvdet_inventlineal = @rcinvdet_inventlineal, rcinvdet_inventbodega = @rcinvdet_inventbodega, rcinvdet_observacion =  @rcinvdet_observacion, rcinvdet_marca =@rcinvdet_marca, rcinvdet_date=@rcinvdet_date, rcinvdet_time = @rcinvdet_time " +
                                              "WHERE cinvdet_cmpy = @cinvdet_cmpy AND rcinvdet_codusu = @rcinvdet_codusu AND rcinvdet_consec = @rcinvdet_consec AND rcinvdet_id = @rcinvdet_id";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rcinvdet_inventlineal", item.rcinvdet_inventlineal),
                                 new DB2Parameter("rcinvdet_inventbodega", item.rcinvdet_inventbodega),
                                 new DB2Parameter("rcinvdet_observacion", item.rcinvdet_observacion),
                                 new DB2Parameter("rcinvdet_marca", item.rcinvdet_marca),
                                 new DB2Parameter("rcinvdet_date", item.rcinvdet_date),
                                 new DB2Parameter("rcinvdet_time", item.rcinvdet_time),
                                 new DB2Parameter("rcinvdet_cmpy", item.rcinvdet_cmpy),
                                 new DB2Parameter("rcinvdet_codusu", item.rcinvdet_codusu),
                                 new DB2Parameter("rcinvdet_consec", item.rcinvdet_consec),
                                 new DB2Parameter("rcinvdet_id", item.rcinvdet_id)
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

        public int Delete(string cmpy, string consec, string codUsu)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcheqinventarios " +
                                          "WHERE rcinv_cmpy = '" +cmpy+ "' AND rcinv_consec = '" +consec+ "' AND rcinv_codusu = '" +codUsu+ "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rcheqinventarios " +
                                              "WHERE rcinv_cmpy = '" +cmpy+ "' AND rcinv_consec = '" +consec+ "' AND rcinv_codusu = '" +codUsu+ "'";

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

        public int DeleteDetailByProd(string cmpy, string consec, string codUsu, string id)
        {
            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rcheqinvdetalle " +
                                          "WHERE rcinvdet_cmpy = '" +cmpy+ "' AND rcinvdet_consec = '" +consec+ "' AND rcinvdet_codusu = '" +codUsu+ "' AND rcinvdet_id = '" +id+ "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rcheqinvdetalle " +
                                              "WHERE rcinvdet_cmpy = '" +cmpy+ "' AND rcinvdet_consec = '" +consec+ "' AND rcinvdet_codusu = '" +codUsu+ "' AND rcinvdet_id = '" +id+ "'";

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

        public int DeleteDetail(string cmpy, string consec, string codUsu)
        {
            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                     command.CommandText = "SELECT * FROM mob_rcheqinvdetalle " +
                                          "WHERE rcinvdet_cmpy = '" + cmpy + "' AND rcinvdet_consec = '" + consec + "' AND rcinvdet_codusu = '" + codUsu + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "DELETE FROM mob_rcheqinvdetalle " +
                                              "WHERE rcinvdet_cmpy = '" + cmpy + "' AND rcinvdet_consec = '" + consec + "' AND rcinvdet_codusu = '" + codUsu + "'";

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
