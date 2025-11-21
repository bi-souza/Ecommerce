using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Ecommerce.Models;
using Ecommerce.Repositories;

namespace Ecommerce.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly IProdutoRepository repository;

        public CarrinhoController(IProdutoRepository repository)
        {
            this.repository = repository;
        }

        [RequireLogin]
        public ActionResult Index()
        {
            var json = HttpContext.Session.GetString("CART");
            var cart = string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : (JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>());

            ViewBag.Total = cart.Sum(i => i.Subtotal);
            return View(cart);
        }

        [HttpGet]
        [RequireLogin]
        public ActionResult Create(int id)
        {
            var item = new CartItem { IdProduto = id, Quantidade = 1 };
            return View(item);
        }

        [HttpPost]
        [RequireLogin]
        public ActionResult Create(CartItem form, string? returnUrl)
        {
            var p = repository.Read(form.IdProduto);
            if (p == null)
                return RedirectToAction("Index", "Home");

            if (form.Quantidade > p.Estoque)
            {
                TempData["Msg"] =
                    $"Estoque insuficiente para '{p.NomeProduto}'. Disponível: {p.Estoque} unidades.";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            var json = HttpContext.Session.GetString("CART");
            var cart = string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : (JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>());

            var qtd = form.Quantidade < 1 ? 1 : form.Quantidade;

            var item = cart.FirstOrDefault(i => i.IdProduto == p.IdProduto);
            if (item == null)
            {
                cart.Add(new CartItem
                {
                    IdProduto     = p.IdProduto,
                    NomeProduto   = p.NomeProduto,
                    PrecoUnitario = p.Preco,
                    Quantidade    = qtd,
                    ImagemUrl     = p.ImagemUrl
                });
            }
            else
            {
                item.Quantidade += qtd;
            }

            HttpContext.Session.SetString("CART", JsonSerializer.Serialize(cart));
            TempData["Msg"] = "Produto adicionado ao carrinho!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Produto");
        }

        [HttpGet]
        [RequireLogin]
        public ActionResult Update(int id)
        {
            var json = HttpContext.Session.GetString("CART");
            var cart = string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : (JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>());

            var item = cart.FirstOrDefault(i => i.IdProduto == id);
            if (item == null) return RedirectToAction("Index");

            return View(item);
        }

        [HttpPost]
        [RequireLogin]
        public ActionResult Update(int id, CartItem form)
        {
            var json = HttpContext.Session.GetString("CART");
            var cart = string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : (JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>());

            var item = cart.FirstOrDefault(i => i.IdProduto == id);
            if (item != null)
            {
                item.Quantidade = form.Quantidade < 1 ? 1 : form.Quantidade;

                HttpContext.Session.SetString("CART", JsonSerializer.Serialize(cart));
            }

            return RedirectToAction("Index");
        }

        [RequireLogin]
        public ActionResult Delete(int id)
        {
            var json = HttpContext.Session.GetString("CART");
            var cart = string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : (JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>());

            cart = cart.Where(i => i.IdProduto != id).ToList();

            HttpContext.Session.SetString("CART", JsonSerializer.Serialize(cart));

            return RedirectToAction("Index");
        }
    }
}
