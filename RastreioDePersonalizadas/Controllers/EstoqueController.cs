using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RastreioDePersonalizadas.Contextos;
using RastreioDePersonalizadas.Entidades;
using RastreioDePersonalizadas.Fachadas;
using RastreioDePersonalizadas.ObjetoDeTransferencia;
using RastreioDePersonalizadas.Utilitarios.Excecoes;

namespace RastreioDePersonalizadas.Controllers
{
    [Authorize(Roles = "EstoqueSJC, EstoqueMG, Administrador")]
    public class EstoqueController : Controller
    {
        private readonly ContextoDeBancoDeDados _contextoDeBancoDeDados;
        PrincipalController _principalController;

        public EstoqueController(ContextoDeBancoDeDados contextoDeBancoDeDados)
        {
            _contextoDeBancoDeDados = contextoDeBancoDeDados;
            _principalController = new PrincipalController(_contextoDeBancoDeDados);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(int numeroDoPedido, string localConexao, string modeloChave, string responsavel)
        {
            try
            {
                //Caso o número do pedido seja menor ou igual a 0.
                if (numeroDoPedido <= 0)
                {
                    throw new PropriedadeRequeridaExcecao("Informe o número do pedido");
                }
                //Caso o local de conexão não tenha sido informado.
                if (string.IsNullOrWhiteSpace(localConexao))
                {
                    throw new PropriedadeRequeridaExcecao("Informe o local de conexão");
                }
                //Caso o modelo da chave não tenha sido informado.
                if (string.IsNullOrWhiteSpace(modeloChave))
                {
                    throw new PropriedadeRequeridaExcecao("Informe o modelo da chave");
                }

                //Caso o personalizador não tenha sido informado
                if (string.IsNullOrWhiteSpace(responsavel))
                {
                    throw new PropriedadeRequeridaExcecao("Informe o responsável do pedido");
                }

                //Caso o modelo da chave seja 2 ou 4 e o personalizador não seja o sales
                if ((modeloChave == "MOD2" || modeloChave == "MOD4") && responsavel != "Sales")
                {
                    throw new PropriedadeRequeridaExcecao("O modelo 2 e o modelo 4 só pode ter como responsável o Sales");
                }

                var existePedidoComOMesmoNumeroInformado =
                    (from entPedido_ in _contextoDeBancoDeDados.Pedidos.AsQueryable()
                     where entPedido_.PDV_NUMERO == numeroDoPedido
                     select entPedido_).Any();

                if (existePedidoComOMesmoNumeroInformado)
                {
                    throw new PropriedadeRequeridaExcecao("Não é permitido ter mais de um pedido com o mesmo número.");
                }

                var formularioDePesquisaOTE = new FormularioDePesquisaOTE
                {
                    NumeroDoPedido = numeroDoPedido,
                    LocalDaConexao = localConexao,
                    ModeloDaChave = modeloChave,
                    Responsavel = responsavel
                };

                return RedirectToAction("Incluir", formularioDePesquisaOTE);
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
                return View();
            }
        }

        [HttpGet]
        public IActionResult Incluir(FormularioDePesquisaOTE formularioDePesquisaOTE)
        {
            try
            {
                //Cria a instância para a fachada de pedidods
                var facPedidoFachada = new PedidoFachada();
                //Obtém a lista de pedidos baseado no numero do pedido, da conexão e do modelo da chave.
                var entPedido = facPedidoFachada.Selecionar(formularioDePesquisaOTE.NumeroDoPedido, formularioDePesquisaOTE.LocalDaConexao);

                //Caso não tenha identificado nenhum registro para o pedido informado
                if (entPedido == null)
                {
                    throw new PropriedadeRequeridaExcecao($"Não existe registros para o pedido {formularioDePesquisaOTE.NumeroDoPedido}");
                }

                //Seta o usuário conectado para a propriedade Usuario dentro da entidade de Pedido
                entPedido.Usuario = User.Identity.Name;

                //Seta a data atual para a propriedade Data dentro da entidade de Pedido
                entPedido.Data = DateTime.Now;

                //Seta o responsável pela demanda
                entPedido.Responsavel = formularioDePesquisaOTE.Responsavel;

                entPedido.Modelo = formularioDePesquisaOTE.ModeloDaChave;

                //Seta a filial responsável pela demanda
                entPedido.Filial = formularioDePesquisaOTE.LocalDaConexao == "saojose" ? "São José dos Campos" : "Sapucaí Mirim";

                // Enviar os status para a View
                ViewBag.Status = _principalController.SelecionarStatus(entPedido);

                return View(entPedido);
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
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult Incluir(PedidoEntidade entPedido, List<string> SelectedStatuses)
        {
            try
            {
                var facPedido = new PedidoFachada(_contextoDeBancoDeDados);
                var entStatusDoPedido = new StatusDoPedidoEntidade();

                if (SelectedStatuses.Count() > 1)
                {
                    throw new PropriedadeRequeridaExcecao("Não é possível selecionar mais que 1 status.");
                }

                if (SelectedStatuses.Count <= 0)
                {
                    throw new PropriedadeRequeridaExcecao("Informe pelo menos 1 status para o pedido.");
                }

                if (SelectedStatuses[0] != "1")
                {
                    throw new PropriedadeRequeridaExcecao("O status inicial deve ser obrigátoriamente marcado como Pedido Recebido");
                }

                switch (SelectedStatuses.Select(p => p).FirstOrDefault())
                {
                    case "1":
                        entStatusDoPedido.Status_1 = true;
                        entStatusDoPedido.Status_ID = 1;
                        entPedido.Status_ID = 1;
                        entStatusDoPedido.Usu_1 = User.Identity.Name;
                        entStatusDoPedido.DataStatus_1 = DateTime.Now;
                        break;
                    case "2":
                        entStatusDoPedido.Status_2 = true;
                        entStatusDoPedido.Status_ID = 2;
                        entPedido.Status_ID = 2;
                        entStatusDoPedido.Usu_2 = User.Identity.Name;
                        entStatusDoPedido.DataStatus_2 = DateTime.Now;
                        break;
                    case "3":
                        entStatusDoPedido.Status_3 = true;
                        entStatusDoPedido.Status_ID = 3;
                        entPedido.Status_ID = 3;
                        entStatusDoPedido.Usu_3 = User.Identity.Name;
                        entStatusDoPedido.DataStatus_3 = DateTime.Now;
                        break;
                    case "4":
                        entStatusDoPedido.Status_4 = true;
                        entStatusDoPedido.Status_ID = 4;
                        entPedido.Status_ID = 4;
                        entStatusDoPedido.Usu_4 = User.Identity.Name;
                        entStatusDoPedido.DataStatus_4 = DateTime.Now;
                        break;
                    case "5":
                        entStatusDoPedido.Status_5 = true;
                        entStatusDoPedido.Status_ID = 5;
                        entPedido.Status_ID = 5;
                        entStatusDoPedido.Usu_5 = User.Identity.Name;
                        entStatusDoPedido.DataStatus_5 = DateTime.Now;
                        break;
                    case "6":
                        entStatusDoPedido.Status_6 = true;
                        entStatusDoPedido.Status_ID = 6;
                        entPedido.Status_ID = 6;
                        entStatusDoPedido.Usu_6 = User.Identity.Name;
                        entStatusDoPedido.DataStatus_6 = DateTime.Now;
                        break;
                    case "7":
                        entStatusDoPedido.Status_7 = true;
                        entStatusDoPedido.Status_ID = 7;
                        entPedido.Status_ID = 7;
                        entStatusDoPedido.Usu_7 = User.Identity.Name;
                        entStatusDoPedido.DataStatus_7 = DateTime.Now;
                        break;
                    case "8":
                        entStatusDoPedido.Status_8 = true;
                        entStatusDoPedido.Status_ID = 8;
                        entPedido.Status_ID = 8;
                        entStatusDoPedido.Usu_8 = User.Identity.Name;
                        entStatusDoPedido.DataStatus_8 = DateTime.Now;
                        break;
                    case "9":
                        entStatusDoPedido.Status_9 = true;
                        entStatusDoPedido.Status_ID = 9;
                        entPedido.Status_ID = 9;
                        entStatusDoPedido.Usu_9 = User.Identity.Name;
                        entStatusDoPedido.DataStatus_9 = DateTime.Now;
                        break;
                    case "10":
                        entStatusDoPedido.Status_10 = true;
                        entStatusDoPedido.Status_ID = 10;
                        entPedido.Status_ID = 10;
                        entStatusDoPedido.Usu_10 = User.Identity.Name;
                        entStatusDoPedido.DataStatus_10 = DateTime.Now;
                        break;
                }

                facPedido.Incluir(entPedido, entStatusDoPedido);

                ViewBag.Status = _principalController.SelecionarStatus(entPedido);

                new LogController(_contextoDeBancoDeDados).GravarLog(new LogEntidade
                {
                    Usuario = User.Identity.Name,
                    Data = DateTime.Now,
                    Detalhe = $"Incluído o pedido {entPedido.ID} com as seguintes informações" +
                    $"ID:{entPedido.ID}," +
                    $"PDV_NUMERO: {entPedido.PDV_NUMERO}," +
                    $"REP_NOME: {entPedido.REP_NOME}," +
                    $"PDV_OBS1: {entPedido.PDV_OBS1}," +
                    $"PDV_OBS2: {entPedido.PDV_OBS2}," +
                    $"PDV_OBS3: {entPedido.PDV_OBS3}," +
                    $"PDV_OBS4: {entPedido.PDV_OBS4}," +
                    $"MODELO: {entPedido.Modelo}," +
                    $"QUANTIDADE_YALE: {entPedido.QUANTIDADE_YALE}," +
                    $"QUANTIDADE_TETRA: {entPedido.QUANTIDADE_TETRA}," +
                    $"STATUS_ID: {entPedido.Status_ID}," +
                    $"FILIAL: {entPedido.Filial}," +
                    $"RESPONSAVEL: {entPedido.Responsavel}," +
                    $"NOTA_FISCAL: {entPedido.Nota_Fiscal}," +
                    $"VOLUME: {entPedido.Volume}," +
                    $"ROMANEIO: {entPedido.Romaneio}.",

                });

                return RedirectToAction(nameof(Listar));
            }
            catch (PropriedadeRequeridaExcecao ex)
            {
                ViewBag.Status = _principalController.SelecionarStatus(entPedido);
                TempData["Erro"] = ex.Message;
                new LogController(_contextoDeBancoDeDados).GravarLog(new LogEntidade
                {
                    Usuario = User.Identity.Name,
                    Data = DateTime.Now,
                    Detalhe = ex.Message,

                });
                return View(entPedido);
            }
            catch (Exception ex)
            {
                ViewBag.Status = _principalController.SelecionarStatus(entPedido);
                TempData["Erro"] = "Erro inesperado: " + ex.Message;
                new LogController(_contextoDeBancoDeDados).GravarLog(new LogEntidade
                {
                    Usuario = User.Identity.Name,
                    Data = DateTime.Now,
                    Detalhe = ex.Message,
                });
                return View(entPedido);
            }
        }

        [HttpGet]
        public IActionResult Listar()
        {
            var lstPedido = _contextoDeBancoDeDados.Pedidos.AsQueryable();
            var lstStatus = _contextoDeBancoDeDados.Status.AsQueryable();

            var entPedidoAConsiderar =
                (from entPedido in lstPedido
                join entStatus in lstStatus on entPedido.Status_ID equals entStatus.ID
                select new PedidoOTE
                {
                    ID = entPedido.ID,
                    Data = entPedido.Data,
                    PDV_NUMERO = entPedido.PDV_NUMERO,
                    PDV_OBS1 = entPedido.PDV_OBS1,
                    PDV_OBS2 = entPedido.PDV_OBS2,
                    PDV_OBS3 = entPedido.PDV_OBS3,
                    PDV_OBS4 = entPedido.PDV_OBS4,
                    QUANTIDADE_TETRA = entPedido.QUANTIDADE_TETRA,
                    QUANTIDADE_YALE = entPedido.QUANTIDADE_YALE,
                    REP_NOME = entPedido.REP_NOME,
                    Status = entStatus.Status_Nome,
                    Usuario = entPedido.Usuario,
                    Volume = entPedido.Volume,
                    Nota_Fiscal = entPedido.Nota_Fiscal,
                    Romaneio = entPedido.Romaneio
                }).OrderByDescending(entP => entP.ID);

            return View(entPedidoAConsiderar);
        }

        [HttpGet]
        public IActionResult Alterar(int ID)
        {
            try
            {
                // Obtém o pedido com base no ID
                var entPedido = _contextoDeBancoDeDados.Pedidos
                    .FirstOrDefault(entP => entP.ID == ID);

                if (entPedido == null)
                {
                    throw new PropriedadeRequeridaExcecao($"Não existe pedido para o ID {ID}");
                }

                var lstEntProdutosPedido = _contextoDeBancoDeDados.ProdutosPedido.Where(p => p.NumeroPedido == entPedido.PDV_NUMERO).ToList();

                foreach (var entProdutoPedido in lstEntProdutosPedido)
                {
                    entPedido.Itens.Add(entProdutoPedido);
                }

                // Recupera o status atual do pedido
                var statusAtual = _contextoDeBancoDeDados.StatusDoPedido
                    .FirstOrDefault(x => x.Pedido_ID == entPedido.ID);

                // Se o status do pedido não existir, cria um novo (isso pode ocorrer se for um novo pedido)
                if (statusAtual == null)
                {
                    statusAtual = new StatusDoPedidoEntidade { Pedido_ID = entPedido.ID };
                }

                // Enviar os status para a View
                ViewBag.Status = _principalController.SelecionarStatus(entPedido);

                return View(entPedido);
            }
            catch (PropriedadeRequeridaExcecao ex)
            {
                new LogController(_contextoDeBancoDeDados).GravarLog(new LogEntidade
                {
                    Usuario = User.Identity.Name,
                    Data = DateTime.Now,
                    Detalhe = ex.Message,

                });
                TempData["Erro"] = ex.Message;
                return View();
            }
            catch (Exception ex)
            {               
                new LogController(_contextoDeBancoDeDados).GravarLog(new LogEntidade
                {
                    Usuario = User.Identity.Name,
                    Data = DateTime.Now,
                    Detalhe = ex.ToString(),
                });
                return View();
            }
        }

        [HttpPost]
        public IActionResult Alterar(PedidoEntidade entPedido, List<string> SelectedStatuses)
        {
            try
            {
                var facPedido = new PedidoFachada(_contextoDeBancoDeDados);
                var entStatusDoPedido = new StatusDoPedidoEntidade();

                var entPedido_Original = _contextoDeBancoDeDados.Pedidos.Where(entP => entP.ID == entPedido.ID).FirstOrDefault();

                //Verifica se os campos Nota Fiscal, Volume ou Romaneio não foram alterados
                if(entPedido.Nota_Fiscal == entPedido_Original.Nota_Fiscal && entPedido.Romaneio == entPedido_Original.Romaneio && entPedido.Volume == entPedido_Original.Volume)
                {
                    // Valida que somente 1 status foi selecionado
                    if (SelectedStatuses.Count != 1)
                    {
                        throw new PropriedadeRequeridaExcecao(SelectedStatuses.Count > 1
                            ? "Não é possível selecionar mais que 1 status"
                            : "Informe pelo menos 1 status para o pedido");
                    }
                }               

                // Busca o status atual do pedido
                var statusAtual = _contextoDeBancoDeDados.StatusDoPedido
                    .FirstOrDefault(s => s.Pedido_ID == entPedido.ID) ??
                    new StatusDoPedidoEntidade { Pedido_ID = entPedido.ID };

                // Atualiza os valores de status baseado nos selecionados
                foreach (var status in SelectedStatuses)
                {
                    switch (status)
                    {
                        case "1":
                            statusAtual.Status_1 = true;
                            statusAtual.Status_ID = 1;
                            entPedido.Status_ID = 1;
                            statusAtual.Usu_1 ??= User.Identity.Name;
                            statusAtual.DataStatus_1 = DateTime.Now;
                            break;
                        case "2":
                            statusAtual.Status_2 = true;
                            statusAtual.Status_ID = 2;
                            entPedido.Status_ID = 2;
                            statusAtual.Usu_2 ??= User.Identity.Name;
                            statusAtual.DataStatus_2 = DateTime.Now;
                            break;
                        case "3":
                            statusAtual.Status_3 = true;
                            statusAtual.Status_ID = 3;
                            entPedido.Status_ID = 3;
                            statusAtual.Usu_3 ??= User.Identity.Name;
                            statusAtual.DataStatus_3 = DateTime.Now;
                            break;
                        case "4":
                            statusAtual.Status_4 = true;
                            statusAtual.Status_ID = 4;
                            entPedido.Status_ID = 4;
                            statusAtual.Usu_4 ??= User.Identity.Name;
                            statusAtual.DataStatus_4 = DateTime.Now;
                            break;
                        case "5":
                            statusAtual.Status_5 = true;
                            statusAtual.Status_ID = 5;
                            entPedido.Status_ID = 5;
                            statusAtual.Usu_5 ??= User.Identity.Name;
                            statusAtual.DataStatus_5 = DateTime.Now;
                            break;
                        case "6":
                            statusAtual.Status_6 = true;
                            statusAtual.Status_ID = 6;
                            entPedido.Status_ID = 6;
                            statusAtual.Usu_6 ??= User.Identity.Name;
                            statusAtual.DataStatus_6 = DateTime.Now;
                            break;
                        case "7":
                            statusAtual.Status_7 = true;
                            statusAtual.Status_ID = 7;
                            entPedido.Status_ID = 7;
                            statusAtual.Usu_7 ??= User.Identity.Name;
                            statusAtual.DataStatus_7 = DateTime.Now;
                            break;
                        case "8":
                            statusAtual.Status_8 = true;
                            statusAtual.Status_ID = 8;
                            entPedido.Status_ID = 8;
                            statusAtual.Usu_8 ??= User.Identity.Name;
                            statusAtual.DataStatus_8 = DateTime.Now;
                            break;
                        case "9":
                            statusAtual.Status_9 = true;
                            statusAtual.Status_ID = 9;
                            entPedido.Status_ID = 9;
                            statusAtual.Usu_9 ??= User.Identity.Name;
                            statusAtual.DataStatus_9 = DateTime.Now;
                            break;
                        case "10":
                            statusAtual.Status_10 = true;
                            statusAtual.Status_ID = 10;
                            entPedido.Status_ID = 10;
                            statusAtual.Usu_10 ??= User.Identity.Name;
                            statusAtual.DataStatus_10 = DateTime.Now;
                            break;
                    }
                }

                //Atualiza a Hora para a hora atual da ação
                entPedido.Data = DateTime.Now;

                // Atualiza o pedido e o status no banco de dados
                var pedidoAlterado = facPedido.Alterar(entPedido, entPedido_Original, statusAtual);

                // Se o status do pedido não existir, cria um novo (isso pode ocorrer se for um novo pedido)
                if (statusAtual == null)
                {
                    statusAtual = new StatusDoPedidoEntidade { Pedido_ID = entPedido.ID };
                }
                // Enviar os status para a View
                ViewBag.Status = _principalController.SelecionarStatus(entPedido);

                new LogController(_contextoDeBancoDeDados).GravarLog(new LogEntidade
                {
                    Usuario = User.Identity.Name,
                    Data = DateTime.Now,
                    Detalhe = $"Alterado o pedido {entPedido.ID} com as seguintes informações" +
                    $"STATUS: {statusAtual.Status_ID}"
                });


                return View(pedidoAlterado);
            }
            catch (PropriedadeRequeridaExcecao ex)
            {
                ViewBag.Status = _principalController.SelecionarStatus(entPedido);

                // Exibe o erro se houver exceção
                TempData["Erro"] = ex.Message;

                new LogController(_contextoDeBancoDeDados).GravarLog(new LogEntidade
                {
                    Usuario = User.Identity.Name,
                    Data = DateTime.Now,
                    Detalhe = ex.Message,

                });

                return View(entPedido);
            }
            catch (Exception ex)
            {
                ViewBag.Status = _principalController.SelecionarStatus(entPedido);
                TempData["Erro"] = "Erro inesperado: " + ex.Message;
                new LogController(_contextoDeBancoDeDados).GravarLog(new LogEntidade
                {
                    Usuario = User.Identity.Name,
                    Data = DateTime.Now,
                    Detalhe = ex.Message,
                });
                return View(entPedido);
            }
        }

        [HttpGet]
        public IActionResult SelecionarPedido(string status, string representante, DateTime? dataInicio, DateTime? dataFim, int? numeroPedido, int? numeroRomaneio, int? notaFiscal)
        {
            var lstPedido = _contextoDeBancoDeDados.Pedidos.AsQueryable();
            var lstStatus = _contextoDeBancoDeDados.Status.AsQueryable();

            var entPedidoAConsiderar =
                from entPedido in lstPedido
                join entStatus in lstStatus on entPedido.Status_ID equals entStatus.ID
                select new PedidoOTE
                {
                    ID = entPedido.ID,
                    Data = entPedido.Data,
                    PDV_NUMERO = entPedido.PDV_NUMERO,
                    PDV_OBS1 = entPedido.PDV_OBS1,
                    PDV_OBS2 = entPedido.PDV_OBS2,
                    PDV_OBS3 = entPedido.PDV_OBS3,
                    PDV_OBS4 = entPedido.PDV_OBS4,
                    QUANTIDADE_TETRA = entPedido.QUANTIDADE_TETRA,
                    QUANTIDADE_YALE = entPedido.QUANTIDADE_YALE,
                    REP_NOME = entPedido.REP_NOME,
                    Status = entStatus.Status_Nome,
                    Usuario = entPedido.Usuario,
                    Volume = entPedido.Volume,
                    Nota_Fiscal = entPedido.Nota_Fiscal,
                    Romaneio = entPedido.Romaneio
                };

            //Filtra pelo Status
            if (!string.IsNullOrWhiteSpace(status))
            {
                entPedidoAConsiderar = entPedidoAConsiderar.Where(entP => entP.Status == status);
            }            
           
            //Filtra pelo Representante
            if (!string.IsNullOrWhiteSpace(representante))
            {
                var representanteAConsiderar = representante.ToLower();

                entPedidoAConsiderar = entPedidoAConsiderar.Where(entP => !string.IsNullOrEmpty(entP.REP_NOME) &&
                                         entP.REP_NOME.ToLower().Contains(representanteAConsiderar));
            }

            //Filtra pelo número do pedido
            if (numeroPedido.HasValue)
            {
                entPedidoAConsiderar = entPedidoAConsiderar.Where(entP => entP.PDV_NUMERO == numeroPedido.Value);
            }

            //Filtra pela nota fiscal
            if (notaFiscal.HasValue)
            {
                entPedidoAConsiderar = entPedidoAConsiderar.Where(entP => entP.Nota_Fiscal == notaFiscal.Value);
            }

            //Filtra pelo número do romaneio
            if (numeroRomaneio.HasValue)
            {
                entPedidoAConsiderar = entPedidoAConsiderar.Where(entP => entP.Romaneio == numeroRomaneio.Value);
            }

            //Filtra pela data
            if (dataInicio.HasValue && dataFim.HasValue)
            {
                entPedidoAConsiderar = entPedidoAConsiderar
                    .Where(entP => entP.Data >= dataInicio.Value.Date && entP.Data <= dataFim.Value.Date);
            }
            else if (dataInicio.HasValue)
            {
                entPedidoAConsiderar = entPedidoAConsiderar
                    .Where(entP => entP.Data.Value == dataInicio.Value.Date);
            }
            else if (dataFim.HasValue)
            {
                entPedidoAConsiderar = entPedidoAConsiderar
                    .Where(entP => entP.Data.Value == dataFim.Value.Date);
            }
          
            return View("Listar", entPedidoAConsiderar);
        }
    }
}
