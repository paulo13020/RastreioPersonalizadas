namespace RastreioDePersonalizadas.Utilitarios.Excecoes
{
    public class ExcecaoBase: Exception
    {
        #region Propriedades

        /// <summary>
        /// Parametros que serão passados para a mensagem de erro.
        /// </summary>
        public object[] Parametros { get; set; }

        #endregion

        #region Construtores
        /// <summary>
        /// Cria uma nova instância da exceção.
        /// </summary>
        /// <param name="identificador">Identificador da Exceção.</param>
        /// <param name="parametros">Parametros que serão passados para a mensagem de erro.</param>
        public ExcecaoBase(string identificador, params object[] parametros)
            : this(identificador, null, parametros) { }

        /// <summary>
        /// Cria uma nova instância da exceção.
        /// </summary>
        /// <param name="identificador">Identificador da Exceção.</param>
        /// <param name="excecaoInterna">Mensagem de detalhe do erro.</param>
        /// <param name="parametros">Parametros que serão passados para a mensagem de erro.</param>
        public ExcecaoBase(string identificador, Exception excecaoInterna, params object[] parametros)
            : base(identificador, excecaoInterna)
        {
            //Atribui os parametros
            Parametros = parametros;
        }
        #endregion
    }
}
