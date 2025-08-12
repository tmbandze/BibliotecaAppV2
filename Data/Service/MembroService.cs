using BibliotecaAppV2.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BibliotecaAppV2.Data.Service
{
    public class MembroService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MembroService> _logger;

        public MembroService(ApplicationDbContext context, ILogger<MembroService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Membro> ListarMembrosAtendidosPorFuncionario(string funcionarioUserId)
        {
            return _context.Emprestimos
                .Where(e => e.FuncionarioUserId == funcionarioUserId)
                .Select(e => e.Membro)
                .Where(m => m != null)
                .Distinct()
                .Cast<Membro>()
                .ToList();
        }

        public int ContarMembrosAtendidosPorFuncionario(string funcionarioUserId)
        {
            return ListarMembrosAtendidosPorFuncionario(funcionarioUserId).Count;
        }

        public Membro? ObterMembroPorId(int id)
        {
            return _context.Membros.Find(id);
        }

        public void AtualizarMembro(Membro membro)
        {
            var entidade = _context.Membros.Find(membro.Id);
            if (entidade != null)
            {
                entidade.Nome = membro.Nome;
                entidade.Email = membro.Email;
                entidade.DataNascimento = membro.DataNascimento;
                entidade.Telefone = membro.Telefone;
                entidade.Endereco = membro.Endereco;
                entidade.Ativo = membro.Ativo;
                _context.SaveChanges();
            }
        }

        public void ExcluirMembro(int id)
        {
            // Exclusão restrita
        }

        public Membro? ObterMembroPorUserId(string userId)
        {
            try
            {
                return _context.Membros.FirstOrDefault(m => m.UserId == userId && m.Ativo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter membro pelo IdentityUserId.");
                return null;
            }
        }

        public void ResetarSenha(int membroId, string novaSenhaHash)
        {
            var membro = _context.Membros.Find(membroId);
            if (membro != null)
            {
                //membro.SenhaHash = novaSenhaHash;
                _context.SaveChanges();
            }
        }

    }
}