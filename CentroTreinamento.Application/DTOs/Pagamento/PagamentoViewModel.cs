using System;
using CentroTreinamento.Domain.Enums; // Para o StatusPagamento

namespace CentroTreinamento.Application.DTOs.Pagamento
{
    public class PagamentoViewModel
    {
        public Guid Id { get; set; }
        public Guid AlunoId { get; set; }
        public string? NomeAluno { get; set; } // Para exibir o nome do aluno associado
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; }
        public string? MetodoPagamento { get; set; } // Supondo que existirá na entidade
        public string? Observacoes { get; set; } // Supondo que existirá na entidade
        public StatusPagamento StatusPagamento { get; set; } // Mapeado do enum StatusPagamento
        // Se a entidade tivesse uma DataCriacao, você adicionaria aqui.
        // public DateTime DataCriacao { get; set; }
    }
}