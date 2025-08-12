using BibliotecaAppV2.Data.Service;
using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System;
using System.Security.Claims;


namespace BibliotecaAppV2.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    [Authorize(Roles = "Funcionario")]
    public class EmprestimoController : Controller
    {
        private readonly EmprestimoService _emprestimoService;
        private readonly ILogger<EmprestimoController> _logger;

        public EmprestimoController(EmprestimoService emprestimoService, ILogger<EmprestimoController> logger)
        {
            _emprestimoService = emprestimoService;
            _logger = logger;
        }

        // GET: Emprestimo
        public IActionResult Index()
        {
            try
            {
                var emprestimos = _emprestimoService.ListarEmprestimosAtivos();
                return View(emprestimos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar empréstimos no método Index.");
                TempData["Error"] = "Erro ao carregar a lista de empréstimos.";
                return View(Array.Empty<Emprestimo>());
            }
        }

        // GET: Emprestimo/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var emprestimo = _emprestimoService.ObterEmprestimoPorId(id.Value);
                if (emprestimo == null) return NotFound();
                return View(emprestimo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do empréstimo.");
                TempData["Error"] = "Erro ao carregar detalhes do empréstimo.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Emprestimo/Create
        public IActionResult Create()
        {
            try
            {
                var livros = _emprestimoService.ObterLivrosDisponiveis();
                var membros = _emprestimoService.ObterMembrosAtivos();

                ViewBag.Livros = livros.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Titulo
                }).ToList();

                ViewBag.Membros = membros.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Nome
                }).ToList();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados para criação de empréstimo.");
                TempData["Error"] = "Erro ao carregar dados para criação de empréstimo.";
                return RedirectToAction(nameof(Index));
            }
        }


        // POST: Emprestimo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Emprestimo emprestimo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Associa o funcionário autenticado ao empréstimo
                    var funcionarioUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (funcionarioUserId == null)
                    {
                        ModelState.AddModelError("", "Erro ao identificar o funcionário autenticado.");
                        ViewBag.Livros = _emprestimoService.ObterLivrosDisponiveis()
                                   .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Titulo }).ToList();

                        ViewBag.Membros = _emprestimoService.ObterMembrosAtivos()
                                   .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Nome }).ToList();

                        return View(emprestimo);
                    }
                    emprestimo.FuncionarioUserId = funcionarioUserId;
                    _emprestimoService.RegistrarEmprestimo(emprestimo);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao registrar empréstimo.");
                    ModelState.AddModelError("", "Erro ao registrar empréstimo. Tente novamente.");
                }
            }

            ViewBag.Livros = _emprestimoService.ObterLivrosDisponiveis()
                       .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Titulo }).ToList();

            ViewBag.Membros = _emprestimoService.ObterMembrosAtivos()
                       .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Nome }).ToList();

            return View(emprestimo);
        }


        // GET: Emprestimo/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var emprestimo = _emprestimoService.ObterEmprestimoPorId(id.Value);
                if (emprestimo == null) return NotFound();

                ViewBag.Livros = _emprestimoService.ObterLivrosDisponiveis(emprestimo.LivroId); // garantir o livro atual está no select
                ViewBag.Membros = _emprestimoService.ObterMembrosAtivos(emprestimo.MembroId); // garantir o membro atual está no select
                return View(emprestimo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados para edição do empréstimo.");
                TempData["Error"] = "Erro ao carregar dados para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Emprestimo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Emprestimo emprestimo)
        {
            if (id != emprestimo.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _emprestimoService.AtualizarEmprestimo(emprestimo);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao editar empréstimo.");
                    ModelState.AddModelError("", "Erro ao editar empréstimo. Tente novamente.");
                }
            }
            ViewBag.Livros = _emprestimoService.ObterLivrosDisponiveis(emprestimo.LivroId);
            ViewBag.Membros = _emprestimoService.ObterMembrosAtivos(emprestimo.MembroId);
            return View(emprestimo);
        }

        // GET: Emprestimo/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var emprestimo = _emprestimoService.ObterEmprestimoPorId(id.Value);
                if (emprestimo == null) return NotFound();
                return View(emprestimo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados para exclusão do empréstimo.");
                TempData["Error"] = "Erro ao carregar dados para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Emprestimo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _emprestimoService.ExcluirEmprestimo(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir empréstimo.");
                TempData["Error"] = "Erro ao excluir empréstimo. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Emprestimo/Devolver/5
        public IActionResult Devolver(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var emprestimo = _emprestimoService.ObterEmprestimoPorId(id.Value);
                if (emprestimo == null) return NotFound();
                return View(emprestimo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados para devolução de empréstimo.");
                TempData["Error"] = "Erro ao carregar dados para devolução.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Emprestimo/Devolver/5
        [HttpPost, ActionName("Devolver")]
        [ValidateAntiForgeryToken]
        public IActionResult DevolverConfirmado(int id)
        {
            try
            {
                _emprestimoService.RegistrarDevolucao(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar devolução.");
                TempData["Error"] = "Erro ao registrar devolução. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}