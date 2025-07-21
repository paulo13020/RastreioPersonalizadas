using System.ComponentModel;

namespace RastreioDePersonalizadas.Enumeraveis
{
    /// <summary>
    /// Classe utilizada para definir o nível de usuário ao realizar o login no sistema, esse nível vai vir do AD.
    /// </summary>
    public enum PermissaoDeUsuario : byte
    {
        [Description("EstoqueSJC")]
        EstoqueSJC = 1,
        [Description("EstoqueMG")]
        EstoqueMG = 2,
        [Description("Vendedor")]
        Vendedor = 3,
        [Description("Financeiro")]
        Financeiro = 4,
        [Description("Administrador")]
        Administrador = 5,
        [Description("Sem Permissão")]
        SemPermissao = 0
    }
}
