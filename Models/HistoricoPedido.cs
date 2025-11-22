using System;

namespace Ecommerce.Models
{
    public class HistoricoPedido
    {
        public int IdPedido { get; set; }
        public DateTime DataPedido { get; set; }
        public decimal ValorTotal { get; set; }
        public string StatusPedido { get; set; } = string.Empty;
        public int QuantidadeItens { get; set; }
        public string Produtos { get; set; } = string.Empty;
    }
}
