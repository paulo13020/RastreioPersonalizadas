using RastreioDePersonalizadas.Entidades;
using RastreioDePersonalizadas.Enumeraveis;

namespace RastreioDePersonalizadas.Fachadas.Contrato
{
    public interface IPedidoFachada
    {
        PedidoEntidade Selecionar(int pedidoID, string localDeConexao);

        PedidoEntidade Incluir(PedidoEntidade entPedido, StatusDoPedidoEntidade entStatusDoPedido);

        PedidoEntidade Alterar(PedidoEntidade entPedido, PedidoEntidade entPedido_Original ,StatusDoPedidoEntidade entStatusDoPedido);
    }
}
