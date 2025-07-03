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
            return await _dbSet
                         .Where(i => i.Status == Domain.Enums.StatusInstrutor.Ativo)
                         .ToListAsync();
        }

        public async Task<Instrutor?> GetByCrefAsync(string cref)
        {
            return await _dbSet.FirstOrDefaultAsync(i => i.Cref == cref);
        }

        public async Task<Instrutor?> GetByCpfAsync(string cpf)
        {
            return await _dbSet.FirstOrDefaultAsync(i => i.Cpf == cpf);
        }
    }
}