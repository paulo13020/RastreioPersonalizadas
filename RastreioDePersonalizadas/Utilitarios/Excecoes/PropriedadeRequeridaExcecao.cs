namespace RastreioDePersonalizadas.Utilitarios.Excecoes
{
    public class PropriedadeRequeridaExcecao : ExcecaoBase
    {
        public PropriedadeRequeridaExcecao(string identificador, params object[] parametros) : base(identificador, parametros)
        {
        }
    }
}
