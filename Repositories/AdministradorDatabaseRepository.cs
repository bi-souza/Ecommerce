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
                // AJUSTE CRÍTICO: INNER JOIN com a tabela Administrador
                // A query busca um registro na tabela Pessoa (P) que possua o Email e SenhaHash corretos
                // E, OBRIGATORIAMENTE, um registro correspondente na tabela Administrador (A)
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
                        // Mapeia os dados da Pessoa (que é o administrador logado)
                        // A propriedade IdPessoa e as demais propriedades são mapeadas de P.*
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

                return null; // Retorna null se não encontrar uma Pessoa com o papel de Administrador
            }
        }

       
    }
}