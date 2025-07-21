using RastreioDePersonalizadas.Contextos;
using RastreioDePersonalizadas.Entidades;
using RastreioDePersonalizadas.Fachadas.Contrato;
using RastreioDePersonalizadas.Utilitarios.Excecoes;

namespace RastreioDePersonalizadas.Fachadas
{
    public class ProdutoPedidoFachada : IProdutoPedidoFachada
    {
        #region Membros
        private readonly ContextoDeBancoDeDados _contextoDeBancoDeDados;
        #endregion

        #region Construtores
        /// <summary>
        /// Cria um construtor passando o contexto para fins de acesso ao banco de dados.
        /// </summary>  
        public ProdutoPedidoFachada(ContextoDeBancoDeDados contextoDeBancoDeDados)
        {
            _contextoDeBancoDeDados = contextoDeBancoDeDados;
        }

        /// <summary>
        /// Cria um construtor sem argumentos para flexibilizar o código
        /// </summary>
        public ProdutoPedidoFachada() { }

        #endregion

        #region Métodos
        public void Incluir(List<ProdutosPedidoEntidade> lstEntProdutoPedido, int pedidoID)
        {
            try
            {
                foreach (var produtoPedido in lstEntProdutoPedido)
                {
                    produtoPedido.NumeroPedido = pedidoID;
                    _contextoDeBancoDeDados.ProdutosPedido.Add(produtoPedido);
                    _contextoDeBancoDeDados.SaveChanges();
                }
            }
            catch (PropriedadeRequeridaExcecao ex)
            {
                throw new PropriedadeRequeridaExcecao(ex.Message);
            }
            catch (Exception ex)
            {                
                throw new PropriedadeRequeridaExcecao(ex.Message);
            }
        }

        public ProdutosPedidoEntidade Selecionar(int pedidoID, string localDeConexao)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
