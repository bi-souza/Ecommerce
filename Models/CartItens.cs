namespace Ecommerce.Models
{
    public class CartItem
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; } = 1;
        public string? ImagemUrl { get; set; }

        public decimal Subtotal => PrecoUnitario * Quantidade;
    }
}
