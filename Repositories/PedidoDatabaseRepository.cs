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


    public List<PedidoDetalheView> BuscarHistoricoPorCliente(int clienteId)
    {
        List<PedidoDetalheView> historico = new List<PedidoDetalheView>();


        SqlCommand cmd = new SqlCommand("SP_BuscarHistoricoCliente", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ClienteId", clienteId);

        using (SqlDataReader reader = cmd.ExecuteReader())
        {

            while (reader.Read())
            {
                historico.Add(new PedidoDetalheView
                {
                    IdPedido = (int)reader["IdPedido"],
                    DataPedido = (DateTime)reader["DataPedido"],
                    ValorTotal = (decimal)reader["ValorTotal"],
                    StatusPedido = reader["StatusPedido"].ToString()
                });
            }


            if (!historico.Any())
            {
                return historico;
            }


            if (reader.NextResult())
            {

                var pedidoLookup = historico.ToDictionary(p => p.IdPedido);

                while (reader.Read())
                {
                    int pedidoId = (int)reader["PedidoId"];


                    if (pedidoLookup.TryGetValue(pedidoId, out var pedido))
                    {
                        pedido.Itens.Add(new ItemPedidoHistoricoView
                        {
                            NomeProduto = reader["NomeProduto"].ToString(),
                            Quantidade = (int)reader["Quantidade"],
                            PrecoUnitario = (decimal)reader["PrecoUnit"]
                        });
                    }
                }
            }
        }

        return historico;

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
}


