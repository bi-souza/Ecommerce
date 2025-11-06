namespace Ecommerce.Repositories
{
    using Ecommerce.Models;

    public interface IClienteRepository
    {
        Cliente Login(LoginViewModel model);
        void Cadastrar(Cliente cliente);
        Cliente BuscarPorEmail(string email);
        Cliente BuscarPorId(int id);
        void Atualizar(Cliente cliente);
        void Excluir(int id);
        bool VerificarSenhaAtual(int id, string senhaAtual);
        void AlterarSenha(int id, string novaSenha);
    }
}
