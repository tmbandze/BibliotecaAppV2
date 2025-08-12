using BibliotecaAppV2.Data.Service;
using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace BibliotecaAppV2.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    [Authorize(Roles = "Funcionario")]
    public class LivroController : Controller
    {
        private readonly LivroService _livroService;
        private readonly ILogger<LivroController> _logger;

        public LivroController(LivroService livroService, ILogger<LivroController> logger)
        {
            _livroService = livroService;
            _logger = logger;
        }

        // GET: Livro
        public IActionResult Index(string busca = "")
        {
            try
            {
                var livros = string.IsNullOrWhiteSpace(busca)
                    ? _livroService.ListarTodos()
                    : _livroService.BuscarLivros(busca);
                ViewBag.Filtro = busca;
                return View(livros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar livros.");
                TempData["Error"] = "Erro ao carregar a lista de livros.";
                return View();
            }
        }

        // GET: Livro/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var livro = _livroService.ObterPorId(id.Value);
                if (livro == null) return NotFound();
                return View(livro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exibir detalhes do livro.");
                TempData["Error"] = "Erro ao exibir detalhes do livro.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Livro/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var livro = _livroService.ObterPorId(id.Value);
                if (livro == null) return NotFound();
                return View(livro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados para edição de livro.");
                TempData["Error"] = "Erro ao carregar dados para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Livro/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Livro livro)
        {
            if (id != livro.Id) return NotFound();

            // Só permitir edição do campo Quantidade
            ModelState.Remove("Titulo");
            ModelState.Remove("Autor");
            ModelState.Remove("Editora");
            ModelState.Remove("AnoPublicacao");
            ModelState.Remove("ISBN");
            ModelState.Remove("Ativo");

            if (ModelState.IsValid)
            {
                try
                {
                    // Recupera o livro original do BD
                    var livroOriginal = _livroService.ObterPorId(id);
                    if (livroOriginal == null) return NotFound();

                    livroOriginal.Quantidade = livro.Quantidade;
                    _livroService.AtualizarLivro(livroOriginal);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao editar livro.");
                    ModelState.AddModelError("", "Erro ao editar livro. Tente novamente.");
                }
            }
            return View(livro);
        }
    }
}