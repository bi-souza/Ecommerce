using Ecommerce.Models; 

namespace Ecommerce.Repositories;

public interface IProdutoRepository
{
    List<Produto> ReadAllByCategoria(int categoriaId);
    List<Produto> ReadAllDestaques();
    List<Produto> ReadAll();
    Produto Read(int id);
    void Create(Produto model);
    bool Delete(int id);
    void Update(Produto model);
    List<Produto> Search(string termo);
    ProdutoAvaliacaoViewModel ReadComAvaliacao(int id);
    List<EstoqueCriticoViewModel> ReadEstoqueCritico(int nivelMinimo, int diasRecentes);
}