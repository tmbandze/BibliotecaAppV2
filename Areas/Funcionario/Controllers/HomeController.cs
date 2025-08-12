using BibliotecaAppV2.Data.Service;
using BibliotecaAppV2.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BibliotecaAppV2.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    [Authorize(Roles = "Funcionario")]
    public class HomeController : Controller
    {
        private readonly EmprestimoService _emprestimoService;
        private readonly MembroService _membroService;

        public HomeController(EmprestimoService emprestimoService, MembroService membroService)
        {
            _emprestimoService = emprestimoService;
            _membroService = membroService;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var model = new DashboardViewModel
            {
                TotalEmprestimos = _emprestimoService.ContarEmprestimosPorFuncionario(userId),
                DevolucoesPendentes = _emprestimoService.ContarDevolucoesPendentesPorFuncionario(userId),
                MembrosAtendidos = _membroService.ContarMembrosAtendidosPorFuncionario(userId),
                TotalAtrasos = _emprestimoService.ContarAtrasosPorFuncionario(userId)
            };

            return View(model);
        }
    }
}
