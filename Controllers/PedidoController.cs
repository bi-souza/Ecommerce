using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Ecommerce.Models;
using Ecommerce.Services;
using Ecommerce.Repositories;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Controllers
{
    public class PedidoController : Controller
    {
        private readonly PixService _pixService;
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoController(PixService pixService, IPedidoRepository pedidoRepository)
        {
            _pixService = pixService;
            _pedidoRepository = pedidoRepository;
        }

        [HttpGet]
        [RequireLogin]
        public IActionResult Create()
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            var papel     = HttpContext.Session.GetString("Papel");

            // Bloqueia admin de fechar pedido nesse fluxo
            if (papel == "Admin")
            {
                TempData["ModalMsg"] = "Você não pode finalizar pedidos neste fluxo. Por favor, acesse com uma conta de Cliente para efetuar a compra.";
                return RedirectToAction("Index", "Carrinho");
            }

            // Se não tiver cliente logado, manda pro login
            if (clienteId is null)
            {
                TempData["Msg"] = "Faça login para finalizar a compra.";
                return RedirectToAction("Login", "Cliente");
            }

            return RedirectToAction("Pagamento");
        }

        [HttpGet]
        [RequireLogin]
        public IActionResult Pagamento()
        {
            var cart = ReadCart();
            if (!cart.Any())
            {
                TempData["Msg"] = "Seu carrinho está vazio.";
                return RedirectToAction("Index", "Carrinho");
            }

            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId is null)
            {
                TempData["Msg"] = "Faça login para finalizar a compra.";
                return RedirectToAction("Login", "Cliente");
            }

            var total = cart.Sum(i => i.Subtotal);

            var pedidoVisualId = new Random().Next(1000, 9999);

            var qrBase64 = _pixService.GerarQrCode(total, pedidoVisualId.ToString());

            var vm = new PixViewModel
            {
                PedidoId     = pedidoVisualId,          // só para exibir na tela
                Total        = total,
                QrCodeBase64 = qrBase64,
                Payload      = $"Simulação PIX - Carrinho {pedidoVisualId}",
                Simulate     = true
            };

            return View(vm);
        }


        [HttpPost]
        [RequireLogin]
        public IActionResult ConfirmarPagamento()
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId is null)
            {
                TempData["Msg"] = "Faça login para finalizar a compra.";
                return RedirectToAction("Login", "Cliente");
            }

            var cart = ReadCart();
            if (!cart.Any())
            {
                TempData["Msg"] = "Seu carrinho está vazio.";
                return RedirectToAction("Index", "Carrinho");
            }

            var total = cart.Sum(i => i.Subtotal);

            int pedidoId;
            try
            {
                pedidoId = _pedidoRepository.CriarPedidoComItens(clienteId.Value, total, cart);

                _pedidoRepository.ConfirmarPagamento(pedidoId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO AO CONFIRMAR PAGAMENTO: " + ex.ToString());
                TempData["Msg"] = "Falha ao confirmar o pagamento.";
                return RedirectToAction("Index", "Carrinho");
            }

            HttpContext.Session.Remove("CART");

            TempData["Msg"] = $"Pagamento do pedido #{pedidoId} confirmado com sucesso!";

            return RedirectToAction("Confirmado");
        }


        [RequireLogin]
        public IActionResult Confirmado()
        {
            ViewBag.Msg = TempData["Msg"];
            return View();
        }

        [RequireLogin]
        public IActionResult Historico()
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId is null)
            {
                TempData["Msg"] = "Faça login para ver seus pedidos.";
                return RedirectToAction("Login", "Cliente");
            }

            var lista = _pedidoRepository.ObterHistorico(clienteId.Value);
            return View(lista);
        }


        private List<CartItem> ReadCart()
        {
            var json = HttpContext.Session.GetString("CART");
            return string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : (JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>());
        }
    }
}
