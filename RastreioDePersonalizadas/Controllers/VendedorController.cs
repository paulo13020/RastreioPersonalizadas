using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RastreioDePersonalizadas.Contextos;
using RastreioDePersonalizadas.Entidades;
using RastreioDePersonalizadas.ObjetoDeTransferencia;
using RastreioDePersonalizadas.Utilitarios.Excecoes;

namespace RastreioDePersonalizadas.Controllers
{
    [Authorize(Roles = "Vendedor, Administrador")]
    public class VendedorController : Controller
    {
        private readonly ContextoDeBancoDeDados _contextoDeBancoDeDados;
        PrincipalController _principalController;

        public VendedorController(ContextoDeBancoDeDados contextoDeBancoDeDados)
        {
            _contextoDeBancoDeDados = contextoDeBancoDeDados;
            _principalController = new PrincipalController(_contextoDeBancoDeDados);
        }

        [HttpGet]
        public IActionResult Index()
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
                    Modelo = entPedido.Modelo,                    
                    QUANTIDADE_TETRA = entPedido.QUANTIDADE_TETRA,
                    QUANTIDADE_YALE = entPedido.QUANTIDADE_YALE,
                    REP_NOME = entPedido.REP_NOME,
                    Status = entStatus.Status_Nome,
                    Usuario = entPedido.Usuario,
                    Filial = entPedido.Filial,
                    Responsavel = entPedido.Responsavel,
                    Romaneio = entPedido.Romaneio,
                    Nota_Fiscal = entPedido.Nota_Fiscal
                }).OrderByDescending(entP => entP.ID);

            return View(entPedidoAConsiderar);
        }

        [HttpGet]
        public IActionResult Listar(int ID)
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
                TempData["Erro"] = ex.Message;
                return View();
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

            //Filtra pelo número do pedido
            if (numeroPedido.HasValue)
            {
                entPedidoAConsiderar = entPedidoAConsiderar.Where(entP => entP.PDV_NUMERO == numeroPedido.Value);
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
            // Sua lógica de filtragem aqui.
            return View("Index", entPedidoAConsiderar);
        }
    }
}
