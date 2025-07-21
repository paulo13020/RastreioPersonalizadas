using Microsoft.AspNetCore.Mvc;
using RastreioDePersonalizadas.Contextos;
using RastreioDePersonalizadas.Entidades;

namespace RastreioDePersonalizadas.Controllers
{
    public class LogController : Controller
    {
        private readonly ContextoDeBancoDeDados _contextoDeBancoDeDados;

        public LogController (ContextoDeBancoDeDados contextoDeBancoDeDados)
        {
            _contextoDeBancoDeDados = contextoDeBancoDeDados;
        }

        /// <summary>
        /// Método responsável por gravar o log da aplicação
        /// </summary>
        /// <param name="entLog"></param>
        public void GravarLog(LogEntidade entLog)
        {
            _contextoDeBancoDeDados.Logs.Add(entLog);
            _contextoDeBancoDeDados.SaveChanges();
        }
    }
}
