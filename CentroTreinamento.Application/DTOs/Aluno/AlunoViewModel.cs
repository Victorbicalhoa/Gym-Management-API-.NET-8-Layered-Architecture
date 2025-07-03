using System;
using CentroTreinamento.Domain.Enums;

namespace CentroTreinamento.Application.DTOs.Aluno
{
    public class AlunoViewModel
    {
        public Guid Id { get; set; }
        public string? Nome { get; set; }
        public string? Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public string? Telefone { get; set; }
        public StatusAluno Status { get; set; }
        public UserRole Role { get; set; }
    }
}