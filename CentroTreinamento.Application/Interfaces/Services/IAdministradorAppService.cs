// CentroTreinamento.Application/Interfaces/IAdministradorAppService.cs
using CentroTreinamento.Application.DTOs.Administrador;
using CentroTreinamento.Domain.Enums; // Para StatusAdministrador
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroTreinamento.Application.Interfaces.Services
{
    public interface IAdministradorAppService
    {
        Task<IEnumerable<AdministradorViewModel>> GetAllAdministradoresAsync();
        Task<AdministradorViewModel?> GetAdministradorByIdAsync(Guid id);
        Task<AdministradorViewModel> CreateAdministradorAsync(AdministradorInputModel administradorInput);
        Task<bool> UpdateAdministradorAsync(Guid id, AdministradorInputModel administradorInput);
        Task<bool> DeleteAdministradorAsync(Guid id);
        Task<bool> UpdateAdministradorStatusAsync(Guid id, StatusAdministrador novoStatus);

        // Método específico para o login do Administrador
        Task<AdministradorViewModel?> LoginAdministradorAsync(string cpf, string senha);
    }
}