using Microsoft.Data.SqlClient;
using Ecommerce.Models; 
using System.Collections.Generic;
namespace Ecommerce.Repositories;

public class CategoriaDatabaseRepository : DbConnection, ICategoriaRepository
{
    public CategoriaDatabaseRepository(string? strConn) : base(strConn) { }

    public List<Categoria> Read()
    {
        List<Categoria> lista = new List<Categoria>();
        SqlCommand cmd = new SqlCommand("SELECT * FROM Categoria", conn);
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
    
    public Categoria Read(int id)
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM Categoria WHERE IdCategoria = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            var categoria = new Categoria
            {
                IdCategoria = (int)reader["IdCategoria"],
                NomeCategoria = (string)reader["NomeCategoria"]
            };            
            return categoria;
        }        
        return null;
    }
}