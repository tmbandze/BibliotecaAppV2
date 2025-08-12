using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotecaAppV2.Models
{
    public class Emprestimo
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Livro")]
        public int LivroId { get; set; }
        [ForeignKey("LivroId")]
        public Livro? Livro { get; set; } 

        [Required]
        [Display(Name = "Membro")]
        public int MembroId { get; set; }
        [ForeignKey("MembroId")]
        public Membro? Membro { get; set; }

        [Required]
        [Display(Name = "Data de Empréstimo")]
        public DateTime DataEmprestimo { get; set; } = DateTime.Now;

        [Display(Name = "Data de Devolução Prevista")]
        public DateTime? DataDevolucaoPrevista { get; set; }

        [Display(Name = "Data de Devolução Real")]
        public DateTime? DataDevolucaoReal { get; set; }

        public bool Ativo { get; set; } = true;

        [Display(Name = "Funcionário Responsável")]
        public string FuncionarioUserId { get; set; } = string.Empty;
        [NotMapped]
        [Display(Name = "Status")]
        public string Status
        {
            get
            {
                if (DataDevolucaoReal.HasValue)
                    return "Devolvido";
                if (DataDevolucaoPrevista.HasValue && DataDevolucaoPrevista.Value.Date < DateTime.Today)
                    return "Atrasado";
                return "Em andamento";
            }
        }
    }
}