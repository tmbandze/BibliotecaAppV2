using System;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAppV2.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Livro")]
        public int LivroId { get; set; }

        [Required]
        [Display(Name = "Membro")]
        public int MembroId { get; set; }

        [Required]
        [Display(Name = "Data da Reserva")]
        [DataType(DataType.Date)]
        public DateTime DataReserva { get; set; }

        [Display(Name = "Data Limite Retirada")]
        [DataType(DataType.Date)]
        public DateTime? DataLimiteRetirada { get; set; }

        // Relacionamentos
        public Livro? Livro { get; set; }
        public Membro? Membro { get; set; }
        public bool Ativo { get; set; } = true;
    }
}