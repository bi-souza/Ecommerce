using Ecommerce.Models; 
namespace Ecommerce.Repositories;

public interface IAdministradorRepository
{
    Pessoa Login(LoginViewModel model);
}
    