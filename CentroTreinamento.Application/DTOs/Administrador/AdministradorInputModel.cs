// CentroTreinamento.Application/DTOs/AdministradorInputModel.cs
using System.ComponentModel.DataAnnotations; // Para validações
using CentroTreinamento.Domain.Enums; // Se você quiser definir o Status inicial no input

namespace CentroTreinamento.Application.DTOs.Administrador
{
    public class AdministradorInputModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "O CPF deve ter entre 11 e 14 caracteres (com ou sem formatação).")]
        // Considere usar um regex para validar o formato de CPF aqui, se desejar
        public string? Cpf { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 50 caracteres.")]
        // Adicione validações de complexidade de senha aqui, se desejar
        public string? Senha { get; set; }

        // Se você quiser que o status seja definido na criação, adicione aqui, senão, pode ser default no AppService
        // public StatusAdministrador Status { get; set; }
    }
}