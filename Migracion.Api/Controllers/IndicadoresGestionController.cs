using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models.Dtos;
using Migracion.Service.Queries;
using System.Threading.Tasks;

namespace Migracion.Api.Controllers
{
    /// <summary>
    /// Tablero de indicadores de gestión del vendedor para EVA 4.0. Consume
    /// directamente los procedimientos sp_mob_ind_* de la compañía.
    /// </summary>
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IndicadoresGestionController : ControllerBase
    {
        private readonly IIndicadoresGestionQueries _indicadoresQueries;

        public IndicadoresGestionController(IIndicadoresGestionQueries indicadoresQueries)
        {
            _indicadoresQueries = indicadoresQueries;
        }

        /// <summary>
        /// Las seis secciones del tablero para un mes. Los valores van sin formato:
        /// moneda, miles y porcentajes los presenta la app.
        /// </summary>
        [HttpGet]
        [Route("GetTablero/{cmpy}/{anio}/{per}/{teri}/{reg}")]
        public async Task<ActionResult<IndicadoresGestionDto>> GetTablero(
            string cmpy, int anio, int per, string teri, string reg)
        {
            if (per < 1 || per > 12)
                return BadRequest("El periodo (mes) debe estar entre 1 y 12.");
            if (anio < 2000 || anio > 2100)
                return BadRequest("Año fuera de rango.");

            return Ok(await _indicadoresQueries.GetTablero(cmpy, anio, per, teri, reg));
        }
    }
}
