namespace Ecommerce.Models; 
public class EstoqueCriticoViewModel
{
    public int IdProduto { get; set; }
    public string? NomeProduto { get; set; }
    public string? NomeCategoria { get; set; }
    public int EstoqueAtual { get; set; }
    public int QuantidadeVendidaRecente { get; set; }
    public decimal? DiasDeCobertura { get; set; }
}