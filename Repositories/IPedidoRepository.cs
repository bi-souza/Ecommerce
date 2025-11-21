using Ecommerce.Models;
using System.Collections.Generic;

namespace Ecommerce.Repositories
{
    public interface IPedidoRepository
    {
        bool ClienteComprouProduto(int clienteId, int produtoId);
        int CriarPedidoComItens(int clienteId, decimal total, List<CartItem> cart);
        void ConfirmarPagamento(int pedidoId);
        IEnumerable<HistoricoPedido> ObterHistorico(int clienteId);
    }
}
