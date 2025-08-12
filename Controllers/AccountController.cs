using System;
using System.IO;
using System.Threading.Tasks;
using BibliotecaAppV2.Data;
using BibliotecaAppV2.Models;
using BibliotecaAppV2.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAppV2.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;


        public AccountController(UserManager<ApplicationUser> userManager,
                         SignInManager<ApplicationUser> signInManager,
                         IWebHostEnvironment env,
                         ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string? email, string? password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "E-mail e senha obrigatórios.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    else if (roles.Contains("Funcionario"))
                        return RedirectToAction("Index", "Home", new { area = "Funcionario" });
                    else if (roles.Contains("Membro"))
                        return RedirectToAction("Index", "Home", new { area = "Membro" });
                }
            }
            ModelState.AddModelError("", "Login inválido.");
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string? fotoPath = null;
            if (model.FotoPerfil != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.FotoPerfil.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.FotoPerfil.CopyToAsync(stream);
                }
                fotoPath = "/uploads/" + fileName;
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                NomeCompleto = model.NomeCompleto,
                Role = model.Role,
                FotoPerfil = fotoPath
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);

                // SALVA TAMBÉM NA TABELA MEMBRO
                if (model.Role == "Membro")
                {
                    var membro = new Membro
                    {
                        Nome = model.NomeCompleto,
                        Email = model.Email,
                        UserId = user.Id,
                        DataCadastro = DateTime.Now,
                        Endereco = "",
                        Telefone = "",
                        Ativo = true
                    };

                    _context.Membros.Add(membro);
                    await _context.SaveChangesAsync();
                }

                await _signInManager.SignInAsync(user, isPersistent: false);

                if (model.Role == "Admin")
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                else if (model.Role == "Funcionario")
                    return RedirectToAction("Index", "Home", new { area = "Funcionario" });
                else
                    return RedirectToAction("Index", "Home", new { area = "Membro" });
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View("AccessDenied");
    }
}