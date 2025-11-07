namespace Ecommerce.Models;
public class Produto
{
    public int IdProduto { get; set; }
    public string? NomeProduto { get; set; }
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public string? ImagemUrl { get; set; }
    public int Destaque { get; set; }
    public int CategoriaId { get; set; }
}