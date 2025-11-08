namespace Ecommerce.Models;

public class Avaliacao
{
    public DateTime DataAvaliacao { get; set; }
    public string? Comentario { get; set; }
    public int Nota { get; set; }
    public int ClienteId { get; set; }
    public int ProdutoId { get; set; }
    public string? Nome { get; set; }
}