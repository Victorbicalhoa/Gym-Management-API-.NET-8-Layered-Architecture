// CentroTreinamento.Application/DTOs/Recepcionista/RecepcionistaInputModel.cs
using System.ComponentModel.DataAnnotations;

namespace CentroTreinamento.Application.DTOs.Recepcionista
{
    public class RecepcionistaInputModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo {1} caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "O CPF deve ter entre {2} e {1} caracteres.")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo {2} caracteres.")]
        public string Senha { get; set; } = string.Empty;
    }
}