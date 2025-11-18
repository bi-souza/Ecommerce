using Ecommerce.Models;
using Microsoft.Data.SqlClient;
using System.Data;


namespace Ecommerce.Repositories;


public class PedidoDatabaseRepository : DbConnection, IPedidoRepository
{
    public PedidoDatabaseRepository(string? strConn) : base(strConn) { }


    public bool ClienteComprouProduto(int clienteId, int produtoId)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"
            SELECT COUNT(IP.ProdutoId)
            FROM ItensPedido IP
            JOIN Pedidos P ON IP.PedidoId = P.IdPedido
            WHERE P.ClienteId = @ClienteId 
                AND IP.ProdutoId = @ProdutoId
                AND P.StatusPedido = 'Concluído'";

        cmd.Parameters.AddWithValue("@ClienteId", clienteId);
        cmd.Parameters.AddWithValue("@ProdutoId", produtoId);

        int count = (int)cmd.ExecuteScalar();

        return count > 0;
    }
    

    public int CriarPedidoComItens(int clienteId, decimal total, List<CartItem> cart)
    {     
                    
        SqlTransaction tx = conn.BeginTransaction(); 
        int pedidoId = 0;

        try
        {
            
            SqlCommand cmdPedido = new SqlCommand(); 
            cmdPedido.Connection = conn;
            cmdPedido.Transaction = tx; 
            cmdPedido.CommandText = @"
                INSERT INTO Pedidos (DataPedido, ValorTotal, StatusPedido, ClienteId)
                VALUES (GETDATE(), @total, 'Aguardando pagamento', @cli);
                SELECT SCOPE_IDENTITY();"; 

            cmdPedido.Parameters.AddWithValue("@total", total);
            cmdPedido.Parameters.AddWithValue("@cli", clienteId);

           
            pedidoId = System.Convert.ToInt32(cmdPedido.ExecuteScalar());

            
            foreach (var it in cart)
            {
                SqlCommand cmdItem = new SqlCommand();
                cmdItem.Connection = conn;
                cmdItem.Transaction = tx; 
                cmdItem.CommandText = @"
                    INSERT INTO ItensPedido (Quantidade, PrecoUnit, ValorItem, PedidoId, ProdutoId)
                    VALUES (@qtd, @preco, @valor, @pedido, @prod);";

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
            try { tx.Rollback(); } catch { /* ignore */ }            
            
            throw new Exception("Falha transacional ao criar pedido.", ex); 
        }
        
    }
    
    public void ConfirmarPagamento(int pedidoId)
    {     
                            
        SqlTransaction tx = conn.BeginTransaction(); 

        try
        {
            decimal total;            
            
            using (var getCmd = new SqlCommand("SELECT ValorTotal FROM Pedidos WHERE IdPedido=@id", conn, tx))
            {
                getCmd.Parameters.AddWithValue("@id", pedidoId);
                var obj = getCmd.ExecuteScalar();
                if (obj is null) 
                {
                    
                    throw new InvalidOperationException($"Pedido ID {pedidoId} não encontrado.");
                }
                total = Convert.ToDecimal(obj);
            }
                       
            using (var pagCmd = new SqlCommand(@"
                INSERT INTO Pagamentos (ValorPago, TipoPagamento, PedidoId)
                VALUES (@valor, @tipo, @pedido);", conn, tx))
            {
                pagCmd.Parameters.AddWithValue("@valor", total);                
                pagCmd.Parameters.AddWithValue("@tipo", "Pix"); 
                pagCmd.Parameters.AddWithValue("@pedido", pedidoId);
                pagCmd.ExecuteNonQuery();
            }
            
            using (var updCmd = new SqlCommand(@"
                UPDATE Pedidos SET StatusPedido='Concluído' WHERE IdPedido=@id;", conn, tx))
            {
                updCmd.Parameters.AddWithValue("@id", pedidoId);
                updCmd.ExecuteNonQuery();
            }            
            
            tx.Commit();
        }
        catch (Exception ex)
        {            
            try { tx.Rollback(); } catch { /* ignore */ }            
            
            throw new Exception("Falha transacional ao confirmar pagamento.", ex); 
        }
    }

}


