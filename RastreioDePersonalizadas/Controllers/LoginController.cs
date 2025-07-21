using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RastreioDePersonalizadas.Entidades;
using RastreioDePersonalizadas.Servicos;
using RastreioDePersonalizadas.Utilitarios.Excecoes;
using Microsoft.AspNetCore.Authorization;
using RastreioDePersonalizadas.Utilitarios.EnumExtensions;
using RastreioDePersonalizadas.Contextos;

namespace RastreioDePersonalizadas.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly ContextoDeBancoDeDados _contextoDeBancoDeDados;
        public LoginController(ContextoDeBancoDeDados contextoDeBancoDeDados)
        {
            _contextoDeBancoDeDados = contextoDeBancoDeDados;
        }

        [HttpGet]
        public IActionResult LoginDeUsuario() => View();

        [HttpPost]
        public async Task<IActionResult> LoginDeUsuario(LoginDeUsuarioEntidade loginDeUsuario)
        {
            try
            {
                //Caso a entidade venha null
                if (loginDeUsuario == null)
                {
                    //Informe o campo Usuário ou Senha.
                    throw new PropriedadeRequeridaExcecao("Informe o campo Usuário ou Senha");
                }

                //Caso o campo Usuário venha vazio ou em branco
                if (string.IsNullOrWhiteSpace(loginDeUsuario.Usuario))
                {
                    //Informe o campo Usuário
                    throw new PropriedadeRequeridaExcecao("Informe o campo Usuário");
                }

                //Caso o campo Senha venha vazio ou em branco
                if (string.IsNullOrWhiteSpace(loginDeUsuario.Senha))
                {
                    //Informe o campo Senha
                    throw new PropriedadeRequeridaExcecao("Informe o campo Senha");
                }

                //Chama o serviço de login de usuário da API para realizar o login ao sistema.
                LoginDeUsuarioServico srvLoginDeUsuario = new LoginDeUsuarioServico();

                //Obtém as informações do usuário logado e atribui a sua permissão baseado no que está retornando da API.
                var informacoesDoUsuarioLogado = await srvLoginDeUsuario.RealizarLoginDoUsuario(loginDeUsuario);

                // Serializar o objeto para JSON e armazenar na sessão
                HttpContext.Session.SetString("UsuarioLogado", JsonSerializer.Serialize<LoginDeUsuarioEntidade>(informacoesDoUsuarioLogado));

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, informacoesDoUsuarioLogado.Usuario),
                    new Claim(ClaimTypes.Role, informacoesDoUsuarioLogado.PermissaoDeUsuario.GetDescription())
                };

                var claimsIdentity = new ClaimsIdentity(claims, "loginDeUsuario");

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                new LogController(_contextoDeBancoDeDados).GravarLog(new LogEntidade
                {
                    Usuario = informacoesDoUsuarioLogado.Usuario,
                    Detalhe = "Login efetuado com sucesso.",
                    Data = DateTime.Now,
                });

                return RedirectToAction("Index", "Principal", informacoesDoUsuarioLogado);
            }
            catch (PropriedadeRequeridaExcecao ex)
            {
                TempData["Erro"] = ex.Message;
                return View();
            }

        }

        public IActionResult LogoutDeUsuario()
        {
            HttpContext.SignOutAsync();
            new LogController(_contextoDeBancoDeDados).GravarLog(new LogEntidade
            {
                Usuario = User.Identity.Name,
                Data = DateTime.Now,
                Detalhe = "Logout efetuado com sucesso!"
            });
            return View(nameof(LoginDeUsuario));
        }
    }
}
