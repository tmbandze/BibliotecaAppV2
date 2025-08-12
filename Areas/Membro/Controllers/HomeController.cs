using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAppV2.Areas.Membro.Controllers
{
    [Area("Membro")]
    [Authorize(Roles = "Membro")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}