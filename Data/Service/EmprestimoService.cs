using BibliotecaAppV2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BibliotecaAppV2.Data.Service
{
    public class EmprestimoService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmprestimoService> _logger;

        public EmprestimoService(ApplicationDbContext context, ILogger<EmprestimoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Emprestimo> ListarEmprestimosAtivos()
        {
            return _context.Emprestimos
                .Where(e => e.DataDevolucaoReal == null)
                .ToList();
        }

        public void RegistrarEmprestimo(Emprestimo emprestimo)
        {
            emprestimo.DataEmprestimo = DateTime.Now;
            _context.Emprestimos.Add(emprestimo);
            _context.SaveChanges();
        }

        public Emprestimo? ObterEmprestimoPorId(int id)
        {
            return _context.Emprestimos
                .Where(e => e.Id == id)
                .FirstOrDefault();
        }

        public void RegistrarDevolucao(int id)
        {
            var emprestimo = _context.Emprestimos.Find(id);
            if (emprestimo != null && emprestimo.DataDevolucaoReal == null)
            {
                emprestimo.DataDevolucaoReal = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public List<Livro> ObterLivrosDisponiveis(int? livroAtualId = null)
        {
            var livros = _context.Livros.Where(l => l.Quantidade > 0).ToList();
            if (livroAtualId.HasValue)
            {
                var livroAtual = _context.Livros.Find(livroAtualId.Value);
                if (livroAtual != null && !livros.Contains(livroAtual))
                {
                    livros.Add(livroAtual);
                }
            }
            return livros;
        }

        public List<Membro> ObterMembrosAtivos(int? membroAtualId = null)
        {
            var membros = _context.Membros.Where(m => m.Ativo).ToList();
            if (membroAtualId.HasValue)
            {
                var membroAtual = _context.Membros.Find(membroAtualId.Value);
                if (membroAtual != null && !membros.Contains(membroAtual))
                {
                    membros.Add(membroAtual);
                }
            }
            return membros;
        }

        public void AtualizarEmprestimo(Emprestimo emprestimo)
        {
            var entidade = _context.Emprestimos.Find(emprestimo.Id);
            if (entidade != null)
            {
                entidade.DataDevolucaoPrevista = emprestimo.DataDevolucaoPrevista;
                entidade.DataDevolucaoReal = emprestimo.DataDevolucaoReal;
                entidade.Ativo = emprestimo.Ativo;
                _context.SaveChanges();
            }
        }

        public void ExcluirEmprestimo(int id)
        {
            var emprestimo = _context.Emprestimos.Find(id);
            if (emprestimo != null)
            {
                _context.Emprestimos.Remove(emprestimo);
                _context.SaveChanges();
            }
        }

        public int ContarEmprestimosPorFuncionario(string funcionarioUserId)
        {
            return _context.Emprestimos
                .Count(e => e.FuncionarioUserId == funcionarioUserId);
        }

        public int ContarDevolucoesPendentesPorFuncionario(string funcionarioUserId)
        {
            return _context.Emprestimos
                .Count(e => e.FuncionarioUserId == funcionarioUserId && e.DataDevolucaoReal == null);
        }

        public int ContarAtrasosPorFuncionario(string funcionarioUserId)
        {
            var hoje = DateTime.Now.Date;
            return _context.Emprestimos
                .Count(e => e.FuncionarioUserId == funcionarioUserId && e.DataDevolucaoReal == null && e.DataDevolucaoPrevista < hoje);
        }

        public List<Emprestimo> ListarPorMembroId(int membroId)
        {
            try
            {
                return _context.Emprestimos
                    .Include(e => e.Livro)
                    .Where(e => e.MembroId == membroId && e.Ativo)
                    .OrderByDescending(e => e.DataEmprestimo)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar empréstimos por membro.");
                return new List<Emprestimo>();
            }
        }

        public int ObterMembroIdPorUserId(string userId)
        {
            try
            {
                var membro = _context.Membros.FirstOrDefault(m => m.UserId == userId && m.Ativo);
                return membro?.Id ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter MembroId pelo IdentityUserId.");
                return 0;
            }
        }

        public Emprestimo? ObterPorId(int id)
        {
            return _context.Emprestimos
                .Include(e => e.Livro)
                .Include(e => e.Membro)
                .FirstOrDefault(e => e.Id == id);
        }


    }
}