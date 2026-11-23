using IBM.Data.Db2;
using Microsoft.EntityFrameworkCore;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Migracion.Service.Bonificados
{
    /// <summary>
    /// SQL parametrizado sobre la conexión del DbContext. Las tablas mob_rbon*
    /// no están mapeadas en EF a propósito: todas las sentencias van con
    /// marcadores "?" (nada se concatena) y son SQL estándar, sin funciones
    /// propias de Informix ni de DB2.
    /// </summary>
    internal sealed class BonificadosSql
    {
        private readonly DbConnection _conexion;
        private DbTransaction? _transaccion;

        private BonificadosSql(DbConnection conexion) => _conexion = conexion;

        public static async Task<BonificadosSql> AbrirAsync(ApplicationDbContext contexto)
        {
            var conexion = contexto.Database.GetDbConnection();
            if (conexion.State != ConnectionState.Open) await conexion.OpenAsync();
            return new BonificadosSql(conexion);
        }

        public void Iniciar() => _transaccion = _conexion.BeginTransaction();

        public void Confirmar()
        {
            _transaccion?.Commit();
            _transaccion?.Dispose();
            _transaccion = null;
        }

        public void Deshacer()
        {
            try { _transaccion?.Rollback(); } catch { /* la conexión ya pudo caerse */ }
            _transaccion?.Dispose();
            _transaccion = null;
        }

        /// <summary>Filas con los nombres de columna en minúscula y los CHAR ya recortados.</summary>
        public async Task<List<Fila>> ConsultarAsync(string sql, params object?[] parametros)
        {
            using var comando = Crear(sql, parametros);
            using var lector = await comando.ExecuteReaderAsync();
            var filas = new List<Fila>();
            while (await lector.ReadAsync())
            {
                var valores = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < lector.FieldCount; i++)
                {
                    var valor = lector.IsDBNull(i) ? null : lector.GetValue(i);
                    valores[lector.GetName(i)] = valor is string texto ? texto.Trim() : valor;
                }
                filas.Add(new Fila(valores));
            }
            return filas;
        }

        public async Task<object?> EscalarAsync(string sql, params object?[] parametros)
        {
            using var comando = Crear(sql, parametros);
            var valor = await comando.ExecuteScalarAsync();
            return valor is DBNull ? null : valor;
        }

        public async Task<int> EjecutarAsync(string sql, params object?[] parametros)
        {
            using var comando = Crear(sql, parametros);
            return await comando.ExecuteNonQueryAsync();
        }

        public async Task<int> ContarAsync(string sql, params object?[] parametros) =>
            Convert.ToInt32(await EscalarAsync(sql, parametros) ?? 0, CultureInfo.InvariantCulture);

        /// <summary>"?, ?, ?" para un IN con tantos marcadores como valores.</summary>
        public static string Marcadores(int cantidad) => string.Join(", ", Enumerable.Repeat("?", cantidad));

        private DbCommand Crear(string sql, object?[] parametros)
        {
            var comando = _conexion.CreateCommand();
            comando.CommandText = sql;
            comando.Transaction = _transaccion;
            for (var i = 0; i < parametros.Length; i++)
            {
                comando.Parameters.Add(new DB2Parameter($"p{i}", parametros[i] ?? DBNull.Value));
            }
            return comando;
        }
    }

    internal sealed class Fila
    {
        private readonly Dictionary<string, object?> _valores;

        public Fila(Dictionary<string, object?> valores) => _valores = valores;

        /// <summary>Todas las columnas tal como llegaron; lo usa la copia genérica al duplicar.</summary>
        public IReadOnlyDictionary<string, object?> Valores => _valores;

        private object? Valor(string columna) => _valores.TryGetValue(columna, out var v) ? v : null;

        public string? Texto(string columna)
        {
            var valor = Valor(columna);
            if (valor == null) return null;
            var texto = Convert.ToString(valor, CultureInfo.InvariantCulture)?.Trim();
            return string.IsNullOrEmpty(texto) ? null : texto;
        }

        public string TextoO(string columna, string porDefecto = "") => Texto(columna) ?? porDefecto;

        public decimal? Decimal(string columna)
        {
            var valor = Valor(columna);
            return valor == null ? null : Convert.ToDecimal(valor, CultureInfo.InvariantCulture);
        }

        public int? Entero(string columna)
        {
            var valor = Decimal(columna);
            return valor == null ? null : (int)valor.Value;
        }

        public DateTime? Fecha(string columna)
        {
            var valor = Valor(columna);
            return valor switch
            {
                null => null,
                DateTime fecha => fecha,
                _ => DateTime.TryParse(Convert.ToString(valor, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var leida) ? leida : null,
            };
        }

        public bool SiNo(string columna) => string.Equals(Texto(columna), "S", StringComparison.OrdinalIgnoreCase);
    }
}
