namespace Ecommerce.Models;
using System.ComponentModel.DataAnnotations;
public class Produto
{
    public int IdProduto { get; set; }
    public string? NomeProduto { get; set; }
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "O Preço é obrigatório.")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Preco { get; set; }

    [Required(ErrorMessage = "O Estoque é obrigatório.")]
    [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
    public int Estoque { get; set; }
    public string? ImagemUrl { get; set; }
    public int Destaque { get; set; }
    public int CategoriaId { get; set; }
}