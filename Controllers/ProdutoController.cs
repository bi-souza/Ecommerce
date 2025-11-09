using Microsoft.AspNetCore.Mvc;
using Ecommerce.Repositories;
using Ecommerce.Models;
using Ecommerce.Filters;

namespace Ecommerce.Controllers;

public class ProdutoController : Controller
{
    private IProdutoRepository produtoRepository;
    private ICategoriaRepository categoriaRepository;
    private IAvaliacaoRepository avaliacaoRepository;

    public ProdutoController(IProdutoRepository produtoRepository, ICategoriaRepository categoriaRepository, IAvaliacaoRepository avaliacaoRepository)
    {
        this.produtoRepository = produtoRepository;
        this.categoriaRepository = categoriaRepository;
        this.avaliacaoRepository = avaliacaoRepository;
    }

    
    public ActionResult Index()
    {        
        var listaDeProdutos = produtoRepository.ReadAll();
        
        return View(listaDeProdutos);
    }

    [HttpGet]
    [RequireAdmin]
    public ActionResult Create()
    {        
        ViewBag.Categorias = categoriaRepository.ReadAll();
        return View();
    }

    [HttpPost]
    [RequireAdmin]
    public ActionResult Create(Produto model)
    {      
        produtoRepository.Create(model);
        
        return RedirectToAction("Index", "Home");
    }

    [HttpGet] 
    [RequireAdmin]
    public ActionResult Delete(int id)
    {
        produtoRepository.Delete(id);
        
        return RedirectToAction("Index");
    }

    [HttpGet]
    [RequireAdmin]
    public ActionResult Update(int id)
    {        
        var produto = produtoRepository.Read(id);

        if (produto == null)
        {
            return RedirectToAction("Index");
        }
        
        ViewBag.Categorias = categoriaRepository.ReadAll();
        
        return View(produto);
    }


    [HttpPost]
    [RequireAdmin]
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

        var listaDeAvaliacoes = avaliacaoRepository.ReadByProduto(id);

        ViewBag.Avaliacoes = listaDeAvaliacoes;

        return View(produto);
    }

    [HttpGet]
    public ActionResult PorCategoria(int id)
    {
        List<Produto> listaDeProdutos = produtoRepository.ReadAllByCategoria(id);

        return View(listaDeProdutos);
    }

        
    public ActionResult Search(string termo)
    {
        
        List<Produto> resultados = produtoRepository.Search(termo);        
        
        ViewBag.TermoBuscado = termo;
        
        return View(resultados);
    }
}