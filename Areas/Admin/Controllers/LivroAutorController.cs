using BibliotecaAppV2.Data;
using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

using LivroAutorModel = BibliotecaAppV2.Models.LivroAutor;

namespace BibliotecaAppV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LivroAutorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LivroAutorController> _logger;

        public LivroAutorController(ApplicationDbContext context, ILogger<LivroAutorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: LivroAutor/Create?livroId=1
        public IActionResult Create(int livroId)
        {
            var livro = _context.Livros
                .Include(l => l.LivroAutores)
                .ThenInclude(la => la.Autor)
                .FirstOrDefault(l => l.Id == livroId);

            if (livro == null)
                return NotFound();

            ViewBag.Livro = livro;
            ViewBag.Autores = new SelectList(
                _context.Autores.Where(a => a.Ativo && !livro.LivroAutores.Any(la => la.AutorId == a.Id)),
                "Id", "Nome");

            return View();
        }

        // POST: LivroAutor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int livroId, int autorId)
        {
            if (_context.LivroAutores.Any(la => la.LivroId == livroId && la.AutorId == autorId))
            {
                TempData["Error"] = "Este autor já está associado ao livro.";
                return RedirectToAction("Edit", "Livro", new { area = "Admin", id = livroId });
            }

            var livroAutor = new LivroAutor
            {
                LivroId = livroId,
                AutorId = autorId
            };

            _context.LivroAutores.Add(livroAutor);
            _context.SaveChanges();

            TempData["Success"] = "Autor associado com sucesso!";
            return RedirectToAction("Edit", "Livro", new { area = "Admin", id = livroId });
        }

        // GET: LivroAutor/Delete?livroId=1&autorId=2
        public IActionResult Delete(int livroId, int autorId)
        {
            var livroAutor = _context.LivroAutores
                .Include(la => la.Autor)
                .Include(la => la.Livro)
                .FirstOrDefault(la => la.LivroId == livroId && la.AutorId == autorId);

            if (livroAutor == null)
                return NotFound();

            return View(livroAutor);
        }

        // POST: LivroAutor/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int livroId, int autorId)
        {
            var livroAutor = _context.LivroAutores
                .FirstOrDefault(la => la.LivroId == livroId && la.AutorId == autorId);

            if (livroAutor != null)
            {
                _context.LivroAutores.Remove(livroAutor);
                _context.SaveChanges();
                TempData["Success"] = "Associação removida com sucesso!";
            }

            return RedirectToAction("Edit", "Livro", new { area = "Admin", id = livroId });
        }
    }
}