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

        string sql = @"
            SELECT COUNT(IP.ProdutoId)
            FROM ItensPedido IP
            JOIN Pedido P ON IP.PedidoId = P.IdPedido
            WHERE P.ClienteId = @ClienteId 
                AND IP.ProdutoId = @ProdutoId
                AND P.StatusPedido = 'Concluído'";

        cmd.CommandText = sql;
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
}
