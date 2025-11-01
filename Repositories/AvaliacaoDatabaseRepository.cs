using Microsoft.Data.SqlClient;
using Ecommerce.Models; 
using System.Collections.Generic;
namespace Ecommerce.Repositories;

public class AvaliacaoDatabaseRepository : DbConnection, IAvaliacaoRepository
{
    public AvaliacaoDatabaseRepository(string? strConn) : base(strConn) { }

    public void Create(Avaliacao model)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "INSERT INTO Avaliacao (Nota, Comentario, ClienteId, ProdutoId) VALUES (@nota, @comentario, @clienteId, @produtoId)";

        cmd.Parameters.AddWithValue("@nota", model.Nota);
        
        if (string.IsNullOrEmpty(model.Comentario))
        {
            
            cmd.Parameters.AddWithValue("@comentario", DBNull.Value);
        }
        else
        {
            
            cmd.Parameters.AddWithValue("@comentario", model.Comentario);
        }        
        cmd.Parameters.AddWithValue("@clienteId", model.ClienteId);
        cmd.Parameters.AddWithValue("@produtoId", model.ProdutoId);

        cmd.ExecuteNonQuery();
    }

    public List<Avaliacao> ReadByProduto(int produtoId)
    {
        List<Avaliacao> lista = new List<Avaliacao>();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;        
        
        cmd.CommandText = @"SELECT A.*, C.NomeCliente as NomeCliente 
                            FROM Avaliacao A 
                            JOIN Cliente C ON A.ClienteId = C.IdCliente
                            WHERE A.ProdutoId = @produtoId
                            ORDER BY A.DataAvaliacao DESC";

        cmd.Parameters.AddWithValue("@produtoId", produtoId);
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {            
            var avaliacao = new Avaliacao
            {
                Nota = (int)reader["Nota"],
                DataAvaliacao = (DateTime)reader["DataAvaliacao"],
                ClienteId = (int)reader["ClienteId"],
                ProdutoId = (int)reader["ProdutoId"],
                NomeCliente = (string)reader["NomeCliente"]
                
            };
            
            if (reader["Comentario"] != DBNull.Value)
            {                
                avaliacao.Comentario = (string)reader["Comentario"];
            }
            else
            {                
                avaliacao.Comentario = null; 
            }            
            
            lista.Add(avaliacao);
        }
        return lista;
    }

    public void Delete(int clienteId, int produtoId)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Avaliacao WHERE ClienteId = @clienteId AND ProdutoId = @produtoId";
        cmd.Parameters.AddWithValue("@clienteId", clienteId);
        cmd.Parameters.AddWithValue("@produtoId", produtoId);
        cmd.ExecuteNonQuery();
    }

    public Avaliacao Read(int clienteId, int produtoId)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        
        cmd.CommandText = @"SELECT A.*, C.NomeCliente as NomeCliente 
                            FROM Avaliacao A 
                            JOIN Cliente C ON A.ClienteId = C.IdCliente 
                            WHERE A.ClienteId = @clienteId AND A.ProdutoId = @produtoId";

        cmd.Parameters.AddWithValue("@clienteId", clienteId);
        cmd.Parameters.AddWithValue("@produtoId", produtoId);
        SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            
            var avaliacao = new Avaliacao
            {
                Nota = (int)reader["Nota"],
                DataAvaliacao = (DateTime)reader["DataAvaliacao"],
                ClienteId = (int)reader["ClienteId"],
                ProdutoId = (int)reader["ProdutoId"],
                NomeCliente = (string)reader["NomeCliente"]
                
            };

            
            if (reader["Comentario"] != DBNull.Value)
            {
                
                avaliacao.Comentario = (string)reader["Comentario"];
            }
            else
            {
                
                avaliacao.Comentario = null; 
            }
            
            return avaliacao;
        }
        
        return null;
    }
    
    public void Update(Avaliacao avaliacao)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;        
        
        cmd.CommandText = @"UPDATE Avaliacao 
                            SET Nota = @nota, Comentario = @comentario, DataAvaliacao = getdate() 
                            WHERE ClienteId = @clienteId AND ProdutoId = @produtoId";


        cmd.Parameters.AddWithValue("@nota", avaliacao.Nota);
        
        if (string.IsNullOrEmpty(avaliacao.Comentario))
        {
            
            cmd.Parameters.AddWithValue("@comentario", DBNull.Value);
        }
        else
        {
            
            cmd.Parameters.AddWithValue("@comentario", avaliacao.Comentario);
        }                
        
        cmd.Parameters.AddWithValue("@clienteId", avaliacao.ClienteId);
        cmd.Parameters.AddWithValue("@produtoId", avaliacao.ProdutoId);

        cmd.ExecuteNonQuery();
    } 

}