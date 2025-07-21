using RastreioDePersonalizadas.Entidades;
using RastreioDePersonalizadas.Enumeraveis;

namespace RastreioDePersonalizadas.Fachadas.Contrato
{
    public interface IProdutoPedidoFachada
    {
        ProdutosPedidoEntidade Selecionar(int pedidoID, string localDeConexao);
        
        void Incluir(List<ProdutosPedidoEntidade> lstEntProdutoPedido, int pedidoID);

    }
}
