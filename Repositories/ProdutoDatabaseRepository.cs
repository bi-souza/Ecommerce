using Microsoft.Data.SqlClient;
using Ecommerce.Models; 

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
        cmd.CommandText = @"INSERT INTO Produtos (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId)                             
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

    public bool Delete(int id)
    {        
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        
        cmd.CommandText = "DELETE FROM Produtos WHERE IdProduto = @id";
        
        cmd.Parameters.AddWithValue("@id", id);

        try
        {
            cmd.ExecuteNonQuery();
            return true; 
        }

        catch (SqlException ex)
        {
            
            if (ex.Number == 547)
            {
                return false; 
            }           
            
            throw; 
        }        
       
    }
    
    public void Update(Produto model)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        
        cmd.CommandText = @"UPDATE Produtos 
                            SET NomeProduto = @nome, 
                                Descricao = @desc, 
                                Preco = @preco, 
                                Estoque = @estoque, 
                                ImagemUrl = @img, 
                                Destaque = @destaque, 
                                CategoriaId = @catId
                            WHERE IdProduto = @id"; 
        
        cmd.Parameters.AddWithValue("@nome", model.NomeProduto);
        cmd.Parameters.AddWithValue("@desc", model.Descricao);
        cmd.Parameters.AddWithValue("@preco", model.Preco);
        cmd.Parameters.AddWithValue("@estoque", model.Estoque);
        cmd.Parameters.AddWithValue("@img", model.ImagemUrl);
        cmd.Parameters.AddWithValue("@destaque", model.Destaque);
        cmd.Parameters.AddWithValue("@catId", model.CategoriaId);      
        cmd.Parameters.AddWithValue("@id", model.IdProduto);

        cmd.ExecuteNonQuery();
    }

    public List<Produto> ReadAllByCategoria(int categoriaId)
    {
        List<Produto> produtos = new List<Produto>();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;

        cmd.CommandText = "SELECT * FROM Produtos WHERE CategoriaId = @categoriaId";
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
        reader.Close();
        return produtos;

    }

    public List<Produto> ReadAllDestaques()
    {
        List<Produto> produtos = new List<Produto>();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;

        cmd.CommandText = "SELECT * FROM Produtos WHERE Destaque = 1";

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
        reader.Close();
        return produtos;
    }

    public List<Produto> ReadAllMaisVendidos()
    {
        List<Produto> lista = new List<Produto>();

        string sql = @"
            SELECT TOP 4 
                p.IdProduto,
                p.NomeProduto,
                p.Descricao,
                p.Preco,
                p.Estoque,
                p.ImagemUrl,
                p.Destaque,
                p.CategoriaId,
                SUM(ip.Quantidade) AS TotalVendido
            FROM Produtos p
            INNER JOIN ItensPedido ip 
                ON p.IdProduto = ip.ProdutoId
            GROUP BY 
                p.IdProduto, 
                p.NomeProduto, 
                p.Descricao, 
                p.Preco,
                p.Estoque,
                p.ImagemUrl,
                p.Destaque,
                p.CategoriaId
            ORDER BY TotalVendido DESC";

        SqlCommand cmd = new SqlCommand(sql, conn);
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            lista.Add(new Produto
            {
                IdProduto = (int)reader["IdProduto"],
                NomeProduto = (string)reader["NomeProduto"],
                Descricao = (string)reader["Descricao"],
                Preco = (decimal)reader["Preco"],
                Estoque = (int)reader["Estoque"],
                ImagemUrl = reader["ImagemUrl"].ToString(),
                Destaque = (int)reader["Destaque"],
                CategoriaId = (int)reader["CategoriaId"]
            });
        }
        reader.Close();
        return lista;
    }
    
    public List<Produto> ReadAll()
    {
        List<Produto> lista = new List<Produto>();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Produtos"; 

        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read()) 
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
        reader.Close();  
        return lista;
    }
    public Produto Read(int id)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Produtos WHERE IdProduto = @id";
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
        reader.Close();
        return null;
    }
    
    public List<Produto> Search(string termo)
    {
        List<Produto> lista = new List<Produto>();
        
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        
        cmd.CommandText = "SELECT * FROM Produtos WHERE NomeProduto LIKE @termo";
        cmd.Parameters.AddWithValue("@termo", $"%{termo}%");

        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {        
            
            var produto = new Produto
            {
                IdProduto = (int)reader["IdProduto"],
                NomeProduto = (string)reader["NomeProduto"],
                Preco = (decimal)reader["Preco"],
                Estoque = (int)reader["Estoque"],
                Destaque = (int)reader["Destaque"],
                CategoriaId = (int)reader["CategoriaId"],
                Descricao = (string)reader["Descricao"],
                ImagemUrl = (string)reader["ImagemUrl"] 
            };        

            lista.Add(produto);
        }
        reader.Close();
        return lista;
    }

}