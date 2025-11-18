using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Ecommerce.Models;
using Ecommerce.Services; 
using Ecommerce.Repositories; 


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

            if (clienteId is null && papel == "Admin")
            {   
                TempData["ModalMsg"] = "Você não pode finalizar pedidos neste fluxo. Por favor, acesse com uma conta de Cliente para efetuar a compra.";

                return RedirectToAction("Index", "Carrinho");
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
            
            int pedidoId;
            try
            {
                
                pedidoId = _pedidoRepository.CriarPedidoComItens(clienteId.Value, total, cart);
            }
            catch (Exception) 
            {
                
                TempData["Msg"] = "Erro ao criar o pedido. Tente novamente.";
                return RedirectToAction("Index", "Carrinho");
            }
           
            var qrBase64 = _pixService.GerarQrCode(total, pedidoId.ToString());

            var vm = new PixViewModel
            {
                PedidoId     = pedidoId,
                Total        = total,
                QrCodeBase64 = qrBase64,
                Payload      = $"Simulação PIX - Pedido {pedidoId}",
                Simulate     = true
            };

            return View(vm); 
        }
        

        [HttpPost]
        [RequireLogin]     
        public IActionResult ConfirmarPagamento(int pedidoId)
        {
            try
            {                
                _pedidoRepository.ConfirmarPagamento(pedidoId);                 
               
                HttpContext.Session.Remove("CART");
                TempData["Msg"] = $"Pagamento do pedido #{pedidoId} confirmado com sucesso!";
                return RedirectToAction("Confirmado");
            }
            catch 
            {                
                TempData["Msg"] = "Falha ao confirmar o pagamento.";
                return RedirectToAction("Index", "Carrinho");
            }
        }

        [RequireLogin]
        public IActionResult Confirmado()
        {
            ViewBag.Msg = TempData["Msg"];
            return View(); 
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
