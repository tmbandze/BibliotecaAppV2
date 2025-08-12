using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotecaAppV2.Models
{
    public class Livro
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

        // ➕ CAMPOS NOVOS
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Display(Name = "Data de Cadastro")]
        [DataType(DataType.Date)]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        [Required(ErrorMessage = "A editora é obrigatória.")]
        [Display(Name = "Editora")]
        public int EditoraId { get; set; }
        public Editora? Editora { get; set; }

        public ICollection<LivroAutor> LivroAutores { get; set; } = new List<LivroAutor>();
    }
}
