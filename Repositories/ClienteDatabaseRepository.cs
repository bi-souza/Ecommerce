using Ecommerce.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Ecommerce.Repositories
{
    public class ClienteDatabaseRepository : DbConnection, IClienteRepository
    {       

        public ClienteDatabaseRepository(string? strConn) : base(strConn) { }       

        public Cliente Login(LoginViewModel model)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT P.*, C.IdCliente
                               FROM Pessoas P
                               INNER JOIN Clientes C ON P.IdPessoa = C.IdCliente
                               WHERE P.Email = @Email AND P.SenhaHash = @SenhaHash";                
                
            cmd.Parameters.AddWithValue("@Email", model.Email ?? string.Empty);
            cmd.Parameters.AddWithValue("@SenhaHash", model.Senha ?? string.Empty);

                
            SqlDataReader reader = cmd.ExecuteReader();
                
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

            return null;
            
        }


        public void Cadastrar(Cliente cliente)
        {         
                               
            SqlTransaction tx = conn.BeginTransaction();
            SqlCommand cmdPessoa = new SqlCommand();
            cmdPessoa.Connection = conn;
            cmdPessoa.Transaction = tx;
            
            try
            {                
                cmdPessoa.CommandText = @"
                    INSERT INTO Pessoas (Nome, Cpf, Email, Telefone, DataNasc, SenhaHash)
                    VALUES (@Nome, @Cpf, @Email, @Telefone, @DataNasc, @SenhaHash);
                    SELECT CAST(scope_identity() AS INT);";             
                                                    
                cmdPessoa.Parameters.AddWithValue("@Nome", cliente.Nome ?? string.Empty);
                cmdPessoa.Parameters.AddWithValue("@Cpf", cliente.Cpf ?? string.Empty);
                cmdPessoa.Parameters.AddWithValue("@Email", cliente.Email ?? string.Empty);
                cmdPessoa.Parameters.AddWithValue("@Telefone", cliente.Telefone ?? string.Empty);
                cmdPessoa.Parameters.AddWithValue("@DataNasc", cliente.DataNasc == default(DateTime) ? (object)DBNull.Value : cliente.DataNasc);
                cmdPessoa.Parameters.AddWithValue("@SenhaHash", cliente.SenhaHash ?? string.Empty);
                    
                    
                object result = cmdPessoa.ExecuteScalar();
                if (result == null) throw new InvalidOperationException("Falha ao obter o IdPessoa após a inserção.");
                int idCliente = Convert.ToInt32(result);
                
                cmdPessoa.Parameters.Clear();
                cmdPessoa.CommandText = "INSERT INTO Clientes (IdCliente) VALUES (@IdCliente)";                
                
                cmdPessoa.Parameters.AddWithValue("@IdCliente", idCliente);
                cmdPessoa.ExecuteNonQuery();

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

        public Cliente BuscarPorEmail(string email)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT P.*, C.IdCliente 
                    FROM Pessoas P
                    INNER JOIN Clientes C ON P.IdPessoa = C.IdCliente
                    WHERE P.Email = @Email";             
                                                    
            cmd.Parameters.AddWithValue("@Email", email);
                
            using (SqlDataReader reader = cmd.ExecuteReader()) 
            {
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
            }
            return null;            
        }

        public Cliente BuscarPorCpf(string cpf)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT P.*, C.IdCliente 
                    FROM Pessoas P
                    INNER JOIN Clientes C ON P.IdPessoa = C.IdCliente
                    WHERE P.Cpf = @Cpf";             
                                                    
            cmd.Parameters.AddWithValue("@Cpf", cpf);
                
            using (SqlDataReader reader = cmd.ExecuteReader()) 
            {
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
            }
            return null;            
        }

        public Cliente BuscarPorId(int id)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @" SELECT P.*, C.IdCliente 
                FROM Pessoas P
                INNER JOIN Clientes C ON P.IdPessoa = C.IdCliente
                WHERE C.IdCliente = @Id";    
                                
            cmd.Parameters.AddWithValue("@Id", id);

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
        public void Atualizar(Cliente cliente)
        {   
            
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            
            cmd.CommandText = @"UPDATE Pessoas
                               SET Nome = @Nome,
                                   Email = @Email,
                                   Telefone = @Telefone
                               WHERE IdPessoa = @IdCliente";        
                                                              
                
            cmd.Parameters.AddWithValue("@Nome", cliente.Nome ?? string.Empty);
            cmd.Parameters.AddWithValue("@Email", cliente.Email ?? string.Empty);
            cmd.Parameters.AddWithValue("@Telefone", cliente.Telefone ?? string.Empty);
            cmd.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);          
            
            cmd.ExecuteNonQuery();
            
        }

        public bool VerificarSenhaAtual(int id, string senhaAtual)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            
            cmd.CommandText =  "SELECT SenhaHash FROM Pessoas WHERE IdPessoa = @Id";                 
                
            cmd.Parameters.AddWithValue("@Id", id);
            
            object result = cmd.ExecuteScalar();
            if (result == null || result == DBNull.Value) return false;
            string senhaNoBanco = Convert.ToString(result);
            return senhaNoBanco == (senhaAtual ?? string.Empty);
            
        }

        public void AlterarSenha(int id, string novaSenha)
        {   
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "UPDATE Pessoas SET SenhaHash = @SenhaHash WHERE IdPessoa = @Id";               
               
            cmd.Parameters.AddWithValue("@SenhaHash", novaSenha ?? string.Empty);
            cmd.Parameters.AddWithValue("@Id", id);
            
            cmd.ExecuteNonQuery();            
        }

        public void Excluir(int id)
        {
            SqlTransaction tx = conn.BeginTransaction();
            SqlCommand cmdPessoa = new SqlCommand();
            cmdPessoa.Connection = conn;
            cmdPessoa.Transaction = tx;

            try
            {   
                cmdPessoa.CommandText = "DELETE FROM Clientes WHERE IdCliente = @Id";                                       
                               
                cmdPessoa.Parameters.AddWithValue("@Id", id);
                cmdPessoa.ExecuteNonQuery();
                
                cmdPessoa.Parameters.Clear();

                cmdPessoa.CommandText = "DELETE FROM Pessoas WHERE IdPessoa = @Id";
                cmdPessoa.Parameters.AddWithValue("@Id", id);
                cmdPessoa.ExecuteNonQuery();

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
