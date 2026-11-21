namespace Migracion.Service.Services
{
    /// <summary>Tipo de documento que se genera para un pago de cartera.</summary>
    public enum TipoDocumentoRecaudo
    {
        /// <summary>No se pudo determinar con los datos recibidos.</summary>
        Indeterminado = 0,
        /// <summary>Recibo de caja definitivo (el pago ya tiene consecutivo, csh_num2).</summary>
        ReciboCaja,
        /// <summary>Soporte de recaudo en tránsito: el vendedor recibió el efectivo.</summary>
        RecaudoEfectivo,
        /// <summary>Referencia de pago (corresponsal bancario en CO, código CIP en PE).</summary>
        ReferenciaCorresponsal
    }

    /// <summary>
    /// Clasificación del documento de un pago, en un solo lugar porque de ella dependen
    /// tres cosas que antes se calculaban por separado y podían quedar descuadradas:
    /// el título del PDF, el prefijo del nombre del archivo y la etiqueta que viaja
    /// como parámetro del template de WhatsApp y en el asunto del correo.
    ///
    /// csh_event_type y csh_cash_received son opcionales en el contrato: la integración
    /// de Perú (DD) no los envía. Cuando faltan se deducen de csh_tipo_abono y
    /// csh_referencia. Sin esa deducción el documento salía sin título ni observaciones
    /// y el nombre del archivo sin prefijo, lo que además dejaba vacío el último
    /// parámetro del template de WhatsApp y hacía que la pasarela rechazara el envío.
    /// </summary>
    public static class DocumentoRecaudo
    {
        public static TipoDocumentoRecaudo Determinar(
            int? cshNum2,
            string? eventType,
            string? cashReceived,
            string? tipoAbono = null,
            string? referencia = null)
        {
            // Un pago con consecutivo ya aplicado es siempre recibo de caja.
            if (cshNum2.HasValue && cshNum2.Value != 0)
                return TipoDocumentoRecaudo.ReciboCaja;

            string evento = eventType?.Trim().ToLowerInvariant() ?? string.Empty;
            string efectivoRecibido = cashReceived?.Trim().ToUpperInvariant() ?? string.Empty;

            if (evento == "cash" && efectivoRecibido == "Y")
                return TipoDocumentoRecaudo.RecaudoEfectivo;

            if (evento == "ref" && efectivoRecibido == "N")
                return TipoDocumentoRecaudo.ReferenciaCorresponsal;

            // Deducción solo cuando la integración no envía el par event_type/cash_received.
            // Si los envía se respeta lo que mandó, aunque no corresponda a un par conocido.
            if (evento.Length == 0 && efectivoRecibido.Length == 0)
            {
                if (!string.IsNullOrWhiteSpace(referencia))
                    return TipoDocumentoRecaudo.ReferenciaCorresponsal;

                if (!string.IsNullOrWhiteSpace(tipoAbono) &&
                    tipoAbono.Trim().Equals("EFECTIVO", StringComparison.OrdinalIgnoreCase))
                    return TipoDocumentoRecaudo.RecaudoEfectivo;
            }

            return TipoDocumentoRecaudo.Indeterminado;
        }

        /// <summary>Prefijo del nombre del archivo PDF.</summary>
        public static string Prefijo(TipoDocumentoRecaudo tipo) => tipo switch
        {
            TipoDocumentoRecaudo.ReciboCaja => "Recibo_Caja",
            TipoDocumentoRecaudo.RecaudoEfectivo => "Recaudo",
            TipoDocumentoRecaudo.ReferenciaCorresponsal => "Referencia",
            _ => "Documento"
        };

        /// <summary>
        /// Etiqueta legible del documento. Es el texto que antes se sacaba de
        /// <c>namefile.Split('_')[0]</c>; se mantienen los mismos valores para no
        /// cambiar lo que reciben los templates ya aprobados en la pasarela.
        /// </summary>
        public static string Etiqueta(TipoDocumentoRecaudo tipo) => tipo switch
        {
            TipoDocumentoRecaudo.ReciboCaja => "Recibo",
            TipoDocumentoRecaudo.RecaudoEfectivo => "Recaudo",
            TipoDocumentoRecaudo.ReferenciaCorresponsal => "Referencia",
            _ => "Documento"
        };
    }
}
