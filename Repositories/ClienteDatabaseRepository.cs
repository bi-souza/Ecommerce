using Ecommerce.Models;
using Microsoft.Data.SqlClient;

namespace Ecommerce.Repositories;

public class ClienteDatabaseRepository : DbConnection, IClienteRepository
{
    public ClienteDatabaseRepository(string? strConn) : base(strConn) { }


    public Cliente Login(LoginViewModel model)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;


        cmd.CommandText = "SELECT * FROM Cliente WHERE Email = @email AND Senha = @senha";
        cmd.Parameters.AddWithValue("@email", model.Email);
        cmd.Parameters.AddWithValue("@senha", model.Senha);

        SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {

            return new Cliente
            {
                IdCliente = (int)reader["IdCliente"],
                Email = (string)reader["Email"],
                NomeCliente = (string)reader["NomeCliente"]
            };
        }

        return null; 
    }
}