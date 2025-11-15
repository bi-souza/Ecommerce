using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
            var cfg = HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration
                    ?? throw new InvalidOperationException("IConfiguration não disponível.");
            var cs = cfg.GetConnectionString("default")
                    ?? throw new InvalidOperationException("ConnectionString 'default' não encontrada.");

            using var con = new SqlConnection(cs);
            con.Open();
            using var tx = con.BeginTransaction();

            try
            {
                decimal total;
                using (var getCmd = new SqlCommand("SELECT ValorTotal FROM Pedidos WHERE IdPedido=@id", con, tx))
                {
                    getCmd.Parameters.AddWithValue("@id", pedidoId);
                    var obj = getCmd.ExecuteScalar();
                    if (obj is null) throw new Exception("Pedido não encontrado.");
                    total = Convert.ToDecimal(obj);
                }
                using (var pagCmd = new SqlCommand(@"
                    INSERT INTO Pagamentos (ValorPago, TipoPagamento, PedidoId)
                    VALUES (@valor, @tipo, @pedido);", con, tx))
                {
                    pagCmd.Parameters.AddWithValue("@valor", total);
                    pagCmd.Parameters.AddWithValue("@tipo",  "Pix");
                    pagCmd.Parameters.AddWithValue("@pedido", pedidoId);
                    pagCmd.ExecuteNonQuery();
                }

                using (var updCmd = new SqlCommand(@"
                    UPDATE Pedidos SET StatusPedido='Concluído' WHERE IdPedido=@id;", con, tx))
                {
                    updCmd.Parameters.AddWithValue("@id", pedidoId);
                    updCmd.ExecuteNonQuery();
                }
                
                tx.Commit();
                HttpContext.Session.Remove("CART");
                TempData["Msg"] = $"Pagamento do pedido #{pedidoId} confirmado com sucesso!";
                return RedirectToAction("Confirmado");
            }
            catch
            {
                try { tx.Rollback(); } catch { /* ignore */ }
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
