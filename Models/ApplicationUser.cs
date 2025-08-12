using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAppV2.Models
{
    // Usuário base expandido para papéis e foto de perfil
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string NomeCompleto { get; set; } = string.Empty;

        // Caminho para a foto de perfil (relativo à pasta /uploads)
        public string? FotoPerfil { get; set; } = string.Empty;

        // Papel: "Admin", "Funcionario", "Membro"
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}