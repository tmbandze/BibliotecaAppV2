using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAppV2.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O campo Nome Completo é obrigatório.")]
        [Display(Name = "Nome Completo")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo E-mail é obrigatório.")]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmação de senha obrigatória.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Password", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione o tipo de usuário.")]
        [Display(Name = "Tipo de Usuário")]
        public string Role { get; set; } = string.Empty;// "Admin", "Funcionario", "Membro"

        [Display(Name = "Foto de Perfil")]
        public required IFormFile FotoPerfil { get; set; }
    }
}