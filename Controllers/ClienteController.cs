using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models; 
using Ecommerce.Repositories; 

namespace Ecommerce.Controllers; 

public class ClienteController : Controller
{
    private IClienteRepository repository;
    
    public ClienteController(IClienteRepository repository)
    {
        this.repository = repository;
    }

    
    public ActionResult Login()
    {
        
        return View(new LoginViewModel());
    }

    
    [HttpPost]
    public ActionResult Login(LoginViewModel model)
    {
        
        Cliente cliente = repository.Login(model);

        
        if (cliente == null)
        {
            ViewBag.Error = "Usuário ou senha inválidos";
            return View(model);
        }

        
        HttpContext.Session.SetInt32("ClienteId", cliente.IdCliente);
        HttpContext.Session.SetString("NomeCliente", cliente.NomeCliente);

        
        return RedirectToAction("Index", "Home");
    }

    
    public ActionResult Logout()
    {
        
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}