using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Migracion.Models.Bonificados
{
    // ---------------------------------------------------------------------
    // Grupos mixtos (mob_rbongrupo + mob_rbongrupodet)
    // ---------------------------------------------------------------------

    public class BonGrupoEditarDto
    {
        [JsonPropertyName("cmpy")] public string Cmpy { get; set; } = "";
        /// <summary>grp_grupo. Lo asigna el API al crear (GRP000001...).</summary>
        [JsonPropertyName("codigo")] public string? Codigo { get; set; }
        [JsonPropertyName("nombre")] public string Nombre { get; set; } = "";
        [JsonPropertyName("activo")] public bool Activo { get; set; } = true;
        [JsonPropertyName("elementos")] public List<BonGrupoElementoDto> Elementos { get; set; } = new();
        /// <summary>Salida: reglas que usan el grupo (no se puede borrar mientras haya).</summary>
        [JsonPropertyName("reglas")] public List<string> Reglas { get; set; } = new();
        [JsonPropertyName("usuario")] public string? Usuario { get; set; }
    }

    public class BonGrupoElementoDto
    {
        /// <summary>LINEA, SUBLINEA o REFERENCIA.</summary>
        [JsonPropertyName("nivel")] public string Nivel { get; set; } = "REFERENCIA";
        /// <summary>rln_linea, "linea|subl" o rpr_id.</summary>
        [JsonPropertyName("valor")] public string Valor { get; set; } = "";
        [JsonPropertyName("nombre")] public string? Nombre { get; set; }
    }

    // ---------------------------------------------------------------------
    // Pedido con bonificados: lo que la app manda junto al pedido (InsertFull)
    // o al validar en línea. El motor del API es la fuente de verdad.
    // ---------------------------------------------------------------------

    public class BonPedidoDto
    {
        [JsonPropertyName("cmpy")] public string? Cmpy { get; set; }
        [JsonPropertyName("reg")] public string? Reg { get; set; }
        [JsonPropertyName("teri")] public string? Teri { get; set; }
        [JsonPropertyName("num")] public string? Num { get; set; }
        /// <summary>Fecha del pedido, MM/dd/yyyy (igual que orh_entdt).</summary>
        [JsonPropertyName("entdt")] public string? Entdt { get; set; }
        [JsonPropertyName("cust")] public string? Cust { get; set; }
        [JsonPropertyName("ship")] public string? Ship { get; set; }
        /// <summary>Vendedor que aplicó los bonificados.</summary>
        [JsonPropertyName("codusu")] public string? Codusu { get; set; }
        /// <summary>
        /// Líneas compradas con el precio unitario (IVA incluido) que usó la
        /// app. El pedido (mob_rordline) no trae precios y los montos mínimos
        /// se calculan con estos.
        /// </summary>
        [JsonPropertyName("lineas")] public List<BonLineaPedidoDto> Lineas { get; set; } = new();
        [JsonPropertyName("bonificados")] public List<BonAplicadoDto> Bonificados { get; set; } = new();
    }

    public class BonLineaPedidoDto
    {
        [JsonPropertyName("part")] public string Part { get; set; } = "";
        [JsonPropertyName("cantidad")] public decimal Cantidad { get; set; }
        [JsonPropertyName("precio")] public decimal? Precio { get; set; }
    }

    /// <summary>Una regla aplicada al pedido (mob_rordbonif).</summary>
    public class BonAplicadoDto
    {
        [JsonPropertyName("regla")] public string Regla { get; set; } = "";
        /// <summary>Nivel que calculó la app; el API recalcula el suyo.</summary>
        [JsonPropertyName("nivel")] public int? Nivel { get; set; }
        [JsonPropertyName("items")] public List<BonItemDto> Items { get; set; } = new();
    }

    /// <summary>Producto bonificado (mob_rordbonline).</summary>
    public class BonItemDto
    {
        [JsonPropertyName("part")] public string Part { get; set; } = "";
        [JsonPropertyName("cantidad")] public decimal Cantidad { get; set; }
        /// <summary>Producto comprado que originó el bonificado, si aplica.</summary>
        [JsonPropertyName("partOrigen")] public string? PartOrigen { get; set; }
        [JsonPropertyName("precioLista")] public decimal? PrecioLista { get; set; }
        [JsonPropertyName("valorBonificado")] public decimal? ValorBonificado { get; set; }
        /// <summary>Beneficio (rbb_seq) al que corresponde; obligatorio en opción múltiple.</summary>
        [JsonPropertyName("seqBenef")] public int? SeqBenef { get; set; }
        /// <summary>Opción elegida (rbo_orden) en opción múltiple.</summary>
        [JsonPropertyName("orden")] public int? Orden { get; set; }
    }

    public class BonResultadoDto
    {
        [JsonPropertyName("valido")] public bool Valido { get; set; }
        [JsonPropertyName("violaciones")] public List<BonViolacionDto> Violaciones { get; set; } = new();
        [JsonPropertyName("reglas")] public List<BonReglaEvaluadaDto> Reglas { get; set; } = new();
    }

    public class BonViolacionDto
    {
        /// <summary>V-01..V-10 (catálogo en BonificadosCodigos, el mismo de la app).</summary>
        [JsonPropertyName("codigo")] public string Codigo { get; set; } = "";
        [JsonPropertyName("regla")] public string Regla { get; set; } = "";
        [JsonPropertyName("part")] public string? Part { get; set; }
        [JsonPropertyName("mensaje")] public string Mensaje { get; set; } = "";
        [JsonPropertyName("correccion")] public string Correccion { get; set; } = "";
    }

    public class BonReglaEvaluadaDto
    {
        [JsonPropertyName("regla")] public string Regla { get; set; } = "";
        [JsonPropertyName("campana")] public string? Campana { get; set; }
        [JsonPropertyName("nivel")] public int? Nivel { get; set; }
        [JsonPropertyName("unidades")] public decimal Unidades { get; set; }
        [JsonPropertyName("monto")] public decimal Monto { get; set; }
        /// <summary>Veces que se repite el beneficio (múltiplos).</summary>
        [JsonPropertyName("veces")] public int Veces { get; set; } = 1;
        [JsonPropertyName("aceptada")] public bool Aceptada { get; set; }
    }

    // ---------------------------------------------------------------------
    // Descarga para la app
    // ---------------------------------------------------------------------

    public class BonSyncDto
    {
        /// <summary>Momento de la descarga (MM/dd/yyyy HH:mm:ss): con qué datos valida la app sin señal.</summary>
        [JsonPropertyName("fecha")] public string Fecha { get; set; } = "";
        /// <summary>Catálogo V-01..V-10 con título y corrección, para que la app diga lo mismo que el API.</summary>
        [JsonPropertyName("codigos")] public List<BonCodigoDto> Codigos { get; set; } = new();
        /// <summary>
        /// Filas por tabla espejo, con los nombres de columna de la base. Las
        /// fechas van en MM/dd/yyyy o MM/dd/yyyy HH:mm:ss.
        /// </summary>
        [JsonPropertyName("tablas")] public Dictionary<string, List<Dictionary<string, object?>>> Tablas { get; set; } = new();
        /// <summary>Bonificados que el API rechazó en los últimos 30 días, para mostrarlos al vendedor.</summary>
        [JsonPropertyName("rechazos")] public List<BonRechazoDto> Rechazos { get; set; } = new();
    }

    public class BonCodigoDto
    {
        [JsonPropertyName("codigo")] public string Codigo { get; set; } = "";
        [JsonPropertyName("titulo")] public string Titulo { get; set; } = "";
        [JsonPropertyName("correccion")] public string Correccion { get; set; } = "";
    }

    public class BonRechazoDto
    {
        [JsonPropertyName("num")] public string Num { get; set; } = "";
        [JsonPropertyName("cust")] public string? Cust { get; set; }
        [JsonPropertyName("regla")] public string Regla { get; set; } = "";
        [JsonPropertyName("fecha")] public string? Fecha { get; set; }
        [JsonPropertyName("motivo")] public string? Motivo { get; set; }
    }
}
