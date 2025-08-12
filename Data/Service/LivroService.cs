using BibliotecaAppV2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BibliotecaAppV2.Data.Service
{
    public class LivroService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LivroService> _logger;

        public LivroService(ApplicationDbContext context, ILogger<LivroService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Livro> ListarTodos()
        {
            try
            {
                return _context.Livros
                    .Where(l => l.Ativo)
                    .OrderBy(l => l.Titulo)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar todos os livros.");
                return new List<Livro>();
            }
        }

        public List<Livro> BuscarLivros(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo))
                    return ListarTodos();

                termo = termo.Trim().ToLower();

                return _context.Livros
                    .Include(l => l.LivroAutores)
                        .ThenInclude(la => la.Autor)
                    .Include(l => l.Editora)
                    .Where(l => l.Ativo &&
                        (
                            l.Titulo.ToLower().Contains(termo) ||
                            l.LivroAutores.Any(la => la.Autor != null && la.Autor.Nome != null && la.Autor.Nome.ToLower().Contains(termo)) ||
                            (l.Editora != null && l.Editora.Nome.ToLower().Contains(termo)) ||
                            l.ISBN.ToLower().Contains(termo)
                        ))
                    .OrderBy(l => l.Titulo)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar livros.");
                return new List<Livro>();
            }
        }

        public Livro? ObterPorId(int id)
        {
            try
            {
                return _context.Livros
                    .Include(l => l.LivroAutores)
                        .ThenInclude(la => la.Autor)
                    .Include(l => l.Editora)
                    .Include(l => l.Categoria)
                    .FirstOrDefault(l => l.Id == id && l.Ativo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter livro com ID {id}");
                return null;
            }
        }


        public void AtualizarLivro(Livro livro)
        {
            try
            {
                var livroExistente = _context.Livros.AsNoTracking().FirstOrDefault(l => l.Id == livro.Id && l.Ativo);
                if (livroExistente == null)
                    throw new Exception("Livro não encontrado ou inativo.");

                livroExistente.Quantidade = livro.Quantidade;

                _context.Entry(livroExistente).Property(l => l.Quantidade).IsModified = true;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar livro id {livro.Id}.");
                throw;
            }
        }

        public void AtualizarCompleto(Livro livro)
        {
            try
            {
                _context.Livros.Update(livro);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar completamente o livro id {livro.Id}.");
                throw;
            }
        }

        public List<Livro> ListarTodosDisponiveis()
        {
            try
            {
                return _context.Livros
                    .Where(l => l.Ativo && l.Quantidade > 0)
                    .OrderBy(l => l.Titulo)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar livros disponíveis.");
                return new List<Livro>();
            }
        }

        public List<Livro> BuscarLivrosDisponiveis(string termo)
        {
            try
            {
                return _context.Livros
                    .Where(l => l.Ativo && l.Quantidade > 0 &&
                        (l.Titulo.Contains(termo) || l.ISBN.Contains(termo)))
                    .OrderBy(l => l.Titulo)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar livros disponíveis com termo: {termo}");
                return new List<Livro>();
            }
        }
    }
}