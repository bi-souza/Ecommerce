using Microsoft.Data.SqlClient;
using Ecommerce.Models; 
using System.Collections.Generic;

namespace Ecommerce.Repositories;

public class ProdutoDatabaseRepository : DbConnection, IProdutoRepository
{
    public ProdutoDatabaseRepository(string? strConn) : base(strConn)
    {
    }

    public List<Produto> ReadAllByCategoria(int categoriaId)
    {
        List<Produto> produtos = new List<Produto>();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;

        cmd.CommandText = "SELECT * FROM Produto WHERE CategoriaId = @categoriaId";
        cmd.Parameters.AddWithValue("@categoriaId", categoriaId);

        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            produtos.Add(new Produto
            {
                IdProduto = (int)reader["IdProduto"],
                NomeProduto = (string)reader["NomeProduto"],
                Descricao = (string)reader["Descricao"],
                Preco = (decimal)reader["Preco"],
                Estoque = (int)reader["Estoque"],
                ImagemUrl = (string)reader["ImagemUrl"],
                Destaque = (int)reader["Destaque"],
                CategoriaId = (int)reader["CategoriaId"]
            });
        }
        return produtos;

    }

    public List<Produto> ReadAllDestaques()
    {
        List<Produto> produtos = new List<Produto>();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;

        cmd.CommandText = "SELECT * FROM Produto WHERE Destaque = 1";

        SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            produtos.Add(new Produto
            {
                IdProduto = (int)reader["IdProduto"],
                NomeProduto = (string)reader["NomeProduto"],
                Descricao = (string)reader["Descricao"],
                Preco = (decimal)reader["Preco"],
                Estoque = (int)reader["Estoque"],
                ImagemUrl = (string)reader["ImagemUrl"],
                Destaque = (int)reader["Destaque"],
                CategoriaId = (int)reader["CategoriaId"]
            });
        }
        return produtos;
    }
    
    public Produto Read(int id)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Produto WHERE IdProduto = @id";
        cmd.Parameters.AddWithValue("@id", id);

        SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            var produto = new Produto
            {
                IdProduto = (int)reader["IdProduto"],
                NomeProduto = (string)reader["NomeProduto"],
                Descricao = (string)reader["Descricao"],
                Preco = (decimal)reader["Preco"],
                Estoque = (int)reader["Estoque"],
                ImagemUrl = (string)reader["ImagemUrl"],
                Destaque = (int)reader["Destaque"],
                CategoriaId = (int)reader["CategoriaId"]

            };            
            return produto;
        }        
        return null; 
    }

}