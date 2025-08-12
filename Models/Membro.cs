using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotecaAppV2.Models
{
    public class Membro
    {
        public int Id { get; set; }

        [Display(Name = "Usuário do Sistema")]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        [Display(Name = "Telefone")]
        public string Telefone { get; set; } = string.Empty;

        [Display(Name = "Endereço")]
        public string Endereco { get; set; } = string.Empty;

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Campo para soft delete (desativação)
        public bool Ativo { get; set; } = true;

    }
}