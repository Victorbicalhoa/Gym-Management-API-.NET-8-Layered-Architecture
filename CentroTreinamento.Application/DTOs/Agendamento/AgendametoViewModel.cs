using System;
using CentroTreinamento.Domain.Enums; // Adicionar para usar o enum de status

namespace CentroTreinamento.Application.DTOs.Agendamento
{
    public class AgendamentoViewModel
    {
        public Guid Id { get; set; }
        public Guid AlunoId { get; set; }
        public string? NomeAluno { get; set; } // Assumindo que você queira exibir o nome do aluno
        public Guid InstrutorId { get; set; }
        public string? NomeInstrutor { get; set; } // Assumindo que você queira exibir o nome do instrutor
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public StatusAgendamento Status { get; set; } // Mapeado do enum StatusAgendamento
        public string? Descricao { get; set; }
        // Se a entidade tivesse uma DataCriacao, você adicionaria aqui. Como não tem, mantive o padrão anterior.
        // public DateTime DataCriacao { get; set; }
    }
}