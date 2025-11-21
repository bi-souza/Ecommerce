using Ecommerce.Models; 
namespace Ecommerce.Repositories;

public interface IAdministradorRepository
{
    Administrador Login(LoginViewModel model);
}
    