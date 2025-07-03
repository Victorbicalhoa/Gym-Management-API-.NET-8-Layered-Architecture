// CentroTreinamento.Application/Services/InstrutorAppService.cs
using CentroTreinamento.Application.DTOs.Instrutor;
using CentroTreinamento.Application.Interfaces;
using CentroTreinamento.Application.Interfaces.Services;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums;
using CentroTreinamento.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentroTreinamento.Application.Services
{
    public class InstrutorAppService : IInstrutorAppService
    {
        private readonly IInstrutorRepository _instrutorRepository;
        private readonly IPasswordHasher _passwordHasher; // Injetar o serviço de hashing de senha

        // Construtor com injeção de dependência para IInstrutorRepository e IPasswordHasher
        public InstrutorAppService(IInstrutorRepository instrutorRepository, IPasswordHasher passwordHasher)
        {
            _instrutorRepository = instrutorRepository;
            _passwordHasher = passwordHasher; // Atribua o serviço injetado
        }

        public async Task<InstrutorViewModel> GetByIdAsync(Guid id)
        {
            var instrutor = await _instrutorRepository.GetByIdAsync(id);
            if (instrutor == null)
            {
                throw new ApplicationException($"Instrutor com ID {id} não encontrado.");
            }
            return MapToViewModel(instrutor);
        }

        public async Task<IEnumerable<InstrutorViewModel>> GetAllAsync()
        {
            var instrutores = await _instrutorRepository.GetAllAsync();
            return instrutores.Select(MapToViewModel);
        }

        public async Task<InstrutorViewModel> CreateInstrutorAsync(InstrutorInputModel inputModel)
        {
            if (string.IsNullOrWhiteSpace(inputModel.Senha))
            {
                throw new ArgumentException("A senha é obrigatória para a criação de um instrutor.", nameof(inputModel.Senha));
            }

            // Verificar unicidade de CPF e CREF antes de criar
            if (!string.IsNullOrWhiteSpace(inputModel.Cpf))
            {
                var existingCpf = await _instrutorRepository.GetByCpfAsync(inputModel.Cpf);
                if (existingCpf != null)
                {
                    throw new ArgumentException($"Já existe um instrutor com o CPF '{inputModel.Cpf}'.", nameof(inputModel.Cpf));
                }
            }
            var existingCref = await _instrutorRepository.GetByCrefAsync(inputModel.Cref);
            if (existingCref != null)
            {
                throw new ArgumentException($"Já existe um instrutor com o CREF '{inputModel.Cref}'.", nameof(inputModel.Cref));
            }

            // Hashing da senha (USANDO O SERVIÇO REAL)
            string senhaHash = _passwordHasher.HashPassword(inputModel.Senha);

            // Criar a entidade de domínio
            var instrutor = new Instrutor(
                Guid.NewGuid(),
                inputModel.Nome,
                inputModel.Cpf,
                senhaHash, // Passar a senha com hash REAL
                inputModel.Status ?? StatusInstrutor.Ativo,
                inputModel.Cref,
                UserRole.Instrutor
            );

            // Persistir no banco de dados
            await _instrutorRepository.AddAsync(instrutor);
            await _instrutorRepository.SaveChangesAsync(); // SALVAR AGORA

            return MapToViewModel(instrutor);
        }

        public async Task UpdateInstrutorAsync(Guid id, InstrutorInputModel inputModel)
        {
            var instrutor = await _instrutorRepository.GetByIdAsync(id);
            if (instrutor == null)
            {
                throw new ApplicationException($"Instrutor com ID {id} não encontrado para atualização.");
            }

            // 1. Verificar unicidade de CPF e CREF (se forem alterados)
            if (!string.IsNullOrWhiteSpace(inputModel.Cpf) && instrutor.Cpf != inputModel.Cpf)
            {
                var existingCpf = await _instrutorRepository.GetByCpfAsync(inputModel.Cpf);
                if (existingCpf != null && existingCpf.Id != id)
                {
                    throw new ArgumentException($"Já existe outro instrutor com o CPF '{inputModel.Cpf}'.", nameof(inputModel.Cpf));
                }
            }

            if (instrutor.Cref != inputModel.Cref)
            {
                var existingCref = await _instrutorRepository.GetByCrefAsync(inputModel.Cref);
                if (existingCref != null && existingCref.Id != id)
                {
                    throw new ArgumentException($"Já existe outro instrutor com o CREF '{inputModel.Cref}'.", nameof(inputModel.Cref));
                }
            }

            // 2. Hashing da nova senha (se fornecida)
            string? novaSenhaHash = null;
            if (!string.IsNullOrEmpty(inputModel.Senha))
            {
                novaSenhaHash = _passwordHasher.HashPassword(inputModel.Senha); // Usar o serviço real
            }

            // 3. Atualizar a entidade de domínio usando os métodos da entidade
            instrutor.AtualizarDados(
                inputModel.Nome,
                inputModel.Cpf,
                novaSenhaHash,
                inputModel.Cref
            );

            if (inputModel.Status.HasValue)
            {
                instrutor.AtualizarStatus(inputModel.Status.Value);
            }

            // 4. Persistir as alterações
            _instrutorRepository.Update(instrutor); // Chamar a versão não-async
            await _instrutorRepository.SaveChangesAsync(); // Salvar as mudanças
        }

        public async Task DeleteInstrutorAsync(Guid id)
        {
            var instrutor = await _instrutorRepository.GetByIdAsync(id);
            if (instrutor == null)
            {
                throw new ApplicationException($"Instrutor com ID {id} não encontrado para exclusão.");
            }
            _instrutorRepository.Delete(instrutor); // Chamar a versão não-async
            await _instrutorRepository.SaveChangesAsync(); // Salvar as mudanças
        }

        public async Task UpdateInstrutorStatusAsync(Guid id, StatusInstrutor novoStatus)
        {
            var instrutor = await _instrutorRepository.GetByIdAsync(id);
            if (instrutor == null)
            {
                throw new ApplicationException($"Instrutor com ID {id} não encontrado para atualização de status.");
            }

            instrutor.AtualizarStatus(novoStatus);
            _instrutorRepository.Update(instrutor); // Chamar a versão não-async
            await _instrutorRepository.SaveChangesAsync(); // Salvar as mudanças
        }

        public async Task<InstrutorViewModel> GetByCrefAsync(string cref)
        {
            var instrutor = await _instrutorRepository.GetByCrefAsync(cref);
            if (instrutor == null)
            {
                throw new ApplicationException($"Instrutor com CREF '{cref}' não encontrado.");
            }
            return MapToViewModel(instrutor);
        }

        public async Task<InstrutorViewModel> GetByCpfAsync(string cpf)
        {
            var instrutor = await _instrutorRepository.GetByCpfAsync(cpf);
            if (instrutor == null)
            {
                throw new ApplicationException($"Instrutor com CPF '{cpf}' não encontrado.");
            }
            return MapToViewModel(instrutor);
        }

        public async Task<IEnumerable<InstrutorViewModel>> GetAvailableInstrutoresAsync(DateTime data, TimeSpan horario)
        {
            var instrutores = await _instrutorRepository.GetInstrutoresDisponiveisAsync(data, horario);
            return instrutores.Select(MapToViewModel);
        }

        private InstrutorViewModel MapToViewModel(Instrutor instrutor)
        {
            return new InstrutorViewModel
            {
                Id = instrutor.Id,
                Nome = instrutor.Nome ?? string.Empty,
                Cpf = instrutor.Cpf,
                Cref = instrutor.Cref ?? string.Empty,
                Status = instrutor.Status,
                Role = instrutor.Role
            };
        }
    }
}