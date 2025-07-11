﻿using CentroTreinamento.Application.DTOs.Aluno;
using CentroTreinamento.Domain.Enums; // Para StatusAluno
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroTreinamento.Application.Interfaces.Services
{
    public interface IAlunoAppService
    {
        Task<IEnumerable<AlunoViewModel>> GetAllAlunosAsync();
        Task<AlunoViewModel?> GetAlunoByIdAsync(Guid id);
        Task<AlunoViewModel> CreateAlunoAsync(AlunoInputModel alunoInput);
        Task<AlunoViewModel?> UpdateAlunoAsync(Guid id, AlunoInputModel alunoInput);      
        Task<bool> DeleteAlunoAsync(Guid id);
        Task<bool> UpdateAlunoStatusAsync(Guid id, StatusAluno novoStatus);
    }
}