using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using Ecommerce.Models;
using Ecommerce.Services; 
using Ecommerce; 


namespace Ecommerce.Controllers
{
    public class PedidoController : Controller
    {
        
        private readonly PixService _pixService;

        public PedidoController(PixService pixService)
        {
            _pixService = pixService;
        }

        [HttpGet]
        [RequireLogin]
        public IActionResult Create()
        {
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

            var cfg = HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration
                    ?? throw new InvalidOperationException("IConfiguration não disponível.");
            var cs = cfg.GetConnectionString("default")
                    ?? throw new InvalidOperationException("ConnectionString 'default' não encontrada.");

            int pedidoId;

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                using var tx = con.BeginTransaction();

                try
                {
                    pedidoId = InsertPedido(con, tx, clienteId.Value, total);

                    foreach (var it in cart)
                        InsertItem(con, tx, pedidoId, it);

                    tx.Commit();
                }
                catch
                {
                    try { tx.Rollback(); } catch { /* ignore */ }
                    TempData["Msg"] = "Erro ao criar o pedido. Tente novamente.";
                    return RedirectToAction("Index", "Carrinho");
                }
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
                using (var getCmd = new SqlCommand("SELECT ValorTotal FROM Pedido WHERE IdPedido=@id", con, tx))
                {
                    getCmd.Parameters.AddWithValue("@id", pedidoId);
                    var obj = getCmd.ExecuteScalar();
                    if (obj is null) throw new Exception("Pedido não encontrado.");
                    total = Convert.ToDecimal(obj);
                }
                using (var pagCmd = new SqlCommand(@"
                    INSERT INTO Pagamento (ValorPago, TipoPagamento, PedidoId)
                    VALUES (@valor, @tipo, @pedido);", con, tx))
                {
                    pagCmd.Parameters.AddWithValue("@valor", total);
                    pagCmd.Parameters.AddWithValue("@tipo",  "Pix");
                    pagCmd.Parameters.AddWithValue("@pedido", pedidoId);
                    pagCmd.ExecuteNonQuery();
                }

                using (var updCmd = new SqlCommand(@"
                    UPDATE Pedido SET StatusPedido='Concluído' WHERE IdPedido=@id;", con, tx))
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

        
        private static int InsertPedido(SqlConnection con, SqlTransaction tx, int clienteId, decimal total)
        {
            using var cmd = new SqlCommand(@"
                INSERT INTO Pedido (DataPedido, ValorTotal, StatusPedido, ClienteId)
                VALUES (GETDATE(), @total, 'Aguardando pagamento', @cli);
                SELECT SCOPE_IDENTITY();", con, tx);

            cmd.Parameters.AddWithValue("@total", total);
            cmd.Parameters.AddWithValue("@cli", clienteId);

            return System.Convert.ToInt32(cmd.ExecuteScalar());
        }

        
        private static void InsertItem(SqlConnection con, SqlTransaction tx, int pedidoId, CartItem it)
        {
            using var cmd = new SqlCommand(@"
                INSERT INTO ItensPedido (Quantidade, PrecoUnit, ValorItem, PedidoId, ProdutoId)
                VALUES (@qtd, @preco, @valor, @pedido, @prod);", con, tx);

            cmd.Parameters.AddWithValue("@qtd", it.Quantidade);
            cmd.Parameters.AddWithValue("@preco", it.PrecoUnitario);
            cmd.Parameters.AddWithValue("@valor", it.Subtotal);
            cmd.Parameters.AddWithValue("@pedido", pedidoId);
            cmd.Parameters.AddWithValue("@prod", it.IdProduto);

            cmd.ExecuteNonQuery();
        }
    }
}
