using Dapper;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore;
using RastreioDePersonalizadas.Contextos;
using RastreioDePersonalizadas.Entidades;
using RastreioDePersonalizadas.Fachadas.Contrato;
using RastreioDePersonalizadas.Utilitarios.Excecoes;

namespace RastreioDePersonalizadas.Fachadas
{
    public class PedidoFachada : IPedidoFachada
    {
        #region Membros
        private readonly ContextoDeBancoDeDados _contextoDeBancoDeDados;
        #endregion

        #region Construtores
        /// <summary>
        /// Cria um construtor passando o contexto para fins de acesso ao banco de dados.
        /// </summary>  
        public PedidoFachada(ContextoDeBancoDeDados contextoDeBancoDeDados)
        {
            _contextoDeBancoDeDados = contextoDeBancoDeDados;
        }

        /// <summary>
        /// Cria um construtor sem argumentos para flexibilizar o código
        /// </summary>
        public PedidoFachada() { }
        #endregion

        #region Métodos
        /// <summary>
        /// Método responsável por selecionar um pedido direto da fonte do microsys.
        /// Esse método não deve ser utilizado para consultar pedidos persistidos, pois a base de dados é diferente.
        /// </summary>
        /// <returns></returns>
        public PedidoEntidade Selecionar(int pedidoID, string localDeConexao)
        {
            using (var conexao = new FbConnection(new ContextoDeBancoDeDados().MontarStringDeConexao(localDeConexao)))
            {
                // Primeiro, obtenha os dados básicos do pedido
                var qryPedido = @$"
        select 
            ped.pdv_numero, 
            r.rep_nome,
            ped.pdv_obs1, 
            ped.pdv_obs2,
            ped.pdv_obs3,
            ped.pdv_obs4,
            sum(case when p.pro_nivel3 = '1' then i.pvi_quantidade else 0 end) as quantidade_yale,
            sum(case when p.pro_nivel3 = '2' then i.pvi_quantidade else 0 end) as quantidade_tetra
        from 
            pedidos_vendas ped
            inner join pedidos_vendas_itens i on i.pvi_numero = ped.pdv_numero
            inner join produtos p on p.pro_codigo = i.pvi_pro_codigo
            inner join representantes r on r.rep_codigo = ped.pdv_rep_codigo
        where 
            ped.pdv_numero = '{pedidoID}'
        group by 
            ped.pdv_numero, r.rep_nome, ped.pdv_obs1, ped.pdv_obs2, ped.pdv_obs3, ped.pdv_obs4";

                var pedido = conexao.Query<PedidoEntidade>(qryPedido).FirstOrDefault();

                if (pedido != null)
                {
                    // Consulta para os itens
                    var qryItens = @$"
            SELECT 
                pvi_pro_codigo AS CodigoProduto,
                pvi_quantidade AS Quantidade
            FROM 
                pedidos_vendas_itens
            WHERE 
                pvi_numero = '{pedidoID}'";

                    pedido.Itens = conexao.Query<ProdutosPedidoEntidade>(qryItens).Distinct().ToList();
                }

                return pedido;
            }
        }

        /// <summary>
        /// Método responsável por válidar as regras de negócio antes de persistir um registro.
        /// </summary>
        /// <param name="entPedido"></param>
        /// <exception cref="PropriedadeRequeridaExcecao"></exception>
        public void Validar(PedidoEntidade entPedido)
        {
            //Verifica se o campo Numero do Pedido foi informado.
            if (entPedido.PDV_NUMERO <= 0)
            {
                throw new PropriedadeRequeridaExcecao("Informe o campo Número do Pedido");
            }

            //Verifica se o campo Observação foi informado.
            if (string.IsNullOrWhiteSpace(entPedido.PDV_OBS1))
            {
                throw new PropriedadeRequeridaExcecao("Informe o campo Observação");
            }

            //Verifica se o campo Modelo da Chave foi informado.
            if (string.IsNullOrWhiteSpace(entPedido.PDV_OBS2))
            {
                throw new PropriedadeRequeridaExcecao("Informe o campo Modelo da Chave");
            }

            //Verifica se o campo Representante foi informado
            if (string.IsNullOrWhiteSpace(entPedido.REP_NOME))
            {
                throw new PropriedadeRequeridaExcecao("Informe o campo Representante");
            }

            //Verifica se o campo Usuário foi informado
            if (string.IsNullOrWhiteSpace(entPedido.Usuario))
            {
                throw new PropriedadeRequeridaExcecao("Informe o campo Usuário");
            }

            //Verifica se o campo Quantidade Tetra está negativo, pois pode informar como 0.
            if (entPedido.QUANTIDADE_TETRA < 0)
            {
                throw new PropriedadeRequeridaExcecao("Informe o campo Quantidade Tetra");
            }

            //Verifica se o campo Quantidade Yale está negativo, pois pode informar como 0.
            if (entPedido.QUANTIDADE_YALE < 0)
            {
                throw new PropriedadeRequeridaExcecao("Informe o campo Quantidade Yale");
            }

            //Verifica se o campo Romaneio está sendo informado
            if (entPedido.Romaneio <= 0)
            {
                throw new PropriedadeRequeridaExcecao("Informe o campo Romaneio");
            }

            //Verifica se o campo Volume foi informado
            if (entPedido.Volume <= 0)
            {
                throw new PropriedadeRequeridaExcecao("Informe o campo Volume");
            }

            //Verifica seo campo Nota Fiscal foi informado
            if (entPedido.Nota_Fiscal <= 0)
            {
                throw new PropriedadeRequeridaExcecao("Informe o campo Nota Fiscal");
            }
        }

        /// <summary>
        /// Inclui um Pedido.
        /// </summary>
        /// <param name="entPedido"></param>
        /// <returns></returns>
        public PedidoEntidade Incluir(PedidoEntidade entPedido, StatusDoPedidoEntidade entStatusDoPedido)
        {
            // Valida o pedido antes de salvar
            this.Validar(entPedido);

            // Inicia uma transação para garantir consistência
            using (var transaction = _contextoDeBancoDeDados.Database.BeginTransaction())
            {
                try
                {
                    // Salva o pedido
                    var entPedidoPersistido = _contextoDeBancoDeDados.Pedidos.Add(entPedido);
                    _contextoDeBancoDeDados.SaveChanges();

                    // Atualiza o ID do pedido no status
                    entStatusDoPedido.Pedido_ID = entPedido.ID;

                    // Salva o status do pedido
                    _contextoDeBancoDeDados.StatusDoPedido.Add(entStatusDoPedido);
                    _contextoDeBancoDeDados.SaveChanges();                   

                    //Chama a fachada de ProdutoPedido para incluir os produtos do pedido.
                    new ProdutoPedidoFachada(_contextoDeBancoDeDados).Incluir(entPedido.Itens, entPedido.PDV_NUMERO);

                    //Chama da fachada de NotaFiscal_Pedido para incluir o XML da nota fiscal do pedido em uma tabela separada
                    new NotaFiscalPedidoFachada(_contextoDeBancoDeDados).Incluir(entPedido.Nota_Fiscal, entPedido.PDV_NUMERO);

                    // Confirma a transação
                    transaction.Commit();

                    return entPedidoPersistido.Entity;
                }
                catch (PropriedadeRequeridaExcecao)
                {
                    transaction.Rollback();
                    throw; // mantém o tipo
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new PropriedadeRequeridaExcecao("Erro ao salvar pedido e status: " + ex.Message);
                }
            }
        }

        public PedidoEntidade Alterar(PedidoEntidade entPedido, PedidoEntidade entPedido_Original ,StatusDoPedidoEntidade entStatusDoPedido)
        {
            // Valida o pedido antes de salvar
            this.Validar(entPedido);

            using (var transaction = _contextoDeBancoDeDados.Database.BeginTransaction())
            {
                try
                {     
                    //Caso o campo Nota Fiscal esteja sendo alterado
                    if(entPedido_Original.Nota_Fiscal != entPedido.Nota_Fiscal)
                    {
                        //Chama a fachada de nota fiscal de pedido pra alteração
                        new NotaFiscalPedidoFachada(_contextoDeBancoDeDados).Alterar(entPedido.Nota_Fiscal, entPedido.PDV_NUMERO);
                    }

                    // Desanexa a entidade anterior se já estiver sendo rastreada
                    var trackedEntity = _contextoDeBancoDeDados.ChangeTracker
                        .Entries<PedidoEntidade>()
                        .FirstOrDefault(e => e.Entity.ID == entPedido.ID);

                    if (trackedEntity != null)
                    {
                        trackedEntity.State = EntityState.Detached;
                    }

                    // Salva o pedido
                    var entPedidoPersistido = _contextoDeBancoDeDados.Pedidos.Update(entPedido);
                    _contextoDeBancoDeDados.SaveChanges();

                    // Atualiza ou insere o status do pedido
                    if (entStatusDoPedido.ID == 0)
                    {
                        _contextoDeBancoDeDados.StatusDoPedido.Add(entStatusDoPedido);
                    }
                    else
                    {                       
                        _contextoDeBancoDeDados.StatusDoPedido.Update(entStatusDoPedido);
                    }

                    _contextoDeBancoDeDados.SaveChanges();

                    // Confirma a transação
                    transaction.Commit();

                    return entPedidoPersistido.Entity;
                }
                catch (PropriedadeRequeridaExcecao ex)
                {
                    // Em caso de erro, desfaz a transação
                    transaction.Rollback();
                    throw new PropriedadeRequeridaExcecao(ex.Message);                    
                }
                catch (Exception ex)
                {
                    throw new PropriedadeRequeridaExcecao(ex.Message);
                }
            }
        }
        #endregion
    }
}
