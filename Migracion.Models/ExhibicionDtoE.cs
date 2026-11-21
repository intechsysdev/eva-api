using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Models
{
    public class ExhibicionDtoE
    {
        [Key]
		public string? CODIGO_EXHIBICION { get; set; }
		public string? rexh_cmpy { get; set; }
		public int ID_ESTADO { get; set; }
        public string? ESTADO { get; set; }
        public string? ICONO { get; set; }
        public string? ID_USUARIO { get; set; }
        public string? NOMBRE { get; set; }
        public string? ID_REGION { get; set; }
        public string? REGION { get; set; }
        public string? ID_TERRITORIO { get; set; }
        public string? TERRITORIO { get; set; }
        public string? ID_PERFIL { get; set; }
        public string? LATITUD_EXHIBICION { get; set; }
		public string? LONGITUD_EXHIBICION { get; set; }
		public string? USUARIO { get; set; }
		public int ID_CLASE { get; set; }
        public string? CLASE { get; set; }
        public int ID_TIPO { get; set; }
        public string? TIPO { get; set; }
        public string? OTRO_TIPO { get; set; }
		public string? ID_CLIENTE { get; set; }
		public string? ID_SUCURSAL { get; set; }
        public string? RAZON_SOCIAL { get; set; }
        public string? CLIENTE { get; set; }
        public string? DIRECCION { get; set; }
        public string? ID_CANAL { get; set; }
        public string? CANAL { get; set; }
        public string? ID_GRUPOCANAL { get; set; }
        public string? GRUPO_CANAL { get; set; }
        public string? ID_CIUDAD { get; set; }
        public string? CIUDAD { get; set; }
        public string? LATITUD { get; set; }
        public string? LONGITUD { get; set; }
        public string? ID_LINEA { get; set; }
        public string? LINEA { get; set; }
        public string? ID_SUBLINEA { get; set; }
        public string? SUB_LINEA { get; set; }
		public int ID_CATEGORIA { get; set; }
        public string? CATEGORIA { get; set; }
        public string? OTRA_CATEGORIA { get; set; }
		public string? UNASOLAVEZ { get; set; }
		public string? TIPO_FOTO { get; set; }
		public string? OBSERVACION { get; set; }
		public string? PREMIO_DESEADO { get; set; }
		public string? CREADO_POR { get; set; }
		public string? DISPOSITIVO { get; set; }
		public string? ACTUALIZADO_POR { get; set; }
		public string? FECHA_ACTUALIZADO { get; set; }
		public string? FECHA_CREADO { get; set; }
		public string? HORA_CREADO { get; set; }
		public string? NOTA { get; set; }
	}

    public class FilterExhibicionesFotosDto
    {
        public string? fecIni { get; set; }
        public string? fecFin { get; set; }
        public string? cust { get; set; }
        public string? succli { get; set; }
        public int idclase { get; set; }
        public int idtipo { get; set; }
        public string? linea { get; set; }
        public string? sublinea { get; set; }
    }

    public class ExhibicionesFotosAllDto
    {
            public string? CODIGO_EXHIBICION { get; set; }
            public string? rexh_cmpy { get; set; }
            public int ID_ESTADO { get; set; }
            public string? ESTADO { get; set; }
            public string? ICONO { get; set; }
            public string? ID_USUARIO { get; set; }
            public string? NOMBRE { get; set; }
            public string? ID_REGION { get; set; }
            public string? REGION { get; set; }
            public string? ID_TERRITORIO { get; set; }
            public string? TERRITORIO { get; set; }
            public string? ID_PERFIL { get; set; }
            public string? LATITUD_EXHIBICION { get; set; }
            public string? LONGITUD_EXHIBICION { get; set; }
            public string? USUARIO { get; set; }
            public int ID_CLASE { get; set; }
            public string? CLASE { get; set; }
            public int ID_TIPO { get; set; }
            public string? TIPO { get; set; }
            public string? OTRO_TIPO { get; set; }
            public string? ID_CLIENTE { get; set; }
            public string? ID_SUCURSAL { get; set; }
            public string? RAZON_SOCIAL { get; set; }
            public string? CLIENTE { get; set; }
            public string? DIRECCION { get; set; }
            public string? ID_CANAL { get; set; }
            public string? CANAL { get; set; }
            public string? ID_GRUPOCANAL { get; set; }
            public string? GRUPO_CANAL { get; set; }
            public string? ID_CIUDAD { get; set; }
            public string? CIUDAD { get; set; }
            public string? LATITUD { get; set; }
            public string? LONGITUD { get; set; }
            public string? ID_LINEA { get; set; }
            public string? LINEA { get; set; }
            public string? ID_SUBLINEA { get; set; }
            public string? SUB_LINEA { get; set; }
            public int ID_CATEGORIA { get; set; }
            public string? CATEGORIA { get; set; }
            public string? OTRA_CATEGORIA { get; set; }
            public string? UNASOLAVEZ { get; set; }
            public string? TIPO_FOTO { get; set; }
            public string? OBSERVACION { get; set; }
            public string? PREMIO_DESEADO { get; set; }
            public string? CREADO_POR { get; set; }
            public string? DISPOSITIVO { get; set; }
            public string? ACTUALIZADO_POR { get; set; }
            public string? FECHA_ACTUALIZADO { get; set; }
            public string? FECHA_CREADO { get; set; }
            public string? HORA_CREADO { get; set; }
            public string? NOTA { get; set; }
            public string? URLIMAGEN { get; set; }
        
    }
}
