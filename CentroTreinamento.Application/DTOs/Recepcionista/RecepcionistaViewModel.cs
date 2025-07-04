// CentroTreinamento.Application/DTOs/Recepcionista/RecepcionistaViewModel.cs
using System;
using CentroTreinamento.Domain.Enums;

namespace CentroTreinamento.Application.DTOs.Recepcionista
{
    public class RecepcionistaViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public StatusRecepcionista Status { get; set; }
        public UserRole Role { get; set; }
    }
}