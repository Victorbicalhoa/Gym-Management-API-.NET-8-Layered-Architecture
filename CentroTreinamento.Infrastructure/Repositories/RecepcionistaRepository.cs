// CentroTreinamento.Infrastructure/Repositories/RecepcionistaRepository.cs
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CentroTreinamento.Infrastructure.Repositories
{
    public class RecepcionistaRepository : Repository<Recepcionista>, IRecepcionistaRepository
    {
        public RecepcionistaRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Recepcionista?> GetByCpfAsync(string cpf)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.Cpf == cpf);
        }
    }
}