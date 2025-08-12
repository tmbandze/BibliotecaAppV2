using BibliotecaAppV2.Data.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace BibliotecaAppV2.Areas.Membro.Controllers
{
    [Area("Membro")]
    [Authorize(Roles = "Membro")]
    public class LivroController : Controller
    {
        private readonly LivroService _livroService;
        private readonly ILogger<LivroController> _logger;

        public LivroController(LivroService livroService, ILogger<LivroController> logger)
        {
            _livroService = livroService;
            _logger = logger;
        }

        public IActionResult Index(string busca = "")
        {
            try
            {
                var livros = string.IsNullOrWhiteSpace(busca)
                    ? _livroService.ListarTodosDisponiveis()
                    : _livroService.BuscarLivrosDisponiveis(busca);

                ViewBag.Filtro = busca;
                return View(livros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar livros disponíveis.");
                TempData["Error"] = "Erro ao carregar a lista de livros.";
                return View();
            }
        }

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var livro = _livroService.ObterPorId(id.Value);
                if (livro == null || !livro.Ativo) return NotFound();
                return View(livro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do livro.");
                TempData["Error"] = "Erro ao carregar os detalhes.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
