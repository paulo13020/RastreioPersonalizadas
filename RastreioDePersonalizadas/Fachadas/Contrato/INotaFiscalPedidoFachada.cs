using RastreioDePersonalizadas.Entidades;
using RastreioDePersonalizadas.Enumeraveis;

namespace RastreioDePersonalizadas.Fachadas.Contrato
{
    public interface INotaFiscalPedidoFachada
    {    
        void Incluir(int numeroNotaFiscal, int pedidoID);
        void Alterar(int numeroNotaFiscal, int pedidoID);
    }
}
