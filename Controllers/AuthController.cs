using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Ecommerce.Repositories;

namespace Ecommerce.Controllers;

public class AuthController : Controller
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IAdministradorRepository _administradorRepository;

    public AuthController(IClienteRepository clienteRepository, IAdministradorRepository administradorRepository)
    {
        _clienteRepository = clienteRepository;
        _administradorRepository = administradorRepository;
    }

    public ActionResult Login()    {
        
        return View("~/Views/Cliente/Login.cshtml", new LoginViewModel());
    }
   

    [HttpPost]
    public ActionResult Login(LoginViewModel model)
    {
        
        Cliente cliente = _clienteRepository.Login(model);

        if (cliente != null)
        {
            HttpContext.Session.SetInt32("ClienteId", cliente.IdCliente);
            HttpContext.Session.SetString("NomeCliente", cliente.Nome);           
            HttpContext.Session.SetString("Papel", "Cliente"); 
            
            return RedirectToAction("Index", "Home");
        }
        
        
        Pessoa admin = _administradorRepository.Login(model); 

        if (admin != null)
        {
            HttpContext.Session.SetInt32("AdminId", admin.IdPessoa);
            HttpContext.Session.SetString("NomeAdmin", admin.Nome);
            HttpContext.Session.SetString("Papel", "Admin");             
            
            return RedirectToAction("Index", "Home"); 
        }
        
        ViewBag.Error = "Usuário ou senha inválidos";
        return View("~/Views/Cliente/Login.cshtml", model);
    }

    
    public ActionResult Logout()
    {
        HttpContext.Session.Clear();        
        return RedirectToAction("Login");
    }
    

}
