using Dapper;
using FirebirdSql.Data.FirebirdClient;
using RastreioDePersonalizadas.Contextos;
using RastreioDePersonalizadas.Entidades;
using RastreioDePersonalizadas.Fachadas.Contrato;
using RastreioDePersonalizadas.Utilitarios.Excecoes;

namespace RastreioDePersonalizadas.Fachadas
{
    public class NotaFiscalPedidoFachada : INotaFiscalPedidoFachada
    {
        #region Membros
        private readonly ContextoDeBancoDeDados _contextoDeBancoDeDados;
        #endregion

        #region Construtores
        /// <summary>
        /// Cria um construtor passando o contexto para fins de acesso ao banco de dados.
        /// </summary>  
        public NotaFiscalPedidoFachada(ContextoDeBancoDeDados contextoDeBancoDeDados)
        {
            _contextoDeBancoDeDados = contextoDeBancoDeDados;
        }

        /// <summary>
        /// Cria um construtor sem argumentos para flexibilizar o código
        /// </summary>
        public NotaFiscalPedidoFachada() { }

        #endregion

        #region Métodos
        internal string SelecionarNotaFiscal(int numeroNotaFiscal)
        {
           
            using (var conexao = new FbConnection(new ContextoDeBancoDeDados().MontarStringDeConexao("saojose")))
            {
                var qry = @$"SELECT f.nfe_xml
                    FROM nfe f
                    INNER JOIN NOTAS_VENDAS n on n.ntv_numero = f.nfe_ntv_numero                   
                    WHERE  n.ntv_notafiscal = {numeroNotaFiscal}";

                var xmlDaNota = conexao.Query(qry).Select(p => p.NFE_XML).FirstOrDefault();

                return xmlDaNota;
            }
        }

        public void Incluir(int numeroNotaFiscal, int pedidoID)
        {
            try
            {
                NotaFiscalPedidoEntidade entNotaFiscalPedido = new NotaFiscalPedidoEntidade();

                var xmlDaNotaFiscal = SelecionarNotaFiscal(numeroNotaFiscal);

                if (string.IsNullOrWhiteSpace(xmlDaNotaFiscal))
                {
                    throw new PropriedadeRequeridaExcecao($"Não existe nenhum xml disponível para a nota fiscal número {numeroNotaFiscal}");
                }

                entNotaFiscalPedido.NumeroPedido = pedidoID;
                entNotaFiscalPedido.Xml = xmlDaNotaFiscal;
                entNotaFiscalPedido.NumeroNotaFiscal = numeroNotaFiscal;

                _contextoDeBancoDeDados.NotaFiscalPedido.Add(entNotaFiscalPedido);
                _contextoDeBancoDeDados.SaveChanges();
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


        public void Alterar(int numeroNotaFiscal, int numeroPedido)
        {
            try
            {
                NotaFiscalPedidoEntidade entNotaFiscalPedido = new NotaFiscalPedidoEntidade();

                var xmlDaNotaFiscal = SelecionarNotaFiscal(numeroNotaFiscal);

                if (string.IsNullOrWhiteSpace(xmlDaNotaFiscal))
                {
                    throw new PropriedadeRequeridaExcecao($"Não existe nenhum xml disponível para a nota fiscal número {numeroNotaFiscal}");
                }

                var entNotaFiscalPedido_Original = _contextoDeBancoDeDados.NotaFiscalPedido.Where(entP => entP.NumeroPedido == numeroPedido).FirstOrDefault();

                if (entNotaFiscalPedido_Original == null)
                {
                    throw new PropriedadeRequeridaExcecao($"Não existe nenhuma nota fiscal para o pedido {numeroPedido}");
                }

                entNotaFiscalPedido.NumeroPedido = entNotaFiscalPedido_Original.NumeroPedido;
                entNotaFiscalPedido.Xml = xmlDaNotaFiscal;
                entNotaFiscalPedido.NumeroNotaFiscal = numeroNotaFiscal;

                _contextoDeBancoDeDados.NotaFiscalPedido.Update(entNotaFiscalPedido);
                _contextoDeBancoDeDados.SaveChanges();
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
        #endregion
    }
}
