using System.Text;
using Newtonsoft.Json;
using RastreioDePersonalizadas.Entidades;
using RastreioDePersonalizadas.ObjetoDeTransferencia;
using RastreioDePersonalizadas.Utilitarios.Excecoes;

namespace RastreioDePersonalizadas.Servicos
{
    public class LoginDeUsuarioServico
    {
        /// <summary>
        /// Método responsável por realizar login de usuário através da API da dovale.
        /// </summary>
        /// <param name="loginDeUsuarioEntidade"></param>
        /// <returns>Retorna as informações do usuário</returns>
        public async Task<LoginDeUsuarioEntidade> RealizarLoginDoUsuario(LoginDeUsuarioEntidade loginDeUsuarioEntidade)
        {

            string url = "https://api.dovale.com.br/LoginUsuario1";

            var usuarioParaRequisicao = new
            {
                Usuario = loginDeUsuarioEntidade.Usuario,
                Senha = loginDeUsuarioEntidade.Senha,
            };

            // Serializa o objeto para JSON
            string usuarioParaRequisicaoSerializado = JsonConvert.SerializeObject(usuarioParaRequisicao);
            HttpContent content = new StringContent(usuarioParaRequisicaoSerializado, Encoding.UTF8, "application/json");

            using HttpClient client = new HttpClient();

            // Faz a requisição POST enviando o JSON no corpo
            HttpResponseMessage response = await client.PostAsync(url, content);

            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new PropriedadeRequeridaExcecao("Usuário ou senha inválido!");
            }

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            // Deserializa o JSON para o modelo
            var apiResponse = JsonConvert.DeserializeObject<ApiResponseOTE>(responseBody);

            // Obtém o primeiro usuário da lista (se existir)
            var informacoesDoUsuarioLogado = apiResponse?.InformacoesUsuario?.FirstOrDefault();

            if (informacoesDoUsuarioLogado == null)
            {
                throw new PropriedadeRequeridaExcecao("Informações do usuário não encontradas.");
            }

            // Preenche as informações do usuário
            loginDeUsuarioEntidade.Email = informacoesDoUsuarioLogado.EmailAddress;

            loginDeUsuarioEntidade.PermissaoDeUsuario =
                informacoesDoUsuarioLogado.PermissaoPerso == "1" ? Enumeraveis.PermissaoDeUsuario.EstoqueSJC :
                informacoesDoUsuarioLogado.PermissaoPerso == "2" ? Enumeraveis.PermissaoDeUsuario.EstoqueMG :
                informacoesDoUsuarioLogado.PermissaoPerso == "3" ? Enumeraveis.PermissaoDeUsuario.Vendedor :
                informacoesDoUsuarioLogado.PermissaoPerso == "4" ? Enumeraveis.PermissaoDeUsuario.Financeiro :
                informacoesDoUsuarioLogado.PermissaoPerso == "5" ? Enumeraveis.PermissaoDeUsuario.Administrador :
                Enumeraveis.PermissaoDeUsuario.SemPermissao;

            return loginDeUsuarioEntidade;
        }
    }
}
