using BibliotecaAppV2.Models;
using System.Linq;

namespace BibliotecaAppV2.Data.Service
{
    public class FuncionarioService
    {
        private readonly ApplicationDbContext _context;

        public FuncionarioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Funcionario? ObterFuncionarioPorUserId(string userId)
        {
            return _context.Funcionarios.FirstOrDefault(f => f.IdentityUserId == userId);
        }

        public void AtualizarFuncionario(Funcionario funcionario)
        {
            _context.Funcionarios.Update(funcionario);
            _context.SaveChanges();
        }

        public void ResetarSenha(int funcionarioId, string novaSenhaHash)
        {
            var funcionario = _context.Funcionarios.Find(funcionarioId);
            if (funcionario != null)
            {
                //funcionario.SenhaHash = novaSenhaHash;
                _context.SaveChanges();
            }
        }

    }
}