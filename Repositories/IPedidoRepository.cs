using Ecommerce.Models;

namespace Ecommerce.Repositories;

public interface IPedidoRepository
{    
    bool ClienteComprouProduto(int clienteId, int produtoId);
        
    int CriarPedidoComItens(int clienteId, decimal total, List<CartItem> cartItems);

    void ConfirmarPagamento(int pedidoId);
}
