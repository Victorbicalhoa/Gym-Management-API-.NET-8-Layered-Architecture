using System;
using System.ComponentModel.DataAnnotations;
using CentroTreinamento.Domain.Enums; // Adicionar para usar o enum de status

namespace CentroTreinamento.Application.DTOs.Agendamento
{
    public class AgendamentoUpdateModel
    {
        [Required(ErrorMessage = "O ID do aluno é obrigatório.")]
        public Guid AlunoId { get; set; }

        [Required(ErrorMessage = "O ID do instrutor é obrigatório.")]
        public Guid InstrutorId { get; set; }

        [Required(ErrorMessage = "A data e hora de início do agendamento são obrigatórias.")]
        public DateTime DataHoraInicio { get; set; } // Ajustado para DataHoraInicio

        [Required(ErrorMessage = "A data e hora de fim do agendamento são obrigatórias.")]
        public DateTime DataHoraFim { get; set; } // Adicionado DataHoraFim para atualização

        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
        public string? Descricao { get; set; } // Permitir nulo para alinhar com o domínio

        [Required(ErrorMessage = "O status do agendamento é obrigatório.")]
        public StatusAgendamento Status { get; set; } // Adicionado para permitir atualização de status
    }
}