using System.Collections.Generic;

namespace Migracion.Models.Dtos
{
    /// <summary>
    /// Tablero de indicadores de gestión del vendedor: el resultado de los seis
    /// procedimientos sp_mob_ind_* para un periodo, territorio y región.
    /// </summary>
    public class IndicadoresGestionDto
    {
        public string cmpy { get; set; } = string.Empty;
        public int anio { get; set; }
        public int per { get; set; }
        public string teri { get; set; } = string.Empty;
        public string reg { get; set; } = string.Empty;

        /// <summary>
        /// Una entrada por sección (resumen, ventas, cobros, efectividad, pedidos,
        /// clientes) con la primera fila del procedimiento: nombre de columna en
        /// minúscula -> valor sin formato. Nulo si el procedimiento no devolvió filas.
        /// </summary>
        public Dictionary<string, Dictionary<string, object?>?> secciones { get; set; } = new();

        /// <summary>
        /// Secciones que fallaron y por qué. Cada procedimiento se ejecuta por
        /// separado: uno caído no deja sin tablero al vendedor.
        /// </summary>
        public Dictionary<string, string> errores { get; set; } = new();
    }
}
