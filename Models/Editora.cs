using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAppV2.Models
{
    public class Editora
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome da Editora")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }
        public bool Ativo { get; set; } = true;

        public ICollection<Livro> Livros { get; set; } = new List<Livro>();
    }
}