using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Models.Command;
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
    public interface IProductosPreciosQueries
    {
        Task<List<mob_rprodpreciosE>> GetListByMarca(string cmpy, string codfab, string marca);
        Task<List<mob_rprodpreciosE>> GetListByCodigo(string cmpy, string codfab, string codigo);
        Task<List<mob_rprodpreciosE>> GetListByEan(string cmpy, string ean);

        int Insert(mob_rprodpreciosCommand precio);
        int Update(mob_rprodpreciosCommand item);

    }
    public class ProductosPreciosQueries : IProductosPreciosQueries
    {
        private readonly ApplicationDbContext _context;
        public ProductosPreciosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<mob_rprodpreciosE>> GetListByMarca(string cmpy, string codfab, string marca)
        {
            List<mob_rprodpreciosE> lstPrecios = null;
            try
            {

                lstPrecios = await _context.mob_rprodprecios.Where(x => x.rpropre_cmpy == cmpy && x.rpropre_codfab == codfab && x.rpropre_marca == marca).ToListAsync();
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstPrecios), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstPrecios = JsonSerializer.Deserialize<List<mob_rprodpreciosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstPrecios;
        }
        public async Task<List<mob_rprodpreciosE>> GetListByCodigo(string cmpy, string codfab, string codigo)
        {
            List<mob_rprodpreciosE> lstPrecios = null;
            try
            {

                lstPrecios = await _context.mob_rprodprecios.Where(x => x.rpropre_cmpy == cmpy && x.rpropre_codfab == codfab && x.rpropre_codigo == codigo).ToListAsync();
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstPrecios), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstPrecios = JsonSerializer.Deserialize<List<mob_rprodpreciosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstPrecios;
        }
        public async Task<List<mob_rprodpreciosE>> GetListByEan(string cmpy, string ean)
        {
            List<mob_rprodpreciosE> lstPrecios = null;
            try
            {

                lstPrecios = await _context.mob_rprodprecios.Where(x => x.rpropre_cmpy == cmpy && x.rpropre_eanprofab == ean).ToListAsync();
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstPrecios), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstPrecios = JsonSerializer.Deserialize<List<mob_rprodpreciosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstPrecios;
        }

        public int Insert(mob_rprodpreciosCommand precio)
        {
            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * FROM mob_rprodprecios WHERE rpropre_cmpy = '" + precio.rpropre_cmpy + "' AND rpropre_codfab = '" + precio.rpropre_codfab + "' AND rpropre_codigo = '" + precio.rpropre_codigo + "'";
                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (!resultCommand.HasRows)
                        {
                            var commandText = "Insert into mob_rprodprecios (rpropre_cmpy, rpropre_codfab, rpropre_codigo, rpropre_desc, rpropre_eanprofab, rpropre_marca, rpropre_mercado, rpropre_submercado, rpropre_urlimagen," +
                                " rpropre_present, rpropre_porcprecinf, rpropre_porcprecsup, rpropre_precant, rpropre_precact, rpropre_porcestrategia, rpropre_codusu, rpropre_observacion, rpropre_estado, rpropre_nuevo, rpropre_feccreacion, rpropre_enter, rpropre_date, rpropre_time) values ( @rpropre_cmpy, @rpropre_codfab, @rpropre_codigo, @rpropre_desc, @rpropre_eanprofab, @rpropre_marca, @rpropre_mercado, @rpropre_submercado, @rpropre_urlimagen, @rpropre_present, @rpropre_porcprecinf, @rpropre_porcprecsup, @rpropre_precant, @rpropre_precact, @rpropre_porcestrategia, @rpropre_codusu, @rpropre_observacion, @rpropre_estado, @rpropre_nuevo, @rpropre_feccreacion, @rpropre_enter, @rpropre_date, @rpropre_time)";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rpropre_cmpy", precio.rpropre_cmpy),
                                 new DB2Parameter("rpropre_codfab", precio.rpropre_codfab),
                                 new DB2Parameter("rpropre_codigo", precio.rpropre_codigo),
                                 new DB2Parameter("rpropre_desc", precio.rpropre_desc),
                                 new DB2Parameter("rpropre_eanprofab", precio.rpropre_eanprofab),
                                 new DB2Parameter("rpropre_marca", precio.rpropre_marca),
                                 new DB2Parameter("rpropre_mercado", precio.rpropre_mercado),
                                 new DB2Parameter("rpropre_submercado", precio.rpropre_submercado),
                                 new DB2Parameter("rpropre_urlimagen", precio.rpropre_urlimagen),
                                 new DB2Parameter("rpropre_present", precio.rpropre_present),
                                 new DB2Parameter("rpropre_porcprecinf", precio.rpropre_porcprecinf),
                                 new DB2Parameter("rpropre_porcprecsup", precio.rpropre_porcprecsup),
                                 new DB2Parameter("rpropre_precant", precio.rpropre_precant),
                                 new DB2Parameter("rpropre_precact", precio.rpropre_precact),
                                 new DB2Parameter("rpropre_porcestrategia", precio.rpropre_porcestrategia),
                                 new DB2Parameter("rpropre_codusu", precio.rpropre_codusu),
                                 new DB2Parameter("rpropre_observacion", precio.rpropre_observacion),
                                 new DB2Parameter("rpropre_estado", precio.rpropre_estado),
                                 new DB2Parameter("rpropre_nuevo", precio.rpropre_nuevo),
                                 new DB2Parameter("rpropre_feccreacion", precio.rpropre_feccreacion),
                                 new DB2Parameter("rpropre_enter", precio.rpropre_enter),
                                 new DB2Parameter("rpropre_date", precio.rpropre_date),
                                 new DB2Parameter("rpropre_time", precio.rpropre_time)

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

        public int Update(mob_rprodpreciosCommand item)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rprodprecios " +
                                    "WHERE rpropre_cmpy = '" + item.rpropre_cmpy + "' AND rpropre_codfab = '" + item.rpropre_codfab + "' AND rpropre_codigo = '" + item.rpropre_codigo + "'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "UPDATE mob_rprodprecios SET rpropre_desc = @rpropre_desc,rpropre_eanprofab= @rpropre_eanprofab,rpropre_marca=@rpropre_marca,rpropre_mercado=@rpropre_mercado,rpropre_submercado=@rpropre_submercado,rpropre_urlimagen=@rpropre_urlimagen," +
                                               "rpropre_present=@rpropre_present,rpropre_porcprecinf=@rpropre_porcprecinf,rpropre_porcprecsup=@rpropre_porcprecsup,rpropre_precant=@rpropre_precant,rpropre_precact=@rpropre_precact,rpropre_porcestrategia=@rpropre_porcestrategia, " +
                                               "rpropre_observacion=@rpropre_observacion,rpropre_estado=@rpropre_estado,rpropre_nuevo=@rpropre_nuevo,rpropre_feccreacion=@rpropre_feccreacion,rpropre_enter=@rpropre_enter,rpropre_date=@rpropre_date,rpropre_time@rpropre_time " +
                                               "WHERE rpropre_cmpy = @rpropre_cmpy AND rpropre_codfab = @rpropre_codfab AND rpropre_codigo = @rpropre_codigo";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rpropre_desc", item.rpropre_desc),
                                 new DB2Parameter("rpropre_eanprofab", item.rpropre_eanprofab),
                                 new DB2Parameter("rpropre_marca", item.rpropre_marca),
                                 new DB2Parameter("rpropre_mercado", item.rpropre_mercado),
                                 new DB2Parameter("rpropre_submercado", item.rpropre_submercado),
                                 new DB2Parameter("rpropre_urlimagen", item.rpropre_urlimagen),
                                 new DB2Parameter("rpropre_present", item.rpropre_present),
                                 new DB2Parameter("rpropre_porcprecinf", item.rpropre_porcprecinf),
                                 new DB2Parameter("rpropre_porcprecsup", item.rpropre_porcprecsup),
                                 new DB2Parameter("rpropre_precant", item.rpropre_precant),
                                 new DB2Parameter("rpropre_precact", item.rpropre_precact),
                                 new DB2Parameter("rpropre_porcestrategia", item.rpropre_porcestrategia),
                                 new DB2Parameter("rpropre_observacion", item.rpropre_observacion),
                                 new DB2Parameter("rpropre_estado", item.rpropre_estado),
                                 new DB2Parameter("rpropre_nuevo", item.rpropre_nuevo),
                                 new DB2Parameter("rpropre_feccreacion", item.rpropre_feccreacion),
                                 new DB2Parameter("rpropre_enter", item.rpropre_enter),
                                 new DB2Parameter("rpropre_date", item.rpropre_date),
                                 new DB2Parameter("rpropre_time", item.rpropre_time),
                                 new DB2Parameter("rpropre_cmpy", item.rpropre_cmpy),
                                 new DB2Parameter("rpropre_codfab", item.rpropre_codfab),
                                 new DB2Parameter("rpropre_codigo", item.rpropre_codigo)
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
