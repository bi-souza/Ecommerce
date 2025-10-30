using Ecommerce.Models; 
namespace Ecommerce.Repositories;

public interface ICategoriaRepository
{
    List<Categoria> Read(); 
    Categoria Read(int id); 
}