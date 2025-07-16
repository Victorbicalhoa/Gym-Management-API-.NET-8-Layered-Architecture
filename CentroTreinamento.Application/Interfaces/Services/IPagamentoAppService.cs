using System;
using System.Threading.Tasks;
using CentroTreinamento.Application.DTOs.Pagamento;
using System.Collections.Generic; // Adicionar para métodos de listagem futuros

namespace CentroTreinamento.Application.Interfaces.Services
{
    public interface IPagamentoAppService
    {
        Task<PagamentoViewModel> RegistrarPagamentoAsync(PagamentoInputModel inputModel);
        Task<PagamentoViewModel?> GetPagamentoByIdAsync(Guid id); // Pode retornar nulo
        Task<IEnumerable<PagamentoViewModel>> GetPagamentosByAlunoIdAsync(Guid alunoId); // Adicionado para RF4.5
        // Você pode adicionar outros métodos para consultas, updates, etc.
    }
}