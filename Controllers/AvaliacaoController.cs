using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models; 
using Ecommerce.Repositories; 

namespace Ecommerce.Controllers; 

public class AvaliacaoController : Controller
{
    private IAvaliacaoRepository repository;
    private IPedidoRepository pedidoRepository;
    public AvaliacaoController(IAvaliacaoRepository repository, IPedidoRepository pedidoRepository)
    {
        this.repository = repository;
        this.pedidoRepository = pedidoRepository;
    }

    [HttpPost]
    public ActionResult Create(Avaliacao model)
    {

        var clienteId = (int)HttpContext.Session.GetInt32("ClienteId");

        model.ClienteId = clienteId;

        repository.Create(model);

        return RedirectToAction("Detalhes", "Produto", new { id = model.ProdutoId });
    }
    
    [HttpPost]
    public ActionResult Delete(int id) 
    {
        var clienteId = (int)HttpContext.Session.GetInt32("ClienteId");     
               
        var produtoId = id; 
        
        repository.Delete(clienteId, produtoId);        
        
        return RedirectToAction("Detalhes", "Produto", new { id = produtoId });
    }

   
    [HttpGet]
    public ActionResult Update(int id) 
    {

        var clienteId = (int)HttpContext.Session.GetInt32("ClienteId");     
                        
        var avaliacao = repository.Read(clienteId, id);      
        
        return View(avaliacao);
    }

    
    [HttpPost]
    public ActionResult Update(int id, Avaliacao avaliacao) 
    {
        
        var clienteId = (int)HttpContext.Session.GetInt32("ClienteId");        
        
        avaliacao.ProdutoId = id;
        avaliacao.ClienteId = clienteId;

        
        repository.Update(avaliacao);

        
        return RedirectToAction("Detalhes", "Produto", new { id = avaliacao.ProdutoId });
    }
}