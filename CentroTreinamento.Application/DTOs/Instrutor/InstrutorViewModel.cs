// CentroTreinamento.Application/DTOs/Instrutor/InstrutorViewModel.cs
using System;
using CentroTreinamento.Domain.Enums;

namespace CentroTreinamento.Application.DTOs.Instrutor // Namespace atualizado
{
    public class InstrutorViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Cpf { get; set; }
        public string Cref { get; set; } = string.Empty;
        public StatusInstrutor Status { get; set; }
        public UserRole Role { get; set; }
    }
}