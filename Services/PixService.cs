using QRCoder;


namespace Ecommerce.Services
{
    public class PixService
    {
        private readonly IConfiguration _config;

        public PixService(IConfiguration config)
        {
            _config = config;
        }

        public string GerarQrCode(decimal valor, string pedidoId)
        {
            
            if (_config["Pix:Simulate"] == "true")
            {
                string textoSimulado = $"Pagamento PIX Simulado\nPedido: {pedidoId}\nValor: R$ {valor:F2}";
                return GerarImagemQr(textoSimulado);
            }

            
            string chave = _config["Pix:PixKey"];
            string merchant = _config["Pix:MerchantName"];
            string cidade = _config["Pix:MerchantCity"];

            string conteudoPix = $"00020126360014BR.GOV.BCB.PIX0114{chave}520400005303986540{valor:F2}5802BR5925{merchant}6009{cidade}62070503***6304ABCD";
            return GerarImagemQr(conteudoPix);
        }

        private string GerarImagemQr(string texto)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrData);
            using var bitmap = qrCode.GetGraphic(20);
            using var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return "data:image/png;base64," + Convert.ToBase64String(stream.ToArray());
        }
    }
}
