using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Persistence.DataBase;

namespace Migracion.Service.Queries
{
    public interface IUsuariosQueries
    {
        Task<List<mob_rusuariosE>> GetList(string codusu, string passwd, string codper);
        Task<List<mob_rusuariosE>> GetListCmpy(string cmpy, string codusu, string codper);
        Task<List<mob_rusuariosE>> GetListAll(string cmpy, string codper);
        Task<mob_rusuariosE> GetCodUsuByCed(string cmpy, string ced);
        int Update(mob_rusuariosE usuario);
        

    }

    public class UsuariosQueries : IUsuariosQueries
    {
        private readonly ApplicationDbContext _context;
        public UsuariosQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<mob_rusuariosE>> GetList(string codusu, string passwd, string codper)
        {
            List<mob_rusuariosE> lstUsuarios = null;
            try
            {

                lstUsuarios = await _context.mob_rusuarios.Where(x => x.rusu_codusu == codusu && x.rusu_passwd == passwd && x.rusu_codper == codper).ToListAsync();
                lstUsuarios.ForEach(x =>
                {
                    x.rusu_date = Convert.ToDateTime(x.rusu_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstUsuarios), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstUsuarios = JsonSerializer.Deserialize<List<mob_rusuariosE>>(responseJson);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstUsuarios;
        }

        public async Task<List<mob_rusuariosE>> GetListCmpy(string cmpy, string codusu, string codper)
        {
            List<mob_rusuariosE> lstUsuarios = null;
            try
            {

                lstUsuarios = await _context.mob_rusuarios.Where(x => x.rusu_cmpy == cmpy && x.rusu_codusu == codusu && x.rusu_codper == codper).ToListAsync();
                lstUsuarios.ForEach(x =>
                {
                    x.rusu_date = Convert.ToDateTime(x.rusu_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstUsuarios), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstUsuarios = JsonSerializer.Deserialize<List<mob_rusuariosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstUsuarios;
        }


        public int Update(mob_rusuariosE usuario)
        {

            int result = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM mob_rusuarios " +
                                          "WHERE rusu_cmpy = '" + usuario.rusu_cmpy + "' AND rusu_codusu = '" + usuario.rusu_codusu + " AND rusu_passwd = '" + usuario.rusu_passwd + " AND rusu_codper='" + usuario.rusu_codper  +"'";

                    _context.Database.OpenConnection();
                    using (var resultCommand = command.ExecuteReader())
                    {
                        if (resultCommand.HasRows)
                        {
                            var commandText = "UPDATE mob_rusuarios SET rusu_passwd = @rusu_passwd, rusu_nombre = @rusu_nombre, rusu_correo = @rusu_correo, rusu_reg = @rusu_reg " +
                                              "WHERE rusu_cmpy = @rusu_cmpy AND rusu_codusu = @rusu_codusu AND rusu_passwd = @rusu_passwd AND rusu_codper=@rusu_codper";

                            result = _context.Database.ExecuteSqlRaw(commandText,
                                 new DB2Parameter("rusu_nombre", usuario.rusu_nombre),
                                 new DB2Parameter("rusu_correo", usuario.rusu_correo),
                                 new DB2Parameter("rusu_reg", usuario.rusu_reg),
                                 new DB2Parameter("rusu_cmpy", usuario.rusu_cmpy),
                                 new DB2Parameter("rusu_codusu", usuario.rusu_codusu),
                                 new DB2Parameter("rusu_passwd", usuario.rusu_passwd),
                                 new DB2Parameter("rusu_codper", usuario.rusu_codper)
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

        public async Task<List<mob_rusuariosE>> GetListAll(string cmpy, string codper)
        {
            List<mob_rusuariosE> lstUsuarios = null;
            try
            {

                if(codper == "%")
                    lstUsuarios = await  _context.mob_rusuarios.Where(x => x.rusu_cmpy == cmpy).ToListAsync();
                else
                    lstUsuarios = await _context.mob_rusuarios.Where(x => x.rusu_cmpy == cmpy && x.rusu_codper == codper).ToListAsync();

                lstUsuarios.ForEach(x =>
                {
                    x.rusu_date = Convert.ToDateTime(x.rusu_date).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstUsuarios), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstUsuarios = JsonSerializer.Deserialize<List<mob_rusuariosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstUsuarios;
        }

        public async Task<mob_rusuariosE> GetCodUsuByCed(string cmpy, string ced)
        {
            List<mob_rusuariosE> lstUsuarios = null;
            try
            {

                lstUsuarios = await _context.mob_rusuarios.Where(x => x.rusu_cmpy == cmpy && x.rusu_cedula == ced).ToListAsync();

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstUsuarios), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstUsuarios = JsonSerializer.Deserialize<List<mob_rusuariosE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstUsuarios.FirstOrDefault();
        }


    }
}
