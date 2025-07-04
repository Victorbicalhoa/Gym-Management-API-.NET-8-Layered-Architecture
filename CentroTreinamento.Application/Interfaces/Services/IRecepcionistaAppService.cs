// CentroTreinamento.Application/Interfaces/Services/IRecepcionistaAppService.cs
using CentroTreinamento.Application.DTOs.Recepcionista;
using CentroTreinamento.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroTreinamento.Application.Interfaces.Services
{
    public interface IRecepcionistaAppService
    {
        Task<RecepcionistaViewModel> CreateRecepcionistaAsync(RecepcionistaInputModel inputModel);
        Task<RecepcionistaViewModel?> GetRecepcionistaByIdAsync(Guid id);
        Task<IEnumerable<RecepcionistaViewModel>> GetAllRecepcionistasAsync();
        Task UpdateRecepcionistaAsync(Guid id, RecepcionistaInputModel updateModel); // Usando InputModel para update
        Task UpdateRecepcionistaStatusAsync(Guid id, StatusRecepcionista newStatus); // Método específico para status
        Task DeleteRecepcionistaAsync(Guid id);
        Task<RecepcionistaViewModel?> GetRecepcionistaByCpfAsync(string cpf);
    }
}