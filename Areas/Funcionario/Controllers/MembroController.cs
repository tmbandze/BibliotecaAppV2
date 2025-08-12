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
    public class MembroController : Controller
    {
        private readonly MembroService _membroService;
        private readonly ILogger<MembroController> _logger;

        public MembroController(MembroService membroService, ILogger<MembroController> logger)
        {
            _membroService = membroService;
            _logger = logger;
        }

        // GET: Membro
        public IActionResult Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(); // ou RedirectToAction("Login", "Account"), etc.
                }

                var membrosAtendidos = _membroService.ListarMembrosAtendidosPorFuncionario(userId);
                return View(membrosAtendidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar membros atendidos.");
                TempData["Error"] = "Erro ao carregar a lista de membros atendidos.";
                return View();
            }
        }

        // GET: Membro/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var membro = _membroService.ObterMembroPorId(id.Value);
                if (membro == null) return NotFound();
                return View(membro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exibir detalhes do membro.");
                TempData["Error"] = "Erro ao exibir detalhes do membro.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Membro/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var membro = _membroService.ObterMembroPorId(id.Value);
                if (membro == null) return NotFound();
                return View(membro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados para edição de membro.");
                TempData["Error"] = "Erro ao carregar dados para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Membro/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, BibliotecaAppV2.Models.Membro membro)
        {
            if (id != membro.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _membroService.AtualizarMembro(membro);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao editar membro.");
                    ModelState.AddModelError("", "Erro ao editar membro. Tente novamente.");
                }
            }
            return View(membro);
        }

        // GET: Membro/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var membro = _membroService.ObterMembroPorId(id.Value);
                if (membro == null) return NotFound();
                return View(membro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados para exclusão de membro.");
                TempData["Error"] = "Erro ao carregar dados para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Membro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            TempData["Error"] = "Sem Permissao para desativar membros.";
            return RedirectToAction(nameof(Index));
        }
    }
}