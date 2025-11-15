using Microsoft.Data.SqlClient;
using Ecommerce.Models; 
namespace Ecommerce.Repositories;

public class CategoriaDatabaseRepository : DbConnection, ICategoriaRepository
{
    public CategoriaDatabaseRepository(string? strConn) : base(strConn) { }
    
    public void Create(Categoria categoria)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        
        cmd.CommandText = "INSERT INTO Categorias (NomeCategoria) VALUES (@NomeCategoria)";

        cmd.Parameters.AddWithValue("@NomeCategoria", categoria.NomeCategoria ?? string.Empty);

        cmd.ExecuteNonQuery();
    }

    public List<Categoria> ReadAll()
    {
        List<Categoria> lista = new List<Categoria>();
        SqlCommand cmd = new SqlCommand("SELECT * FROM Categorias", conn);

        SqlDataReader reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            lista.Add(new Categoria
            {
                IdCategoria = (int)reader["IdCategoria"],
                NomeCategoria = (string)reader["NomeCategoria"]
            });
        }
        
        return lista;
    }
    
    
    public Categoria ReadById(int id)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        
        cmd.CommandText = "SELECT * FROM Categorias WHERE IdCategoria = @IdCategoria";
        
        cmd.Parameters.AddWithValue("@IdCategoria", id);

        SqlDataReader reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            return new Categoria
            {
                IdCategoria = (int)reader["IdCategoria"],
                NomeCategoria = (string)reader["NomeCategoria"]
            };
        }
        
        return null;
    }
    
    
    public void Update(Categoria categoria)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        
        cmd.CommandText = "UPDATE Categorias SET NomeCategoria = @NomeCategoria WHERE IdCategoria = @IdCategoria";
        
        cmd.Parameters.AddWithValue("@NomeCategoria", categoria.NomeCategoria ?? string.Empty);
        cmd.Parameters.AddWithValue("@IdCategoria", categoria.IdCategoria);

        cmd.ExecuteNonQuery();
    } 

    
    public bool Delete(int id)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        
        cmd.CommandText = "DELETE FROM Categorias WHERE IdCategoria = @IdCategoria";
        
        cmd.Parameters.AddWithValue("@IdCategoria", id);

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

    
}
