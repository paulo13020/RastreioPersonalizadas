using System.Text.Json;
using RastreioDePersonalizadas.Entidades;

namespace RastreioDePersonalizadas.Contextos
{
    public static class UsuarioContexto
    {
        public static LoginDeUsuarioEntidade GetUsuarioLogado(HttpContext httpContext)
        {
            var usuarioJson = httpContext.Session.GetString("UsuarioLogado");
            return string.IsNullOrEmpty(usuarioJson) ? null : JsonSerializer.Deserialize<LoginDeUsuarioEntidade>(usuarioJson);
        }

        public static void SetUsuarioLogado(HttpContext httpContext, LoginDeUsuarioEntidade usuario)
        {
            httpContext.Session.SetString("UsuarioLogado", JsonSerializer.Serialize(usuario));
        }
    }
}
