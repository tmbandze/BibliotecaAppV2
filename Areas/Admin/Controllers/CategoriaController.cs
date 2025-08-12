using BibliotecaAppV2.Data;
using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

using CategoriaModel = BibliotecaAppV2.Models.Categoria;
namespace BibliotecaAppV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoriaController> _logger;

        public CategoriaController(ApplicationDbContext context, ILogger<CategoriaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                return View(_context.Categorias.Where(c => c.Ativo).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar categorias no método Index.");
                TempData["Error"] = "Erro ao carregar a lista de categorias.";
                return View(Enumerable.Empty<CategoriaModel>());
            }
        }

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);
                if (categoria == null) return NotFound();
                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter detalhes da categoria no método Details.");
                TempData["Error"] = "Erro ao carregar detalhes da categoria.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoriaModel categoria)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    categoria.Ativo = true;
                    _context.Categorias.Add(categoria);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao cadastrar categoria no método Create.");
                    ModelState.AddModelError("", "Erro ao cadastrar categoria. Tente novamente.");
                }
            }
            return View(categoria);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var categoria = _context.Categorias.Find(id);
                if (categoria == null) return NotFound();
                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar categoria no método Edit.");
                TempData["Error"] = "Erro ao carregar categoria para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CategoriaModel categoria)
        {
            if (id != categoria.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar categoria no método Edit.");
                    ModelState.AddModelError("", "Erro ao atualizar categoria. Tente novamente.");
                }
            }
            return View(categoria);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);
                if (categoria == null) return NotFound();
                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar categoria para exclusão no método Delete.");
                TempData["Error"] = "Erro ao carregar categoria para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var categoria = _context.Categorias.Find(id);
                if (categoria != null)
                {
                    categoria.Ativo = false;
                    _context.Update(categoria);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar categoria no método DeleteConfirmed.");
                TempData["Error"] = "Erro ao desativar a categoria. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}