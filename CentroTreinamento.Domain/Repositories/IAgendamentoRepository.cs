using CentroTreinamento.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroTreinamento.Domain.Repositories
{
    public interface IAgendamentoRepository : IRepository<Agendamento>
    {
        // Exemplo de métodos específicos para Agendamento
        Task<IEnumerable<Agendamento>> GetAgendamentosPorAlunoAsync(Guid alunoId);
        Task<IEnumerable<Agendamento>> GetAgendamentosPorPeriodoAsync(DateTime inicio, DateTime fim);
        Task<IEnumerable<Agendamento>> GetAgendamentosPorInstrutorAsync(Guid instrutorId);
    }
}