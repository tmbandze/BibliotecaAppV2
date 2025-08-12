using BibliotecaAppV2.Data.Service;
using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;


namespace BibliotecaAppV2.Areas.Membro.Controllers
{
    [Area("Membro")]
    [Authorize(Roles = "Membro")]
    public class EmprestimoController : Controller
    {
        private readonly EmprestimoService _emprestimoService;
        private readonly ILogger<EmprestimoController> _logger;

        public EmprestimoController(EmprestimoService emprestimoService, ILogger<EmprestimoController> logger)
        {
            _emprestimoService = emprestimoService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var membroId = ObterMembroId();
                var emprestimos = _emprestimoService.ListarPorMembroId(membroId);
                return View(emprestimos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar empréstimos do membro.");
                TempData["Error"] = "Erro ao carregar seus empréstimos.";
                return View(new List<Emprestimo>()); // <-- Garante uma lista vazia na view
            }
        }


        private int ObterMembroId()
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("Usuário não autenticado.");
            }

            return _emprestimoService.ObterMembroIdPorUserId(userId);
        }


        public IActionResult Details(int id)
        {
            try
            {
                var membroId = ObterMembroId();
                var emprestimo = _emprestimoService.ObterPorId(id);

                if (emprestimo == null || emprestimo.MembroId != membroId)
                    return NotFound();

                return View(emprestimo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do empréstimo.");
                TempData["Error"] = "Erro ao carregar os detalhes.";
                return RedirectToAction(nameof(Index));
            }
        }



    }
}
