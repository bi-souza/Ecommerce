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
    private IPedidoRepository pedidoRepository;

    public ProdutoController(IProdutoRepository produtoRepository, ICategoriaRepository categoriaRepository, IAvaliacaoRepository avaliacaoRepository, IPedidoRepository pedidoRepository)
    {
        this.produtoRepository = produtoRepository;
        this.categoriaRepository = categoriaRepository;
        this.avaliacaoRepository = avaliacaoRepository;
        this.pedidoRepository = pedidoRepository;
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
        if (ModelState.IsValid) 
        {        
            produtoRepository.Create(model);
            return RedirectToAction("Index");
        }
        
        ViewBag.Categorias = categoriaRepository.ReadAll();
        return View(model);       
        
    }

    [HttpPost] 
    [RequireAdmin]
    public ActionResult Delete(int id)
    {
       bool deleted = produtoRepository.Delete(id);

        if (deleted)
        {
            return RedirectToAction("Index");
        }

        else
        {
            TempData["DeleteError"] = $"A exclusão do produto (ID: {id}) não foi permitida. O item possui histórico de pedidos e não pode ser removido.";
            return RedirectToAction("Index");
        }       
        
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

        if (ModelState.IsValid) 
        {     
            produtoRepository.Update(model);
            return RedirectToAction("Index");
        }

        ViewBag.Categorias = categoriaRepository.ReadAll();
        return View(model);         
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
        
        var clienteId = HttpContext.Session.GetInt32("ClienteId");

        bool podeAvaliar = false;

        if (clienteId.HasValue)
        {            
            podeAvaliar = pedidoRepository.ClienteComprouProduto(clienteId.Value, id);
        }
        
        ViewBag.PodeAvaliar = podeAvaliar;

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