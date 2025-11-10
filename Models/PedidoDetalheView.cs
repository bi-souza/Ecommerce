namespace Ecommerce.Models;

public class PedidoDetalheView
{
    public int IdPedido { get; set; }
    public DateTime DataPedido { get; set; }
    public decimal ValorTotal { get; set; }
    public string? StatusPedido { get; set; }     
    
    public List<ItemPedidoHistoricoView> Itens { get; set; } = new List<ItemPedidoHistoricoView>();
}
