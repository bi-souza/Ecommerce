using Microsoft.AspNetCore.Mvc;
using Ecommerce.Repositories;

namespace Ecommerce.Controllers;

public class CategoriaController : Controller
{
    private ICategoriaRepository categoriaRepository;
    private IProdutoRepository produtoRepository;

    public CategoriaController(ICategoriaRepository categoriaRepository, IProdutoRepository produtoRepository)
    {
        this.categoriaRepository = categoriaRepository;
        this.produtoRepository = produtoRepository;
    }
  
}