using Ecommerce.Models; 

namespace Ecommerce.Repositories;

public interface IProdutoRepository
{
    List<Produto> ReadAllByCategoria(int categoriaId);
    List<Produto> ReadAllDestaques();
    Produto Read(int id);
}