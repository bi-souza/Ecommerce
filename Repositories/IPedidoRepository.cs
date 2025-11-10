using Ecommerce.Models;

namespace Ecommerce.Repositories;

public interface IPedidoRepository
{    
    bool ClienteComprouProduto(int clienteId, int produtoId);    
    
    List<PedidoDetalheView> BuscarHistoricoPorCliente(int clienteId);
}
