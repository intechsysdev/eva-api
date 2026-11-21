using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Migracion.Api.Models
{
    public class FiltersDetalleDto
    {
        public string[] Territorios { get; set; }
        public string[] Regiones { get; set; }
        public string[] MedioVisita { get; set; }
        public string[] MotivoVisita { get; set; }
        public string[] Acciones { get; set; }
        public string[] noCompra { get; set; }
        public string[] Tipomovimiento { get; set; }
        public string[] Ciudades { get; set; }
        public int duracionVisitaDesde { get; set; }
        public int duracionVisitaHasta { get; set; }
        public string? horaDesde { get; set; }
        public string? horaHasta { get; set; }
        public int cantVisitaDesde { get; set; }
        public int cantVisitaHasta { get; set; }
        public string[] codUsu { get; set; }
        /// <summary>
        /// OJO: no es un filtro. Selecciona la tabla de clientes con la que se
        /// cruza la visita segun el perfil (1 = mob_rcustrans, 3 = mob_rcustases,
        /// cualquier otro = mob_rcustmain). Para filtrar por cliente use
        /// <see cref="custFiltro"/>.
        /// </summary>
        public string? codCust { get; set; }
        /// <summary>
        /// Cliente al que se limita el resultado. Es el filtro real por cliente,
        /// el que faltaba: al bajar de Efectuadas al detalle se enviaba el codigo
        /// en codCust y la consulta seguia devolviendo la jornada completa.
        /// Vacio o nulo = sin filtrar.
        /// </summary>
        public string? custFiltro { get; set; }
        /// <summary>
        /// Sucursal del cliente de <see cref="custFiltro"/>. Solo se aplica si
        /// viene con valor: un cliente sin sucursales no la envia.
        /// </summary>
        public string? succliFiltro { get; set; }
        public string? fecIni { get; set; }
        public string? fecFin { get; set; }
    }

    /// <summary>
    /// Cuerpo de Productos/UpdateImagen.
    /// </summary>
    public class ActualizarImagenDto
    {
        public string? cmpy { get; set; }
        /// <summary>Codigo del producto (rpr_id), con o sin relleno.</summary>
        public string? id { get; set; }
        /// <summary>
        /// URL completa con la que queda rpr_urlimagen.
        /// </summary>
        /// <remarks>
        /// La arma el SPA a partir de las fotos que ya funcionan y NO el API:
        /// FileSettings.UrlPath es la carpeta de disco donde FileUpload escribe
        /// (algo como C:\Imagenes\eva), no la direccion web con la que se
        /// sirven las imagenes.
        /// </remarks>
        public string? url { get; set; }
    }

    public class FiltersRutaDto
    {
        public string? cmpy { get; set; }
        public string[] codUsu { get; set; }
        public string[] rruta_ruta { get; set; }
        public string[] codPer { get; set; }
        public string? fecRuta { get; set; }

    }

    public class FilterExhibicionesDto
    {
        public int[] Estados { get; set; }
        public string[] codUsu { get; set; }
        public string[] Territorios { get; set; }
        public string[] Regiones { get; set; }
        public string[] Perfiles { get; set; }
        public int[] Clases { get; set; }
        public int[] Tipos { get; set; }
        public string[] Canales { get; set; }
        public string[] GrupoCanales { get; set; }
        public string[] Ciudades { get; set; }
        public string[] Lineas { get; set; }
        public string[] Sublineas { get; set; }
        public int[] Categorias { get; set; }
        public string[] UnaSolaVez { get; set; }
        public string[] TipoFotos { get; set; }
        public string[] PremioDeseado { get; set; }
        public string? FecIni { get; set; }
        public string? FecFin { get; set; }
        public string? Cmpy { get; set; }
    }

    public class FilterMaterialPOPDto
    {
        public int[] Materiales { get; set; }
        public int[] Marcas { get; set; }
        public string[] codUsu { get; set; }
        public string[] Territorios { get; set; }
        public string[] Regiones { get; set; }
        public string[] Perfiles { get; set; }
        public string? FecIni { get; set; }
        public string? FecFin { get; set; }
        public string? Cmpy { get; set; }
    }


}
