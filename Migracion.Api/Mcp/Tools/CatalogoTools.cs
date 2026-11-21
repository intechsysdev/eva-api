using Microsoft.Extensions.Options;
using Migracion.Service.Queries;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Migracion.Api.Mcp.Tools
{
    /// <summary>Vendedores, regiones y territorios: el punto de partida de casi toda consulta.</summary>
    [McpServerToolType]
    public class CatalogoTools : EvaToolBase
    {
        private readonly IUsuariosQueries _usuarios;
        private readonly IRegionQueries _regiones;
        private readonly ITerritorioQueries _territorios;

        public CatalogoTools(
            IUsuariosQueries usuarios, IRegionQueries regiones, ITerritorioQueries territorios,
            IOptions<McpSettings> settings, ILogger<CatalogoTools> logger) : base(settings, logger)
        {
            _usuarios = usuarios;
            _regiones = regiones;
            _territorios = territorios;
        }

        [McpServerTool(Name = "eva_listar_vendedores", Title = "Listar vendedores y usuarios de EVA",
            ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
        [Description(
            "Lista los usuarios de EVA (vendedores, supervisores, etc.) de una compañía. Úsala primero para " +
            "obtener el código del vendedor (rusu_codusu), su región (rusu_reg) y su territorio (rusu_teri): " +
            "las demás herramientas piden esos tres datos. Devuelve también nombre (rusu_nombre), perfil " +
            "(rusu_codper) y correo (rusu_correo). Nunca devuelve contraseñas ni cédulas.")]
        public async Task<string> ListarVendedores(
            [Description(DescripcionCmpy)] string? cmpy = null,
            [Description("Texto a buscar en el nombre o el código del usuario (sin distinguir mayúsculas ni tildes).")] string? buscar = null,
            [Description("Código de perfil para filtrar (campo rusu_codper). Omite para traer todos los perfiles.")] string? perfil = null,
            [Description(DescripcionLimite)] int limite = 25,
            [Description(DescripcionDesplazamiento)] int desplazamiento = 0)
        {
            string compania = Cmpy(cmpy);
            string filtroPerfil = McpEntrada.CodigoOpcional(perfil, "perfil") ?? "%";
            string? texto = McpEntrada.Busqueda(buscar);
            var (lim, desp) = Pagina(limite, desplazamiento);

            var usuarios = await Consultar("eva_listar_vendedores",
                () => _usuarios.GetListAll(compania, filtroPerfil));

            var filas = (usuarios ?? new())
                .Where(u => texto == null || McpEntrada.Contiene(u.rusu_nombre, texto) || McpEntrada.Contiene(u.rusu_codusu, texto))
                .OrderBy(u => u.rusu_nombre)
                .Select(u => new
                {
                    u.rusu_codusu,
                    u.rusu_nombre,
                    u.rusu_codper,
                    u.rusu_correo,
                    u.rusu_reg,
                    u.rusu_teri
                })
                .ToList();

            return McpSalida.Pagina(filas, lim, desp,
                siVacio: "No hay usuarios con ese filtro. Prueba sin 'buscar' o sin 'perfil', o revisa el código de compañía.");
        }

        [McpServerTool(Name = "eva_listar_regiones_territorios", Title = "Listar regiones y territorios",
            ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
        [Description(
            "Devuelve las regiones (reg_regi, reg_desc) y los territorios (ter_teri, ter_desc) de la compañía, " +
            "para traducir los códigos de región y territorio a su nombre o para saber cuáles existen.")]
        public async Task<string> ListarRegionesTerritorios(
            [Description(DescripcionCmpy)] string? cmpy = null)
        {
            string compania = Cmpy(cmpy);

            var regiones = await Consultar("eva_listar_regiones_territorios", () => _regiones.GetList());
            var territorios = await Consultar("eva_listar_regiones_territorios", () => _territorios.GetList());

            return McpSalida.Json(new
            {
                cmpy = compania,
                regiones = (regiones ?? new())
                    .Where(r => McpEntrada.Igual(r.reg_cmpy, compania))
                    .OrderBy(r => r.reg_regi)
                    .Select(r => new { r.reg_regi, r.reg_desc }),
                territorios = (territorios ?? new())
                    .Where(t => McpEntrada.Igual(t.ter_cmpy, compania))
                    .OrderBy(t => t.ter_teri)
                    .Select(t => new { t.ter_teri, t.ter_desc })
            });
        }
    }
}
