using BibliotecaAppV2.Data;
using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

using FuncionarioModel = BibliotecaAppV2.Models.Funcionario;
namespace BibliotecaAppV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FuncionarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<FuncionarioController> _logger;
        

        public FuncionarioController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<FuncionarioController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Admin/Funcionario
        public IActionResult Index()
        {
            var funcionarios = _context.Funcionarios.Where(f => f.Ativo).ToList();
            return View(funcionarios);
        }

        // GET: Admin/Funcionario/Create
        public IActionResult Create() => View();

        // POST: Admin/Funcionario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FuncionarioModel funcionario, string password)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new ApplicationUser
                    {
                        UserName = funcionario.Email,
                        Email = funcionario.Email,
                        NomeCompleto = funcionario.Nome,
                        EmailConfirmed = true
                    };
                    var result = await _userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Funcionario");

                        funcionario.IdentityUserId = user.Id;
                        funcionario.DataCadastro = DateTime.Now;
                        funcionario.Ativo = true;
                        _context.Funcionarios.Add(funcionario);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao cadastrar funcionário no método Create.");
                    ModelState.AddModelError("", "Erro inesperado ao cadastrar funcionário. Tente novamente ou contate o suporte.");
                }
            }
            return View(funcionario);
        }

        // GET: Admin/Funcionario/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            var funcionario = _context.Funcionarios.FirstOrDefault(f => f.Id == id);
            if (funcionario == null) return NotFound();
            return View(funcionario);
        }

        // GET: Admin/Funcionario/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var funcionario = _context.Funcionarios.Find(id);
            if (funcionario == null) return NotFound();
            return View(funcionario);
        }

        // POST: Admin/Funcionario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, FuncionarioModel funcionario)
        {
            if (id != funcionario.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funcionario);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar funcionário no método Edit.");
                    ModelState.AddModelError("", "Erro ao atualizar funcionário. Tente novamente.");
                }
            }
            return View(funcionario);
        }

        // GET: Admin/Funcionario/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var funcionario = _context.Funcionarios.Find(id);
            if (funcionario == null) return NotFound();
            return View(funcionario);
        }

        // POST: Admin/Funcionario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var funcionario = _context.Funcionarios.Find(id);
                if (funcionario != null)
                {
                    funcionario.Ativo = false;
                    _context.Update(funcionario);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar funcionário no método DeleteConfirmed.");
                TempData["Error"] = "Erro ao desativar o funcionário. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult ResetarSenha(int id)
        {
            var funcionario = _context.Funcionarios.FirstOrDefault(f => f.Id == id);
            if (funcionario == null) return NotFound();

            ViewBag.FuncionarioId = funcionario.Id;
            ViewBag.Nome = funcionario.Nome;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetarSenha(int id, string novaSenha)
        {
            var funcionario = _context.Funcionarios.FirstOrDefault(f => f.Id == id);
            if (funcionario == null || string.IsNullOrEmpty(funcionario.IdentityUserId))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(funcionario.IdentityUserId);
            if (user == null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, novaSenha);

            if (result.Succeeded)
            {
                TempData["Success"] = "Senha redefinida com sucesso.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Erro ao redefinir senha: " + string.Join(", ", result.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }

    }
}