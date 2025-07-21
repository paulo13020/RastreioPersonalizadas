using System.ComponentModel.DataAnnotations.Schema;

namespace RastreioDePersonalizadas.Entidades
{
    [Table("NOTAFISCAL_PEDIDOS")]
    public class NotaFiscalPedidoEntidade
    {
        public int ID { get; set; }
        public int NumeroPedido { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public string Xml { get; set; }
    }
}
