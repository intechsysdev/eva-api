using IBM.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Migracion.Api.Handlers;
using Migracion.Api.HealthChecks;
using Migracion.Api.Logging;
using Migracion.Api.Mcp;
using Migracion.Api.Middleware;
using Migracion.Api.Models;
using Migracion.Persistence.DataBase;
using Migracion.Service.Commands;
using Migracion.Service.Queries;
using Migracion.Service.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Log de errores a archivo (Logs/eva-api-yyyyMMdd.log) para análisis de
// causa raíz de incidentes, independiente del stdout log de IIS.
try
{
    var logDirectory = configuration["FileLogging:Directory"] ?? "Logs";
    if (!Path.IsPathRooted(logDirectory))
    {
        logDirectory = Path.Combine(builder.Environment.ContentRootPath, logDirectory);
    }
    var minLevel = Enum.TryParse<LogLevel>(configuration["FileLogging:MinLevel"], out var lvl)
        ? lvl
        : LogLevel.Warning;
    builder.Logging.AddProvider(new FileLoggerProvider(logDirectory, minLevel));
}
catch
{
    // Si no se puede crear la carpeta de logs, el API debe arrancar igual
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationDbContext>(
options => options.UseDb2(
                  configuration.GetConnectionString("DefaultConnection"), p => p.SetServerInfo(IBMDBServerType.AS400, IBMDBServerVersion.AS400_07_01)
              )
          );

builder.Services.Configure<PropertiesSettings>(opts => configuration.GetSection("PropertiesSettings").Bind(opts));
builder.Services.Configure<FileSettings>(opts => configuration.GetSection("FileSettings").Bind(opts));
builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

// Endpoint de salud (GET /health) para monitoreo externo y alerta temprana
builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database");

// Límite de tasa por IP: protege al backend de ráfagas de reintentos
// (durante el incidente de jun/jul 2026 los dispositivos reenviaban el mismo
// registro varias veces en segundos). El límite es holgado para no afectar
// una sincronización completa legítima.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = (context, cancellationToken) =>
    {
        context.HttpContext.Response.Headers["Retry-After"] = "60";
        return ValueTask.CompletedTask;
    };
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 600,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

builder.Services.AddTransient<IAccionesQueries, AccionesQueries>();
builder.Services.AddTransient<ICarteraQueries, CarteraQueries>();
builder.Services.AddTransient<IDepositosQueries, DepositosQueries>();
builder.Services.AddTransient<IDistrictQueries, DistrictQueries>();
builder.Services.AddTransient<IIndicaClienteQueries, IndicaClienteQueries>();
builder.Services.AddTransient<IIndicadoresQueries, IndicadoresQueries>();
builder.Services.AddTransient<IIndicaVendedorQueries, IndicaVendedorQueries>();
builder.Services.AddTransient<IMotivosQueries, MotivosQueries>();
builder.Services.AddTransient<IMotivisQueries, MotivisQueries>();
builder.Services.AddTransient<IBancoQueries, BancoQueries>();
builder.Services.AddTransient<IAsociadoQueries, AsociadoQueries>();
builder.Services.AddTransient<IRCheDetalleQueries, RCheDetalleQueries>();
builder.Services.AddTransient<IConPerClienteQueries, ConPerClienteQueries>();
builder.Services.AddTransient<IConsecutivosQueries, ConsecutivosQueries>();
builder.Services.AddTransient<IContactImageQueries, ContactImageQueries>();
builder.Services.AddTransient<IContactosQueries, ContactosQueries>();
builder.Services.AddTransient<ITiposDevolucionQueries, TiposDevolucionQueries>();
builder.Services.AddTransient<IClientesQueries, ClientesQueries>();
builder.Services.AddTransient<ITransferencistasQueries, TransferencistasQueries>();
builder.Services.AddTransient<ICanalesQueries, CanalesQueries>();
builder.Services.AddTransient<IPresupuestosQueries, PresupuestosQueries>();
builder.Services.AddTransient<IFabricantesQueries, FabricantesQueries>();
builder.Services.AddTransient<IFrecuenciaVisitaQueries, FrecuenciaVisitaQueries>();
builder.Services.AddTransient<IReferenciasQueries, ReferenciasQueries>();
builder.Services.AddTransient<IListasPreciosQueries, ListasPreciosQueries>();
builder.Services.AddTransient<ILineasProductosQueries, LineasProductosQueries>();
builder.Services.AddTransient<IMediosVisitasQueries, MediosVisitasQueries>();
builder.Services.AddTransient<IMenuPpalQueries, MenuPpalQueries>();
builder.Services.AddTransient<IPerfilesQueries, PerfilesQueries>();
builder.Services.AddTransient<IProductosQueries, ProductosQueries>();
builder.Services.AddTransient<IRutasQueries, RutasQueries>();
builder.Services.AddTransient<ISubMenuQueries, SubMenuQueries>();
builder.Services.AddTransient<ITiposImagenQueries, TiposImagenQueries>();
builder.Services.AddTransient<ITransferenciasQueries, TransferenciasQueries>();
builder.Services.AddTransient<IUsuariosQueries, UsuariosQueries>();
builder.Services.AddTransient<IVisitasQueries, VisitasQueries>();
builder.Services.AddTransient<ITiposDocumentosQueries, TiposDocumentosQueries>();
builder.Services.AddTransient<IPedidosQueries, PedidosQueries>();
builder.Services.AddTransient<IDevolucionesQueries, DevolucionesQueries>();
builder.Services.AddTransient<IRecibosCajasQueries, RecibosCajasQueries>();
builder.Services.AddTransient<IChequePosfechadoQueries, ChequePosfechadoQueries>();
builder.Services.AddTransient<INotasCreditosQueries, NotasCreditosQueries>();
builder.Services.AddTransient<IReglasProntopagoQueries, ReglasProntopagoQueries>();
builder.Services.AddTransient<IAbonosQueries, AbonosQueries>();
builder.Services.AddTransient<IConsignacionesQueries, ConsignacionesQueries>();
builder.Services.AddTransient<IInventariosQueries, InventariosQueries>();
builder.Services.AddTransient<IMercadosQueries, MercadosQueries>();
builder.Services.AddTransient<IMarcasQueries, MarcasQueries>();
builder.Services.AddTransient<IProductosPreciosQueries, ProductosPreciosQueries>();
builder.Services.AddTransient<INovedadesPreciosQueries, NovedadesPreciosQueries>();
builder.Services.AddTransient<ITiposMensajesQueries, TiposMensajesQueries>();
builder.Services.AddTransient<IMensajesQueries, MensajesQueries>();
builder.Services.AddTransient<IClientesAsesoresQueries, ClientesAsesoresQueries>();
builder.Services.AddTransient<ICuotasAsesoriasQueries, CuotasAsesoriasQueries>();
builder.Services.AddTransient<IActualizacionQueries, ActualizacionQueries>();
builder.Services.AddTransient<IProductosIvaQueries, ProductosIvaQueries>();
builder.Services.AddTransient<IRegionQueries, RegionQueries>();
builder.Services.AddTransient<ITerritorioQueries, TerritorioQueries>();
builder.Services.AddTransient<ITipoMovimientoQueries, TipoMovimientoQueries>();
builder.Services.AddTransient<ICuposCompromisoQueries, CuposCompromisoQueries>();
builder.Services.AddTransient<ILineasQueries, LineasQueries>();
builder.Services.AddTransient<ISubLineasQueries, SubLineasQueries>();
builder.Services.AddTransient<IEmpleadosQueries, EmpleadosQueries>();
builder.Services.AddTransient<IGruposCanalQueries, GruposCanalQueries>();
builder.Services.AddTransient<IExhibicionCommand, ExhibicionCommand>();
builder.Services.AddTransient<IExhibicionesQueries, ExhibicionesQueries>();
builder.Services.AddTransient<IMaterialPOPCommand, MaterialPOPCommand>();
builder.Services.AddTransient<IMaterialPOPQueries, MaterialPOPQueries>();
builder.Services.AddTransient<IMarcasPOPCommand, MarcasPOPCommand>();
builder.Services.AddTransient<IMarcasPOPQueries, MarcasPOPQueries>();
builder.Services.AddTransient<IMovimientoPOPCommand, MovimientoPOPCommand>();
builder.Services.AddTransient<IMovimientoPOPQueries, MovimientoPOPQueries>();
builder.Services.AddTransient<IDetallePOPCommand, DetallePOPCommand>();
builder.Services.AddTransient<IDetallePOPQueries, DetallePOPQueries>();
builder.Services.AddTransient<IMessageCommand, MessageCommand>();
builder.Services.AddTransient<IOptionConsentQueries, OptionConsentQueries>();
builder.Services.AddTransient<IDashboardQueries, DashboardQueries>();
builder.Services.AddTransient<IIndicadoresGestionQueries, IndicadoresGestionQueries>();
builder.Services.AddTransient<IWhatsappHelper, WhatsappHelper>();
builder.Services.AddTransient<IEmailHelper, EmailHelper>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "application/json"
    });
});

builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EVA API v2" });
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Auth Header"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                          Reference = new OpenApiReference
                          {
                           Type = ReferenceType.SecurityScheme,
                           Id = "basic"
                           }
                          },
                        new string[]{ }

                    }

                 });
});

// Servidor MCP (/mcp): consultas de solo lectura para modelos de IA. Ver Mcp/README.md
builder.AddEvaMcp();

var app = builder.Build();

// Debe ir de primero para capturar cualquier excepción del pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EVA.API V2"));
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseResponseCompression();
app.MapControllers();
app.MapHealthChecks("/health").DisableRateLimiting();
app.MapEvaMcp();

app.Run();
