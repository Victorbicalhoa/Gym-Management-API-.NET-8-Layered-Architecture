using System;
using System.ComponentModel.DataAnnotations;

namespace CentroTreinamento.Application.DTOs.Agendamento
{
    public class AgendamentoInputModel
    {
        [Required(ErrorMessage = "O ID do aluno é obrigatório.")]
        public Guid AlunoId { get; set; }

        [Required(ErrorMessage = "O ID do instrutor é obrigatório.")]
        public Guid InstrutorId { get; set; }

        [Required(ErrorMessage = "A data e hora de início do agendamento são obrigatórias.")]
        public DateTime DataHoraInicio { get; set; } // Ajustado para DataHoraInicio

        // DataHoraFim não é diretamente um input, será calculada ou inferida no AppService

        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
        public string? Descricao { get; set; } // Permitir nulo para alinhar com o domínio
    }
}