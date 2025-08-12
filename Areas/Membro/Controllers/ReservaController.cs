using BibliotecaAppV2.Data.Service;
using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;

namespace BibliotecaAppV2.Areas.Membro.Controllers
{
    [Area("Membro")]
    [Authorize(Roles = "Membro")]
    public class ReservaController : Controller
    {
        private readonly ReservaService _reservaService;
        private readonly ILogger<ReservaController> _logger;

        public ReservaController(ReservaService reservaService, ILogger<ReservaController> logger)
        {
            _reservaService = reservaService;
            _logger = logger;
        }

        // GET: Reservas do membro
        public IActionResult Index()
        {
            try
            {
                var membroId = ObterMembroId();
                var reservas = _reservaService.ListarPorMembroId(membroId);
                return View(reservas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar reservas.");
                TempData["Error"] = "Erro ao carregar suas reservas.";
                return View();
            }
        }

        // GET: Criar reserva
        public IActionResult Create(int livroId)
        {
            try
            {
                var membroId = ObterMembroId();
                var sucesso = _reservaService.RegistrarReserva(membroId, livroId);
                if (!sucesso)
                {
                    TempData["Error"] = "Não foi possível reservar o livro. Verifique a disponibilidade.";
                }
                else
                {
                    TempData["Success"] = "Livro reservado com sucesso.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar reserva.");
                TempData["Error"] = "Erro ao criar reserva.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Cancelar reserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var membroId = ObterMembroId();
                var sucesso = _reservaService.CancelarReserva(id, membroId);
                if (sucesso)
                    TempData["Success"] = "Reserva cancelada com sucesso.";
                else
                    TempData["Error"] = "Não foi possível cancelar a reserva.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar reserva.");
                TempData["Error"] = "Erro ao cancelar reserva.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Utilitário: obter ID do membro logado
        private int ObterMembroId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                // Você pode lançar uma exceção, redirecionar, ou retornar 0
                // Aqui vou lançar uma exceção para evitar comportamento inesperado
                throw new InvalidOperationException("Usuário não autenticado.");
            }

            return _reservaService.ObterMembroIdPorUserId(userId);
        }

    }
}
