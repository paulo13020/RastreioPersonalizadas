using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RastreioDePersonalizadas.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdministradorController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }        
    }
}
