using BibliotecaAppV2.Data;
using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

using AutorModel = BibliotecaAppV2.Models.Autor;
namespace BibliotecaAppV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AutorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AutorController> _logger;

        public AutorController(ApplicationDbContext context, ILogger<AutorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Autor
        public IActionResult Index()
        {
            try
            {
                return View(_context.Autores.Where(a => a.Ativo).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar autores no método Index.");
                TempData["Error"] = "Erro ao carregar a lista de autores.";
                return View(Enumerable.Empty<AutorModel>());
            }
        }

        // GET: Autor/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var autor = _context.Autores.FirstOrDefault(a => a.Id == id);
                if (autor == null) return NotFound();
                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter detalhes do autor no método Details.");
                TempData["Error"] = "Erro ao carregar detalhes do autor.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Autor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Autor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AutorModel autor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    autor.Ativo = true;
                    _context.Autores.Add(autor);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao cadastrar autor no método Create.");
                    ModelState.AddModelError("", "Erro ao cadastrar autor. Tente novamente.");
                }
            }
            return View(autor);
        }

        // GET: Autor/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var autor = _context.Autores.Find(id);
                if (autor == null) return NotFound();
                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar autor no método Edit.");
                TempData["Error"] = "Erro ao carregar autor para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Autor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AutorModel autor)
        {
            if (id != autor.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(autor);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar autor no método Edit.");
                    ModelState.AddModelError("", "Erro ao atualizar autor. Tente novamente.");
                }
            }
            return View(autor);
        }

        // GET: Autor/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var autor = _context.Autores.FirstOrDefault(a => a.Id == id);
                if (autor == null) return NotFound();
                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar autor para exclusão no método Delete.");
                TempData["Error"] = "Erro ao carregar autor para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Autor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var autor = _context.Autores.Find(id);
                if (autor != null)
                {
                    autor.Ativo = false;
                    _context.Update(autor);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar autor no método DeleteConfirmed.");
                TempData["Error"] = "Erro ao desativar o autor. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}