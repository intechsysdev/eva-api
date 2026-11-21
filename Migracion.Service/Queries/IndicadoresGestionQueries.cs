using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Migracion.Models.Dtos;
using Migracion.Persistence.DataBase;
using Migracion.Service.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Migracion.Service.Queries
{
    public interface IIndicadoresGestionQueries
    {
        Task<IndicadoresGestionDto> GetTablero(string cmpy, int anio, int per, string teri, string reg);
    }

    /// <summary>
    /// Ejecuta los procedimientos de indicadores de gestión del vendedor. Son los
    /// mismos cálculos que alimentan BI4Web, instalados con igual nombre y firma en
    /// RE, DD, LA y KR: (p_company, p_year, p_per, p_teri, p_reg).
    /// </summary>
    public class IndicadoresGestionQueries : IIndicadoresGestionQueries
    {
        private static readonly (string Seccion, string Procedimiento)[] Secciones =
        {
            ("resumen", "sp_mob_ind_resumen"),
            ("ventas", "sp_mob_ind_ventas"),
            ("cobros", "sp_mob_ind_cobros"),
            ("efectividad", "sp_mob_ind_efectividad"),
            ("pedidos", "sp_mob_ind_pedidos"),
            ("clientes", "sp_mob_ind_clientes"),
        };

        private readonly ApplicationDbContext _context;

        public IndicadoresGestionQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IndicadoresGestionDto> GetTablero(string cmpy, int anio, int per, string teri, string reg)
        {
            var compania = CompanyRules.Normalize(cmpy);
            var resultado = new IndicadoresGestionDto
            {
                cmpy = compania,
                anio = anio,
                per = per,
                teri = (teri ?? string.Empty).Trim(),
                reg = (reg ?? string.Empty).Trim(),
            };

            foreach (var (seccion, procedimiento) in Secciones)
            {
                try
                {
                    resultado.secciones[seccion] = await Ejecutar(
                        procedimiento, compania, anio, per, resultado.teri, resultado.reg);
                }
                catch (Exception ex)
                {
                    // Se sigue con el resto: el tablero muestra lo que sí llegó.
                    resultado.secciones[seccion] = null;
                    resultado.errores[seccion] = $"{procedimiento}: {ex.Message}";
                }
            }

            // "Clientes recuperados" solo aplica para RE; en las demás compañías es 0
            // aunque el procedimiento llegara a devolver otra cosa.
            if (compania != CompanyRules.RECAMIER &&
                resultado.secciones.TryGetValue("clientes", out var clientes) &&
                clientes != null)
            {
                clientes["clientes_recuperados"] = 0m;
            }

            return resultado;
        }

        /// <summary>
        /// Primera fila del procedimiento como nombre de columna -> valor. Se lee
        /// por nombre y no por posición: si mañana se agrega una columna, no se
        /// corre el resto.
        /// </summary>
        private async Task<Dictionary<string, object?>?> Ejecutar(
            string procedimiento, string cmpy, int anio, int per, string teri, string reg)
        {
            using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    Dictionary<string, object?>? fila = null;

                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.Transaction = transaction.GetDbTransaction();
                        // El proveedor enlaza por posición: los parámetros van en el
                        // mismo orden que los marcadores.
                        command.CommandText =
                            $"EXECUTE PROCEDURE {procedimiento}(@p_company, @p_year, @p_per, @p_teri, @p_reg)";
                        command.Parameters.Add(new DB2Parameter("p_company", cmpy));
                        command.Parameters.Add(new DB2Parameter("p_year", anio));
                        command.Parameters.Add(new DB2Parameter("p_per", per));
                        command.Parameters.Add(new DB2Parameter("p_teri", teri));
                        command.Parameters.Add(new DB2Parameter("p_reg", reg));

                        await _context.Database.OpenConnectionAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                fila = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var nombre = reader.GetName(i)?.Trim().ToLowerInvariant();
                                    if (string.IsNullOrEmpty(nombre)) nombre = $"col{i}";
                                    var valor = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                    fila[nombre] = valor is string texto ? texto.Trim() : valor;
                                }
                            }
                        }
                    }

                    transaction.Commit();
                    return fila;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
