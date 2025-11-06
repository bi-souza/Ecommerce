namespace Ecommerce.Models
{
    public class PixViewModel
    {
        public int PedidoId { get; set; }
        public decimal Total { get; set; }
        public string Payload { get; set; } = "";      // texto do PIX (simulado ou real)
        public string QrCodeBase64 { get; set; } = ""; // imagem do QR em base64 (data URL)
        public bool Simulate { get; set; } = false;    // true = modo demo/simulação
    }
}
