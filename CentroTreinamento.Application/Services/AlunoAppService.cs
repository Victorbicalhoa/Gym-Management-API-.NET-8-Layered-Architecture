// CentroTreinamento.Application/Services/AlunoAppService.cs
using CentroTreinamento.Application.Interfaces;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentroTreinamento.Application.DTOs.Aluno;
using CentroTreinamento.Application.Interfaces.Services;

namespace CentroTreinamento.Application.Services
{
    public class AlunoAppService : IAlunoAppService
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AlunoAppService(IAlunoRepository alunoRepository, IPasswordHasher passwordHasher)
        {
            _alunoRepository = alunoRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<AlunoViewModel>> GetAllAlunosAsync()
        {
            var alunos = await _alunoRepository.GetAllAsync();
            return alunos.Select(a => new AlunoViewModel
            {
                Id = a.Id,
                Nome = a.Nome,
                Cpf = a.Cpf,
                DataNascimento = a.DataNascimento,
                Telefone = a.Telefone,
                Status = a.Status
            });
        }

        public async Task<AlunoViewModel?> GetAlunoByIdAsync(Guid id)
        {
            var aluno = await _alunoRepository.GetByIdAsync(id);
            if (aluno == null) return null;

            return new AlunoViewModel
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Cpf = aluno.Cpf,
                DataNascimento = aluno.DataNascimento,
                Telefone = aluno.Telefone,
                Status = aluno.Status
            };
        }

        public async Task<AlunoViewModel> CreateAlunoAsync(AlunoInputModel alunoInput)
        {
            // Use o _passwordHasher para gerar o hash da senha
            var senhaHash = _passwordHasher.HashPassword(alunoInput.Senha!);

            // Normaliza o CPF: remove pontos e traços para armazenar apenas números
            var cpfNormalizado = alunoInput.Cpf!.Replace(".", "").Replace("-", ""); // <--- ALTERAÇÃO AQUI

            var aluno = new Aluno(
                Guid.NewGuid(),
                alunoInput.Nome!,
                senhaHash,
                StatusAluno.Ativo,
                cpfNormalizado, 
                alunoInput.DataNascimento,
                alunoInput.Telefone!,
                UserRole.Aluno
            );

            await _alunoRepository.AddAsync(aluno);
            await _alunoRepository.SaveChangesAsync();

            return new AlunoViewModel
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Cpf = aluno.Cpf, // Aqui você pode decidir se retorna o CPF normalizado ou o original do input. Mantenha consistente com o que o frontend espera.
                DataNascimento = aluno.DataNascimento,
                Telefone = aluno.Telefone,
                Status = aluno.Status
            };
        }

        public async Task<AlunoViewModel?> UpdateAlunoAsync(Guid id, AlunoInputModel alunoInput) // Retornando ViewModel, ajuste se sua interface for Task<bool>
        {
            // 1. Carregue a entidade existente do banco de dados
            var existingAluno = await _alunoRepository.GetByIdAsync(id);

            if (existingAluno == null)
            {
                return null; // Aluno não encontrado
            }

            // Normaliza o CPF do input
            var cpfNormalizado = alunoInput.Cpf!.Replace(".", "").Replace("-", "");

            // Hash da nova senha (se fornecida)
            string? novaSenhaHash = null;
            if (!string.IsNullOrEmpty(alunoInput.Senha))
            {
                novaSenhaHash = _passwordHasher.HashPassword(alunoInput.Senha!);
            }

            // 2. CHAMA O MÉTODO DA ENTIDADE (se existir) para atualizar os dados
            // OU atribua diretamente as propriedades se elas tiverem setters públicos
            existingAluno.AtualizarDados(
                alunoInput.Nome!,
                cpfNormalizado,
                novaSenhaHash,
                alunoInput.DataNascimento,
                alunoInput.Telefone
            );

            // 3. Chame o método Update do repositório com a entidade EXISTENTE e modificada
            _alunoRepository.Update(existingAluno);
            await _alunoRepository.SaveChangesAsync(); // Persiste as mudanças

            // 4. Retorne o ViewModel atualizado
            return new AlunoViewModel
            {
                Id = existingAluno.Id,
                Nome = existingAluno.Nome,
                Cpf = existingAluno.Cpf,
                Status = existingAluno.Status,
                Role = existingAluno.Role,
                DataNascimento = existingAluno.DataNascimento,
                Telefone = existingAluno.Telefone
            };
        }

        // Se você tiver um método para atualizar apenas o status (semelhante ao Administrador)
        public async Task<bool> UpdateAlunoStatusAsync(Guid id, StatusAluno novoStatus)
        {
            var existingAluno = await _alunoRepository.GetByIdAsync(id);
            if (existingAluno == null) return false;

            existingAluno.AtualizarStatus(novoStatus); // Chama o método de domínio
            _alunoRepository.Update(existingAluno);
            await _alunoRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAlunoAsync(Guid id)
        {
            var aluno = await _alunoRepository.GetByIdAsync(id);
            if (aluno == null) return false;

            _alunoRepository.Delete(aluno);
            await _alunoRepository.SaveChangesAsync();
            return true;
        }

        public async Task<AlunoViewModel?> LoginAlunoAsync(string cpf, string senha)
        {
            var cpfNormalizado = cpf.Replace(".", "").Replace("-", "");

            var aluno = (await _alunoRepository.FindAsync(a => a.Cpf == cpfNormalizado)).FirstOrDefault();

            if (aluno == null)
            {
                return null;
            }

            if (!_passwordHasher.VerifyPassword(senha, aluno.SenhaHash!))
            {
                return null;
            }

            return new AlunoViewModel
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Cpf = aluno.Cpf,
                Status = aluno.Status,
                Role = aluno.Role
            };
        }
    }
}