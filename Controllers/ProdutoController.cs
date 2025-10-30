using Microsoft.AspNetCore.Mvc;
using Ecommerce.Repositories; 
using Ecommerce.Models;

namespace Ecommerce.Controllers;

public class ProdutoController : Controller
{
    private IProdutoRepository produtoRepository;
    
    public ProdutoController(IProdutoRepository produtoRepository)
    {
        this.produtoRepository = produtoRepository;
    }

    [HttpGet]
    public ActionResult Detalhes(int id)
    {
        var produto = produtoRepository.Read(id);

        if (produto == null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(produto);
    }
    
    [HttpGet]
    public IActionResult PorCategoria(int id)
    {        
        List<Produto> listaDeProdutos = produtoRepository.ReadAllByCategoria(id);        
        
        return View(listaDeProdutos);
    }
}