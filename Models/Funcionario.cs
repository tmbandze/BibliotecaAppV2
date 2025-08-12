using System;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAppV2.Models
{
    public class Funcionario
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; } = string.Empty;

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Relacionamento com o Identity
        public string? IdentityUserId { get; set; } // mantenha esse nome, mas ajuste nos services/controllers!

        public bool Ativo { get; set; } = true;
    }
}