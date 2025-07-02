// CentroTreinamento.Application/Services/AdministradorAppService.cs
using CentroTreinamento.Application.DTOs;
using CentroTreinamento.Application.Interfaces;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Domain.Enums; // Para StatusAdministrador e UserRole
using System;
using System.Collections.Generic;
using System.Linq; // Para o método Select
using System.Threading.Tasks;

namespace CentroTreinamento.Application.Services
{
    public class AdministradorAppService : IAdministradorAppService
    {
        private readonly IAdministradorRepository _administradorRepository;
        private readonly IPasswordHasher _passwordHasher; // Para o hash de senhas

        public AdministradorAppService(IAdministradorRepository administradorRepository, IPasswordHasher passwordHasher)
        {
            _administradorRepository = administradorRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<AdministradorViewModel>> GetAllAdministradoresAsync()
        {
            var administradores = await _administradorRepository.GetAllAsync();
            return administradores.Select(a => new AdministradorViewModel
            {
                Id = a.Id,
                Nome = a.Nome,
                Cpf = a.Cpf,
                Status = a.Status,
                Role = a.Role
            });
        }

        public async Task<AdministradorViewModel?> GetAdministradorByIdAsync(Guid id)
        {
            var administrador = await _administradorRepository.GetByIdAsync(id);
            if (administrador == null) return null;

            return new AdministradorViewModel
            {
                Id = administrador.Id,
                Nome = administrador.Nome,
                Cpf = administrador.Cpf,
                Status = administrador.Status,
                Role = administrador.Role
            };
        }

        public async Task<AdministradorViewModel> CreateAdministradorAsync(AdministradorInputModel administradorInput)
        {
            // 1. Hash da senha
            var senhaHash = _passwordHasher.HashPassword(administradorInput.Senha!);

            // 2. Normalizar o CPF
            var cpfNormalizado = administradorInput.Cpf!.Replace(".", "").Replace("-", "");

            // 3. Criar a entidade de domínio
            var administrador = new Administrador(
                Guid.NewGuid(),
                administradorInput.Nome!,
                senhaHash,
                StatusAdministrador.Ativo, // Status inicial padrão
                UserRole.Administrador, // Definir o papel
                cpfNormalizado
            );

            // 4. Adicionar ao repositório e salvar as mudanças
            await _administradorRepository.AddAsync(administrador);
            await _administradorRepository.SaveChangesAsync(); // CRUCIAL!

            // 5. Retornar ViewModel
            return new AdministradorViewModel
            {
                Id = administrador.Id,
                Nome = administrador.Nome,
                Cpf = administrador.Cpf,
                Status = administrador.Status,
                Role = administrador.Role
            };
        }

        // No método UpdateAdministradorAsync do AdministradorAppService
        public async Task<bool> UpdateAdministradorAsync(Guid id, AdministradorInputModel administradorInput) // Mantendo Task<bool> por enquanto
        {
            var existingAdministrador = await _administradorRepository.GetByIdAsync(id);

            if (existingAdministrador == null)
            {
                return false; // Administrador não encontrado
            }

            // Normaliza o CPF do input
            var cpfNormalizado = administradorInput.Cpf!.Replace(".", "").Replace("-", "");

            // Hash da nova senha (se fornecida)
            string? novaSenhaHash = null;
            if (!string.IsNullOrEmpty(administradorInput.Senha))
            {
                novaSenhaHash = _passwordHasher.HashPassword(administradorInput.Senha!);
            }

            // CHAMA O MÉTODO DA ENTIDADE para atualizar os dados
            existingAdministrador.AtualizarDados(
                administradorInput.Nome!,
                cpfNormalizado,
                novaSenhaHash // Passa null se não houver nova senha, o método da entidade lida com isso
            );

            _administradorRepository.Update(existingAdministrador);
            await _administradorRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAdministradorAsync(Guid id)
        {
            var administrador = await _administradorRepository.GetByIdAsync(id);
            if (administrador == null) return false;

            _administradorRepository.Delete(administrador);
            await _administradorRepository.SaveChangesAsync(); // CRUCIAL!
            return true;
        }

        public async Task<bool> UpdateAdministradorStatusAsync(Guid id, StatusAdministrador novoStatus)
        {
            var administrador = await _administradorRepository.GetByIdAsync(id);
            if (administrador == null) return false;

            administrador.AtualizarStatus(novoStatus); // Chama o método de domínio
            _administradorRepository.Update(administrador);
            await _administradorRepository.SaveChangesAsync(); // CRUCIAL!
            return true;
        }

        // Lógica de Login para Administrador
        public async Task<AdministradorViewModel?> LoginAdministradorAsync(string cpf, string senha)
        {
            var cpfNormalizado = cpf.Replace(".", "").Replace("-", "");

            // Usa o método FindAsync do repositório genérico para buscar por CPF
            // O FirstOrDefault é importante porque FindAsync retorna IEnumerable
            var administrador = (await _administradorRepository.FindAsync(a => a.Cpf == cpfNormalizado))
                                    .FirstOrDefault();

            if (administrador == null)
            {
                return null; // Administrador não encontrado
            }

            // Verifica a senha
            if (!_passwordHasher.VerifyPassword(senha, administrador.SenhaHash!))
            {
                return null; // Senha incorreta
            }

            // Login bem-sucedido, retorna o ViewModel
            return new AdministradorViewModel
            {
                Id = administrador.Id,
                Nome = administrador.Nome,
                Cpf = administrador.Cpf,
                Status = administrador.Status,
                Role = administrador.Role
            };
        }
    }
}