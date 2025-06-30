using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Infrastructure.Data;
using Microsoft.EntityFrameworkCore; // Pode ser útil para métodos FirstOrDefaultAsync, etc.
using System.Threading.Tasks;

namespace CentroTreinamento.Infrastructure.Repositories
{
    // AlunoRepository herda da implementação genérica e da sua interface específica
    public class AlunoRepository : Repository<Aluno>, IAlunoRepository
    {
        public AlunoRepository(ApplicationDbContext context) : base(context)
        {
            // O construtor passa o DbContext para a classe base
        }

        // Implemente aqui quaisquer métodos que você adicionou especificamente a IAlunoRepository.
        // Se IAlunoRepository estiver vazia (como recomendamos), esta classe também será vazia
        // além do construtor, pois herda tudo da base.
        /*
        // Exemplo de como implementar um método específico (se você tivesse GetByCpfAsync na interface):
        public async Task<Aluno?> GetByCpfAsync(string cpf)
        {
            // _dbSet é herdado da classe Repository<TEntity>
            return await _dbSet.FirstOrDefaultAsync(a => a.Cpf == cpf);
        }
        */
    }
}