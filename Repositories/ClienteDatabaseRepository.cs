using Ecommerce.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Ecommerce.Repositories
{
    public class ClienteDatabaseRepository : IClienteRepository
    {
        private readonly string _connectionString;

        public ClienteDatabaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Cliente Login(LoginViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Cliente WHERE Email = @Email AND SenhaHash = @SenhaHash";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Email", model.Email ?? string.Empty);
                cmd.Parameters.AddWithValue("@SenhaHash", model.Senha ?? string.Empty);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Cliente
                        {
                            IdCliente = (int)reader["IdCliente"],
                            NomeCliente = reader["NomeCliente"].ToString(),
                            Email = reader["Email"].ToString(),
                            Cpf = reader["Cpf"].ToString(),
                            Telefone = reader["Telefone"].ToString(),
                            DataNasc = reader["DataNasc"] == DBNull.Value ? default : Convert.ToDateTime(reader["DataNasc"])
                        };
                    }
                }

                return null;
            }
        }


        public void Cadastrar(Cliente cliente)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Cliente (NomeCliente, Cpf, Email, Telefone, DataNasc, SenhaHash)
                               VALUES (@NomeCliente, @Cpf, @Email, @Telefone, @DataNasc, @SenhaHash)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@NomeCliente", cliente.NomeCliente);
                cmd.Parameters.AddWithValue("@Cpf", cliente.Cpf);
                cmd.Parameters.AddWithValue("@Email", cliente.Email);
                cmd.Parameters.AddWithValue("@Telefone", cliente.Telefone);
                cmd.Parameters.AddWithValue("@DataNasc", cliente.DataNasc);
                cmd.Parameters.AddWithValue("@SenhaHash", cliente.SenhaHash);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public Cliente BuscarPorEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Cliente WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Cliente
                    {
                        IdCliente = (int)reader["IdCliente"],
                        NomeCliente = reader["NomeCliente"].ToString(),
                        Email = reader["Email"].ToString(),
                        Cpf = reader["Cpf"].ToString()
                    };
                }

                return null;
            }
        }
    }
}
