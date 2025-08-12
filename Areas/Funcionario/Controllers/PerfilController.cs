using BibliotecaAppV2.Data.Service;
using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;

using FuncionarioModel = BibliotecaAppV2.Models.Funcionario;

namespace BibliotecaAppV2.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    [Authorize(Roles = "Funcionario")]
    public class PerfilController : Controller
    {
        private readonly FuncionarioService _funcionarioService;
        private readonly ILogger<PerfilController> _logger;

        public PerfilController(FuncionarioService funcionarioService, ILogger<PerfilController> logger)
        {
            _funcionarioService = funcionarioService;
            _logger = logger;
        }

        // GET: Perfil
        public IActionResult Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return Unauthorized();
                var funcionario = _funcionarioService.ObterFuncionarioPorUserId(userId);
                if (funcionario == null) return NotFound();
                return View(funcionario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar perfil do funcionário.");
                TempData["Error"] = "Erro ao carregar seu perfil.";
                return View();
            }
        }

        // GET: Perfil/Edit
        public IActionResult Edit()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return Unauthorized();
                var funcionario = _funcionarioService.ObterFuncionarioPorUserId(userId);
                if (funcionario == null) return NotFound();
                return View(funcionario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados para edição de perfil.");
                TempData["Error"] = "Erro ao carregar dados do perfil.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Perfil/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(FuncionarioModel funcionario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _funcionarioService.AtualizarFuncionario(funcionario);
                    TempData["Success"] = "Perfil atualizado com sucesso.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar perfil do funcionário.");
                    ModelState.AddModelError("", "Erro ao atualizar seu perfil. Tente novamente.");
                }
            }
            return View(funcionario);
        }
    }
}