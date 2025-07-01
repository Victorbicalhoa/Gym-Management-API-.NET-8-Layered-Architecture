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

        public AlunoAppService(IAlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
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
            var senhaHash = $"HASH_{alunoInput.Senha}"; // Placeholder para a senha hash

            // Usando o operador null-forgiving (!) para suprimir os avisos de nulidade
            var aluno = new Aluno(
                Guid.NewGuid(),
                alunoInput.Nome!, // O ! diz ao compilador que sabemos que não será nulo
                senhaHash,
                StatusAluno.Ativo,
                alunoInput.Cpf!,   // O ! diz ao compilador que sabemos que não será nulo
                alunoInput.DataNascimento,
                alunoInput.Telefone! // O ! diz ao compilador que sabemos que não será nulo
            );

            await _alunoRepository.AddAsync(aluno);
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

        public async Task<bool> UpdateAlunoAsync(Guid id, AlunoInputModel alunoInput)
        {
            var aluno = await _alunoRepository.GetByIdAsync(id);
            if (aluno == null) return false;

            var senhaHash = aluno.SenhaHash;
            if (!string.IsNullOrEmpty(alunoInput.Senha))
            {
                senhaHash = $"HASH_{alunoInput.Senha}"; // Exemplo: Nova senha hash
            }

            // Usando o operador null-forgiving (!) para suprimir os avisos de nulidade
            var alunoAtualizado = new Aluno(
                aluno.Id,
                alunoInput.Nome!,
                senhaHash!, // Assumimos que senhaHash não será nulo
                aluno.Status,
                alunoInput.Cpf!,
                alunoInput.DataNascimento,
                alunoInput.Telefone!
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

            aluno.AtualizarStatus(novoStatus); // Usa o método de domínio da entidade
            _alunoRepository.Update(aluno);
            await _alunoRepository.SaveChangesAsync();
            return true;
        }
    }
}