using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CentroTreinamento.Application.DTOs.Agendamento;
using CentroTreinamento.Domain.Enums; // Para StatusAgendamento

namespace CentroTreinamento.Application.Interfaces.Services
{
    public interface IAgendamentoAppService
    {
        Task<AgendamentoViewModel> CriarAgendamentoAsync(AgendamentoInputModel inputModel);
        Task<AgendamentoViewModel> AtualizarAgendamentoAsync(Guid id, AgendamentoUpdateModel updateModel);
        Task<AgendamentoViewModel?> GetAgendamentoByIdAsync(Guid id);
        Task<IEnumerable<AgendamentoViewModel>> GetAgendamentosByAlunoIdAsync(Guid alunoId);
        Task<IEnumerable<AgendamentoViewModel>> GetAgendamentosByInstrutorIdAsync(Guid instrutorId);
        Task<IEnumerable<AgendamentoViewModel>> GetAllAgendamentosAsync(); // Para administradores

        Task<AgendamentoViewModel> AprovarAgendamentoAsync(Guid agendamentoId);
        Task<AgendamentoViewModel> RecusarAgendamentoAsync(Guid agendamentoId, string motivo);
        Task<AgendamentoViewModel> CancelarAgendamentoAsync(Guid agendamentoId, string motivo);
    }
}