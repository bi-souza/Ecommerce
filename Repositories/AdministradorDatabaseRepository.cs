using Ecommerce.Models;
using Microsoft.Data.SqlClient;
using System;

namespace Ecommerce.Repositories
{
    public class AdministradorDatabaseRepository : IAdministradorRepository
    {
        private readonly string _connectionString;

        public AdministradorDatabaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Pessoa Login(LoginViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {                
                string sql = @"
                    SELECT P.*, A.IdAdmin 
                    FROM Pessoa P 
                    INNER JOIN Administrador A ON P.IdPessoa = A.IdAdmin 
                    WHERE P.Email = @Email AND P.SenhaHash = @SenhaHash";
                
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Email", model.Email ?? string.Empty);
                cmd.Parameters.AddWithValue("@SenhaHash", model.Senha ?? string.Empty);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
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
                }

                return null; 
            }
        }

       
    }
}