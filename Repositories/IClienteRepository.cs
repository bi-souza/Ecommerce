namespace Ecommerce.Repositories
{
    using Ecommerce.Models;

    public interface IClienteRepository
    {
        Cliente Login(LoginViewModel model);
        void Cadastrar(Cliente cliente);
        Cliente BuscarPorEmail(string email);
    }
}
