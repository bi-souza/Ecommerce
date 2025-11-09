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
                string sql = @"SELECT P.*, C.IdCliente
                               FROM Pessoa P
                               INNER JOIN Cliente C ON P.IdPessoa = C.IdCliente
                               WHERE P.Email = @Email AND P.SenhaHash = @SenhaHash";
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
                            Nome = reader["Nome"].ToString(),
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
                conn.Open();                
                
                SqlTransaction tx = conn.BeginTransaction();
                
                try
                {
                    
                    string sqlPessoa = @"
                        INSERT INTO Pessoa (Nome, Cpf, Email, Telefone, DataNasc, SenhaHash)
                        VALUES (@Nome, @Cpf, @Email, @Telefone, @DataNasc, @SenhaHash);
                        SELECT CAST(scope_identity() AS INT);"; 
                        
                    using (SqlCommand cmdPessoa = new SqlCommand(sqlPessoa, conn, tx))
                    {
                        
                        cmdPessoa.Parameters.AddWithValue("@Nome", cliente.Nome ?? string.Empty);
                        cmdPessoa.Parameters.AddWithValue("@Cpf", cliente.Cpf ?? string.Empty);
                        cmdPessoa.Parameters.AddWithValue("@Email", cliente.Email ?? string.Empty);
                        cmdPessoa.Parameters.AddWithValue("@Telefone", cliente.Telefone ?? string.Empty);
                        cmdPessoa.Parameters.AddWithValue("@DataNasc", cliente.DataNasc);
                        cmdPessoa.Parameters.AddWithValue("@SenhaHash", cliente.SenhaHash ?? string.Empty);
                        
                        
                        object result = cmdPessoa.ExecuteScalar();
                        if (result == null) throw new InvalidOperationException("Falha ao obter o IdPessoa após a inserção.");
                        int idCliente = Convert.ToInt32(result);

                        
                        string sqlCliente = "INSERT INTO Cliente (IdCliente) VALUES (@IdCliente)";

                        using (SqlCommand cmdCliente = new SqlCommand(sqlCliente, conn, tx))
                        {
                            cmdCliente.Parameters.AddWithValue("@IdCliente", idCliente);
                            cmdCliente.ExecuteNonQuery();
                        }
                    }

                    
                    tx.Commit();
                }
                catch (Exception)
                {
                    
                    try
                    {
                        tx.Rollback();
                    }
                    catch 
                    { 
                        // Ignorar erros no Rollback.
                    }
                    
                    
                    throw; 
                }
            }
        }

        public Cliente BuscarPorEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                
                string sql = @"
                    SELECT P.*, C.IdCliente 
                    FROM Pessoa P
                    INNER JOIN Cliente C ON P.IdPessoa = C.IdCliente
                    WHERE P.Email = @Email";
                    
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Cliente
                    {
                        IdCliente = (int)reader["IdCliente"],
                        
                        Nome = reader["Nome"].ToString(), 
                        Email = reader["Email"].ToString(),
                        Cpf = reader["Cpf"].ToString()
                    };
                }

                return null;
            }
        }
        public Cliente BuscarPorId(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                
                string sql = @"
                    SELECT P.*, C.IdCliente 
                    FROM Pessoa P
                    INNER JOIN Cliente C ON P.IdPessoa = C.IdCliente
                    WHERE C.IdCliente = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Cliente
                        {
                            IdCliente = (int)reader["IdCliente"],                            
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
        public void Atualizar(Cliente cliente)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                
                string sql = @"UPDATE Pessoa
                               SET Nome = @Nome,
                                   Email = @Email,
                                   Telefone = @Telefone
                               WHERE IdPessoa = @IdCliente";
                
                SqlCommand cmd = new SqlCommand(sql, conn);
                
                cmd.Parameters.AddWithValue("@Nome", cliente.Nome ?? string.Empty);
                cmd.Parameters.AddWithValue("@Email", cliente.Email ?? string.Empty);
                cmd.Parameters.AddWithValue("@Telefone", cliente.Telefone ?? string.Empty);
                cmd.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public bool VerificarSenhaAtual(int id, string senhaAtual)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                
                string sql = "SELECT SenhaHash FROM Pessoa WHERE IdPessoa = @Id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value) return false;
                string senhaNoBanco = Convert.ToString(result);
                return senhaNoBanco == (senhaAtual ?? string.Empty);
            }
        }

        public void AlterarSenha(int id, string novaSenha)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                
                string sql = "UPDATE Pessoa SET SenhaHash = @SenhaHash WHERE IdPessoa = @Id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SenhaHash", novaSenha ?? string.Empty);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Excluir(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction tx = conn.BeginTransaction(); 
                try
                {
                    
                    string sqlCliente = "DELETE FROM Cliente WHERE IdCliente = @Id";
                    using (SqlCommand cmdCliente = new SqlCommand(sqlCliente, conn, tx))
                    {
                        cmdCliente.Parameters.AddWithValue("@Id", id);
                        cmdCliente.ExecuteNonQuery();
                    }
                    
                    
                    string sqlPessoa = "DELETE FROM Pessoa WHERE IdPessoa = @Id";
                    using (SqlCommand cmdPessoa = new SqlCommand(sqlPessoa, conn, tx))
                    {
                        cmdPessoa.Parameters.AddWithValue("@Id", id);
                        cmdPessoa.ExecuteNonQuery();
                    }

                    tx.Commit(); 
                }
                catch (Exception)
                {
                    try { tx.Rollback(); } catch { /* ignore */ }
                    throw;
                }
            }
        }
    }
}
