using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models.Bonificados;
using Migracion.Service.Bonificados;

namespace Migracion.Api.Controllers
{
    /// <summary>
    /// Administración de bonificados para seguimiento-online: campañas, reglas
    /// y los catálogos del formulario. La lógica vive en BonificadosService.
    /// </summary>
    /// <remarks>
    /// Los errores de negocio salen como { "mensaje": "..." } con su código
    /// (400 datos inválidos, 404 no existe, 409 conflicto con datos en uso).
    /// </remarks>
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BonificadosController : ControllerBase
    {
        private readonly IBonificadosService _servicio;
        private readonly IBonificadosPedidoService _pedidos;

        public BonificadosController(IBonificadosService servicio, IBonificadosPedidoService pedidos)
        {
            _servicio = servicio;
            _pedidos = pedidos;
        }

        // ---- App: descarga y validación en línea ----

        /// <summary>Tablas espejo para la sincronización de la app (reglas vigentes del vendedor).</summary>
        [HttpGet("Sync/{cmpy}/{codusu}/{reg}/{teri}")]
        public Task<ActionResult<BonSyncDto>> Sync(string cmpy, string codusu, string reg, string teri) =>
            Ejecutar(() => _pedidos.SyncAsync(cmpy, codusu, reg, teri));

        /// <summary>Valida bonificados sin guardar nada (V-01..V-10 con datos frescos).</summary>
        [HttpPost("Validar")]
        public Task<ActionResult<BonResultadoDto>> Validar([FromBody] BonPedidoDto pedido) =>
            Ejecutar(() => _pedidos.ValidarAsync(pedido));

        // ---- Grupos mixtos ----

        [HttpGet("Grupos/{cmpy}")]
        public Task<ActionResult<List<BonGrupoEditarDto>>> Grupos(string cmpy) =>
            Ejecutar(() => _servicio.GruposAsync(cmpy));

        [HttpGet("Grupo/{cmpy}/{codigo}")]
        public Task<ActionResult<BonGrupoEditarDto>> Grupo(string cmpy, string codigo) =>
            Ejecutar(() => _servicio.GrupoAsync(cmpy, codigo));

        [HttpPost("Grupo")]
        public Task<ActionResult<BonGrupoEditarDto>> CrearGrupo([FromBody] BonGrupoEditarDto datos) =>
            Ejecutar(() => _servicio.CrearGrupoAsync(datos));

        [HttpPut("Grupo")]
        public Task<ActionResult<BonGrupoEditarDto>> ActualizarGrupo([FromBody] BonGrupoEditarDto datos) =>
            Ejecutar(() => _servicio.ActualizarGrupoAsync(datos));

        [HttpDelete("Grupo/{cmpy}/{codigo}")]
        public Task<ActionResult<bool>> EliminarGrupo(string cmpy, string codigo, [FromQuery] string? usuario) =>
            Ejecutar(async () =>
            {
                await _servicio.EliminarGrupoAsync(cmpy, codigo, usuario);
                return true;
            });

        [HttpGet("Catalogos/{cmpy}")]
        public Task<ActionResult<BonCatalogosDto>> Catalogos(string cmpy) =>
            Ejecutar(() => _servicio.CatalogosAsync(cmpy));

        // ---- Campañas ----

        [HttpGet("Campanas/{cmpy}")]
        public Task<ActionResult<List<BonCampanaDto>>> Campanas(string cmpy) =>
            Ejecutar(() => _servicio.CampanasAsync(cmpy));

        [HttpGet("Campana/{cmpy}/{codigo}")]
        public Task<ActionResult<BonCampanaDto>> Campana(string cmpy, string codigo) =>
            Ejecutar(() => _servicio.CampanaAsync(cmpy, codigo));

        [HttpPost("Campana")]
        public Task<ActionResult<BonCampanaDto>> CrearCampana([FromBody] BonCampanaDto datos) =>
            Ejecutar(() => _servicio.CrearCampanaAsync(datos));

        [HttpPut("Campana")]
        public Task<ActionResult<BonCampanaDto>> ActualizarCampana([FromBody] BonCampanaDto datos) =>
            Ejecutar(() => _servicio.ActualizarCampanaAsync(datos));

        /// <summary>Copia la campaña y todas sus reglas; la copia queda en borrador.</summary>
        [HttpPost("Campana/{cmpy}/{codigo}/Duplicar")]
        public Task<ActionResult<BonCampanaDto>> DuplicarCampana(string cmpy, string codigo, [FromQuery] string? usuario) =>
            Ejecutar(() => _servicio.DuplicarCampanaAsync(cmpy, codigo, usuario));

        [HttpDelete("Campana/{cmpy}/{codigo}")]
        public Task<ActionResult<bool>> EliminarCampana(string cmpy, string codigo, [FromQuery] string? usuario) =>
            Ejecutar(async () =>
            {
                await _servicio.EliminarCampanaAsync(cmpy, codigo, usuario);
                return true;
            });

        // ---- Reglas ----

        [HttpGet("Reglas/{cmpy}/{campana}")]
        public Task<ActionResult<List<BonReglaDto>>> Reglas(string cmpy, string campana) =>
            Ejecutar(() => _servicio.ReglasAsync(cmpy, campana));

        [HttpGet("Regla/{cmpy}/{codigo}")]
        public Task<ActionResult<BonReglaDto>> Regla(string cmpy, string codigo) =>
            Ejecutar(() => _servicio.ReglaAsync(cmpy, codigo));

        [HttpPost("Regla")]
        public Task<ActionResult<BonReglaDto>> CrearRegla([FromBody] BonReglaDto datos) =>
            Ejecutar(() => _servicio.CrearReglaAsync(datos));

        [HttpPut("Regla")]
        public Task<ActionResult<BonReglaDto>> ActualizarRegla([FromBody] BonReglaDto datos) =>
            Ejecutar(() => _servicio.ActualizarReglaAsync(datos));

        [HttpDelete("Regla/{cmpy}/{codigo}")]
        public Task<ActionResult<bool>> EliminarRegla(string cmpy, string codigo, [FromQuery] string? usuario) =>
            Ejecutar(async () =>
            {
                await _servicio.EliminarReglaAsync(cmpy, codigo, usuario);
                return true;
            });

        private async Task<ActionResult<T>> Ejecutar<T>(Func<Task<T>> accion)
        {
            try
            {
                return await accion();
            }
            catch (BonificadosException ex)
            {
                return StatusCode(ex.Codigo, new { mensaje = ex.Message });
            }
        }
    }
}
