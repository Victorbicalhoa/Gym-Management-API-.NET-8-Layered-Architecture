using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentroTreinamento.Infrastructure.Repositories
{
    public class InstrutorRepository : Repository<Instrutor>, IInstrutorRepository
    {
        public InstrutorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Instrutor>> GetInstrutoresDisponiveisAsync(DateTime data, TimeSpan horario)
        {
            // Esta é uma lógica complexa que pode precisar de mais detalhes sobre como você modelou a disponibilidade
            // Por enquanto, é um exemplo simplificado.
            // Poderia envolver:
            // 1. Join com a tabela de Agendamentos para ver quais horários estão ocupados.
            // 2. Consulta a uma propriedade de 'HorariosTrabalho' no Instrutor.

            // Exemplo BEM simplificado:
            return await _dbSet
                         .Where(i => i.Status == Domain.Enums.StatusInstrutor.Ativo) // Assumindo StatusInstrutor enum e Status propriedade
                         .ToListAsync();
        }

        public async Task<Instrutor?> GetByCrefAsync(string cref)
        {
            return await _dbSet.FirstOrDefaultAsync(i => i.Cref == cref); // Assumindo propriedade Cref
        }
    }
}