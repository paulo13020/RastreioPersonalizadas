using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RastreioDePersonalizadas.Contextos;
using RastreioDePersonalizadas.Entidades;
using RastreioDePersonalizadas.ObjetoDeTransferencia;
using RastreioDePersonalizadas.Utilitarios.Excecoes;
using System.Security.Claims;

namespace RastreioDePersonalizadas.Controllers
{
    [Authorize]
    public class PrincipalController : Controller
    {
        private readonly ContextoDeBancoDeDados _contextoDeBancoDeDados;

        #region Construtores
        public PrincipalController(ContextoDeBancoDeDados contextoDeBancoDeDados = null)
        {
            _contextoDeBancoDeDados = contextoDeBancoDeDados;
        }

        #endregion

        #region Métodos
        public IActionResult Index()
        {
            try
            {
                // Recuperando o valor do claim de Role
                var permissaoDoUsuario = User.Identities
                    .SelectMany(identity => identity.Claims)
                    .FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value; // Usando o operador ?. para evitar exceção

                // Verificando se o valor foi encontrado e realizando o switch
                switch (permissaoDoUsuario)
                {
                    case nameof(Enumeraveis.PermissaoDeUsuario.EstoqueMG): // Usando nameof para obter o nome do enum como string
                        return RedirectToAction("Listar", "Estoque");
                    case nameof(Enumeraveis.PermissaoDeUsuario.EstoqueSJC): // Usando nameof para obter o nome do enum como string
                        return RedirectToAction("Listar", "Estoque");
                    case nameof(Enumeraveis.PermissaoDeUsuario.Financeiro):
                        return RedirectToAction("Listar", "Financeiro");
                    case nameof(Enumeraveis.PermissaoDeUsuario.Vendedor):
                        return RedirectToAction("Index", "Vendedor");
                    case nameof(Enumeraveis.PermissaoDeUsuario.Administrador):
                        return RedirectToAction("Index", "Administrador");
                    case nameof(Enumeraveis.PermissaoDeUsuario.SemPermissao):
                        throw new ValorInvalidoExcecao("O usuário conectado não possui permissão de acesso ao sistema.");
                    default:
                        return RedirectToAction("LoginDeUsuario", "Login");
                }
            }
            catch (ValorInvalidoExcecao ex)
            {
                new LogController(_contextoDeBancoDeDados).GravarLog(new LogEntidade
                {
                    Usuario = User.Identity.Name,
                    Data = DateTime.Now,
                    Detalhe = ex.Message,
                });
                return RedirectToAction("LoginDeUsuario", "Login");
            }

        }

        /// <summary>
        /// Método responsável por selecionar os status que vai ser permitido para o pedido.
        /// </summary>
        /// <param name="entPedido"></param>
        /// <returns></returns>
        private List<int> SelecionarStatusPermitidoParaPersonalizar(PedidoEntidade entPedido)
        {
            try
            {
                var statusPermitidosMap = new Dictionary<(string Responsavel, string Modelo, string Filial), List<int>>
                {
                    { ("Eduardo", "MOD1", "Sapucaí Mirim"), new List<int> { 1, 6, 7, 11, 12 } },
                    { ("Eduardo", "MOD1", "São José dos Campos"), new List<int> { 1, 9, 5, 6, 7, 2, 10, 11, 12 } },
                    { ("Sales", "MOD1", "Sapucaí Mirim"), new List<int> { 1, 2, 10, 3, 4, 6, 7, 11, 9, 5, 12 } },
                    { ("Sales", "MOD2", "Sapucaí Mirim"), new List<int> { 1, 2, 10, 3, 4, 6, 7, 11, 9, 5, 12 } },
                    { ("Sales", "MOD4", "Sapucaí Mirim"), new List<int> { 1, 2, 10, 3, 4, 6, 7, 11, 9, 5, 12 } },
                    { ("Sales", "MOD5", "Sapucaí Mirim"), new List<int> { 1, 2, 10, 3, 4, 6, 7, 11, 9, 5, 12 } },
                    { ("Sales", "MOD1", "São José dos Campos"), new List<int> { 1, 3, 4, 6, 2, 11, 12 } },
                    { ("Sales", "MOD2", "São José dos Campos"), new List<int> { 1, 3, 4, 6, 2, 11, 12 } },
                    { ("Sales", "MOD4", "São José dos Campos"), new List<int> { 1, 3, 4, 6, 2, 11, 12 } },
                    { ("Sales", "MOD5", "São José dos Campos"), new List<int> { 1, 3, 4, 6, 2, 11, 12 } }

                };

                var key = (entPedido.Responsavel, entPedido.Modelo, entPedido.Filial);

                if (statusPermitidosMap.TryGetValue(key, out var statusPermitidos))
                {
                    return statusPermitidos;
                }
                else
                {
                    throw new PropriedadeRequeridaExcecao("Não foi possível identificar um status para personalizar o pedido.");
                }
            }
            catch (PropriedadeRequeridaExcecao ex)
            {
                TempData["Erro"] = ex.Message;
                new LogController(_contextoDeBancoDeDados).GravarLog(new LogEntidade
                {
                    Usuario = User.Identity.Name,
                    Data = DateTime.Now,
                    Detalhe = ex.Message,

                });
                return new List<int>();
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
                new LogController(_contextoDeBancoDeDados).GravarLog(new LogEntidade
                {
                    Usuario = User.Identity.Name,
                    Data = DateTime.Now,
                    Detalhe = ex.Message,

                });             
                throw new PropriedadeRequeridaExcecao(ex.Message);
            }
        }

        /// <summary>
        /// Método responsável por verificar se o status está marcado
        /// </summary>
        /// <param name="pedido"></param>
        /// <param name="statusID"></param>
        /// <returns></returns>
        public bool VerificarStatusMarcado(StatusDoPedidoEntidade pedido, int statusID)
        {
            return statusID switch
            {
                1 => pedido.Status_1 ?? false,
                2 => pedido.Status_2 ?? false,
                3 => pedido.Status_3 ?? false,
                4 => pedido.Status_4 ?? false,
                5 => pedido.Status_5 ?? false,
                6 => pedido.Status_6 ?? false,
                7 => pedido.Status_7 ?? false,
                8 => pedido.Status_8 ?? false,
                9 => pedido.Status_9 ?? false,
                10 => pedido.Status_10 ?? false,
                _ => false
            };
        }

        public DateTime? SelecionarDataDoStatus(PedidoEntidade entPedido, int statusID)
        {
            var entStatusDoPedido = _contextoDeBancoDeDados.StatusDoPedido.Where(entSP => entSP.Pedido_ID == entPedido.ID).FirstOrDefault();
            
            if (entStatusDoPedido == null)
            {
                return null;
            }

            DateTime? dataDoStatus = new DateTime();

            switch (statusID)
            {
                case 1:
                    dataDoStatus = entStatusDoPedido.DataStatus_1;
                    break;
                case 2:
                    dataDoStatus = entStatusDoPedido.DataStatus_2;
                    break;
                case 3:
                    dataDoStatus = entStatusDoPedido.DataStatus_3;
                    break;
                case 4:
                    dataDoStatus = entStatusDoPedido.DataStatus_4;
                    break;
                case 5:
                    dataDoStatus = entStatusDoPedido.DataStatus_5;
                    break;
                case 6:
                    dataDoStatus = entStatusDoPedido.DataStatus_6;
                    break;
                case 7:
                    dataDoStatus = entStatusDoPedido.DataStatus_7;
                    break;
                case 8:
                    dataDoStatus = entStatusDoPedido.DataStatus_8;
                    break;
                case 9:
                    dataDoStatus = entStatusDoPedido.DataStatus_9;
                    break;
                case 10:
                    dataDoStatus = entStatusDoPedido.DataStatus_10;
                    break;
                case 11:
                    dataDoStatus = entStatusDoPedido.DataStatus_11;
                    break;
                case 12:
                    dataDoStatus = entStatusDoPedido.DataStatus_11;
                    break;
            }

            return dataDoStatus;

        }

        /// <summary>
        /// Método responsável por selecionar os status para personalizar
        /// </summary>
        /// <param name="entPedido"></param>
        /// <returns></returns>
        public List<StatusAuxiliarOTE> SelecionarStatus(PedidoEntidade entPedido)
        {
            var statusPermitidos = SelecionarStatusPermitidoParaPersonalizar(entPedido);

            // Buscar os status do pedido na tabela StatusDoPedido
            var statusDoPedido = _contextoDeBancoDeDados.StatusDoPedido
                .FirstOrDefault(entP => entP.Pedido_ID == entPedido.ID);

            // Buscar os status permitidos                
            var statusLista = _contextoDeBancoDeDados.Status
                .ToList()  // Carrega todos os status para memória
                .Where(s => statusPermitidos.Contains(s.ID))  // Filtra os que estão na lista permitida
                .ToList(); // Executa a query no banco antes do processamento

            // Criar a lista de status
            var lstStatus = statusLista
                .Select(entStatus => new StatusAuxiliarOTE
                {
                    ID = entStatus.ID,
                    Nome = entStatus.Status_Nome,
                    Marcado = statusDoPedido != null &&
                              VerificarStatusMarcado(statusDoPedido, entStatus.ID),
                    StatusData = SelecionarDataDoStatus(entPedido, entStatus.ID),
                    IsLast = false, // Inicialmente falso
                    Data = entPedido.Data
                })
                .ToList();

            // Definir o último status marcado corretamente
            var ultimoMarcado = lstStatus.LastOrDefault(s => s.Marcado);

            if (ultimoMarcado != null)
            {
                ultimoMarcado.IsLast = true;
            }

            return lstStatus;
        }
    }
    #endregion
}
