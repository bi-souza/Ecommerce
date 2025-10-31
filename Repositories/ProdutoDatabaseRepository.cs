using Microsoft.Data.SqlClient;
using Ecommerce.Models; 
using System.Collections.Generic;

namespace Ecommerce.Repositories;

public class ProdutoDatabaseRepository : DbConnection, IProdutoRepository
{
    public ProdutoDatabaseRepository(string? strConn) : base(strConn)
    {
    }

    public void Create(Produto model)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;

        cmd.CommandText = @"INSERT INTO Produto (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId)                             
                            VALUES (@nome, @desc, @preco, @estoque, @img, @destaque, @catId)";


        cmd.Parameters.AddWithValue("@nome", model.NomeProduto);
        cmd.Parameters.AddWithValue("@desc", model.Descricao);
        cmd.Parameters.AddWithValue("@preco", model.Preco);
        cmd.Parameters.AddWithValue("@estoque", model.Estoque);
        cmd.Parameters.AddWithValue("@img", model.ImagemUrl);
        cmd.Parameters.AddWithValue("@destaque", model.Destaque);
        cmd.Parameters.AddWithValue("@catId", model.CategoriaId);


        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        // 1. Cria o comando
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn; // Usa a conexão da classe base

        // 2. Define o comando SQL
        // (Assumindo que sua chave primária é 'IdProduto' como no seu Model)
        cmd.CommandText = "DELETE FROM Produto WHERE IdProduto = @id";

        // 3. Adiciona o parâmetro de ID
        cmd.Parameters.AddWithValue("@id", id);

        // 4. Executa o comando
        cmd.ExecuteNonQuery();
    }
    
    public void Update(Produto model)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;

        // Comando SQL de atualização
        cmd.CommandText = @"UPDATE Produto 
                            SET NomeProduto = @nome, 
                                Descricao = @desc, 
                                Preco = @preco, 
                                Estoque = @estoque, 
                                ImagemUrl = @img, 
                                Destaque = @destaque, 
                                CategoriaId = @catId
                            WHERE IdProduto = @id"; // A condição WHERE é crucial

        // Adiciona todos os parâmetros
        cmd.Parameters.AddWithValue("@nome", model.NomeProduto);
        cmd.Parameters.AddWithValue("@desc", model.Descricao);
        cmd.Parameters.AddWithValue("@preco", model.Preco);
        cmd.Parameters.AddWithValue("@estoque", model.Estoque);
        cmd.Parameters.AddWithValue("@img", model.ImagemUrl);
        cmd.Parameters.AddWithValue("@destaque", model.Destaque);
        cmd.Parameters.AddWithValue("@catId", model.CategoriaId);
        
        // Adiciona o ID para a cláusula WHERE
        cmd.Parameters.AddWithValue("@id", model.IdProduto);

        cmd.ExecuteNonQuery();
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
    
    public List<Produto> ReadAll()
    {
        List<Produto> lista = new List<Produto>();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Produto"; // (Poderia ter um JOIN com Categoria)

        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read()) // Loop em todos os resultados
        {
            lista.Add(new Produto
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
        return lista;
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