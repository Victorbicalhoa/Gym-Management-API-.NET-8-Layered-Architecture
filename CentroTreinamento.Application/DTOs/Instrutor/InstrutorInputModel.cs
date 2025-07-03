// CentroTreinamento.Application/DTOs/Instrutor/InstrutorInputModel.cs
using System.ComponentModel.DataAnnotations;
using CentroTreinamento.Domain.Enums;

namespace CentroTreinamento.Application.DTOs.Instrutor // Namespace atualizado
{
    public class InstrutorInputModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 caracteres.")]
        public string? Cpf { get; set; }

        [Required(ErrorMessage = "O CREF é obrigatório.")]
        [StringLength(20, ErrorMessage = "O CREF não pode exceder 20 caracteres.")]
        public string Cref { get; set; } = string.Empty;

        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres se fornecida.")]
        public string? Senha { get; set; }

        public StatusInstrutor? Status { get; set; }
    }
}