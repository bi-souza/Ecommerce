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
            return View(new Cliente());
        }

        [HttpPost]

        public ActionResult Cadastro(Cliente cliente)
        {
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
        public ActionResult Perfil()
        {
            var id = HttpContext.Session.GetInt32("ClienteId");
            if (id == null)
                return RedirectToAction("Login");
            var cliente = repository.BuscarPorId(id.Value);
            if (cliente == null) // ⚠️ Adicionado para evitar passar null para a view
        {
            HttpContext.Session.Clear(); // Limpa sessão inválida
            return RedirectToAction("Login");
        }
            return View(cliente);
        }

        [HttpPost]
        public ActionResult EditarPerfil(Cliente cliente)
        {
            if (!ModelState.IsValid)
                return View("Perfil", cliente);
            var existente = repository.BuscarPorEmail(cliente.Email);
            if (existente != null && existente.IdCliente != cliente.IdCliente)
            {
                ViewBag.Error = "E-mail já cadastrado por outro usuário.";
                return View("Perfil", cliente);
            }
            repository.Atualizar(cliente);
            TempData["Mensagem"] = "Perfil atualizado com sucesso!";
            return RedirectToAction("Perfil");
        }

        [HttpPost]
        public ActionResult AlterarSenha(string senhaAtual, string novaSenha, string confirmarSenha)
        {
            var id = HttpContext.Session.GetInt32("ClienteId");
            if (id == null)
                return RedirectToAction("Login");
            if (novaSenha != confirmarSenha)
            {
                TempData["ErroSenha"] = "As novas senhas não conferem.";
                return RedirectToAction("Perfil");
            }
            if (novaSenha.Length < 7 || !System.Text.RegularExpressions.Regex.IsMatch(novaSenha, "[A-Za-z]") || !System.Text.RegularExpressions.Regex.IsMatch(novaSenha, "[0-9]"))
            {
                TempData["ErroSenha"] = "A nova senha deve ter pelo menos 7 caracteres, incluindo uma letra e um número.";
                return RedirectToAction("Perfil");
            }
            bool valida = repository.VerificarSenhaAtual(id.Value, senhaAtual);
            if (!valida)
            {
                TempData["ErroSenha"] = "A senha atual está incorreta.";
                return RedirectToAction("Perfil");
            }
            repository.AlterarSenha(id.Value, novaSenha);
            TempData["Mensagem"] = "Senha alterada com sucesso!";
            return RedirectToAction("Perfil");
        }

        [HttpPost]
        public ActionResult Excluir()
        {
            var id = HttpContext.Session.GetInt32("ClienteId");
            if (id != null)
            {
                repository.Excluir(id.Value);
                HttpContext.Session.Clear();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}