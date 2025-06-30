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
    public class AgendamentoRepository : Repository<Agendamento>, IAgendamentoRepository
    {
        public AgendamentoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Agendamento>> GetAgendamentosPorAlunoAsync(Guid alunoId)
        {
            return await _dbSet
                         .Where(a => a.AlunoId == alunoId) // Assumindo que Agendamento tem AlunoId
                         .ToListAsync();
        }

        public async Task<IEnumerable<Agendamento>> GetAgendamentosPorPeriodoAsync(DateTime inicio, DateTime fim)
        {
            return await _dbSet
                         .Where(a => a.DataHoraInicio >= inicio && a.DataHoraFim <= fim) // Assumindo DataHoraInicio/Fim
                         .ToListAsync();
        }

        public async Task<IEnumerable<Agendamento>> GetAgendamentosPorInstrutorAsync(Guid instrutorId)
        {
            return await _dbSet
                         .Where(a => a.InstrutorId == instrutorId) // Assumindo que Agendamento tem InstrutorId
                         .ToListAsync();
        }
    }
}