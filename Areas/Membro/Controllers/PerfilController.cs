using BibliotecaAppV2.Data.Service;
using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;

using MembroModel = BibliotecaAppV2.Models.Membro;

namespace BibliotecaAppV2.Areas.Membro.Controllers
{
    [Area("Membro")]
    [Authorize(Roles = "Membro")]
    public class PerfilController : Controller
    {
        private readonly MembroService _membroService;
        private readonly ILogger<PerfilController> _logger;

        public PerfilController(MembroService membroService, ILogger<PerfilController> logger)
        {
            _membroService = membroService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var membro = _membroService.ObterMembroPorUserId(userId);
                if (membro == null) return NotFound();

                return View(membro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar perfil do membro.");
                TempData["Error"] = "Erro ao carregar seu perfil.";
                return View();
            }
        }

        public IActionResult Edit()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var membro = _membroService.ObterMembroPorUserId(userId);
                if (membro == null) return NotFound();

                return View(membro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados para edição de perfil.");
                TempData["Error"] = "Erro ao carregar seu perfil.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MembroModel membro)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _membroService.AtualizarMembro(membro);
                    TempData["Success"] = "Perfil atualizado com sucesso.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar perfil do membro.");
                    ModelState.AddModelError("", "Erro ao atualizar seu perfil. Tente novamente.");
                }
            }
            return View(membro);
        }
    }
}
