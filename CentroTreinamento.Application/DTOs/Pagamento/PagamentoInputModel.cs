using System;
using System.ComponentModel.DataAnnotations;
using CentroTreinamento.Domain.Enums; // Para o StatusPagamento

namespace CentroTreinamento.Application.DTOs.Pagamento
{
    public class PagamentoInputModel
    {
        [Required(ErrorMessage = "O ID do aluno é obrigatório.")]
        public Guid AlunoId { get; set; }

        [Required(ErrorMessage = "O valor do pagamento é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor do pagamento deve ser positivo.")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "A data do pagamento é obrigatória.")]
        public DateTime DataPagamento { get; set; }

        // Supondo que você adicionará essas propriedades na entidade Pagamento
        [Required(ErrorMessage = "O método de pagamento é obrigatório.")]
        [StringLength(100, ErrorMessage = "O método de pagamento não pode exceder 100 caracteres.")]
        public string? MetodoPagamento { get; set; }

        [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres.")]
        public string? Observacoes { get; set; }

        [Required(ErrorMessage = "O status do pagamento é obrigatório.")]
        public StatusPagamento StatusPagamento { get; set; } // Para registrar o status inicial
    }
}