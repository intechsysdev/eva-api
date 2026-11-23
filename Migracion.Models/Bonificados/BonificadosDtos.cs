using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Migracion.Models.Bonificados
{
    // Contrato del módulo web de bonificados (seguimiento-online). No son
    // entidades de EF: el servicio lee y escribe las tablas mob_rbon* con SQL
    // parametrizado y arma/desarma estos objetos. Ver BonificadosService.

    /// <summary>Campaña (mob_rboncampana).</summary>
    public class BonCampanaDto
    {
        [JsonPropertyName("cmpy")] public string Cmpy { get; set; } = "";
        /// <summary>cmp_campana. Lo asigna el API al crear (CAM000001...).</summary>
        [JsonPropertyName("codigo")] public string? Codigo { get; set; }
        /// <summary>cmp_descr (150).</summary>
        [JsonPropertyName("nombre")] public string Nombre { get; set; } = "";
        [JsonPropertyName("fechaInicio")] public DateTime FechaInicio { get; set; }
        /// <summary>Último día de vigencia. Se guarda a las 23:59:59 para que el día cuente completo.</summary>
        [JsonPropertyName("fechaFin")] public DateTime FechaFin { get; set; }
        /// <summary>A = activa, I = inactiva, B = borrador.</summary>
        [JsonPropertyName("estado")] public string Estado { get; set; } = "B";
        /// <summary>Campaña de la que se duplicó.</summary>
        [JsonPropertyName("origen")] public string? Origen { get; set; }
        [JsonPropertyName("totalReglas")] public int TotalReglas { get; set; }
        [JsonPropertyName("usuarioCreacion")] public string? UsuarioCreacion { get; set; }
        [JsonPropertyName("fechaCreacion")] public DateTime? FechaCreacion { get; set; }
        [JsonPropertyName("usuarioModificacion")] public string? UsuarioModificacion { get; set; }
        [JsonPropertyName("fechaModificacion")] public DateTime? FechaModificacion { get; set; }
        /// <summary>Entrada: usuario del portal que hace el cambio.</summary>
        [JsonPropertyName("usuario")] public string? Usuario { get; set; }
    }

    /// <summary>
    /// Regla completa: mob_rbonreglas más sus hijas (alcance, condiciones,
    /// niveles, beneficios, opciones y stock). Se guarda siempre entera.
    /// </summary>
    public class BonReglaDto
    {
        [JsonPropertyName("cmpy")] public string Cmpy { get; set; } = "";
        /// <summary>rbr_regla. Lo asigna el API al crear (REG000001...).</summary>
        [JsonPropertyName("codigo")] public string? Codigo { get; set; }
        [JsonPropertyName("campana")] public string Campana { get; set; } = "";
        /// <summary>rbr_descr (150).</summary>
        [JsonPropertyName("nombre")] public string Nombre { get; set; } = "";
        [JsonPropertyName("activa")] public bool Activa { get; set; } = true;
        [JsonPropertyName("prioridad")] public int Prioridad { get; set; } = 1;
        [JsonPropertyName("acumulable")] public bool Acumulable { get; set; }
        /// <summary>Opcionales: acotan la vigencia dentro de la campaña.</summary>
        [JsonPropertyName("fechaInicio")] public DateTime? FechaInicio { get; set; }
        [JsonPropertyName("fechaFin")] public DateTime? FechaFin { get; set; }
        /// <summary>Salida: C-01, C-02 o C-03, deducido de la condición.</summary>
        [JsonPropertyName("tipo")] public string? Tipo { get; set; }

        // Alcance (mob_rbonalcance). Vacío = todos.
        [JsonPropertyName("canales")] public List<string> Canales { get; set; } = new();
        [JsonPropertyName("regiones")] public List<string> Regiones { get; set; } = new();
        [JsonPropertyName("territorios")] public List<string> Territorios { get; set; } = new();

        /// <summary>PRODUCTO, LINEA, SUBLINEA o GRUPO_MIXTO.</summary>
        [JsonPropertyName("agrupacion")] public string Agrupacion { get; set; } = "PRODUCTO";
        /// <summary>
        /// Códigos según la agrupación: rpr_id, rln_linea, "linea|subl" o el
        /// grupo. Cada uno es una fila de mob_rbonregcond y las cantidades se
        /// suman entre todas (operador OR).
        /// </summary>
        [JsonPropertyName("agrupacionValores")] public List<string> AgrupacionValores { get; set; } = new();

        [JsonPropertyName("usaEscalonamiento")] public bool UsaEscalonamiento { get; set; }
        /// <summary>UNIDADES o MONTO: qué miden los niveles del escalonamiento.</summary>
        [JsonPropertyName("medida")] public string Medida { get; set; } = "UNIDADES";
        [JsonPropertyName("cantidadMinima")] public decimal? CantidadMinima { get; set; }
        [JsonPropertyName("montoMinimo")] public decimal? MontoMinimo { get; set; }

        /// <summary>Beneficios de una regla sin escalonamiento (mob_rbonregbenef).</summary>
        [JsonPropertyName("beneficios")] public List<BonBeneficioDto> Beneficios { get; set; } = new();
        /// <summary>Niveles del escalonamiento (mob_rbonnivel + mob_rbonnivelbenef).</summary>
        [JsonPropertyName("niveles")] public List<BonNivelDto> Niveles { get; set; } = new();

        [JsonPropertyName("precioMinimoBonificado")] public decimal? PrecioMinimoBonificado { get; set; }
        /// <summary>N = sin límite, M = por mes, U = una sola vez por cliente.</summary>
        [JsonPropertyName("frecuencia")] public string Frecuencia { get; set; } = "N";
        /// <summary>rbr_maxclimes: veces por cliente en el periodo. Null = sin tope.</summary>
        [JsonPropertyName("maximoPorCliente")] public int? MaximoPorCliente { get; set; }

        /// <summary>C-11: la regla se apaga al agotar el cupo de mob_rbonstock.</summary>
        [JsonPropertyName("stockLimitado")] public bool StockLimitado { get; set; }
        [JsonPropertyName("stock")] public List<BonStockDto> Stock { get; set; } = new();

        [JsonPropertyName("usuarioCreacion")] public string? UsuarioCreacion { get; set; }
        [JsonPropertyName("fechaCreacion")] public DateTime? FechaCreacion { get; set; }
        [JsonPropertyName("usuarioModificacion")] public string? UsuarioModificacion { get; set; }
        [JsonPropertyName("fechaModificacion")] public DateTime? FechaModificacion { get; set; }
        [JsonPropertyName("usuario")] public string? Usuario { get; set; }
    }

    public class BonBeneficioDto
    {
        /// <summary>SPECIFIC_PRODUCT, SAME_PRODUCT, LOWEST_VALUE o MULTIPLE_OPTION.</summary>
        [JsonPropertyName("tipo")] public string Tipo { get; set; } = "SPECIFIC_PRODUCT";
        [JsonPropertyName("cantidad")] public decimal Cantidad { get; set; } = 1;
        /// <summary>Solo SPECIFIC_PRODUCT.</summary>
        [JsonPropertyName("productoId")] public string? ProductoId { get; set; }
        [JsonPropertyName("productoNombre")] public string? ProductoNombre { get; set; }
        /// <summary>Solo LOWEST_VALUE: grupo dentro del cual se elige el de menor valor.</summary>
        [JsonPropertyName("grupo")] public string? Grupo { get; set; }
        /// <summary>Solo MULTIPLE_OPTION: el vendedor elige una (mob_rbonbenefopc).</summary>
        [JsonPropertyName("opciones")] public List<BonProductoCantidadDto> Opciones { get; set; } = new();
    }

    public class BonProductoCantidadDto
    {
        [JsonPropertyName("productoId")] public string ProductoId { get; set; } = "";
        [JsonPropertyName("productoNombre")] public string? ProductoNombre { get; set; }
        [JsonPropertyName("cantidad")] public decimal Cantidad { get; set; } = 1;
        /// <summary>
        /// Solo en niveles: la cantidad bonificada es igual al total comprado
        /// (rnb_formula = IGUAL_TOTAL; ej. un sachet por cada tinte).
        /// </summary>
        [JsonPropertyName("igualAlTotal")] public bool IgualAlTotal { get; set; }
    }

    public class BonNivelDto
    {
        [JsonPropertyName("nivel")] public int Nivel { get; set; }
        /// <summary>Unidades o monto desde el que aplica, según la medida de la regla.</summary>
        [JsonPropertyName("desde")] public decimal Desde { get; set; }
        /// <summary>Salida: lo calcula el API a partir del nivel siguiente. Null = sin tope.</summary>
        [JsonPropertyName("hasta")] public decimal? Hasta { get; set; }
        [JsonPropertyName("beneficios")] public List<BonProductoCantidadDto> Beneficios { get; set; } = new();
    }

    public class BonStockDto
    {
        [JsonPropertyName("productoId")] public string ProductoId { get; set; } = "";
        [JsonPropertyName("productoNombre")] public string? ProductoNombre { get; set; }
        [JsonPropertyName("asignado")] public int Asignado { get; set; }
        /// <summary>Salida: lo lleva el motor al aplicar bonificados; el portal no lo cambia.</summary>
        [JsonPropertyName("consumido")] public int Consumido { get; set; }
    }

    /// <summary>Todo lo que necesita el formulario de reglas, en una sola llamada.</summary>
    public class BonCatalogosDto
    {
        [JsonPropertyName("canales")] public List<BonOpcionDto> Canales { get; set; } = new();
        [JsonPropertyName("regiones")] public List<BonOpcionDto> Regiones { get; set; } = new();
        [JsonPropertyName("territorios")] public List<BonOpcionDto> Territorios { get; set; } = new();
        [JsonPropertyName("lineas")] public List<BonOpcionDto> Lineas { get; set; } = new();
        [JsonPropertyName("sublineas")] public List<BonSublineaDto> Sublineas { get; set; } = new();
        [JsonPropertyName("productos")] public List<BonProductoDto> Productos { get; set; } = new();
        [JsonPropertyName("grupos")] public List<BonGrupoDto> Grupos { get; set; } = new();
    }

    public class BonOpcionDto
    {
        [JsonPropertyName("codigo")] public string Codigo { get; set; } = "";
        [JsonPropertyName("nombre")] public string Nombre { get; set; } = "";
    }

    public class BonSublineaDto
    {
        [JsonPropertyName("linea")] public string Linea { get; set; } = "";
        [JsonPropertyName("codigo")] public string Codigo { get; set; } = "";
        [JsonPropertyName("nombre")] public string Nombre { get; set; } = "";
    }

    public class BonProductoDto
    {
        [JsonPropertyName("codigo")] public string Codigo { get; set; } = "";
        [JsonPropertyName("nombre")] public string Nombre { get; set; } = "";
        [JsonPropertyName("linea")] public string? Linea { get; set; }
        [JsonPropertyName("sublinea")] public string? Sublinea { get; set; }
    }

    public class BonGrupoDto
    {
        [JsonPropertyName("codigo")] public string Codigo { get; set; } = "";
        [JsonPropertyName("nombre")] public string Nombre { get; set; } = "";
        [JsonPropertyName("activo")] public bool Activo { get; set; }
        [JsonPropertyName("elementos")] public int Elementos { get; set; }
    }
}
