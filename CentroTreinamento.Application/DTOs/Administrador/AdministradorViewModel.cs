// CentroTreinamento.Application/DTOs/AdministradorViewModel.cs
using CentroTreinamento.Domain.Enums; // Para StatusAdministrador e UserRole
using System;

namespace CentroTreinamento.Application.DTOs.Administrador
{
    public class AdministradorViewModel
    {
        public Guid Id { get; set; }
        public string? Nome { get; set; }
        public string? Cpf { get; set; } // Retornaremos o CPF normalizado
        public StatusAdministrador Status { get; set; }
        public UserRole Role { get; set; }
        // Não inclua SenhaHash aqui!
    }
}