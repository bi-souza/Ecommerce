namespace Ecommerce.Repositories; // (Use o seu namespace)

using Ecommerce.Models;

public interface IClienteRepository
{
    Cliente Login(LoginViewModel model); 
}