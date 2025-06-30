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
    public class PagamentoRepository : Repository<Pagamento>, IPagamentoRepository
    {
        public PagamentoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Pagamento>> GetPagamentosPorAlunoAsync(Guid alunoId)
        {
            return await _dbSet
                         .Where(p => p.AlunoId == alunoId) // Assumindo que Pagamento tem AlunoId
                         .ToListAsync();
        }

        public async Task<IEnumerable<Pagamento>> GetPagamentosPorPeriodoAsync(DateTime inicio, DateTime fim)
        {
            return await _dbSet
                         .Where(p => p.DataPagamento >= inicio && p.DataPagamento <= fim) // Assumindo DataPagamento
                         .ToListAsync();
        }

        public async Task<IEnumerable<Pagamento>> GetPagamentosPendentesAsync()
        {
            return await _dbSet
                         .Where(p => p.StatusPagamento == Domain.Enums.StatusPagamento.Pendente) // Assumindo StatusPagamento enum
                         .ToListAsync();
        }
    }
}