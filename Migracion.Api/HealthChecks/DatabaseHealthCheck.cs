using Microsoft.Extensions.Diagnostics.HealthChecks;
using Migracion.Persistence.DataBase;

namespace Migracion.Api.HealthChecks
{
    /// <summary>
    /// Verifica que el API pueda conectarse a la base de datos.
    /// Expuesto en GET /health: responde 200 "Healthy" o 503 "Unhealthy",
    /// para conectarlo a un monitor externo (UptimeRobot, Zabbix, etc.)
    /// y detectar caídas como la del 30 jun - 2 jul de forma temprana.
    /// </summary>
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly ApplicationDbContext _context;

        public DatabaseHealthCheck(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
                return canConnect
                    ? HealthCheckResult.Healthy("Base de datos accesible")
                    : HealthCheckResult.Unhealthy("No se pudo conectar a la base de datos");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Error consultando la base de datos", ex);
            }
        }
    }
}
