using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Migracion.Service.Bonificados
{
    internal sealed class BonAuditoria
    {
        public string Cmpy { get; init; } = "";
        public string? Regla { get; init; }
        public string? Campana { get; init; }
        public decimal? Num { get; init; }
        public string? Cust { get; init; }
        /// <summary>CREADA, MODIFICADA, DESACTIVADA, DUPLICADA, ELIMINADA, APLICADA, RECHAZADA, AGOTADA...</summary>
        public string Accion { get; init; } = "";
        /// <summary>V-xx que rechazó, cuando la acción es RECHAZADA.</summary>
        public string? Validacion { get; init; }
        public object? Anterior { get; init; }
        public object? Nuevo { get; init; }
        public string? Usuario { get; init; }
        public string? Observacion { get; init; }
    }

    /// <summary>
    /// Escribe en mob_rbonaudit. Va siempre fuera de la transacción del cambio
    /// y nunca lanza: la auditoría no debe tumbar algo que ya quedó guardado.
    /// </summary>
    internal static class BonificadosAuditoria
    {
        public static async Task RegistrarAsync(BonificadosSql sql, ILogger logger, BonAuditoria a)
        {
            try
            {
                var maximo = await sql.EscalarAsync("SELECT MAX(rba_consecutivo) FROM mob_rbonaudit WHERE rba_cmpy = ?", a.Cmpy);
                var siguiente = (maximo == null ? 0 : Convert.ToDecimal(maximo, CultureInfo.InvariantCulture)) + 1;
                await sql.EjecutarAsync(
                    "INSERT INTO mob_rbonaudit (rba_cmpy, rba_consecutivo, rba_regla, rba_campana, rba_num, rba_cust, rba_accion, " +
                    "rba_validacion, rba_valoranterior, rba_valornuevo, rba_codusu, rba_fecha, rba_obs) " +
                    "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                    a.Cmpy, siguiente, a.Regla, a.Campana, a.Num, a.Cust, Recortar(a.Accion, 20),
                    a.Validacion, a.Anterior == null ? null : JsonSerializer.Serialize(a.Anterior),
                    a.Nuevo == null ? null : JsonSerializer.Serialize(a.Nuevo),
                    Recortar(a.Usuario, 10), DateTime.Now, a.Observacion);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "No se pudo auditar {Accion} de bonificados (campaña {Campana}, regla {Regla}, pedido {Num}, compañía {Cmpy}).",
                    a.Accion, a.Campana, a.Regla, a.Num, a.Cmpy);
            }
        }

        private static string? Recortar(string? texto, int largo)
        {
            if (string.IsNullOrWhiteSpace(texto)) return null;
            var limpio = texto.Trim();
            return limpio.Length <= largo ? limpio : limpio.Substring(0, largo);
        }
    }
}
