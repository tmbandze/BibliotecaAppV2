using BibliotecaAppV2.Data;
using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using ReservaModel = BibliotecaAppV2.Models.Reserva;

namespace BibliotecaAppV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReservaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReservaController> _logger;

        public ReservaController(ApplicationDbContext context, ILogger<ReservaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Reserva
        public IActionResult Index()
        {
            try
            {
                var reservas = _context.Reservas
                    .Where(r => r.Ativo)
                    .Include(r => r.Livro)
                    .Include(r => r.Membro)
                    .ToList();
                return View(reservas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar reservas no método Index.");
                TempData["Error"] = "Erro ao carregar a lista de reservas.";
                return View(Enumerable.Empty<ReservaModel>());
            }
        }

        // GET: Reserva/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var reserva = _context.Reservas
                    .Include(r => r.Livro)
                    .Include(r => r.Membro)
                    .FirstOrDefault(r => r.Id == id);
                if (reserva == null) return NotFound();
                return View(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter detalhes da reserva no método Details.");
                TempData["Error"] = "Erro ao carregar detalhes da reserva.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Reserva/Create
        public IActionResult Create()
        {
            try
            {
                ViewBag.Livros = _context.Livros.Where(l => l.Ativo).ToList();
                ViewBag.Membros = _context.Membros.Where(m => m.Ativo).ToList();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados para criação de reserva.");
                TempData["Error"] = "Erro ao carregar dados para criação de reserva.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Reserva/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReservaModel reserva)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    reserva.Ativo = true;
                    _context.Reservas.Add(reserva);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao cadastrar reserva no método Create.");
                    ModelState.AddModelError("", "Erro ao cadastrar reserva. Tente novamente.");
                }
            }
            ViewBag.Livros = _context.Livros.Where(l => l.Ativo).ToList();
            ViewBag.Membros = _context.Membros.Where(m => m.Ativo).ToList();
            return View(reserva);
        }

        // GET: Reserva/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var reserva = _context.Reservas
                    .Include(r => r.Livro)
                    .Include(r => r.Membro)
                    .FirstOrDefault(r => r.Id == id);
                if (reserva == null) return NotFound();
                ViewBag.Livros = _context.Livros.Where(l => l.Ativo).ToList();
                ViewBag.Membros = _context.Membros.Where(m => m.Ativo).ToList();
                return View(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar reserva no método Edit.");
                TempData["Error"] = "Erro ao carregar reserva para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Reserva/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ReservaModel reserva)
        {
            if (id != reserva.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar reserva no método Edit.");
                    ModelState.AddModelError("", "Erro ao atualizar reserva. Tente novamente.");
                }
            }
            ViewBag.Livros = _context.Livros.Where(l => l.Ativo).ToList();
            ViewBag.Membros = _context.Membros.Where(m => m.Ativo).ToList();
            return View(reserva);
        }

        // GET: Reserva/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var reserva = _context.Reservas
                    .Include(r => r.Livro)
                    .Include(r => r.Membro)
                    .FirstOrDefault(r => r.Id == id);
                if (reserva == null) return NotFound();
                return View(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar reserva para exclusão no método Delete.");
                TempData["Error"] = "Erro ao carregar reserva para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Reserva/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var reserva = _context.Reservas.Find(id);
                if (reserva != null)
                {
                    reserva.Ativo = false;
                    _context.Update(reserva);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar reserva no método DeleteConfirmed.");
                TempData["Error"] = "Erro ao desativar a reserva. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}