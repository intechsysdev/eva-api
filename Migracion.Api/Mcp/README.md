# Servidor MCP de EVA

Expone consultas de EVA a modelos de IA por [Model Context Protocol](https://modelcontextprotocol.io/).
Vive dentro de `Migracion.Api`, en la ruta `/mcp` (transporte Streamable HTTP, sin sesión), y usa los
mismos servicios `*Queries` que los controladores. Todas las herramientas son de **solo lectura**.

## Activarlo

El endpoint no existe hasta que el servidor tenga una llave de 24 caracteres o más.

| Ajuste | Variable de entorno | Para qué |
|---|---|---|
| `Mcp:ApiKey` | `Mcp__ApiKey` | Llave que envía el cliente. Vacía o corta = `/mcp` no se publica. |
| `Mcp:Compania` | `Mcp__Compania` | Compañía por defecto (`RE`, `DD`, `LA`, `KR`) cuando el modelo no envía `cmpy`. |
| `Mcp:LimiteMaximo` | `Mcp__LimiteMaximo` | Máximo de filas por llamada (100). |

La llave va en la sección `Mcp` de `appsettings.json`; el repositorio debe ser privado mientras esté ahí.
Cada compañía lleva la suya: la del repositorio es la de RE, y en los demás servidores se reemplaza con
la variable de entorno `Mcp__ApiKey` (tiene prioridad sobre el archivo). Para generar una:

```powershell
$b = New-Object byte[] 32
[Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($b)
[Convert]::ToBase64String($b).TrimEnd('=').Replace('+','-').Replace('/','_')
```

Al arrancar, el log dice `MCP habilitado en /mcp` o `MCP deshabilitado: configure Mcp:ApiKey`.

## Conectar un modelo

Cualquier cliente MCP con transporte HTTP sirve. Datos de conexión:

- URL: `https://<servidor>:<puerto>/mcp`
- Cabecera: `Authorization: Bearer <llave>` (también se acepta `X-Api-Key: <llave>`)

**Claude Code**

```bash
claude mcp add --transport http eva https://<servidor>:<puerto>/mcp --header "Authorization: Bearer <llave>"
```

**Archivo `.mcp.json`** (Claude Code, VS Code y otros clientes que leen este formato)

```json
{
  "mcpServers": {
    "eva": {
      "type": "http",
      "url": "https://<servidor>:<puerto>/mcp",
      "headers": { "Authorization": "Bearer ${EVA_MCP_KEY}" }
    }
  }
}
```

**API de Anthropic (MCP connector)**: el servidor se declara en `mcp_servers` con `url` y la llave en
`authorization_token`.

**Probarlo a mano** con el inspector oficial:

```bash
npx @modelcontextprotocol/inspector
```

Elegir transporte *Streamable HTTP*, la URL y la cabecera `Authorization`.

Los conectores personalizados de claude.ai (web) piden OAuth, que este servidor no implementa.

## Herramientas

| Herramienta | Qué responde | Servicio |
|---|---|---|
| `eva_listar_vendedores` | Usuarios con su código, región y territorio. Punto de partida. | `IUsuariosQueries.GetListAll` |
| `eva_listar_regiones_territorios` | Códigos y nombres de regiones y territorios. | `IRegionQueries`, `ITerritorioQueries` |
| `eva_buscar_clientes` | Clientes de un vendedor, con búsqueda. | `IClientesQueries.GetListByCodUsu` |
| `eva_obtener_cliente` | Ficha completa de un cliente. | `IClientesQueries.GetbyCust` |
| `eva_consultar_cartera` | Documentos por cobrar y saldo total. | `ICarteraQueries.GetList` |
| `eva_listar_pagos` | Pagos y abonos de los últimos días. | `ICarteraQueries.GetPaymentByUser/ByCust` |
| `eva_ultimos_pedidos_cliente` | Últimos pedidos de un cliente, con o sin líneas. | `IPedidosQueries.GetHis/GetFull` |
| `eva_obtener_pedido` | Un pedido con sus líneas. | `IPedidosQueries.GetByReg/GetDetail` |
| `eva_buscar_productos` | Catálogo de productos. | `IProductosQueries.GetListAll` |
| `eva_consultar_inventario` | Existencias por producto. | `IInventariosQueries` |
| `eva_resumen_visitas` | Visitas por vendedor y día. | `IVisitasQueries.GetListSummary` |
| `eva_listar_visitas` | Visitas cliente por cliente. | `IVisitasQueries.GetListGroup` |
| `eva_tablero_indicadores` | Tablero de gestión del mes. | `IIndicadoresGestionQueries.GetTablero` |

Convenciones: los argumentos de fecha van en `AAAA-MM-DD`; las respuestas son JSON con los nombres de
columna de la base, sin campos vacíos; las listas se paginan con `limite` y `desplazamiento`.

## Seguridad

- **Llave propia.** `/mcp` no acepta el usuario Basic de las apps: tiene su esquema (`McpApiKey`), así se
  puede cambiar la llave del modelo sin tocar los dispositivos.
- **Solo lectura.** No hay herramientas que inserten, actualicen o borren. Si algún día se agregan, deben
  ir en una clase aparte y marcadas con `ReadOnly = false, Destructive = true`.
- **Entradas validadas.** Varias consultas de `Migracion.Service` arman el SQL concatenando texto. Por eso
  toda herramienta pasa sus argumentos por `McpEntrada` (solo letras, números, `.`, `-`, `_`) antes de
  llamar al servicio. **Ninguna herramienta nueva debe saltarse ese paso.**
- **Sin datos sensibles.** `eva_listar_vendedores` no devuelve contraseña ni cédula. Los errores de la
  base se registran en el log y al modelo le llega un mensaje genérico.
- El límite de 600 peticiones por minuto por IP del API también aplica a `/mcp`.

## Agregar una herramienta

1. Crear o ampliar una clase en `Mcp/Tools` que herede de `EvaToolBase` y lleve `[McpServerToolType]`.
2. Escribir el método con `[McpServerTool(Name = "eva_...", ReadOnly = true, ...)]` y una `[Description]`
   que diga qué devuelve y qué significa cada campo: es lo único que el modelo sabe de la herramienta.
3. Validar cada argumento con `McpEntrada`, llamar al servicio dentro de `Consultar(...)` y devolver con
   `McpSalida.Pagina(...)` o `McpSalida.Json(...)`.
4. Si la clase es nueva, registrarla en `McpServiceExtensions.AddEvaMcp` con `.WithTools<...>()`.
