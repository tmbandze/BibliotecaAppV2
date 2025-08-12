using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using BibliotecaAppV2.Areas.Admin.ViewModels;
using System;
using System.IO;
using System.Linq;

namespace BibliotecaAppV2.Data.Service
{
    public class RelatorioPdfService
    {
        public byte[] GerarRelatorio(RelatorioEmprestimoViewModel model)
        {
            // Caminho absoluto da imagem do logotipo
            var caminhoLogo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "dzovo_semfundo.png");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Cabeçalho com logotipo e título
                    page.Header().Column(col =>
                    {
                        col.Item().AlignCenter().Element(container =>
                        {
                            var img = Image.FromFile(caminhoLogo);
                            container
                                .Height(80)
                                .Image(img)
                                .FitHeight();
                        });



                        col.Item().AlignCenter().Text($"📊 Relatório de Empréstimos - {model.TipoRelatorio.ToUpper()}")
                            .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                        col.Item().AlignCenter().Text($"Data: {model.DataReferencia:dd/MM/yyyy}").FontSize(10);
                    });




                    // Corpo principal
                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Item().Text($"Total de Empréstimos: {model.TotalEmprestimos}");
                        col.Item().Text($"Devoluções Feitas: {model.TotalDevolvidos}");
                        col.Item().Text($"Empréstimos Pendentes: {model.TotalPendentes}");

                        // Tabela com livros
                        col.Item().PaddingTop(15).Element(CreateTabelaLivros(model));
                    });

                    // Rodapé
                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.Span("BibliotecaAppV2 © ").FontSize(10);
                        txt.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(10).FontColor(Colors.Grey.Darken1);
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static Action<IContainer> CreateTabelaLivros(RelatorioEmprestimoViewModel model)
        {
            return container =>
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Livro").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("Qtd.").Bold();
                    });

                    foreach (var livro in model.LivrosMaisEmprestados)
                    {
                        table.Cell().Element(CellStyle).Text(livro.Titulo);
                        table.Cell().Element(CellStyle).AlignRight().Text(livro.Quantidade.ToString());
                    }

                    static IContainer CellStyle(IContainer container) =>
                        container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                });
            };
        }
    }
}
