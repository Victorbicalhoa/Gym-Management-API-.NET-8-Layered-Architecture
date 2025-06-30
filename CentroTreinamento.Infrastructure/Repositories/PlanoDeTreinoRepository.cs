using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentroTreinamento.Infrastructure.Repositories
{
    public class PlanoDeTreinoRepository : Repository<PlanoDeTreino>, IPlanoDeTreinoRepository
    {
        public PlanoDeTreinoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PlanoDeTreino>> GetPlanosDeTreinoPorAlunoAsync(Guid alunoId)
        {
            return await _dbSet
                         .Where(p => p.AlunoId == alunoId) // Assumindo que PlanoDeTreino tem AlunoId
                         .ToListAsync();
        }

        public async Task<IEnumerable<PlanoDeTreino>> GetPlanosDeTreinoAtivosAsync()
        {
            return await _dbSet
                         .Where(p => p.Status == Domain.Enums.StatusPlano.Ativo) // Assumindo StatusPlano enum e propriedade Status
                         .ToListAsync();
        }
    }
}