using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace BibliotecaAppV2.Areas.Admin.ViewModels
{
    public class EmprestimoFiltroViewModel
    {
        public string? Status { get; set; }
        public string? NomeMembro { get; set; }         // NOVO
        public string? FuncionarioSelecionado { get; set; }        // NOVO
        public DateTime? DataInicio { get; set; }       // NOVO
        public DateTime? DataFim { get; set; }          // NOVO
        public string? NomePesquisa { get; set; } // ADICIONE isso se for necessário
        public List<SelectListItem>? FuncionariosDisponiveis { get; set; }


        public IEnumerable<EmprestimoViewModel>? Emprestimos { get; set; }
    }

    public class EmprestimoViewModel
    {
        public int Id { get; set; }
        public string? Livro { get; set; }
        public string? Membro { get; set; }
        public string? Funcionario { get; set; }    // NOVO: usado para filtros detalhados
        public DateTime DataEmprestimo { get; set; }
        public DateTime? DataDevolucaoPrevista { get; set; }
        public DateTime? DataDevolucaoReal { get; set; }
        public string? Status { get; set; }
    }
}
