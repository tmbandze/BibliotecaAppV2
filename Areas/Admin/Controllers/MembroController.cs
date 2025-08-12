using BibliotecaAppV2.Models;
using BibliotecaAppV2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using BibliotecaAppV2.Data.Service;


using MembroModel = BibliotecaAppV2.Models.Membro;

namespace BibliotecaAppV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MembroController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<MembroController> _logger;
        private readonly MembroService _membroService;

        public MembroController(ApplicationDbContext context,
                        UserManager<ApplicationUser> userManager,
                        ILogger<MembroController> logger,
                        MembroService membroService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _membroService = membroService;
        }


        // GET: Membro
        public IActionResult Index()
        {
            try
            {
                var membros = _context.Membros
                    .Include(m => m.User)
                    .Where(m => m.Ativo)
                    .ToList();
                return View(membros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar membros no método Index.");
                TempData["Error"] = "Erro ao carregar a lista de membros.";
                return View(Enumerable.Empty<MembroModel>());
            }
        }

        // GET: Membro/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var membro = _context.Membros.Include(m => m.User).FirstOrDefault(m => m.Id == id);
                if (membro == null) return NotFound();
                return View(membro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter detalhes do membro no método Details.");
                TempData["Error"] = "Erro ao carregar detalhes do membro.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Membro/Create
        public IActionResult Create()
        {
            try
            {
                ViewBag.Users = _userManager.GetUsersInRoleAsync("Membro").Result;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar usuários para criação de membro.");
                TempData["Error"] = "Erro ao carregar dados para criação de membro.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Membro/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MembroModel membro)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    membro.DataCadastro = DateTime.Now;
                    membro.Ativo = true;
                    _context.Membros.Add(membro);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao cadastrar membro no método Create.");
                    ModelState.AddModelError("", "Erro ao cadastrar membro. Tente novamente.");
                }
            }
            ViewBag.Users = _userManager.GetUsersInRoleAsync("Membro").Result;
            return View(membro);
        }

        // GET: Membro/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var membro = _context.Membros.Find(id);
                if (membro == null) return NotFound();
                ViewBag.Users = _userManager.GetUsersInRoleAsync("Membro").Result;
                return View(membro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar membro no método Edit.");
                TempData["Error"] = "Erro ao carregar membro para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Membro/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MembroModel membro)
        {
            if (id != membro.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membro);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar membro no método Edit.");
                    ModelState.AddModelError("", "Erro ao atualizar membro. Tente novamente.");
                }
            }
            ViewBag.Users = _userManager.GetUsersInRoleAsync("Membro").Result;
            return View(membro);
        }

        // GET: Membro/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var membro = _context.Membros.Include(m => m.User).FirstOrDefault(m => m.Id == id);
                if (membro == null) return NotFound();
                return View(membro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar membro para exclusão no método Delete.");
                TempData["Error"] = "Erro ao carregar membro para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Membro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var membro = _context.Membros.Find(id);
                if (membro != null)
                {
                    membro.Ativo = false;
                    _context.Update(membro);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar membro no método DeleteConfirmed.");
                TempData["Error"] = "Erro ao desativar o membro. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Membro/ResetarSenha/5
        [HttpGet]
        public IActionResult ResetarSenha(int id)
        {
            var membro = _membroService.ObterMembroPorId(id);
            if (membro == null) return NotFound();

            ViewBag.MembroId = membro.Id;
            ViewBag.Nome = membro.Nome;
            return View();
        }


        // POST: Membro/ResetarSenha/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetarSenha(int id, string novaSenha)
        {
            var membro = _membroService.ObterMembroPorId(id);
            if (membro == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(membro.UserId);
            if (user == null)
                return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, novaSenha);

            if (result.Succeeded)
            {
                TempData["Success"] = "Senha redefinida com sucesso.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Erro ao redefinir senha: " + string.Join(", ", result.Errors.Select(e => e.Description));
            return RedirectToAction("Index");
        }


    }
}