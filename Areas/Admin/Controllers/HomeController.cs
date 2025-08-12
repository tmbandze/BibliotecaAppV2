using BibliotecaAppV2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace BibliotecaAppV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Ajuste os nomes das tabelas conforme suas entidades reais!
            var totalLivros = _context.Livros.Count(l => l.Ativo);

            // Exemplo: se ainda não houver Emprestimos ou Membros, comente estas linhas
            var totalEmprestimos = _context.Emprestimos.Count(e => e.Ativo); // Substitua pelo nome real se necessário
            var totalMembros = _context.Membros.Count(m => m.Ativo);         // Substitua pelo nome real se necessário
            var totalAtrasos = _context.Emprestimos.Count(e => e.Ativo && e.DataDevolucaoReal < DateTime.Now); // Ajuste conforme necessário

            ViewBag.TotalLivros = totalLivros;
            ViewBag.TotalEmprestimos = totalEmprestimos;
            ViewBag.TotalMembros = totalMembros;
            ViewBag.TotalAtrasos = totalAtrasos;

            return View();
        }
    }
}