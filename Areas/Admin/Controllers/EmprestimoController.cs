using BibliotecaAppV2.Areas.Admin.ViewModels;
using BibliotecaAppV2.Data;
using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using BibliotecaAppV2.Data.Service;
using System.Collections.Generic;
using System.Linq;
using EmprestimoModel = BibliotecaAppV2.Models.Emprestimo;
namespace BibliotecaAppV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EmprestimoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmprestimoController> _logger;

        public EmprestimoController(ApplicationDbContext context, ILogger<EmprestimoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Emprestimo
        [HttpGet]
        public IActionResult Index(EmprestimoFiltroViewModel filtro)
        {
            try
            {
                var emprestimos = _context.Emprestimos
                    .Where(e => e.Ativo)
                    .Include(e => e.Livro)
                    .Include(e => e.Membro)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(filtro.NomeMembro))
                    emprestimos = emprestimos.Where(e => e.Membro != null && e.Membro.Nome.Contains(filtro.NomeMembro));

                if (!string.IsNullOrEmpty(filtro.FuncionarioSelecionado))
                    emprestimos = emprestimos.Where(e => e.FuncionarioUserId.Contains(filtro.FuncionarioSelecionado));

                if (!string.IsNullOrEmpty(filtro.Status))
                {
                    switch (filtro.Status)
                    {
                        case "Em andamento":
                            emprestimos = emprestimos.Where(e =>
                                !e.DataDevolucaoReal.HasValue &&
                                (!e.DataDevolucaoPrevista.HasValue || e.DataDevolucaoPrevista >= DateTime.Today));
                            break;

                        case "Devolvido":
                            emprestimos = emprestimos.Where(e => e.DataDevolucaoReal.HasValue);
                            break;

                        case "Atrasado":
                            emprestimos = emprestimos.Where(e =>
                                !e.DataDevolucaoReal.HasValue &&
                                e.DataDevolucaoPrevista.HasValue &&
                                e.DataDevolucaoPrevista.Value.Date < DateTime.Today);
                            break;
                    }
                }

                if (filtro.DataInicio.HasValue)
                    emprestimos = emprestimos.Where(e => e.DataEmprestimo >= filtro.DataInicio.Value);

                if (filtro.DataFim.HasValue)
                    emprestimos = emprestimos.Where(e => e.DataEmprestimo <= filtro.DataFim.Value);

                emprestimos = emprestimos.OrderByDescending(e => e.DataEmprestimo);

                var lista = emprestimos.ToList().Select(e => new EmprestimoViewModel
                {
                    Id = e.Id,
                    Livro = e.Livro?.Titulo,
                    Membro = e.Membro?.Nome,
                    Funcionario = e.FuncionarioUserId ?? "",
                    DataEmprestimo = e.DataEmprestimo,
                    DataDevolucaoPrevista = e.DataDevolucaoPrevista,
                    DataDevolucaoReal = e.DataDevolucaoReal,
                    Status = e.Status
                }).ToList();

                filtro.Emprestimos = lista;

                filtro.FuncionariosDisponiveis = _context.Funcionarios
                .Where(f => f.Ativo)
                .Select(f => new SelectListItem
                {
                    Value = f.IdentityUserId ?? "", // Updated to use IdentityUserId
                    Text = f.Nome
                })
                .ToList();

                return View(filtro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao filtrar empréstimos no método Index.");
                TempData["Error"] = "Erro ao carregar a lista de empréstimos.";
                return View(new EmprestimoFiltroViewModel { Emprestimos = new List<EmprestimoViewModel>() });
            }
        }




        // GET: Emprestimo/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var emprestimo = _context.Emprestimos
                    .Include(e => e.Livro)
                    .Include(e => e.Membro)
                    .FirstOrDefault(e => e.Id == id);
                if (emprestimo == null) return NotFound();
                return View(emprestimo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter detalhes do empréstimo no método Details.");
                TempData["Error"] = "Erro ao carregar detalhes do empréstimo.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Emprestimo/Create
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
                _logger.LogInformation("Entrou no método GET Create de EmprestimoController");
                _logger.LogError(ex, "Erro ao carregar dados para criação de empréstimo.");
                TempData["Error"] = "Erro ao carregar dados para criação de empréstimo.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Emprestimo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmprestimoModel emprestimo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    emprestimo.Ativo = true;
                    _context.Emprestimos.Add(emprestimo);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao cadastrar empréstimo no método Create.");
                    ModelState.AddModelError("", "Erro ao cadastrar empréstimo. Tente novamente.");
                }
            }
            ViewBag.Livros = _context.Livros.Where(l => l.Ativo).ToList();
            ViewBag.Membros = _context.Membros.Where(m => m.Ativo).ToList();
            return View(emprestimo);
        }

        // GET: Emprestimo/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var emprestimo = _context.Emprestimos.Find(id);
                if (emprestimo == null) return NotFound();
                ViewBag.Livros = _context.Livros.Where(l => l.Ativo).ToList();
                ViewBag.Membros = _context.Membros.Where(m => m.Ativo).ToList();
                return View(emprestimo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar empréstimo no método Edit.");
                TempData["Error"] = "Erro ao carregar empréstimo para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Emprestimo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EmprestimoModel emprestimo)
        {
            if (id != emprestimo.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emprestimo);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar empréstimo no método Edit.");
                    ModelState.AddModelError("", "Erro ao atualizar empréstimo. Tente novamente.");
                }
            }
            ViewBag.Livros = _context.Livros.Where(l => l.Ativo).ToList();
            ViewBag.Membros = _context.Membros.Where(m => m.Ativo).ToList();
            return View(emprestimo);
        }

        // GET: Emprestimo/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var emprestimo = _context.Emprestimos
                    .Include(e => e.Livro)
                    .Include(e => e.Membro)
                    .FirstOrDefault(e => e.Id == id);
                if (emprestimo == null) return NotFound();
                return View(emprestimo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar empréstimo para exclusão no método Delete.");
                TempData["Error"] = "Erro ao carregar empréstimo para exclusão.";
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
                var emprestimo = _context.Emprestimos.Find(id);
                if (emprestimo != null)
                {
                    emprestimo.Ativo = false;
                    _context.Update(emprestimo);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar empréstimo no método DeleteConfirmed.");
                TempData["Error"] = "Erro ao desativar o empréstimo. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult Relatorio(string tipoRelatorio = "diario")
        {
            DateTime hoje = DateTime.Today;
            DateTime dataInicio;
            DateTime dataFim = hoje.AddDays(1).AddTicks(-1); // Incluir o dia atual por completo

            switch (tipoRelatorio.ToLower())
            {
                case "mensal":
                    dataInicio = new DateTime(hoje.Year, hoje.Month, 1);
                    break;
                case "semanal":
                    dataInicio = hoje.AddDays(-6); // Últimos 7 dias, incluindo hoje
                    break;
                default: // diário
                    tipoRelatorio = "diario";
                    dataInicio = hoje;
                    break;
            }

            var emprestimosPeriodo = _context.Emprestimos
                .Where(e => e.Ativo && e.DataEmprestimo >= dataInicio && e.DataEmprestimo <= dataFim)
                .Include(e => e.Livro)
                .ToList();

            var totalDevolvidos = emprestimosPeriodo.Count(e => e.DataDevolucaoReal != null);
            var totalPendentes = emprestimosPeriodo.Count(e => e.DataDevolucaoReal == null);

            var livrosMaisEmprestados = emprestimosPeriodo
                .Where(e => e.Livro != null)
                .GroupBy(e => e.Livro!.Titulo)
                .Select(g => new LivroEmprestadoViewModel
                {
                    Titulo = g.Key,
                    Quantidade = g.Count()
                })
                .OrderByDescending(g => g.Quantidade)
                .Take(5)
                .ToList();

            var viewModel = new RelatorioEmprestimoViewModel
            {
                TipoRelatorio = tipoRelatorio,
                DataReferencia = hoje,
                DataInicio = dataInicio,
                DataFim = dataFim,
                TotalEmprestimos = emprestimosPeriodo.Count,
                TotalDevolvidos = totalDevolvidos,
                TotalPendentes = totalPendentes,
                LivrosMaisEmprestados = livrosMaisEmprestados
            };


            return View(viewModel);
        }

        [HttpGet]
        public IActionResult RelatorioPdf(string tipoRelatorio = "diario")
        {
            DateTime hoje = DateTime.Today;
            DateTime dataFim = hoje.AddDays(1).AddTicks(-1);
            DateTime dataInicio = tipoRelatorio.ToLower() switch
            {
                "mensal" => new DateTime(hoje.Year, hoje.Month, 1),
                "semanal" => hoje.AddDays(-6),
                _ => hoje
            };

            var emprestimos = _context.Emprestimos
                .Where(e => e.Ativo && e.DataEmprestimo >= dataInicio && e.DataEmprestimo <= dataFim)
                .Include(e => e.Livro)
                .ToList();

            var model = new RelatorioEmprestimoViewModel
            {
                TipoRelatorio = tipoRelatorio,
                DataReferencia = hoje,
                TotalEmprestimos = emprestimos.Count,
                TotalDevolvidos = emprestimos.Count(e => e.DataDevolucaoReal != null),
                TotalPendentes = emprestimos.Count(e => e.DataDevolucaoReal == null),
                LivrosMaisEmprestados = emprestimos
                    .Where(e => e.Livro != null)
                    .GroupBy(e => e.Livro!.Titulo)
                    .Select(g => new LivroEmprestadoViewModel
                    {
                        Titulo = g.Key,
                        Quantidade = g.Count()
                    })
                    .OrderByDescending(g => g.Quantidade)
                    .Take(5)
                    .ToList()
            };

            var pdfService = new RelatorioPdfService();
            var pdf = pdfService.GerarRelatorio(model);

            return File(pdf, "application/pdf", $"relatorio_{tipoRelatorio}_{hoje:yyyyMMdd}.pdf");
        }


    }
}