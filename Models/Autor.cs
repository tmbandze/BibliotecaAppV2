using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAppV2.Models
{
    public class Autor
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome do Autor")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        [Display(Name = "Nacionalidade")]
        public string? Nacionalidade { get; set; }
        public bool Ativo { get; set; } = true;

        public ICollection<LivroAutor> LivroAutores { get; set; } = new List<LivroAutor>();
    }
}