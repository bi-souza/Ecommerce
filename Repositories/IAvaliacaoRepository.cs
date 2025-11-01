using Ecommerce.Models; 
namespace Ecommerce.Repositories;

public interface IAvaliacaoRepository
{
    void Create(Avaliacao model);
    void Delete(int clienteId, int produtoId);
    void Update(Avaliacao model);
    Avaliacao Read(int clienteId, int produtoId);
    List<Avaliacao> ReadByProduto(int produtoId);
}