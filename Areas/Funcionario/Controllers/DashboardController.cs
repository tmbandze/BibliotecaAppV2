using BibliotecaAppV2.Data.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;

namespace BibliotecaAppV2.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    [Authorize(Roles = "Funcionario")]
    public class DashboardController : Controller
    {
        private readonly EmprestimoService _emprestimoService;
        private readonly MembroService _membroService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            EmprestimoService emprestimoService,
            MembroService membroService,
            ILogger<DashboardController> logger)
        {
            _emprestimoService = emprestimoService;
            _membroService = membroService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(); // ou RedirectToAction("Login", "Account"), etc.
                }

                var resumo = new
                {
                    EmprestimosRealizados = _emprestimoService.ContarEmprestimosPorFuncionario(userId),
                    DevolucoesPendentes = _emprestimoService.ContarDevolucoesPendentesPorFuncionario(userId),
                    MembrosAtendidos = _membroService.ContarMembrosAtendidosPorFuncionario(userId),
                    Atrasos = _emprestimoService.ContarAtrasosPorFuncionario(userId)
                };

                return View(resumo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados do dashboard do funcionário.");
                TempData["Error"] = "Erro ao carregar os dados do painel.";
                return View(new { });
            }
        }
    }
}