using System;
using System.Collections.Generic;

namespace BibliotecaAppV2.Areas.Admin.ViewModels
{
    public class RelatorioEmprestimoViewModel
    {
        public string TipoRelatorio { get; set; } = "diario"; // "diario", "semanal", "mensal"
        public DateTime DataReferencia { get; set; } = DateTime.Today;
        public DateTime DataInicio { get; set; }       // NOVO
        public DateTime DataFim { get; set; }
        public int TotalEmprestimos { get; set; }
        public int TotalDevolvidos { get; set; }
        public int TotalPendentes { get; set; }

        public List<LivroEmprestadoViewModel> LivrosMaisEmprestados { get; set; } = new();
    }

    public class LivroEmprestadoViewModel
    {
        public string Titulo { get; set; } = string.Empty;
        public int Quantidade { get; set; }
    }
}
