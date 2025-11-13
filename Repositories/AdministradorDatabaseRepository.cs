using Ecommerce.Models;
using Microsoft.Data.SqlClient;
using System;

namespace Ecommerce.Repositories;

public class AdministradorDatabaseRepository : DbConnection, IAdministradorRepository
{
    public AdministradorDatabaseRepository(string? strConn) : base(strConn) { }        

    public Pessoa Login(LoginViewModel model)
    {   
        SqlCommand cmd = new SqlCommand(); 
        cmd.Connection = conn; 
        cmd.CommandText = @"
            SELECT P.*, A.IdAdmin 
            FROM Pessoas P 
            INNER JOIN Administradores A ON P.IdPessoa = A.IdAdmin 
            WHERE P.Email = @Email AND P.SenhaHash = @SenhaHash";                        
        
                    
        cmd.Parameters.AddWithValue("@Email", model.Email ?? string.Empty);
        cmd.Parameters.AddWithValue("@SenhaHash", model.Senha ?? string.Empty);

            
        SqlDataReader reader = cmd.ExecuteReader();
            
        if (reader.Read())
        {
            
            return new Pessoa
            {
                IdPessoa = (int)reader["IdPessoa"],
                Nome = reader["Nome"].ToString(),
                Email = reader["Email"].ToString(),
                Cpf = reader["Cpf"].ToString(),
                Telefone = reader["Telefone"].ToString(),
                DataNasc = reader["DataNasc"] == DBNull.Value ? default : Convert.ToDateTime(reader["DataNasc"]),
                SenhaHash = reader["SenhaHash"].ToString()
            };
        }                

        return null; 
        
    }

    
}
