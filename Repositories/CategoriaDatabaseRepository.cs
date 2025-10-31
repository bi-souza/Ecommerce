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
    
}
