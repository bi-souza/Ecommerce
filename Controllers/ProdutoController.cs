using Microsoft.AspNetCore.Mvc;
using Ecommerce.Repositories; 
using Ecommerce.Models;

namespace Ecommerce.Controllers;

public class ProdutoController : Controller
{
    private IProdutoRepository produtoRepository;
    
    private ICategoriaRepository categoriaRepository;

    public ProdutoController(IProdutoRepository produtoRepository, ICategoriaRepository categoriaRepository)
    {
        this.produtoRepository = produtoRepository;
        this.categoriaRepository = categoriaRepository;
    }

    public ActionResult Index()
    {        
        var listaDeProdutos = produtoRepository.ReadAll();
        
        return View(listaDeProdutos);
    }

    [HttpGet]
    public ActionResult Create()
    {        
        ViewBag.Categorias = categoriaRepository.Read();
        return View();
    }

    [HttpPost]
    public ActionResult Create(Produto model)
    {      
        produtoRepository.Create(model);
        
        return RedirectToAction("Index", "Home");
    }

    [HttpGet] 
    public ActionResult Delete(int id)
    {
        produtoRepository.Delete(id);
        
        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Update(int id)
    {        
        var produto = produtoRepository.Read(id);

        if (produto == null)
        {
            return RedirectToAction("Index");
        }
        
        ViewBag.Categorias = categoriaRepository.Read();
        
        return View(produto);
    }

    
    [HttpPost]
    public ActionResult Update(int id, Produto model)
    {        
        model.IdProduto = id;      
        
        produtoRepository.Update(model);
        
        return RedirectToAction("Index");
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
    public ActionResult PorCategoria(int id)
    {
        List<Produto> listaDeProdutos = produtoRepository.ReadAllByCategoria(id);

        return View(listaDeProdutos);
    }
}