// CentroTreinamento.Application/Interfaces/Services/IInstrutorService.cs
using CentroTreinamento.Application.DTOs.Instrutor; // Referência atualizada
using CentroTreinamento.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroTreinamento.Application.Interfaces.Services
{
    public interface IInstrutorAppService
    {
        Task<InstrutorViewModel> GetByIdAsync(Guid id);
        Task<IEnumerable<InstrutorViewModel>> GetAllAsync();

        Task<InstrutorViewModel> CreateInstrutorAsync(InstrutorInputModel inputModel);
        Task UpdateInstrutorAsync(Guid id, InstrutorInputModel inputModel);
        Task DeleteInstrutorAsync(Guid id);

        Task UpdateInstrutorStatusAsync(Guid id, StatusInstrutor novoStatus);
        Task<InstrutorViewModel> GetByCrefAsync(string cref);
        Task<InstrutorViewModel> GetByCpfAsync(string cpf);
        Task<IEnumerable<InstrutorViewModel>> GetAvailableInstrutoresAsync(DateTime data, TimeSpan horario);
    }
}