using RastreioDePersonalizadas.Enumeraveis;

namespace RastreioDePersonalizadas.Entidades
{
    public class LoginDeUsuarioEntidade
    {
        /// <summary>
        /// Usuario / Login do usuário. O mesmo que é utilizado no AD
        /// </summary>
        public string Usuario { get; set; }

        /// <summary>
        /// Senha do usuário. A mesma que é utilizada no AD.
        /// </summary>
        public string Senha { get; set; }

        /// <summary>
        /// Permissão na qual o usuário faz parte.
        /// </summary>
        public PermissaoDeUsuario? PermissaoDeUsuario { get; set; }

        /// <summary>
        /// E-mail do usuário
        /// </summary>
        public string Email { get; set; }
    }
}
