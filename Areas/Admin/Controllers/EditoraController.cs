using BibliotecaAppV2.Data;
using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

using EditoraModel = BibliotecaAppV2.Models.Editora;
namespace BibliotecaAppV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EditoraController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EditoraController> _logger;

        public EditoraController(ApplicationDbContext context, ILogger<EditoraController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                return View(_context.Editoras.Where(e => e.Ativo).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar editoras no método Index.");
                TempData["Error"] = "Erro ao carregar a lista de editoras.";
                return View(Enumerable.Empty<EditoraModel>());
            }
        }

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var editora = _context.Editoras.FirstOrDefault(e => e.Id == id);
                if (editora == null) return NotFound();
                return View(editora);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter detalhes da editora no método Details.");
                TempData["Error"] = "Erro ao carregar detalhes da editora.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EditoraModel editora)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    editora.Ativo = true;
                    _context.Editoras.Add(editora);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao cadastrar editora no método Create.");
                    ModelState.AddModelError("", "Erro ao cadastrar editora. Tente novamente.");
                }
            }
            return View(editora);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var editora = _context.Editoras.Find(id);
                if (editora == null) return NotFound();
                return View(editora);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar editora no método Edit.");
                TempData["Error"] = "Erro ao carregar editora para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EditoraModel editora)
        {
            if (id != editora.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(editora);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar editora no método Edit.");
                    ModelState.AddModelError("", "Erro ao atualizar editora. Tente novamente.");
                }
            }
            return View(editora);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var editora = _context.Editoras.FirstOrDefault(e => e.Id == id);
                if (editora == null) return NotFound();
                return View(editora);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar editora para exclusão no método Delete.");
                TempData["Error"] = "Erro ao carregar editora para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var editora = _context.Editoras.Find(id);
                if (editora != null)
                {
                    editora.Ativo = false;
                    _context.Update(editora);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar editora no método DeleteConfirmed.");
                TempData["Error"] = "Erro ao desativar a editora. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}