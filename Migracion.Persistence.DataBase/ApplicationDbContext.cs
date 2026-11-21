using System;
using IBM.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IBM.EntityFrameworkCore.Storage.Internal;
using Migracion.Models;
using Migracion.Models.Dtos;

namespace Migracion.Persistence.DataBase
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
         DbContextOptions<ApplicationDbContext> options
     )
         : base(options)
        {

        }
        public ApplicationDbContext()
        {

        }

        public DbSet<mob_accionesE> mob_acciones { get; set; }
        public DbSet<mob_bankacctE> mob_bankacct { get; set; }
        public DbSet<mob_carteraE> mob_cartera { get; set; }
        public DbSet<mob_depositosE> mob_depositos { get; set; }
        public DbSet<mob_districtE> mob_district { get; set; }
        public DbSet<mob_indicaclienteE> mob_indicacliente { get; set; }
        public DbSet<mob_indicadoresE> mob_indicadores { get; set; }
        public DbSet<mob_indicavendedorE> mob_indicavendedor { get; set; }
        public DbSet<mob_marcaE> mob_marca { get; set; }
        public DbSet<mob_mdiasE> mob_mdias { get; set; }
        public DbSet<mob_mercadoE> mob_mercado { get; set; }
        public DbSet<mob_motivisE> mob_motivis { get; set; }
        public DbSet<mob_motivosE> mob_motivos { get; set; }
        public DbSet<mob_novedadpreciosE> mob_novedadprecios { get; set; }
        public DbSet<mob_prodganchoE> mob_prodgancho { get; set; }
        public DbSet<mob_rabonosE> mob_rabonos { get; set; }
        public DbSet<mob_rasociadoE> mob_rasociado { get; set; }
        public DbSet<mob_rbancosE> mob_rbancos { get; set; }
        public DbSet<mob_rcashapplE> mob_rcashappl { get; set; }
        public DbSet<mob_rcashrecpE> mob_rcashrecp { get; set; }
        public DbSet<mob_rcheqdetalleE> mob_rcheqdetalle { get; set; }
        public DbSet<mob_rcheqinvdetalleE> mob_rcheqinvdetalle { get; set; }
        public DbSet<mob_rcheqinventariosE> mob_rcheqinventarios { get; set; }
        public DbSet<mob_rchequeoE> mob_rchequeo { get; set; }
        public DbSet<mob_rcliproE> mob_rclipro { get; set; }
        public DbSet<mob_rconperclienteE> mob_rconpercliente { get; set; }
        public DbSet<mob_rconsecutivosE> mob_rconsecutivos { get; set; }
        public DbSet<mob_rconsignacionE> mob_rconsignacion { get; set; }
        public DbSet<mob_rcontacimageE> mob_rcontacimage { get; set; }
        public DbSet<mob_rcontactoclienteE> mob_rcontactocliente { get; set; }
        public DbSet<mob_rcontactosE> mob_rcontactos { get; set; }
        public DbSet<mob_rcredapplE> mob_rcredappl { get; set; }
        public DbSet<mob_rcusdetalleE> mob_rcusdetalle { get; set; }
        public DbSet<mob_rcustasesE> mob_rcustases { get; set; }
        public DbSet<mob_rcustmainE> mob_rcustmain { get; set; }
        public DbSet<mob_rcustshipE> mob_rcustship { get; set; }
        public DbSet<mob_rcustransE> mob_rcustrans { get; set; }
        public DbSet<mob_rcusttypeE> mob_rcusttype { get; set; }
        public DbSet<mob_rdetentincenE> mob_rdetentincen { get; set; }
        public DbSet<mob_rdetentobsE> mob_rdetentobs { get; set; }
        public DbSet<mob_rdetentreqincentivoE> mob_rdetentreqincentivo { get; set; }
        public DbSet<mob_rdetentreqpremioE> mob_rdetentreqpremio { get; set; }
        public DbSet<mob_rdetimgnovproE> mob_rdetimgnovpro { get; set; }
        public DbSet<mob_rdetpremioincE> mob_rdetpremioinc { get; set; }
        public DbSet<mob_reclamosE> mob_reclamos { get; set; }
        public DbSet<mob_reglasprontopagoE> mob_reglasprontopago { get; set; }
        public DbSet<mob_rentincentivoE> mob_rentincentivo { get; set; }
        public DbSet<mob_rentpremioincE> mob_rentpremioinc { get; set; }
        public DbSet<mob_rentreobseE> mob_rentreobse { get; set; }
        public DbSet<mob_restzonaE> mob_restzona { get; set; }
        public DbSet<mob_rfabricanteE> mob_rfabricante { get; set; }
        public DbSet<mob_rfrecuenciavisitaE> mob_rfrecuenciavisita { get; set; }
        public DbSet<mob_rinsentivosE> mob_rinsentivos { get; set; }
        public DbSet<mob_rinvclassE> mob_rinvclass { get; set; }
        public DbSet<mob_rinvenincenE> mob_rinvenincen { get; set; }
        public DbSet<mob_rinvenincpreE> mob_rinvenincpre { get; set; }
        public DbSet<mob_rinvenobseE> mob_rinvenobse { get; set; }
        public DbSet<mob_ritemfabriE> mob_ritemfabri { get; set; }
        public DbSet<mob_ritempricE> mob_ritempric { get; set; }
        public DbSet<mob_rlineaE> mob_rlinea { get; set; }
        public DbSet<mob_rlistpricE> mob_rlistpric { get; set; }
        public DbSet<mob_rmediovisitaE> mob_rmediovisita { get; set; }
        public DbSet<mob_rmensajesE> mob_rmensajes { get; set; }
        public DbSet<mob_rmenuppalE> mob_rmenuppal { get; set; }
        public DbSet<mob_rnovproductoE> mob_rnovproducto { get; set; }
        public DbSet<mob_robsequiosE> mob_robsequios { get; set; }
        public DbSet<mob_rordheadE> mob_rordhead { get; set; }
        public DbSet<mob_rordhead_hisE> mob_rordhead_his { get; set; }
        public DbSet<mob_rordlineE> mob_rordline { get; set; }
        public DbSet<mob_rordline_hisE> mob_rordline_his { get; set; }
        public DbSet<mob_rperfilcontactoE> mob_rperfilcontacto { get; set; }
        public DbSet<mob_rperfilesE> mob_rperfiles { get; set; }
        public DbSet<mob_rperfilmenuppalE> mob_rperfilmenuppal { get; set; }
        public DbSet<mob_rpremioincE> mob_rpremioinc { get; set; }
        public DbSet<mob_rproddepositoE> mob_rproddeposito { get; set; }
        public DbSet<mob_rprodpreciosE> mob_rprodprecios { get; set; }
        public DbSet<mob_rproductosE> mob_rproductos { get; set; }
        public DbSet<mob_rprodvendedorE> mob_rprodvendedor { get; set; }
        public DbSet<mob_rrcheckposE> mob_rrcheckpos { get; set; }
        public DbSet<mob_rrelacreclamoE> mob_rrelacreclamo { get; set; }
        public DbSet<mob_rreqincenpreE> mob_rreqincenpre { get; set; }
        public DbSet<mob_rreqincentE> mob_rreqincent { get; set; }
        public DbSet<mob_rrmaheadE> mob_rrmahead { get; set; }
        public DbSet<mob_rrmalineE> mob_rrmaline { get; set; }
        public DbSet<mob_rrutasE> mob_rrutas { get; set; }
        public DbSet<mob_rrutasDto> mob_rrutasDto { get; set; }
        public DbSet<mob_rsublineasE> mob_rsublineas { get; set; }
        public DbSet<mob_rsubmenuE> mob_rsubmenu { get; set; }
        public DbSet<mob_rtipoimagenE> mob_rtipoimagen { get; set; }
        public DbSet<mob_rtipoincentivoE> mob_rtipoincentivo { get; set; }
        public DbSet<mob_rtransdetE> mob_rtransdet { get; set; }
        public DbSet<mob_rtransdet_hisE> mob_rtransdet_his { get; set; }
        public DbSet<mob_rtransferenciaE> mob_rtransferencia { get; set; }
        public DbSet<mob_rtransferencia_hisE> mob_rtransferencia_his { get; set; }
        public DbSet<mob_rtransferencistaE> mob_rtransferencista { get; set; }
        public DbSet<mob_rusuariosE> mob_rusuarios { get; set; }
        public DbSet<mob_rvisitaE> mob_rvisita { get; set; }
        public DbSet<mob_submercadoE> mob_submercado { get; set; }
        public DbSet<mob_temporalsqlE> mob_temporalsql { get; set; }
        public DbSet<mob_tipodevE> mob_tipodev { get; set; }
        public DbSet<mob_tipodocE> mob_tipodoc { get; set; }
        public DbSet<mob_tipomensajesE> mob_tipomensajes { get; set; }
        public DbSet<mob_rcontactoDto> mob_rcontactoDtos { get; set; }
        public DbSet<mob_rcustmainDto> mob_rcustmainDtos { get; set; }
        public DbSet<mob_rcustshipDto> mob_rcustshipDtos { get; set; }
        public DbSet<mob_rcustmainSimp> mob_rcustmainSimps { get; set; }
        public DbSet<mob_rcustransDto> mob_rcustransDtos { get; set; }
        public DbSet<mob_ritempricDto> mob_ritempricDtos { get; set; }
        public DbSet<mob_rprodvendedorDto> mob_rprodvendedorDtos { get; set; }
        public DbSet<mob_ractualiza> mob_ractualiza { get; set; }
        public DbSet<mob_rtransferenciahisE> mob_rtransferenciahisEs { get; set; }
        public DbSet<mob_rvisitsellerDto> mob_rvisitsellerDtos { get; set; }
        public DbSet<mob_rvisitsellerGroupDto> mob_rvisitsellerGroupDtos { get; set; }
        public DbSet<mob_rvisitSummaryDto> mob_rvisitSummaryDtos { get; set; }
        public DbSet<RegionE> region { get; set; }
        public DbSet<TerritorioE> territorio { get; set; }
        public DbSet<mob_docvisitaE> mob_docvisita { get; set; }
        public DbSet<mob_cupocompromisoE> mob_cupocompromiso { get; set; }
        public DbSet<EmpleadosE> empleado1 { get; set; }
        public DbSet<grupo_canalE> grupo_canal { get; set; }
        public DbSet<mob_rexhibicionE> mob_rexhibicion { get; set; }
        public DbSet<mob_rexhibicionfotoE> mob_rexhibicionfotos { get; set; }
        public DbSet<mob_rstatusexhibicionE> mob_rstatusexhibicion { get; set; }
        public DbSet<mob_rcategoriaexhibicionE> mob_rcategoriaexhibicion { get; set; }
        public DbSet<mob_rclasexhibicionE> mob_rclasexhibicion { get; set; }
        public DbSet<mob_rtipoexhibicionE> mob_rtipoexhibicion { get; set; }
        public DbSet<mob_rclasetipoeexhibicionE> mob_rclasetipoeexhibicion { get; set; }
        public DbSet<ExhibicionDtoE> ExhibicionDtoEs { get; set; }
        public DbSet<ExhibicionesFotosAllDto> ExhibicionesFotosAllDtoEs { get; set; }
        public DbSet<mob_rclasetipoeexhibicionDto> mob_rclasetipoeexhibicionDtos { get; set; }
        public DbSet<mob_rmaterialpopE> mob_rmaterialpop { get; set; }
        public DbSet<mob_rmarcaspopE> mob_rmarcaspop { get; set; }
        public DbSet<mob_rpope> mob_rpop { get; set; }
        public DbSet<mob_rdetallepopE> mob_rdetallepop { get; set; }
        public DbSet<MovimientoMaterialDto> MovimientoMaterialDtoEs { get; set; }
        public DbSet<mob_rexcepcionabonosE> mob_rexcepcionabonos { get; set; }
        public DbSet<mob_rprodpuntoE> mob_rprodpuntos { get; set; }
        public DbSet<dash_E> dash { get; set; }
        public DbSet<EstadoClienteE> estadocliente { get; set; }
        public DbSet<InventarioStockDto> inventarioStock { get; set; }
        public DbSet<mob_fact_pedido_eva_Dto> mob_fact_pedido_eva { get; set; }
        public DbSet<option_consent> wa_optin_consent { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<mob_rclasetipoeexhibicionE>().HasKey(
                x => new
                {
                    x.rclastipoexh_idclase,
                    x.rclastipoexh_idtipo
                }
                );
            modelBuilder.Entity<EstadoClienteE>().HasNoKey();
            modelBuilder.Entity<mob_rprodpuntoE>().HasNoKey();
            modelBuilder.Entity<dash_E>().HasNoKey();
            modelBuilder.Entity<mob_rexcepcionabonosE>().HasNoKey();
            modelBuilder.Entity<mob_rdetallepopE>().HasNoKey();
            modelBuilder.Entity<mob_rclasetipoeexhibicionDto>().HasNoKey();
            modelBuilder.Entity<mob_cupocompromisoE>().HasNoKey();
            modelBuilder.Entity<mob_docvisitaE>().HasNoKey();
            modelBuilder.Entity<TerritorioE>().HasNoKey();
            modelBuilder.Entity<RegionE>().HasNoKey();
            modelBuilder.Entity<EmpleadosE>().HasNoKey();
            modelBuilder.Entity<grupo_canalE>().HasNoKey();
            modelBuilder.Entity<mob_rvisitSummaryDto>().HasNoKey();
            modelBuilder.Entity<mob_rvisitsellerDto>().HasNoKey();
            modelBuilder.Entity<mob_rvisitsellerGroupDto>().HasNoKey();
            modelBuilder.Entity<mob_rtransferenciahisE>().HasNoKey();
            modelBuilder.Entity<mob_ractualiza>().HasNoKey();
            modelBuilder.Entity<mob_ritempricDto>().HasNoKey();
            modelBuilder.Entity<mob_rcustransDto>().HasNoKey();
            modelBuilder.Entity<mob_rcustmainDto>().HasNoKey();
            modelBuilder.Entity<mob_rcustshipDto>().HasNoKey();
            modelBuilder.Entity<mob_rprodvendedorDto>().HasNoKey();
            modelBuilder.Entity<mob_rcustmainSimp>().HasNoKey();
            modelBuilder.Entity<mob_tipodevE>().HasNoKey();
            modelBuilder.Entity<mob_rcontactoDto>().HasNoKey();
            modelBuilder.Entity<mob_accionesE>().HasNoKey();
            modelBuilder.Entity<mob_bankacctE>().HasNoKey();
            modelBuilder.Entity<mob_carteraE>().HasNoKey();
            modelBuilder.Entity<mob_depositosE>().HasNoKey();
            modelBuilder.Entity<mob_districtE>().HasNoKey();
            modelBuilder.Entity<mob_indicaclienteE>().HasNoKey();
            modelBuilder.Entity<mob_indicadoresE>().HasNoKey();
            modelBuilder.Entity<mob_indicavendedorE>().HasNoKey();
            modelBuilder.Entity<mob_marcaE>().HasNoKey();
            modelBuilder.Entity<mob_mdiasE>().HasNoKey();
            modelBuilder.Entity<mob_mercadoE>().HasNoKey();
            modelBuilder.Entity<mob_motivisE>().HasNoKey();
            modelBuilder.Entity<mob_motivosE>().HasNoKey();
            modelBuilder.Entity<mob_novedadpreciosE>().HasNoKey();
            modelBuilder.Entity<mob_prodganchoE>().HasNoKey();
            modelBuilder.Entity<mob_rabonosE>().HasNoKey();
            modelBuilder.Entity<mob_rasociadoE>().HasNoKey();
            modelBuilder.Entity<mob_rbancosE>().HasNoKey();
            modelBuilder.Entity<mob_rcashapplE>().HasNoKey();
            modelBuilder.Entity<mob_rcashrecpE>().HasNoKey();
            modelBuilder.Entity<mob_rcheqdetalleE>().HasNoKey();
            modelBuilder.Entity<mob_rcheqinvdetalleE>().HasNoKey();
            modelBuilder.Entity<mob_rcheqinventariosE>().HasNoKey();
            modelBuilder.Entity<mob_rchequeoE>().HasNoKey();
            modelBuilder.Entity<mob_rcliproE>().HasNoKey();
            modelBuilder.Entity<mob_rconperclienteE>().HasNoKey();
            modelBuilder.Entity<mob_rconsecutivosE>().HasNoKey();
            modelBuilder.Entity<mob_rconsignacionE>().HasNoKey();
            modelBuilder.Entity<mob_rcontacimageE>().HasNoKey();
            modelBuilder.Entity<mob_rcontactoclienteE>().HasNoKey();
            modelBuilder.Entity<mob_rcontactosE>().HasNoKey();
            modelBuilder.Entity<mob_rcredapplE>().HasNoKey();
            modelBuilder.Entity<mob_rcusdetalleE>().HasNoKey();
            modelBuilder.Entity<mob_rcustasesE>().HasNoKey();
            modelBuilder.Entity<mob_rcustmainE>().HasNoKey();
            modelBuilder.Entity<mob_rcustransE>().HasNoKey();
            modelBuilder.Entity<mob_rcusttypeE>().HasNoKey();
            modelBuilder.Entity<mob_rdetentincenE>().HasNoKey();
            modelBuilder.Entity<mob_rdetentobsE>().HasNoKey();
            modelBuilder.Entity<mob_rdetentreqincentivoE>().HasNoKey();
            modelBuilder.Entity<mob_rdetentreqpremioE>().HasNoKey();
            modelBuilder.Entity<mob_rdetimgnovproE>().HasNoKey();
            modelBuilder.Entity<mob_rdetpremioincE>().HasNoKey();
            modelBuilder.Entity<mob_reclamosE>().HasNoKey();
            modelBuilder.Entity<mob_reglasprontopagoE>().HasNoKey();
            modelBuilder.Entity<mob_rentincentivoE>().HasNoKey();
            modelBuilder.Entity<mob_rentpremioincE>().HasNoKey();
            modelBuilder.Entity<mob_rentreobseE>().HasNoKey();
            modelBuilder.Entity<mob_restzonaE>().HasNoKey();
            modelBuilder.Entity<mob_rfabricanteE>().HasNoKey();
            modelBuilder.Entity<mob_rfrecuenciavisitaE>().HasNoKey();
            modelBuilder.Entity<mob_rinsentivosE>().HasNoKey();
            modelBuilder.Entity<mob_rinvclassE>().HasNoKey();
            modelBuilder.Entity<mob_rinvenincenE>().HasNoKey();
            modelBuilder.Entity<mob_rinvenincpreE>().HasNoKey();
            modelBuilder.Entity<mob_rinvenobseE>().HasNoKey();
            modelBuilder.Entity<mob_ritemfabriE>().HasNoKey();
            modelBuilder.Entity<mob_ritempricE>().HasNoKey();
            modelBuilder.Entity<mob_rlineaE>().HasNoKey();
            modelBuilder.Entity<mob_rlistpricE>().HasNoKey();
            modelBuilder.Entity<mob_rmediovisitaE>().HasNoKey();
            modelBuilder.Entity<mob_rmensajesE>().HasNoKey();
            modelBuilder.Entity<mob_rmenuppalE>().HasNoKey();
            modelBuilder.Entity<mob_rnovproductoE>().HasNoKey();
            modelBuilder.Entity<mob_robsequiosE>().HasNoKey();
            modelBuilder.Entity<mob_rordheadE>().HasNoKey();
            modelBuilder.Entity<mob_rordhead_hisE>().HasNoKey();
            modelBuilder.Entity<mob_rordlineE>().HasNoKey();
            modelBuilder.Entity<mob_rordline_hisE>().HasNoKey();
            modelBuilder.Entity<mob_rperfilcontactoE>().HasNoKey();
            modelBuilder.Entity<mob_rperfilesE>().HasNoKey();
            modelBuilder.Entity<mob_rperfilmenuppalE>().HasNoKey();
            modelBuilder.Entity<mob_rpremioincE>().HasNoKey();
            modelBuilder.Entity<mob_rproddepositoE>().HasNoKey();
            modelBuilder.Entity<mob_rprodpreciosE>().HasNoKey();
            modelBuilder.Entity<mob_rproductosE>().HasNoKey();
            modelBuilder.Entity<mob_rprodvendedorE>().HasNoKey();
            modelBuilder.Entity<mob_rrcheckposE>().HasNoKey();
            modelBuilder.Entity<mob_rrelacreclamoE>().HasNoKey();
            modelBuilder.Entity<mob_rreqincenpreE>().HasNoKey();
            modelBuilder.Entity<mob_rreqincentE>().HasNoKey();
            modelBuilder.Entity<mob_rrmaheadE>().HasNoKey();
            modelBuilder.Entity<mob_rrmalineE>().HasNoKey();
            modelBuilder.Entity<mob_rrutasE>().HasNoKey();
            modelBuilder.Entity<mob_rrutasDto>().HasNoKey();
            modelBuilder.Entity<mob_rsublineasE>().HasNoKey();
            modelBuilder.Entity<mob_rsubmenuE>().HasNoKey();
            modelBuilder.Entity<mob_rtipoimagenE>().HasNoKey();
            modelBuilder.Entity<mob_rtipoincentivoE>().HasNoKey();
            modelBuilder.Entity<mob_rtransdetE>().HasNoKey();
            modelBuilder.Entity<mob_rtransdet_hisE>().HasNoKey();
            modelBuilder.Entity<mob_rtransferenciaE>().HasNoKey();
            modelBuilder.Entity<mob_rtransferencia_hisE>().HasNoKey();
            modelBuilder.Entity<mob_rtransferencistaE>().HasNoKey();
            modelBuilder.Entity<mob_rusuariosE>().HasNoKey();
            modelBuilder.Entity<mob_rvisitaE>().HasNoKey();
            modelBuilder.Entity<mob_submercadoE>().HasNoKey();
            modelBuilder.Entity<mob_temporalsqlE>().HasNoKey();
            modelBuilder.Entity<mob_tipodevE>().HasNoKey();
            modelBuilder.Entity<mob_tipodocE>().HasNoKey();
            modelBuilder.Entity<mob_tipomensajesE>().HasNoKey();
            modelBuilder.Entity<mob_rcustshipE>().HasNoKey();
            modelBuilder.Entity<ExhibicionesFotosAllDto>().HasNoKey();
            modelBuilder.Entity<InventarioStockDto>().HasNoKey();
            modelBuilder.Entity<mob_fact_pedido_eva_Dto>().HasNoKey();
            modelBuilder.Entity<option_consent>().HasKey(x => x.woc_id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
