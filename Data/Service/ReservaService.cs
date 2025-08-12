using BibliotecaAppV2.Data;
using BibliotecaAppV2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BibliotecaAppV2.Data.Service
{
    public class ReservaService
    {
        private readonly ApplicationDbContext _context;

        public ReservaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Reserva> ListarPorMembroId(int membroId)
        {
            return _context.Reservas
                .Include(r => r.Livro)
                .Where(r => r.MembroId == membroId && r.Ativo)
                .OrderByDescending(r => r.DataReserva)
                .ToList();
        }

        public bool RegistrarReserva(int membroId, int livroId)
        {
            var livro = _context.Livros.FirstOrDefault(l => l.Id == livroId && l.Ativo && l.Quantidade > 0);
            if (livro == null)
                return false;

            var reservaExistente = _context.Reservas.Any(r =>
                r.MembroId == membroId && r.LivroId == livroId && r.Ativo);
            if (reservaExistente)
                return false;

            var novaReserva = new Reserva
            {
                LivroId = livroId,
                MembroId = membroId,
                DataReserva = DateTime.Now,
                DataLimiteRetirada = DateTime.Now.AddDays(3), // Por exemplo, 3 dias para retirar
                Ativo = true
            };

            _context.Reservas.Add(novaReserva);
            _context.SaveChanges();
            return true;
        }

        public bool CancelarReserva(int reservaId, int membroId)
        {
            var reserva = _context.Reservas.FirstOrDefault(r => r.Id == reservaId && r.MembroId == membroId && r.Ativo);
            if (reserva == null)
                return false;

            reserva.Ativo = false;
            _context.SaveChanges();
            return true;
        }

        public int ObterMembroIdPorUserId(string userId)
        {
            var membro = _context.Membros.FirstOrDefault(m => m.UserId == userId && m.Ativo);
            return membro?.Id ?? 0;
        }
    }
}
