using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RastreioDePersonalizadas.Entidades
{
    [Table("PRODUTOS_PEDIDO")]
    public class ProdutosPedidoEntidade
    {
        public int ID { get; set; }
        public int NumeroPedido { get; set; }
        [Display(Name = "Código do Produto")]
        public int CodigoProduto { get; set; }
        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

    }
}
