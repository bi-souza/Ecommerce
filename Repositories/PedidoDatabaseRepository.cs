using Ecommerce.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Ecommerce.Repositories
{
    public class PedidoDatabaseRepository : DbConnection, IPedidoRepository
    {
        public PedidoDatabaseRepository(string? strConn) : base(strConn) { }

        public bool ClienteComprouProduto(int clienteId, int produtoId)
        {
            using var cmd = new SqlCommand(@"
                SELECT COUNT(IP.ProdutoId)
                FROM ItensPedido IP
                JOIN Pedidos P ON IP.PedidoId = P.IdPedido
                WHERE P.ClienteId = @ClienteId 
                  AND IP.ProdutoId = @ProdutoId
                  AND P.StatusPedido = 'Concluído';", conn);

            cmd.Parameters.AddWithValue("@ClienteId", clienteId);
            cmd.Parameters.AddWithValue("@ProdutoId", produtoId);

            var result = cmd.ExecuteScalar();
            var count = result is int i ? i : Convert.ToInt32(result);

            return count > 0;
        }

        public int CriarPedidoComItens(int clienteId, decimal total, List<CartItem> cart)
        {
            using var tx = conn.BeginTransaction();

            try
            {
                using var cmdPedido = new SqlCommand(@"
                    INSERT INTO Pedidos (DataPedido, ValorTotal, StatusPedido, ClienteId)
                    VALUES (GETDATE(), @total, 'Aguardando pagamento', @cli);
                    SELECT CAST(SCOPE_IDENTITY() AS int);", conn, tx);

                cmdPedido.Parameters.AddWithValue("@total", total);
                cmdPedido.Parameters.AddWithValue("@cli", clienteId);

                var pedidoIdObj = cmdPedido.ExecuteScalar();
                var pedidoId = pedidoIdObj is int i ? i : Convert.ToInt32(pedidoIdObj);

                foreach (var it in cart)
                {
                    using var cmdItem = new SqlCommand(@"
                        INSERT INTO ItensPedido (Quantidade, PrecoUnit, ValorItem, PedidoId, ProdutoId)
                        VALUES (@qtd, @preco, @valor, @pedido, @prod);", conn, tx);

                    cmdItem.Parameters.AddWithValue("@qtd", it.Quantidade);
                    cmdItem.Parameters.AddWithValue("@preco", it.PrecoUnitario);
                    cmdItem.Parameters.AddWithValue("@valor", it.Subtotal);
                    cmdItem.Parameters.AddWithValue("@pedido", pedidoId);
                    cmdItem.Parameters.AddWithValue("@prod", it.IdProduto);

                    cmdItem.ExecuteNonQuery();
                }

                tx.Commit();
                return pedidoId;
            }
            catch (Exception ex)
            {
                try { tx.Rollback(); } catch { }
                throw new Exception("Falha transacional ao criar pedido.", ex);
            }
        }

        public void ConfirmarPagamento(int pedidoId)
        {
            using var tx = conn.BeginTransaction();

            try
            {
                using (var updCmd = new SqlCommand(@"
                    UPDATE Pedidos 
                    SET StatusPedido = 'Concluído' 
                    WHERE IdPedido = @id;", conn, tx))
                {
                    updCmd.Parameters.AddWithValue("@id", pedidoId);

                    var linhas = updCmd.ExecuteNonQuery();
                    if (linhas == 0)
                    {
                        throw new InvalidOperationException($"Pedido ID {pedidoId} não encontrado.");
                    }
                }

                tx.Commit();
            }
            catch (Exception ex)
            {
                try { tx.Rollback(); } catch { }
                throw new Exception("Falha transacional ao confirmar pagamento.", ex);
            }
        }

        public IEnumerable<HistoricoPedido> ObterHistorico(int clienteId)
        {
            var lista = new List<HistoricoPedido>();

            using var cmd = new SqlCommand(@"
                SELECT 
                    p.IdPedido,
                    p.DataPedido,
                    p.ValorTotal,
                    p.StatusPedido,
                    COUNT(i.PedidoId) AS QuantidadeItens
                FROM Pedidos p
                LEFT JOIN ItensPedido i ON i.PedidoId = p.IdPedido
                WHERE p.ClienteId = @cliente
                GROUP BY 
                    p.IdPedido, p.DataPedido, p.ValorTotal, p.StatusPedido
                ORDER BY p.DataPedido DESC;", conn);

            cmd.Parameters.AddWithValue("@cliente", clienteId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new HistoricoPedido
                {
                    IdPedido        = reader.GetInt32(0),
                    DataPedido      = reader.GetDateTime(1),
                    ValorTotal      = reader.GetDecimal(2),
                    StatusPedido    = reader.GetString(3),
                    QuantidadeItens = reader.GetInt32(4)
                });
            }

            return lista;
        }
    }
}
