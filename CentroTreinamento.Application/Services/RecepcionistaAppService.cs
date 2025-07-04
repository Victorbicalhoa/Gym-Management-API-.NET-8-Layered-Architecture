// CentroTreinamento.Application/Services/RecepcionistaAppService.cs
using CentroTreinamento.Application.DTOs.Recepcionista;
using CentroTreinamento.Application.Interfaces; // Para IPasswordHasher
using CentroTreinamento.Application.Interfaces.Services;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums;
using CentroTreinamento.Domain.Repositories; // Para IRecepcionistaRepository
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentroTreinamento.Application.Services
{
    public class RecepcionistaAppService : IRecepcionistaAppService
    {
        private readonly IRecepcionistaRepository _recepcionistaRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RecepcionistaAppService(IRecepcionistaRepository recepcionistaRepository, IPasswordHasher passwordHasher)
        {
            _recepcionistaRepository = recepcionistaRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<RecepcionistaViewModel> CreateRecepcionistaAsync(RecepcionistaInputModel inputModel)
        {
            var cpfLimpo = inputModel.Cpf.Replace(".", "").Replace("-", "");

            var existingRecepcionista = await _recepcionistaRepository.GetByCpfAsync(cpfLimpo);
            if (existingRecepcionista != null)
            {
                throw new ApplicationException("Já existe um recepcionista cadastrado com este CPF.");
            }

            var senhaHash = _passwordHasher.HashPassword(inputModel.Senha);

            var recepcionista = new Recepcionista(Guid.NewGuid(), inputModel.Nome, cpfLimpo, senhaHash);

            await _recepcionistaRepository.AddAsync(recepcionista);
            await _recepcionistaRepository.SaveChangesAsync();

            return new RecepcionistaViewModel
            {
                Id = recepcionista.Id,
                Nome = recepcionista.Nome,
                Cpf = recepcionista.Cpf,
                Status = recepcionista.Status,
                Role = recepcionista.Role
            };
        }

        public async Task<RecepcionistaViewModel?> GetRecepcionistaByIdAsync(Guid id)
        {
            var recepcionista = await _recepcionistaRepository.GetByIdAsync(id);
            if (recepcionista == null)
            {
                return null;
            }

            return new RecepcionistaViewModel
            {
                Id = recepcionista.Id,
                Nome = recepcionista.Nome,
                Cpf = recepcionista.Cpf,
                Status = recepcionista.Status,
                Role = recepcionista.Role
            };
        }

        public async Task<IEnumerable<RecepcionistaViewModel>> GetAllRecepcionistasAsync()
        {
            var recepcionistas = await _recepcionistaRepository.GetAllAsync();
            return recepcionistas.Select(r => new RecepcionistaViewModel
            {
                Id = r.Id,
                Nome = r.Nome,
                Cpf = r.Cpf,
                Status = r.Status,
                Role = r.Role
            }).ToList();
        }

        public async Task UpdateRecepcionistaAsync(Guid id, RecepcionistaInputModel updateModel)
        {
            var recepcionista = await _recepcionistaRepository.GetByIdAsync(id);
            if (recepcionista == null)
            {
                throw new ApplicationException("Recepcionista não encontrado.");
            }

            var cpfLimpo = updateModel.Cpf.Replace(".", "").Replace("-", "");

            // Verifica unicidade do CPF se ele for alterado
            if (cpfLimpo != recepcionista.Cpf)
            {
                var existingRecepcionista = await _recepcionistaRepository.GetByCpfAsync(cpfLimpo);
                if (existingRecepcionista != null && existingRecepcionista.Id != id)
                {
                    throw new ApplicationException("Já existe outro recepcionista com este CPF.");
                }
            }

            var newSenhaHash = !string.IsNullOrEmpty(updateModel.Senha) ? _passwordHasher.HashPassword(updateModel.Senha) : recepcionista.SenhaHash;

            recepcionista.AtualizarDados(updateModel.Nome, cpfLimpo, newSenhaHash);

            _recepcionistaRepository.Update(recepcionista); // Marca para atualização
            await _recepcionistaRepository.SaveChangesAsync(); // Salva as mudanças
        }

        public async Task UpdateRecepcionistaStatusAsync(Guid id, StatusRecepcionista newStatus)
        {
            var recepcionista = await _recepcionistaRepository.GetByIdAsync(id);
            if (recepcionista == null)
            {
                throw new ApplicationException("Recepcionista não encontrado.");
            }

            recepcionista.AtualizarStatus(newStatus);

            _recepcionistaRepository.Update(recepcionista);
            await _recepcionistaRepository.SaveChangesAsync();
        }

        public async Task DeleteRecepcionistaAsync(Guid id)
        {
            var recepcionista = await _recepcionistaRepository.GetByIdAsync(id);
            if (recepcionista == null)
            {
                throw new ApplicationException("Recepcionista não encontrado.");
            }

            _recepcionistaRepository.Delete(recepcionista);
            await _recepcionistaRepository.SaveChangesAsync();
        }

        public async Task<RecepcionistaViewModel?> GetRecepcionistaByCpfAsync(string cpf)
        {
            var cpfLimpo = cpf.Replace(".", "").Replace("-", "");
            var recepcionista = await _recepcionistaRepository.GetByCpfAsync(cpfLimpo);
            if (recepcionista == null)
            {
                return null;
            }

            return new RecepcionistaViewModel
            {
                Id = recepcionista.Id,
                Nome = recepcionista.Nome,
                Cpf = recepcionista.Cpf,
                Status = recepcionista.Status,
                Role = recepcionista.Role
            };
        }
    }
}