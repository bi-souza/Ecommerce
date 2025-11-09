using Microsoft.AspNetCore.Mvc;
using Ecommerce.Repositories; 

namespace Ecommerce.Controllers;

public class HomeController : Controller
{
    private ICategoriaRepository categoriaRepository;
    private IProdutoRepository produtoRepository;
    
    public HomeController(ICategoriaRepository categoriaRepository, IProdutoRepository produtoRepository)
    {
        this.categoriaRepository = categoriaRepository;
        this.produtoRepository = produtoRepository;
    }
    
    public ActionResult Index()
    {
        ViewBag.TodasCategorias = categoriaRepository.ReadAll();        
        
        var listaDeProdutos = produtoRepository.ReadAllDestaques();       
        
        return View(listaDeProdutos);
    }
}