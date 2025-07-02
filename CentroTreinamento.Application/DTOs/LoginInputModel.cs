using System.ComponentModel.DataAnnotations;

namespace CentroTreinamento.Application.DTOs
{
    public class LoginInputModel
    {
        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos.")]
        public string Cpf { get; set; } = string.Empty; // Inicializa para evitar null warnings

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 50 caracteres.")]
        public string Senha { get; set; } = string.Empty; // Inicializa para evitar null warnings
    }
}