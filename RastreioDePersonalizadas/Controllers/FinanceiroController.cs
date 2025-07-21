using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RastreioDePersonalizadas.Controllers
{
    [Authorize(Roles = "Financeioro, Administrador")]
    public class FinanceiroController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
