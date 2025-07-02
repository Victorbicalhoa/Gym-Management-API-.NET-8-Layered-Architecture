// CentroTreinamento.Application/Services/AlunoAppService.cs
using CentroTreinamento.Application.DTOs;
using CentroTreinamento.Application.Interfaces;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                cpfNormalizado, // <--- Use o CPF normalizado aqui
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

        public async Task<bool> UpdateAlunoAsync(Guid id, AlunoInputModel alunoInput)
        {
            var aluno = await _alunoRepository.GetByIdAsync(id);
            if (aluno == null) return false;

            var senhaHash = aluno.SenhaHash;
            if (!string.IsNullOrEmpty(alunoInput.Senha))
            {
                // Use o _passwordHasher para gerar o hash da nova senha
                senhaHash = _passwordHasher.HashPassword(alunoInput.Senha!);
            }

            // Normaliza o CPF para atualização
            var cpfNormalizado = alunoInput.Cpf!.Replace(".", "").Replace("-", ""); // <--- ALTERAÇÃO AQUI

            var alunoAtualizado = new Aluno(
                aluno.Id,
                alunoInput.Nome!,
                senhaHash!,
                aluno.Status,
                cpfNormalizado, // <--- Use o CPF normalizado aqui
                alunoInput.DataNascimento,
                alunoInput.Telefone!,
                aluno.Role // Garanta que UserRole seja mantido ou definido corretamente
            );

            _alunoRepository.Update(alunoAtualizado);
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

        public async Task<bool> UpdateAlunoStatusAsync(Guid id, StatusAluno novoStatus)
        {
            var aluno = await _alunoRepository.GetByIdAsync(id);
            if (aluno == null) return false;

            aluno.AtualizarStatus(novoStatus);
            _alunoRepository.Update(aluno);
            await _alunoRepository.SaveChangesAsync();
            return true;
        }
    }
}