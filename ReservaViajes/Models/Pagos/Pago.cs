namespace ReservaViajes.Models.Pagos
{
    public class Pago
    {
        public string pk_tsal001 { get; set; }
        public string terminalId { get; set; }
        public string transactionType { get; set; }
        public string invoice { get; set; }
        public string totalAmount { get; set; }
        public string taxAmount { get; set; }
        public string tipAmount { get; set; }
        public string clientEmail { get; set; }
    }
}
