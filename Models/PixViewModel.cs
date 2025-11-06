namespace Ecommerce.Models
{
    public class PixViewModel
    {
        public int PedidoId { get; set; }
        public decimal Total { get; set; }
        public string Payload { get; set; } = "";      
        public string QrCodeBase64 { get; set; } = ""; 
        public bool Simulate { get; set; } = false;    
    }
}
