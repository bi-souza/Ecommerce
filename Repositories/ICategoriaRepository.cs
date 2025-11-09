using Ecommerce.Models; 
namespace Ecommerce.Repositories;

public interface ICategoriaRepository
{
    List<Categoria> ReadAll();
    void Create(Categoria categoria);
    Categoria ReadById(int id);
    void Update(Categoria categoria);
    void Delete(int id);  
}