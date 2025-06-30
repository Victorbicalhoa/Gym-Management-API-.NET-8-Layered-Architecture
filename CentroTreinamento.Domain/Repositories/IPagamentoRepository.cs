using CentroTreinamento.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroTreinamento.Domain.Repositories
{
    public interface IPagamentoRepository : IRepository<Pagamento>
    {
        // Exemplo de métodos específicos para Pagamento
        Task<IEnumerable<Pagamento>> GetPagamentosPorAlunoAsync(Guid alunoId);
        Task<IEnumerable<Pagamento>> GetPagamentosPorPeriodoAsync(DateTime inicio, DateTime fim);
        Task<IEnumerable<Pagamento>> GetPagamentosPendentesAsync();
    }
}