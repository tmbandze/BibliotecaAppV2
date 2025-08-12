using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAppV2.Models
{
    public class LivroEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Display(Name = "Ano de Publicação")]
        public int AnoPublicacao { get; set; }

        [Display(Name = "ISBN")]
        public string ISBN { get; set; } = string.Empty;

        [Display(Name = "Quantidade em Estoque")]
        public int Quantidade { get; set; }

        public bool Ativo { get; set; } = true;

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "A editora é obrigatória.")]
        [Display(Name = "Editora")]
        public int EditoraId { get; set; }

        [Display(Name = "Autores")]
        public List<int> AutoresIds { get; set; } = new();

        public List<Autor> AutoresDisponiveis { get; set; } = new();
    }
}