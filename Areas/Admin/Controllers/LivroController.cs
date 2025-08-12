using BibliotecaAppV2.Data;
using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using LivroModel = BibliotecaAppV2.Models.Livro;

namespace BibliotecaAppV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LivroController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LivroController> _logger;

        public LivroController(ApplicationDbContext context, ILogger<LivroController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Livro
        public IActionResult Index()
        {
            try
            {
                var livros = _context.Livros
                    .Where(l => l.Ativo)
                    .Include(l => l.Editora)
                    .Include(l => l.Categoria)
                    .Include(l => l.LivroAutores)
                        .ThenInclude(la => la.Autor)
                    .ToList();

                return View(livros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar livros no método Index.");
                TempData["Error"] = "Erro ao carregar a lista de livros.";
                return View(Enumerable.Empty<Livro>());
            }
        }

        // GET: Livro/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var livro = _context.Livros.FirstOrDefault(l => l.Id == id);
                if (livro == null) return NotFound();
                return View(livro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter detalhes do livro no método Details.");
                TempData["Error"] = "Erro ao carregar detalhes do livro.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Livro/Create
        public IActionResult Create()
        {
            var model = new LivroEditViewModel();
            model.AutoresDisponiveis = _context.Autores.Where(a => a.Ativo).ToList();
            ViewBag.Editoras = new SelectList(_context.Editoras.Where(e => e.Ativo), "Id", "Nome");
            ViewBag.Categorias = new SelectList(_context.Categorias.Where(c => c.Ativo), "Id", "Nome");
            return View(model);
        }

        // POST: Livro/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LivroEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var livroEntidade = new Livro
                    {
                        Titulo = model.Titulo,
                        CategoriaId = model.CategoriaId,
                        EditoraId = model.EditoraId,
                        AnoPublicacao = model.AnoPublicacao,
                        ISBN = model.ISBN,
                        Quantidade = model.Quantidade,
                        Ativo = true
                        // (adicione outras propriedades necessárias)
                    };

                    // Relacionamento muitos-para-muitos de autores
                    if (model.AutoresIds != null && model.AutoresIds.Any())
                    {
                        livroEntidade.LivroAutores = model.AutoresIds.Select(aid => new LivroAutor { AutorId = aid }).ToList();
                    }

                    _context.Livros.Add(livroEntidade);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao cadastrar livro no método Create.");
                    ModelState.AddModelError("", "Erro ao cadastrar livro. Tente novamente.");
                }
            }

            // Se houver erro, recarregue as listas do ViewModel
            model.AutoresDisponiveis = _context.Autores.Where(a => a.Ativo).ToList();
            ViewBag.Categorias = new SelectList(_context.Categorias.Where(c => c.Ativo), "Id", "Nome", model.CategoriaId);
            ViewBag.Editoras = new SelectList(_context.Editoras.Where(e => e.Ativo), "Id", "Nome", model.EditoraId);

            return View(model); // <-- aqui sempre retorna o model do tipo ViewModel, nunca Livro!
        }

        // GET: Livro/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var livro = _context.Livros.Find(id);
                if (livro == null) return NotFound();
                return View(livro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar livro no método Edit.");
                TempData["Error"] = "Erro ao carregar livro para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Livro/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LivroModel livro)
        {
            if (id != livro.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(livro);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar livro no método Edit.");
                    ModelState.AddModelError("", "Erro ao atualizar livro. Tente novamente.");
                }
            }
            return View(livro);
        }

        // GET: Livro/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var livro = _context.Livros.Find(id);
                if (livro == null) return NotFound();
                return View(livro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar livro para exclusão no método Delete.");
                TempData["Error"] = "Erro ao carregar livro para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Livro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var livro = _context.Livros.Find(id);
                if (livro != null)
                {
                    livro.Ativo = false;
                    _context.Update(livro);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar livro no método DeleteConfirmed.");
                TempData["Error"] = "Erro ao desativar o livro. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}