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
    public interface IProductosQueries
    {
        Task<List<mob_rproddepositoE>> GetListDeposits(string cmpy, string custdep);
        Task<List<mob_rproductosE>> GetList(string cmpy, string linea);
        Task<List<mob_rproductosE>> GetListBySubL(string cmpy, string linea, string subl);
        Task<List<mob_rprodvendedorE>> GetListByVendedor(string cmpy, string codUsu);
        Task<List<mob_rprodvendedorE>> GetListByVendedorLinea(string cmpy, string codUsu, string linea);
        Task<List<mob_rprodvendedorE>> GetListByVendedorSubLinea(string cmpy, string codUsu, string linea, string subl);
        Task<List<mob_rsublineasE>> GetListSubLines(string cmpy);
        Task<List<mob_rsublineasE>> GetListSubLinesByCodUsu(string cmpy, string codUsu);
        Task<List<mob_prodganchoE>> GetProdGancho(string cmpy, string reg, string teri, string cust, string succli);
        Task<List<mob_rprodvendedorE>> GetListAll(string cmpy);
        Task<List<mob_rprodpuntoE>> GetProdPuntosLis(string cmpy);
        int UpdateImagen(string cmpy, string id, string url);

    }
    public class ProductosQueries : IProductosQueries 
    {
        private readonly ApplicationDbContext _context;
        public ProductosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rproddepositoE>> GetListDeposits(string cmpy, string custdep)
        {
            List<mob_rproddepositoE> lstAcciones = null;
            try
            {
                lstAcciones = await _context.mob_rproddeposito.Where(x => x.rprodep_cmpy == cmpy && x.rprodep_custdep == custdep).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rproddepositoE>>(responseJson);
                //lstAcciones.ForEach(x => x.rprodep_date = x.rprodep_date.Date);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

        public async Task<List<mob_rproductosE>> GetList(string cmpy, string linea)
        {
            List<mob_rproductosE> lstAcciones = null;
            try
            {
                lstAcciones = await _context.mob_rproductos.Where(x => x.rpr_cmpy == cmpy && x.rpr_linea == linea).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rproductosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

        public async Task<List<mob_rproductosE>> GetListBySubL(string cmpy, string linea, string subl)
        {
            List<mob_rproductosE> lstAcciones = null;
            try
            {
                lstAcciones = await _context.mob_rproductos.Where(x => x.rpr_cmpy == cmpy && x.rpr_linea == linea && x.rpr_subl == subl).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rproductosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

        public async Task<List<mob_rprodvendedorE>> GetListByVendedorLinea(string cmpy, string codUsu, string linea)
        {
            List<mob_rprodvendedorE> lstProductos = null;
            try
            {
                string sqlQuery = "Select rpr_cmpy, rpr_id, rpr_desc, rpr_linea, rpr_subl, rpr_nuevo, rpr_componentes, rpr_aplicacion, rpr_mediospubli, rpr_urlvideoclip, rpr_urlimagen, rpr_imagen, rpr_date, rpr_time, rpr_estadoupd, rpr_ean " +
                                  "FROM mob_rproductos rproductos,mob_rprodvendedor rprodvendedor " +
                                  "WHERE rproductos.rpr_id=rprodvendedor.rprodven_id AND rpr_cmpy='" + cmpy + "' AND rprodven_codusu='" + codUsu + "' AND rpr_linea='" + linea + "'";
                lstProductos = await _context.mob_rprodvendedor.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstProductos), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstProductos = JsonSerializer.Deserialize<List<mob_rprodvendedorE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstProductos;
        }

        public async Task<List<mob_rprodvendedorE>> GetListByVendedor(string cmpy, string codUsu)
        {
            List<mob_rprodvendedorE> lstProductos = null;
            try
            {
                string sqlQuery = "Select rpr_cmpy, rpr_id, rpr_desc, rpr_linea, rpr_subl, rpr_nuevo, rpr_componentes, rpr_aplicacion, rpr_mediospubli, rpr_urlvideoclip, rpr_urlimagen, rpr_imagen, rpr_date, rpr_time, rpr_estadoupd, rpr_ean " +
                                  "FROM mob_rproductos rproductos,mob_rprodvendedor rprodvendedor " +
                                  "WHERE rproductos.rpr_id=rprodvendedor.rprodven_id AND rpr_cmpy='" + cmpy + "' AND rprodven_codusu='" + codUsu + "'";
                lstProductos = await _context.mob_rprodvendedor.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstProductos), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstProductos = JsonSerializer.Deserialize<List<mob_rprodvendedorE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstProductos;
        }

        public async Task<List<mob_rprodvendedorE>> GetListAll(string cmpy)
        {
            List<mob_rprodvendedorE> lstProductos = null;
            try
            {
                string sqlQuery = "Select rpr_cmpy, rpr_id, rpr_desc, rpr_linea, rpr_subl, rpr_nuevo, rpr_componentes, rpr_aplicacion, rpr_mediospubli, rpr_urlvideoclip, rpr_urlimagen, rpr_imagen, rpr_date, rpr_time, rpr_estadoupd, rpr_ean " +
                                  "FROM mob_rproductos rproductos " +
                                  "WHERE rpr_cmpy='" + cmpy + "'";
                lstProductos = await _context.mob_rprodvendedor.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstProductos), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstProductos = JsonSerializer.Deserialize<List<mob_rprodvendedorE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstProductos;
        }

        public async Task<List<mob_rprodvendedorE>> GetListByVendedorSubLinea(string cmpy,string codUsu,string linea, string subl)
        { 
            
            List<mob_rprodvendedorE> lstProductos = null;
            try
            {
                string sqlQuery = "Select rpr_cmpy, rpr_id, rpr_desc, rpr_linea, rpr_subl, rpr_nuevo, rpr_componentes, rpr_aplicacion, rpr_mediospubli, rpr_urlvideoclip, rpr_urlimagen, rpr_imagen, rpr_date, rpr_time, rpr_estadoupd, rpr_ean " +
                                   "FROM mob_rproductos rproductos,mob_rprodvendedor rprodvendedor " +
                                   "WHERE rproductos.rpr_id=rprodvendedor.rprodven_id AND rpr_cmpy='" + cmpy + "' AND rprodven_codusu='" + codUsu + "' AND rpr_linea='" + linea + "' AND rpr_subl='" + subl+"'";
                lstProductos = await _context.mob_rprodvendedor.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstProductos), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstProductos = JsonSerializer.Deserialize<List<mob_rprodvendedorE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstProductos;
        }

        public async Task<List<mob_rsublineasE>> GetListSubLines(string cmpy)
        {
            List<mob_rsublineasE> lstAcciones = null;
            try
            {
                lstAcciones = await _context.mob_rsublineas.Where(x => x.rsb_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rsublineasE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

        public async Task<List<mob_rprodpuntoE>> GetProdPuntosLis(string cmpy)
        {
            List<mob_rprodpuntoE> lstAcciones = null;
            try
            {
                lstAcciones = await _context.mob_rprodpuntos.Where(x => x.rcp_cmpy == cmpy).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rprodpuntoE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }


        public async Task<List<mob_rsublineasE>> GetListSubLinesByCodUsu(string cmpy, string codUsu)
        {

            List<mob_rsublineasE> lstProductos = null;
            try
            {
                string sqlQuery = "Select DISTINCT rsb_cmpy, rsb_linea, rsb_subl, rsb_descr " +
                                  "FROM mob_rsublineas rsublineas,mob_rproductos rproductos,mob_rprodvendedor rprodvendedor " +
                                  "WHERE  rsublineas.rsb_linea=rproductos.rpr_linea AND rsublineas.rsb_subl=rproductos.rpr_subl AND rproductos.rpr_id=rprodvendedor.rprodven_id AND rsb_cmpy='" + cmpy + "' AND rprodven_codusu='" + codUsu + "'";
                lstProductos = await _context.mob_rsublineas.FromSqlRaw(sqlQuery).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstProductos), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstProductos = JsonSerializer.Deserialize<List<mob_rsublineasE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstProductos;
        }

        public async Task<List<mob_prodganchoE>> GetProdGancho(string cmpy, string reg, string teri, string cust, string succli)
        {
            List<mob_prodganchoE> lstProdGancho = null;
            try
            {
                lstProdGancho = await _context.mob_prodgancho.Where(x => x.pgan_cmpy == cmpy && x.pgan_reg == reg && x.pgan_teri == teri && x.pgan_cust == cust && x.pgan_succli == succli).ToListAsync();
                
                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstProdGancho), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstProdGancho = JsonSerializer.Deserialize<List<mob_prodganchoE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstProdGancho;
        }
        /// <summary>
        /// Guarda la URL de la foto del producto.
        /// </summary>
        /// <remarks>
        /// Es la unica escritura del modulo: el resto de ProductosQueries es de
        /// solo lectura. Hace falta porque al cargar la foto de un producto que
        /// no tenia ninguna, el archivo queda en el servidor de imagenes pero el
        /// catalogo no lo muestra hasta que rpr_urlimagen apunte a el.
        ///
        /// El codigo se compara con RTRIM: rpr_id es un campo de ancho fijo y
        /// llega con relleno segun de donde venga.
        /// </remarks>
        public int UpdateImagen(string cmpy, string id, string url)
        {
            try
            {
                var commandText =
                    "UPDATE mob_rproductos SET rpr_urlimagen = @url " +
                    "WHERE rpr_cmpy = @cmpy AND RTRIM(rpr_id) = @id";

                return _context.Database.ExecuteSqlRaw(commandText,
                    new DB2Parameter("url", url ?? string.Empty),
                    new DB2Parameter("cmpy", (cmpy ?? string.Empty).Trim()),
                    new DB2Parameter("id", (id ?? string.Empty).Trim()));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
