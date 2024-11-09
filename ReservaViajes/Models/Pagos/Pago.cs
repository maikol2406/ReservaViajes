using System.ComponentModel.DataAnnotations;

namespace ReservaViajes.Models.Pagos
{
    public class Pago
    {
        [Display(Name = "Número de factura electrónica")]
        public string pk_tsal001 { get; set; }
        [Display(Name = "Id de reserva")]
        public int idReserva { get; set; }
        [Display(Name = "Terminal")]
        public string terminalId { get; set; }
        [Display(Name = "Tipo de transacción")]
        public string transactionType { get; set; }
        [Display(Name = "Factura")]
        public string invoice { get; set; }
        [Display(Name = "Total pagado")]
        public string totalAmount { get; set; }
        [Display(Name = "Total de impuestos")]
        public string taxAmount { get; set; }
        [Display(Name = "Propina")]
        public string tipAmount { get; set; }
        [Display(Name = "Correo")]
        public string clientEmail { get; set; }
        [Display(Name = "Identificación del cliente")]
        public int idCliente { get; set; }
    }
}
