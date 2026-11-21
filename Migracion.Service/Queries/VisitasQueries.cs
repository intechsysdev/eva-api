using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Migracion.Models;
using Migracion.Models.Dtos;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Migracion.Service.Queries
{
    public interface IVisitasQueries
    {
        Task<List<mob_rvisitaE>> GetList(string codUsu, string fecVist, string fecvisFin);
        Task<List<mob_rvisitsellerDto>> GetListCompuesto(string[] codUsu, string codcust, string fecVist, string fecvisFin, string custFiltro = null, string succliFiltro = null);
        Task<List<mob_rvisitSummaryDto>> GetListSummary(string fecVist, string fecvisFin);
        Task<List<mob_rvisitsellerGroupDto>> GetListGroup(string[] codUsu, string codcust, string fecVist, string fecvisFin, string custFiltro = null, string succliFiltro = null);
        Task<List<mob_rvisitsellerGroupDto>> GetListGroupRutas(string[] codUsu, string fecVist, string fecvisFin);
        int Insert(mob_rvisitaE item);
    } 
    public class VisitasQueries : IVisitasQueries
    {
        private readonly ApplicationDbContext _context;
        public VisitasQueries(
          ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Convierte un string de hora en formato 12 horas (con AM/PM) o 24 horas a TimeSpan
        /// </summary>
        private TimeSpan ParseTimeToTimeSpan(string timeString)
        {
            if (string.IsNullOrWhiteSpace(timeString))
                return TimeSpan.Zero;

            timeString = timeString.Trim();

            // Primero intentar parsear como TimeSpan directo (formato 24 horas como "14:08:00" o "14:08")
            if (TimeSpan.TryParse(timeString, out TimeSpan result))
                return result;

            // Si falla, intentar parsear como formato de 12 horas con AM/PM
            string[] formats = { "h:mm tt", "hh:mm tt", "h:mm:ss tt", "hh:mm:ss tt", "h:mmtt", "hh:mmtt" };
            
            if (DateTime.TryParseExact(timeString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime.TimeOfDay;
            }

            // Si todo falla, retornar TimeSpan.Zero
            return TimeSpan.Zero;
        }

        public async Task<List<mob_rvisitaE>> GetList(string codUsu, string fecVist, string fecvisFin)
        {
            List<mob_rvisitaE> lstAcciones = null;
            try
            {

                string sqlQuery = "Select * FROM mob_rvisita " +
                   "WHERE rvis_codusu='" + codUsu + "' AND rvis_fecvis BETWEEN  '" + fecVist + "' AND '" + fecvisFin + "'";
                lstAcciones = await _context.mob_rvisita.FromSqlRaw(sqlQuery).ToListAsync();

                // lstAcciones = _context.mob_rvisita.Where(x => x.rvis_codusu == codUsu && x.rvis_fecvis BEETWEN fecVist && fecvisFin).ToList();
                lstAcciones.ForEach(x =>
                {
                    x.rvis_fecvis = Convert.ToDateTime(x.rvis_fecvis).ToString("MM/dd/yyyy");
                    x.rvis_fecprovis = Convert.ToDateTime(x.rvis_fecprovis).ToString("MM/dd/yyyy");
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rvisitaE>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lstAcciones;
        }

        public int Insert(mob_rvisitaE item)
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

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = "Select * FROM mob_rvisita WHERE rvis_cmpy = @p_cmpy AND rvis_codusu = @p_codusu AND rvis_consec = @p_consec AND rvis_fecvis = @p_fecvis";
                                command.Parameters.Add(new DB2Parameter("p_cmpy", item.rvis_cmpy));
                                command.Parameters.Add(new DB2Parameter("p_codusu", item.rvis_codusu));
                                command.Parameters.Add(new DB2Parameter("p_consec", item.rvis_consec));
                                command.Parameters.Add(new DB2Parameter("p_fecvis", item.rvis_fecvis));

                                using (var resultCommand = command.ExecuteReader())
                                {
                                    bool hasRows = resultCommand.Read();
                                    resultCommand.Close();

                                    if (!hasRows)
                                    {
                                        using (var insertCommand = connection.CreateCommand())
                                        {
                                            insertCommand.Transaction = transaction;
                                            insertCommand.CommandText = "Insert into mob_rvisita (rvis_cmpy,rvis_consec,rvis_cust,rvis_succli,rvis_codusu,rvis_coddep,rvis_coddepaso,rvis_codcon,rvis_codmedvis,rvis_codmotncom,rvis_codmotvis,rvis_codaccaseg,rvis_coddoc,rvis_consdoc,rvis_fecvis,rvis_horinivis,rvis_horfinvis,rvis_fecprovis,rvis_horprovis,rvis_obsvis,rvis_latitud,rvis_longitud,rvis_date,rvis_estadoupd,rvis_time,rvis_enter) values (@rvis_cmpy,@rvis_consec,@rvis_cust,@rvis_succli,@rvis_codusu,@rvis_coddep,@rvis_coddepaso,@rvis_codcon,@rvis_codmedvis,@rvis_codmotncom,@rvis_codmotvis,@rvis_codaccaseg,@rvis_coddoc,@rvis_consdoc,@rvis_fecvis,@rvis_horinivis,@rvis_horfinvis,@rvis_fecprovis,@rvis_horprovis,@rvis_obsvis,@rvis_latitud,@rvis_longitud,@rvis_date,@rvis_estadoupd,@rvis_time,@rvis_enter)";
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_cmpy", item.rvis_cmpy));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_consec", item.rvis_consec));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_cust", item.rvis_cust));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_succli", item.rvis_succli));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_codusu", item.rvis_codusu));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_coddep", item.rvis_coddep));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_coddepaso", item.rvis_coddepaso));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_codcon", item.rvis_codcon));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_codmedvis", item.rvis_codmedvis));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_codmotncom", item.rvis_codmotncom));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_codmotvis", item.rvis_codmotvis));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_codaccaseg", item.rvis_codaccaseg));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_coddoc", item.rvis_coddoc));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_consdoc", item.rvis_consdoc));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_fecvis", item.rvis_fecvis));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_horinivis", item.rvis_horinivis));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_horfinvis", item.rvis_horfinvis));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_fecprovis", item.rvis_fecprovis));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_horprovis", item.rvis_horprovis));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_obsvis", item.rvis_obsvis));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_latitud", item.rvis_latitud));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_longitud", item.rvis_longitud));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_date", item.rvis_date));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_estadoupd", item.rvis_estadoupd));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_time", item.rvis_time));
                                            insertCommand.Parameters.Add(new DB2Parameter("rvis_enter", item.rvis_enter));
                                            result = insertCommand.ExecuteNonQuery();
                                        }
                                    }
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
                    throw new Exception($"Error insertando visita después de {attempt + 1} intentos: {ex.Message}", ex);
                }
            }

            return result;
        }

        public async Task<List<mob_rvisitsellerDto>> GetListCompuesto(string[] codUsu, string codcust, string fecVist, string fecvisFin, string custFiltro = null, string succliFiltro = null)
        {
            List<mob_rvisitsellerDto> lstAcciones = null;
            try
            {
                string nombreTable = "mob_rcustmain";
                switch (codcust)
                {
                    case "1":
                        nombreTable = "mob_rcustrans";
                        break;
                    case "2":
                        nombreTable = "mob_rcustmain";
                        break;
                    case "3":
                        nombreTable = "mob_rcustases";
                        break;
                    default:
                        nombreTable = "mob_rcustmain";
                        break;
                }

                string stringJoin = "";
                if(codUsu.Length > 0)
                {
                    foreach (var item in codUsu)
                    {
                        stringJoin +=$"'{item}',";
                    }
                    stringJoin = stringJoin.Substring(0, stringJoin.Length - 1);
                }
               
                string sqlQuery = "SELECT mob_rvisita.rvis_cmpy, mob_rvisita.rvis_consec,mob_rvisita.rvis_codusu,mob_rvisita.rvis_coddep, mob_rvisita.rvis_coddepaso,mob_rvisita.rvis_codcon, mob_rvisita.rvis_codmedvis, mob_rmediovisita.rmedio_codmedvis, mob_rmediovisita.rmedio_desmedvis, " +
                "mob_rvisita.rvis_codmotncom,mob_motivosnocompra.movcod,mob_motivosnocompra.movdesc, mob_rvisita.rvis_codmotvis, mob_motivosvisita.motcod,mob_motivosvisita.motdes,mob_rvisita.rvis_codaccaseg, mob_acciones.acctip,mob_acciones.accdesc, mob_rvisita.rvis_coddoc,mob_docvisita.docvis_desc, " +
                "mob_rvisita.rvis_consdoc, mob_rvisita.rvis_fecvis,mob_rvisita.rvis_horinivis, mob_rvisita.rvis_horfinvis, mob_rvisita.rvis_fecprovis, mob_rvisita.rvis_horprovis,mob_rvisita.rvis_obsvis, mob_rvisita.rvis_urlimagen, " +
                "mob_rvisita.rvis_enter, mob_rvisita.rvis_latitud,mob_rvisita.rvis_longitud, mob_rvisita.rvis_date,mob_rvisita.rvis_time,mob_rusuarios.rusu_reg,region.reg_desc,mob_rusuarios.rusu_teri,territor.ter_desc,mob_rcustmain.rctt_reg,mob_rcustmain.rctt_cust, mob_rcustmain.rctt_tipdoc, " +
                "mob_rcustmain.rctt_resal, mob_rcustmain.rctt_succli,mob_rcustmain.rctt_razsoc, mob_rcustmain.rctt_nomcli,mob_rcustmain.rctt_addr, mob_rcustmain.rctt_tele,mob_rcustmain.rctt_celular, mob_rcustmain.rctt_fax, " +
                "mob_rcustmain.rctt_dist,mob_district.dis_desc,mob_rcustmain.rctt_sic,mob_rcustmain.rctt_codusu,mob_rusuarios.rusu_nombre,mob_rusuarios.rusu_codper, mob_rcustmain.rctt_ruta,mob_rcustmain.rctt_sec, mob_rcustmain.rctt_clinvo,mob_rcustmain.rctt_date, mob_rcustmain.rctt_estrato, " +
                "mob_rcustmain.rctt_latitud,mob_rcustmain.rctt_longitud,mob_rcustmain.rctt_frevisita,mob_rcustmain.rctt_horsugvis,mob_rcustmain.rctt_codabc, mob_rcustmain.rctt_type,mob_rcusttype.cu_desc,mob_rcustmain.rctt_zip, mob_rcustmain.rctt_zip4, " +
                "mob_rcustmain.rctt_vot, mob_rcustmain.rctt_limit,mob_rcustmain.rctt_level, mob_rcustmain.rctt_bal,mob_rcustmain.rctt_cont, mob_rcustmain.rctt_feccum,mob_rcustmain.rctt_pedmini,mob_rcustmain.rctt_diasent, mob_rcustmain.rctt_tax, " +
                "mob_rcustmain.rctt_nomcan, mob_rcustmain.rctt_schrg,mob_rcustmain.rctt_diavisita, mob_rcustmain.rctt_mail,mob_rcustmain.rctt_mppago, mob_rcustmain.rctt_vpmin,mob_rcustmain.rctt_urlimagen,mob_rcustmain.rctt_zone, '' tiempoVis, mob_district.dis_dist  " +
                " FROM mob_rvisita  " +
                "LEFT JOIN " + nombreTable  + " mob_rcustmain ON mob_rcustmain.rctt_cmpy=mob_rvisita.rvis_cmpy AND mob_rcustmain.rctt_cust=mob_rvisita.rvis_cust AND mob_rcustmain.rctt_succli=mob_rvisita.rvis_succli  " +
                "LEFT JOIN mob_rmediovisita mob_rmediovisita ON mob_rvisita.rvis_codmedvis=mob_rmediovisita.rmedio_codmedvis " +
                "LEFT JOIN mob_motivis mob_motivosvisita ON mob_rvisita.rvis_codmotvis=mob_motivosvisita.motcod  " +
                "LEFT JOIN mob_acciones mob_acciones ON rvis_codaccaseg=mob_acciones.acctip  " +
                "LEFT JOIN mob_motivos mob_motivosnocompra ON mob_rvisita.rvis_codmotncom=mob_motivosnocompra.movcod  " +
                "LEFT JOIN mob_rusuarios mob_rusuarios ON mob_rvisita.rvis_enter=mob_rusuarios.rusu_codusu " +
                "LEFT JOIN region ON region.reg_regi = mob_rusuarios.rusu_reg AND region.reg_cmpy = mob_rusuarios.rusu_cmpy " +
                "LEFT JOIN territor ON territor.ter_teri = mob_rusuarios.rusu_teri AND territor.ter_cmpy = mob_rusuarios.rusu_cmpy " +
                "LEFT JOIN mob_docvisita mob_docvisita ON mob_rvisita.rvis_coddoc=mob_docvisita.docvis_coddoc " +
                "LEFT JOIN mob_district mob_district ON mob_rcustmain.rctt_dist=mob_district.dis_dist " +
                "LEFT JOIN mob_rcusttype mob_rcusttype ON mob_rcustmain.rctt_type=mob_rcusttype.cu_type	 " +
                "WHERE  mob_rvisita.rvis_fecvis between '" + fecVist + "' and '" + fecvisFin + "' AND mob_rvisita.rvis_enter in (" + stringJoin + ")";

                /*
                 * Filtro por cliente. No existia: al bajar de Efectuadas al detalle se
                 * mandaba el codigo en codCust, que aqui solo elige la tabla de clientes,
                 * asi que la consulta devolvia la jornada completa del funcionario.
                 * Va parametrizado (el resto de la consulta es anterior y concatena).
                 */
                var filtros = new List<DB2Parameter>();
                if (!string.IsNullOrWhiteSpace(custFiltro))
                {
                    sqlQuery += " AND rtrim(mob_rvisita.rvis_cust) = @custFiltro";
                    filtros.Add(new DB2Parameter("@custFiltro", custFiltro.Trim()));
                }
                /*
                 * "0" y vacio son el mismo caso (cliente sin sucursales) y no se guardan
                 * igual en todas las filas de mob_rvisita: filtrar por ese valor dejaria
                 * la consulta vacia. Solo se filtra cuando hay una sucursal de verdad.
                 */
                if (!string.IsNullOrWhiteSpace(succliFiltro) && succliFiltro.Trim() != "0")
                {
                    sqlQuery += " AND rtrim(mob_rvisita.rvis_succli) = @succliFiltro";
                    filtros.Add(new DB2Parameter("@succliFiltro", succliFiltro.Trim()));
                }

                sqlQuery += " ORDER BY mob_rvisita.rvis_fecvis,mob_rvisita.rvis_horinivis";
                lstAcciones = await _context.mob_rvisitsellerDtos
                    .FromSqlRaw(sqlQuery, filtros.ToArray()).ToListAsync();

                // lstAcciones = _context.mob_rvisita.Where(x => x.rvis_codusu == codUsu && x.rvis_fecvis BEETWEN fecVist && fecvisFin).ToList();
                lstAcciones.ForEach(x =>
                {
                    x.rvis_fecvis = Convert.ToDateTime(x.rvis_fecvis).ToString("MM/dd/yyyy");
                    x.rvis_fecprovis = Convert.ToDateTime(x.rvis_fecprovis).ToString("MM/dd/yyyy");
                    
                    try
                    {
                        var horIni = ParseTimeToTimeSpan(x.rvis_horinivis);
                        var horFin = ParseTimeToTimeSpan(x.rvis_horfinvis);
                        x.tiempoVis = Convert.ToString(horFin - horIni);
                    }
                    catch
                    {
                        x.tiempoVis = "00:00:00";
                    }
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rvisitsellerDto>>(responseJson);

            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            
            return lstAcciones;
        }


        public async Task<List<mob_rvisitsellerGroupDto>> GetListGroup(string[] codUsu, string codcust, string fecVist, string fecvisFin, string custFiltro = null, string succliFiltro = null)
        {
            // 1. Validar inputs temprano (fail fast)
            if (codUsu == null || codUsu.Length == 0)
                throw new ArgumentException("Debe proporcionar al menos un código de usuario.", nameof(codUsu));

            if (string.IsNullOrWhiteSpace(fecVist) || string.IsNullOrWhiteSpace(fecvisFin))
                throw new ArgumentException("Las fechas de visita son requeridas.");

            // 2. Tabla dinámica con whitelist (previene SQL Injection)
            string nombreTable = codcust switch
            {
                "1" => "mob_rcustrans",
                "3" => "mob_rcustases",
                _ => "mob_rcustmain"
            };

            // 3. Generar parámetros individuales para el IN (previene SQL Injection)
            var paramNames = new List<string>();
            var parameters = new List<DB2Parameter>(); // o SqlParameter según tu DB

            for (int i = 0; i < codUsu.Length; i++)
            {
                var paramName = $"@codUsu{i}";
                paramNames.Add(paramName);
                parameters.Add(new DB2Parameter(paramName, codUsu[i]));
            }

            parameters.Add(new DB2Parameter("@fecInicio", fecVist));
            parameters.Add(new DB2Parameter("@fecFin", fecvisFin));

            string inClause = string.Join(", ", paramNames);

            // Filtro por cliente: mismo caso que en GetListCompuesto, el campo
            // "Cliente" de la pantalla no llegaba a la consulta.
            string filtroCliente = "";
            if (!string.IsNullOrWhiteSpace(custFiltro))
            {
                filtroCliente += " AND rtrim(v.rvis_cust) = @custFiltro";
                parameters.Add(new DB2Parameter("@custFiltro", custFiltro.Trim()));
            }
            // Mismo criterio que en GetListCompuesto: "0" y vacio no se filtran.
            if (!string.IsNullOrWhiteSpace(succliFiltro) && succliFiltro.Trim() != "0")
            {
                filtroCliente += " AND rtrim(v.rvis_succli) = @succliFiltro";
                parameters.Add(new DB2Parameter("@succliFiltro", succliFiltro.Trim()));
            }

            // 4. Query limpio, legible y parametrizado
            string sqlQuery = $@"
        SELECT
            v.rvis_cmpy,
            v.rvis_codusu,
            v.rvis_cust,
            v.rvis_succli               AS rctt_succli,
            v.rvis_fecvis,
            MIN(v.rvis_horinivis)       AS rvis_horinivis,
            MAX(v.rvis_horfinvis)       AS rvis_horfinvis,
            v.rvis_enter,
            MAX(v.rvis_latitud)         AS rvis_latitud,
            MIN(v.rvis_longitud)        AS rvis_longitud,
            u.rusu_reg,
            r.reg_desc,
            u.rusu_teri,
            t.ter_desc,
            c.rctt_tipdoc,
            c.rctt_razsoc,
            c.rctt_nomcli,
            c.rctt_addr,
            c.rctt_tele,
            c.rctt_celular,
            c.rctt_fax,
            c.rctt_dist,
            d.dis_desc,
            c.rctt_sic,
            u.rusu_nombre,
            u.rusu_codper,
            c.rctt_ruta,
            c.rctt_sec,
            c.rctt_estrato,
            c.rctt_latitud,
            c.rctt_longitud,
            c.rctt_codabc,
            c.rctt_type,
            ct.cu_desc,
            c.rctt_limit,
            c.rctt_level,
            c.rctt_cont,
            c.rctt_feccum,
            c.rctt_pedmini,
            c.rctt_diasent,
            c.rctt_tax,
            c.rctt_diavisita,
            c.rctt_mail,
            c.rctt_mppago,
            c.rctt_vpmin,
            c.rctt_urlimagen,
            c.rctt_zone,
            ''                          AS tiempoVis,
            d.dis_dist
        FROM mob_rvisita v
        LEFT JOIN {nombreTable} c
               ON c.rctt_cmpy    = v.rvis_cmpy
              AND c.rctt_cust    = v.rvis_cust
              AND c.rctt_succli  = v.rvis_succli
        LEFT JOIN mob_rusuarios u
               ON v.rvis_enter   = u.rusu_codusu
        LEFT JOIN region r
               ON r.reg_regi     = u.rusu_reg
              AND r.reg_cmpy     = u.rusu_cmpy
        LEFT JOIN territor t
               ON t.ter_teri     = u.rusu_teri
              AND t.ter_cmpy     = u.rusu_cmpy
        LEFT JOIN mob_district d
               ON c.rctt_dist    = d.dis_dist
        LEFT JOIN mob_rcusttype ct
               ON c.rctt_type    = ct.cu_type
        WHERE v.rvis_fecvis BETWEEN @fecInicio AND @fecFin
          AND v.rvis_enter IN ({inClause})
          {filtroCliente}
        GROUP BY
            v.rvis_cmpy,       v.rvis_codusu,      v.rvis_cust,
            v.rvis_succli,     v.rvis_fecvis,       v.rvis_enter,
            u.rusu_reg,        r.reg_desc,          u.rusu_teri,
            t.ter_desc,        c.rctt_tipdoc,       c.rctt_razsoc,
            c.rctt_nomcli,     c.rctt_addr,         c.rctt_tele,
            c.rctt_celular,    c.rctt_fax,          c.rctt_dist,
            d.dis_desc,        c.rctt_sic,          u.rusu_nombre,
            u.rusu_codper,     c.rctt_ruta,         c.rctt_sec,
            c.rctt_estrato,    c.rctt_latitud,      c.rctt_longitud,
            c.rctt_codabc,     c.rctt_type,         ct.cu_desc,
            c.rctt_limit,      c.rctt_level,        c.rctt_cont,
            c.rctt_feccum,     c.rctt_pedmini,      c.rctt_diasent,
            c.rctt_tax,        c.rctt_diavisita,    c.rctt_mail,
            c.rctt_mppago,     c.rctt_vpmin,        c.rctt_urlimagen,
            c.rctt_zone,       d.dis_dist
        ORDER BY v.rvis_fecvis, MIN(v.rvis_horinivis)";

            // 5. Ejecutar con parámetros
            var lstAcciones = await _context.mob_rvisitsellerGroupDtos
                .FromSqlRaw(sqlQuery, parameters.ToArray())
                .AsNoTracking()                 // solo lectura → mejor rendimiento
                .ToListAsync();

            // 6. Post-procesamiento simplificado
            foreach (var x in lstAcciones)
            {
                if (DateTime.TryParse(x.rvis_fecvis, out var fecha))
                    x.rvis_fecvis = fecha.ToString("MM/dd/yyyy");

                try
                {
                    var horIni = ParseTimeToTimeSpan(x.rvis_horinivis);
                    var horFin = ParseTimeToTimeSpan(x.rvis_horfinvis);
                    x.tiempoVis = (horFin - horIni).ToString();
                }
                catch
                {
                    x.tiempoVis = "00:00:00";
                }

                // Reemplazar nulls directamente en vez de serializar/deserializar
                x.rctt_razsoc ??= "";
                x.rctt_nomcli ??= "";
                x.rctt_addr ??= "";
                x.rctt_tele ??= "";
                x.rctt_celular ??= "";
                x.rctt_fax ??= "";
                x.rctt_mail ??= "";
                // ... agregar todas las propiedades string que necesites limpiar
            }

            return lstAcciones;
        }
        public async Task<List<mob_rvisitsellerGroupDto>> GetListGroupRutas(string[] codUsu, string fecVist, string fecvisFin)
        {
            List<mob_rvisitsellerGroupDto> lstAcciones = null;
            try
            {
            
                string stringJoin = "";
                if (codUsu.Length > 0)
                {
                    foreach (var item in codUsu)
                    {
                        stringJoin += $"'{item}',";
                    }
                    stringJoin = stringJoin.Substring(0, stringJoin.Length - 1);
                }

                string sqlQuery = "SELECT mob_rvisita.rvis_cmpy,mob_rvisita.rvis_codusu, mob_rvisita.rvis_cust, mob_rvisita.rvis_succli rctt_succli, mob_rvisita.rvis_fecvis,min(mob_rvisita.rvis_horinivis) as rvis_horinivis , max(mob_rvisita.rvis_horfinvis) as rvis_horfinvis , mob_rvisita.rvis_enter,max(mob_rvisita.rvis_latitud" +
") as rvis_latitud,min(mob_rvisita.rvis_longitud) as rvis_longitud,mob_rusuarios.rusu_reg,region.reg_desc,mob_rusuarios.rusu_teri,territor.ter_desc, mob_rcustmain.rctt_tipdoc, mob_rcustmain.rctt_razsoc, mob_rcustmain.rctt_nomcli,mob_rcustmain.rctt_addr, mob_rcustmain.rctt_tele,mob_rcustmain.rctt_celular, mob_rcustmain.rctt_fax, mob_rcustmain.rctt_dist,mob_district.dis_desc,mob_rcustmain.rctt_sic,mob_rusuarios.rusu_nombre,mob_rusuarios.rusu_codper, mob_rcustmain.rctt_ruta,mob_rcustmain.rctt_sec, mob_rcustmain.rctt_estrato, mob_rcustmain.rctt_latitud,mob_rcustmain.rctt_longitud,mob_rcustmain.rctt_codabc, mob_rcustmain.rctt_type,mob_rcusttype.cu_desc, mob_rcustmain.rctt_limit,mob_rcustmain.rctt_level, mob_rcustmain.rctt_cont, mob_rcustmain.rctt_feccum,mob_rcustmain.rctt_pedmini,mob_rcustmain.rctt_diasent, mob_rcustmain.rctt_tax,  mob_rcustmain.rctt_diavisita, mob_rcustmain.rctt_mail,mob_rcustmain.rctt_mppago, mob_rcustmain.rctt_vpmin,mob_rcustmain.rctt_urlimagen,mob_rcustmain.rctt_zone, '' tiempoVis, mob_district.dis_dist " +
                 "FROM mob_rvisita  " +
                 "INNER JOIN  mob_rcustmain  mob_rcustmain ON mob_rcustmain.rctt_cmpy=mob_rvisita.rvis_cmpy AND mob_rcustmain.rctt_cust=mob_rvisita.rvis_cust AND mob_rcustmain.rctt_succli=mob_rvisita.rvis_succli  " +
                 "LEFT JOIN mob_rmediovisita mob_rmediovisita ON mob_rvisita.rvis_codmedvis=mob_rmediovisita.rmedio_codmedvis " +
                 "LEFT JOIN mob_motivis mob_motivosvisita ON mob_rvisita.rvis_codmotvis=mob_motivosvisita.motcod  " +
                 "LEFT JOIN mob_acciones mob_acciones ON rvis_codaccaseg=mob_acciones.acctip  " +
                 "LEFT JOIN mob_motivos mob_motivosnocompra ON mob_rvisita.rvis_codmotncom=mob_motivosnocompra.movcod  " +
                 "LEFT JOIN mob_rusuarios mob_rusuarios ON mob_rvisita.rvis_enter=mob_rusuarios.rusu_codusu " +
                 "LEFT JOIN region ON region.reg_regi = mob_rusuarios.rusu_reg AND region.reg_cmpy = mob_rusuarios.rusu_cmpy " +
                 "LEFT JOIN territor ON territor.ter_teri = mob_rusuarios.rusu_teri AND territor.ter_cmpy = mob_rusuarios.rusu_cmpy " +
                 "LEFT JOIN mob_docvisita mob_docvisita ON mob_rvisita.rvis_coddoc=mob_docvisita.docvis_coddoc " +
                 "LEFT JOIN mob_district mob_district ON mob_rcustmain.rctt_dist=mob_district.dis_dist " +
                 "LEFT JOIN mob_rcusttype mob_rcusttype ON mob_rcustmain.rctt_type=mob_rcusttype.cu_type	 " +
                 "WHERE  mob_rvisita.rvis_fecvis between '" + fecVist + "' and '" + fecvisFin + "' AND mob_rvisita.rvis_enter in (" + stringJoin + ") " +
                 " GROUP BY " +
                 " mob_rvisita.rvis_cmpy, mob_rvisita.rvis_codusu,mob_rvisita.rvis_cust, mob_rvisita.rvis_succli, mob_rvisita.rvis_fecvis, mob_rvisita.rvis_enter,mob_rusuarios.rusu_reg,region.reg_desc,mob_rusuarios.rusu_teri,territor.ter_desc, mob_rcustmain.rctt_tipdoc, mob_rcustmain.rctt_razsoc, mob_rcustmain.rctt_nomcli,mob_rcustmain.rctt_addr, mob_rcustmain.rctt_tele,mob_rcustmain.rctt_celular, mob_rcustmain.rctt_fax, mob_rcustmain.rctt_dist,mob_district.dis_desc,mob_rcustmain.rctt_sic,mob_rusuarios.rusu_nombre,mob_rusuarios.rusu_codper, mob_rcustmain.rctt_ruta,mob_rcustmain.rctt_sec, mob_rcustmain.rctt_estrato, mob_rcustmain.rctt_latitud,mob_rcustmain.rctt_longitud,mob_rcustmain.rctt_codabc, mob_rcustmain.rctt_type,mob_rcusttype.cu_desc, mob_rcustmain.rctt_limit,mob_rcustmain.rctt_level, mob_rcustmain.rctt_cont, mob_rcustmain.rctt_feccum,mob_rcustmain.rctt_pedmini,mob_rcustmain.rctt_diasent, mob_rcustmain.rctt_tax,  mob_rcustmain.rctt_diavisita, mob_rcustmain.rctt_mail,mob_rcustmain.rctt_mppago, mob_rcustmain.rctt_vpmin,mob_rcustmain.rctt_urlimagen,mob_rcustmain.rctt_zone, tiempoVis, mob_district.dis_dist " +
                 " ORDER BY mob_rvisita.rvis_fecvis,rvis_horinivis";
                lstAcciones = await _context.mob_rvisitsellerGroupDtos.FromSqlRaw(sqlQuery).ToListAsync();

                // lstAcciones = _context.mob_rvisita.Where(x => x.rvis_codusu == codUsu && x.rvis_fecvis BEETWEN fecVist && fecvisFin).ToList();
                lstAcciones.ForEach(x =>
                {
                    x.rvis_fecvis = Convert.ToDateTime(x.rvis_fecvis).ToString("MM/dd/yyyy");
                    
                    try
                    {
                        var horIni = ParseTimeToTimeSpan(x.rvis_horinivis.Trim());
                        var horFin = ParseTimeToTimeSpan(x.rvis_horfinvis.Trim());
                        x.tiempoVis = Convert.ToString(horFin - horIni);
                    }
                    catch
                    {
                        x.tiempoVis = "00:00:00";
                    }
                });

                var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
                lstAcciones = JsonSerializer.Deserialize<List<mob_rvisitsellerGroupDto>>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return lstAcciones;
        }



        public async Task<List<mob_rvisitSummaryDto>> GetListSummary(string fecVist, string fecvisFin)
        {
            List<mob_rvisitSummaryDto> lstAcciones = null;
            string sqlQuery = "SELECT mob_rvisita.rvis_cmpy,mob_rusuarios.rusu_nombre,mob_rusuarios.rusu_codper,mob_rperfiles.rper_desper,mob_rusuarios.rusu_reg,mob_rusuarios.rusu_teri,mob_rvisita.rvis_fecvis,min(mob_rvisita.rvis_horinivis) minrvis_horinivis, max(mob_rvisita.rvis_horfinvis)maxrvis_horinivis, mob_rvisita.rvis_enter, " +
                              // Visitas efectuadas = clientes distintos del dia. count(rvis_enter) contaba
                              // filas de mob_rvisita, que es una por movimiento: el numero no cuadraba con
                              // las filas de Efectuadas, que van una por cliente. Se deja tambien el conteo
                              // de movimientos, que es lo que ese count devolvia.
                              " count(distinct rtrim(mob_rvisita.rvis_cust) || '|' || rtrim(mob_rvisita.rvis_succli)) cantvisitas, " +
                              " count(mob_rvisita.rvis_enter) cantmovimientos " +
                              ", '' tiempoVis, region.reg_desc, territor.ter_desc " +
                              " FROM  mob_rvisita mob_rvisita, mob_rusuarios mob_rusuarios, mob_rperfiles mob_rperfiles, region, territor   " +
                              " WHERE  territor.ter_cmpy = mob_rusuarios.rusu_cmpy AND region.reg_cmpy = mob_rusuarios.rusu_cmpy and territor.ter_teri = mob_rusuarios.rusu_teri  AND region.reg_regi = mob_rusuarios.rusu_reg and mob_rvisita.rvis_cmpy = mob_rusuarios.rusu_cmpy and   mob_rvisita.rvis_cmpy = mob_rusuarios.rusu_cmpy AND mob_rvisita.rvis_enter = mob_rusuarios.rusu_codusu AND mob_rusuarios.rusu_codper = mob_rperfiles.rper_codper AND mob_rvisita.rvis_fecvis between '" + fecVist + "' and '" + fecvisFin + "' GROUP BY mob_rvisita.rvis_cmpy, " +
                              " mob_rusuarios.rusu_nombre,mob_rusuarios.rusu_codper,mob_rperfiles.rper_desper, " +
                              " mob_rusuarios.rusu_reg, mob_rusuarios.rusu_teri, " +
                              " mob_rvisita.rvis_fecvis, " +
                              " mob_rvisita.rvis_enter, region.reg_desc, territor.ter_desc ";
            lstAcciones = await _context.mob_rvisitSummaryDtos.FromSqlRaw(sqlQuery).ToListAsync();

            // lstAcciones = _context.mob_rvisita.Where(x => x.rvis_codusu == codUsu && x.rvis_fecvis BEETWEN fecVist && fecvisFin).ToList();
            lstAcciones.ForEach(x =>
            {
                x.rvis_fecvis = Convert.ToDateTime(x.rvis_fecvis).ToString("MM-dd-yyyy");
                
                try
                {
                    var horIni = ParseTimeToTimeSpan(x.minrvis_horinivis.Trim());
                    var horFin = ParseTimeToTimeSpan(x.maxrvis_horinivis.Trim());
                    x.tiempoVis = Convert.ToString(horFin - horIni);
                }
                catch
                {
                    x.tiempoVis = "00:00:00";
                }
            });

            var responseJson = (Regex.Replace(Regex.Replace(JsonSerializer.Serialize(lstAcciones), Constants.MATCHPATTERN, Constants.REPLACEMENTPATTERN), Constants.MATCHPATTERNNULLS, Constants.REPLACEMENTPATTERNNULLS));
            lstAcciones = JsonSerializer.Deserialize<List<mob_rvisitSummaryDto>>(responseJson);

            return lstAcciones;
        }
    }
}
