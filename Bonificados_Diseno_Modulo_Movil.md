# Bonificados en la App EVA — Diseño del módulo móvil

> Documento de diseño (sin cambios de código aún). Complementa el "Resumen de requerimientos y planificación" (`Bonificados_Resumen_y_Planificacion.docx`) y el modelo de datos `sql_bonificados_recomendado.sql`.
> Cliente: RECAMIER · Proveedor: Intechsys S.A.S. · Julio 2026

## Contexto

**Proyecto:** Sistema de Bonificados EVA/Web — sistema **restrictivo**: la app bloquea el guardado del pedido si un bonificado no cumple las condiciones parametrizadas. El motor de reglas del backend es la **única fuente de verdad** (no se calculan bonificados en el front-end). Reglas con vigencia mensual, actualizables por trade sin intervención técnica. 14 semanas en 2 fases: C-01..C-05 (Fase I), C-06..C-11 + QR + cliente nuevo (Fase II).

**Plataforma actual sobre la que se aplica:**

- **API** (`Migracion.Api`, .NET 8): capas Api → Service (Queries/Commands) → Models → Persistence; EF Core con IBM DB2/AS400; Basic Auth; ~70 controladores en `api/v1`.
- **App EVA** (`E:/evagit/eva`, Flutter 1.2.9): sqflite (`eva.db`) con patrón `BaseDatabase<T>` en `lib/helpers/` (78 helpers); sincronización por descarga de catálogos (`DataBase_Sync` / `sync_page`); cola de subida offline con `estado_envio` (`WebService` / `mob_temporalsql`); flujo de pedidos en `lib/screens/dashboard/menu/order/` (`order_detail.dart` concentra líneas, totales y validaciones al guardar, p. ej. pedido mínimo). No existe hoy escáner QR ni módulo de campañas.

**Modelo de datos de referencia** (backend, ver `sql_bonificados_recomendado.sql`): campañas (`mob_rboncampana`), reglas con modificadores C-01..C-11 (`mob_rbonreglas`), alcance (`mob_rbonalcance`), grupos mixtos (`mob_rbongrupo/det`), condiciones (`mob_rbonregcond`), escalonamiento (`mob_rbonnivel/nivelbenef`), beneficios y opciones (`mob_rbonregbenef/benefopc`), stock asignado (`mob_rbonstock`), consumo por cliente/periodo (`mob_rbonconsumo`), eventos QR (`mob_rbonevento/inv/scan`), aplicación en pedido (`mob_rordbonif/bonline`, `mob_rbonopcsel`) y auditoría (`mob_rbonaudit`).

---

## 1. Tablas locales en sqflite

### 1.1 Espejo de solo lectura (se descargan en la sincronización)

Se recargan igual que los demás catálogos: se agregan a la descarga de `sync_page` y al borrado de `DataBase_Sync.deleteAllTables`. El API expone los datos **ya filtrados** para reducir el tamaño de la sync (RNF: sincronización inicial < 30 s).

| Tabla local | Contenido | Filtro de descarga (API) |
|---|---|---|
| `mob_rboncampana` | Campañas activas y vigentes (V-01) | Estado 'A' y fecha vigente |
| `mob_rbonreglas` | Reglas con modificadores C-01..C-11 (frecuencia, días, QR, primer pedido, precio mín., stock limitado) | Solo reglas de campañas activas |
| `mob_rbonalcance` | Segmentación canal / región / territorio / tipo cliente (V-02) | Región y territorio del vendedor |
| `mob_rbongrupo` + `mob_rbongrupodet` | Agrupaciones mixtas (ej. grupo TINTES) | Completas (son livianas) |
| `mob_rbonregcond` | Condiciones de compra (unidades, montos, foco) | De las reglas descargadas |
| `mob_rbonnivel` + `mob_rbonnivelbenef` | Tabla de escalonamiento y bonificados por nivel (C-03) | De las reglas descargadas |
| `mob_rbonregbenef` + `mob_rbonbenefopc` | Beneficios y opciones múltiples | De las reglas descargadas |
| `mob_rbonstock` | Cupo asignado/consumido por regla-producto (V-06 offline: último valor sincronizado) | De las reglas descargadas |
| `mob_rbonconsumo` | Consumos del periodo por cliente (C-05, C-08, V-08) | Solo clientes de la ruta del vendedor |
| `mob_rbonevento` + `mob_rboneventoinv` | Eventos vigentes e invitados (C-07) | Territorio del vendedor y fechas vigentes |

### 1.2 Transaccionales (la app escribe; suben con `estado_envio = 0`)

| Tabla local | Uso |
|---|---|
| `mob_rordbonif` | Encabezado de bonificaciones aplicadas al pedido (misma llave compuesta que `mob_rordhead`; incluye vendedor y nivel alcanzado) |
| `mob_rordbonline` | Ítems bonificados, con producto origen, precio de lista y valor bonificado |
| `mob_rbonopcsel` | Opción elegida por el vendedor cuando el beneficio es de opción múltiple |
| `mob_rboneventoscan` | Escaneos QR realizados (resultado: válido, vencido, evento distinto, no invitado) — Fase II |

Suben junto al pedido por la cola de reintentos existente (`WebService`), con la misma semántica de `estado_envio` que `mob_rordhead`/`mob_rordline`.

### 1.3 Piezas de código nuevas

- **Helpers y modelos:** un modelo en `lib/models/` y un helper en `lib/helpers/` por tabla (p. ej. `bonreglas_helper.dart`), extendiendo `BaseDatabase<T>` con `tableName`, `fromMap`/`toMap` y `createTable` — mismo patrón de `pedidos_helper.dart`.
- **Sincronización:** registrar las tablas espejo en la descarga de `sync_page` y en `DataBase_Sync.deleteAllTables`; guardar la fecha de última sync de bonificados (`sync_date_service`) para mostrar al vendedor con qué datos está validando.
- **Motor:** `lib/services/bonificados_engine.dart`: servicio sin dependencia de UI, consulta solo los helpers locales. Testeable con `sqflite_common_ffi` (ya está como dev dependency).

---

## 2. Motor de evaluación local (`bonificados_engine.dart`)

Dos puntos de entrada desde el flujo de pedido (`order_detail.dart`):

### 2.1 Al agregar o editar líneas — habilitar el botón "Bonificar"

```dart
/// Reglas vigentes cuyo alcance y agrupación coinciden con el
/// cliente y las líneas actuales del pedido.
Future<List<ReglaAplicable>> reglasParaPedido(
    MobRcustmain cliente, List<MobROrdLine> lineas)
```

Secuencia interna: reglas de campañas vigentes (V-01) → cruce de `mob_rbonalcance` con canal/región/territorio del cliente (V-02) → alguna línea del pedido pertenece al grupo/línea/referencia de las condiciones (V-03/V-04). Decisión clave del documento funcional: **solo los productos con regla parametrizada muestran el botón de bonificar**.

### 2.2 Al guardar el pedido — validación restrictiva y bloqueo

Se engancha en el mismo punto de `order_detail` donde hoy se valida el pedido mínimo, antes del `InsertFull`:

```dart
class ResultadoValidacion {
  final bool valido;
  final List<Violacion> violaciones; // V-01..V-10 + mensaje + corrección
}

Future<ResultadoValidacion> validarBonificados(
    MobROrdHead pedido, List<MobROrdLine> lineas,
    List<MobROrdBonLine> bonificados) async {
  final v = <Violacion>[];
  for (final bon in bonificados) {
    final regla = await _reglaDe(bon);            // con su campaña
    if (!_vigente(regla, pedido.fecha))               v.add(V01(regla));
    if (!await _alcanceOk(regla, pedido.cust))        v.add(V02(regla));
    if (!await _enCatalogoYGrupo(bon, regla))         v.add(V03(regla));
    if (!await _agrupacionPedidoOk(lineas, regla))    v.add(V04(regla));
    final nivel = await _nivelAlcanzado(lineas, regla); // C-01/C-02/C-03
    if (nivel == null || !_cantidadOk(bon, nivel))    v.add(V05(regla));
    if (!await _stockOk(bon, regla))                  v.add(V06(regla));
    if (!_precioMinimoOk(bon, regla))                 v.add(V07(regla));
    if (!await _limiteMensualOk(regla, pedido.cust))  v.add(V08(regla));
    if (regla.reqQr == 'S' &&
        !await _scanValido(regla, pedido.cust))       v.add(V09(regla));
    if (regla.soloPrimPed == 'S' &&
        await _tienePedidosPrevios(pedido.cust))      v.add(V10(regla));
  }
  return ResultadoValidacion(v.isEmpty, v);
}
```

| Código | Validación | Fuente local de datos |
|---|---|---|
| V-01 | Vigencia de campaña/regla | `mob_rboncampana`, `mob_rbonreglas` |
| V-02 | Canal / región / territorio del cliente | `mob_rbonalcance` + datos del cliente (`mob_rcustmain`) |
| V-03 | Bonificado existe y pertenece a la agrupación | `mob_rproductos`, `mob_rbongrupodet`, beneficios |
| V-04 | Líneas del pedido cumplen la agrupación | Líneas en memoria + `mob_rbongrupodet` |
| V-05 | Cantidad / monto mínimo y nivel de escalonamiento | `mob_rbonregcond`, `mob_rbonnivel(benef)` |
| V-06 | Stock disponible (> 0) | `mob_rbonstock` e inventario, último valor sincronizado |
| V-07 | Precio mínimo del bonificado | `rbr_preciominbon` + lista de precios local |
| V-08 | Límite mensual / 1 vez por cliente | `mob_rbonconsumo` |
| V-09 | QR de evento válido (Fase II) | `mob_rboneventoscan`, `mob_rbonevento` |
| V-10 | Cliente nuevo / primer pedido (Fase II) | Histórico de pedidos local (`mob_rordhead_his`) |

Si el resultado **no es válido**: diálogo con el mensaje descriptivo de cada violación (qué falló y cómo corregirlo, RNF de usabilidad) y **no se ejecuta el guardado** hasta corregir o eliminar el bonificado. Si **es válido**: se insertan `mob_rordbonif` y `mob_rordbonline` locales junto al pedido y suben por la cola existente. El cálculo de nivel y el conteo por grupo se hacen sobre las líneas en memoria: no requieren red y cumplen el RNF de respuesta < 10 s.

---

## 3. Offline y revalidación en el backend

- La app valida con los parámetros de la **última sincronización** (inventario y consumos: último valor conocido), según el RNF de disponibilidad offline.
- Al sincronizar el pedido, el API ejecuta la misma validación con datos frescos como **fuente de verdad final** (el motor C# manda; el motor Dart es solo experiencia de usuario).
- Si el backend rechaza (p. ej. el stock se agotó entre syncs o el cliente ya consumió la oferta desde otro canal), el ítem queda con `orbl_estado = 'R'`, se audita con el código V-xx en `rba_validacion`, y la app lo muestra al vendedor en la siguiente sincronización.
- Los códigos V-01..V-10 y sus mensajes deben estar **unificados entre Dart y C#** (mismo catálogo), para que la alerta local y el rechazo del servidor digan lo mismo.

---

## 4. Pantallas y UX

- **Flujo de pedido:** en `order_detail`, botón "Bonificar" visible solo en productos con regla aplicable; al tocarlo se muestran los bonificados que otorga la regla (o las opciones si `rbb_multiopc = 'S'`, guardando la elección en `mob_rbonopcsel`); los ítems bonificados se listan marcados y con valor $0 / valor bonificado.
- **Consulta de campañas:** nueva opción de menú con campañas activas filtradas por el perfil del cliente seleccionado (canal/región/territorio), mostrando condiciones y bonificados — criterio de aceptación del documento.
- **Escáner QR (Fase II):** pantalla de escaneo con una dependencia nueva (p. ej. `mobile_scanner`): valida ID de evento, ventana fecha/hora y token; rechaza QR vencidos o de otro evento; registra el resultado en `mob_rboneventoscan`.
- **Transparencia:** indicador de fecha de última sincronización de parámetros de bonificados, para que el vendedor sepa con qué datos está validando en modo offline.

---

## 5. Orden de implementación sugerido (Fase 4, semanas 7–11)

1. **Semana 7:** modelos + helpers de tablas espejo, integración en `sync_page`/`DataBase_Sync`.
2. **Semanas 8–9:** `bonificados_engine` con V-01..V-05 (Fase I funcional: C-01..C-05), botón bonificar e integración del bloqueo en `order_detail`; unit tests del motor con los escenarios del catálogo Abril 2026 (escalonamiento de tintes 5 niveles, portafolio $714k, Deco 3+1).
3. **Semana 10:** V-06..V-08 (stock, precio mínimo, consumos), tablas transaccionales y subida por la cola; consulta de campañas.
4. **Semana 11:** escáner QR + V-09/V-10 (Fase II), manejo de rechazos del backend, pruebas offline.

---

## Documentos relacionados

- `Bonificados_Resumen_y_Planificacion.docx` — resumen de requerimientos y plan de 14 semanas (API + móvil).
- `sql_bonificados_recomendado.sql` — modelo de datos recomendado (backend) con los cambios vs `sqlboni.sql` documentados.
- `Bonificados_Diseno_Modulo_Movil.docx` — versión Word de este documento.
- `Sistema_Bonificados_Resumen_Tecnico.pptx` — documento funcional original (cliente).
