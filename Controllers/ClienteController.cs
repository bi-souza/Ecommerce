using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Ecommerce.Repositories;

namespace Ecommerce.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteRepository repository;

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

        public ActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        
        public ActionResult Cadastro(Cliente cliente)
        {
            // repositorórias
            if (repository.BuscarPorEmail(cliente.Email) != null)
            {
                ViewBag.Error = "E-mail já cadastrado.";
                return View(cliente);
            }

            var senha = cliente.SenhaHash ?? string.Empty;
            var temLetra = System.Text.RegularExpressions.Regex.IsMatch(senha, "[A-Za-z]");
            var temNumero = System.Text.RegularExpressions.Regex.IsMatch(senha, "[0-9]");

            if (senha.Length < 7 || !temLetra || !temNumero)
            {
                ViewBag.Error = "A senha deve ter pelo menos 7 caracteres, incluindo pelo menos uma letra e um número.";
                return View(cliente);
            }

            if (cliente.ConfirmarSenha != cliente.SenhaHash)
            {
                ViewBag.Error = "As senhas não coincidem.";
                return View(cliente);
            }

            if (!ModelState.IsValid)
                return View(cliente);

            repository.Cadastrar(cliente);

            TempData["Mensagem"] = "Cadastro realizado com sucesso!";
            return RedirectToAction("Login", "Cliente");
        }
    }
}