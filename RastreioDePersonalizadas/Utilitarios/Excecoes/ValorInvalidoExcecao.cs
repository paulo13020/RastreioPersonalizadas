namespace RastreioDePersonalizadas.Utilitarios.Excecoes
{
    public class ValorInvalidoExcecao : ExcecaoBase
    {
        public ValorInvalidoExcecao(string identificador, params object[] parametros) : base(identificador, parametros)
        {
        }
    }
}
