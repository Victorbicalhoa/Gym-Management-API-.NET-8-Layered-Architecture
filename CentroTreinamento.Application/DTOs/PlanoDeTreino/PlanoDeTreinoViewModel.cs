using System;
using CentroTreinamento.Domain.Enums; // Para o StatusPlano

namespace CentroTreinamento.Application.DTOs.PlanoDeTreino
{
    public class PlanoDeTreinoViewModel
    {
        public Guid Id { get; set; }
        public Guid InstrutorId { get; set; }
        public string? NomeInstrutor { get; set; }
        public Guid AlunoId { get; set; }
        public string? NomeAluno { get; set; }
        public string? NomePlano { get; set; } // Ajustado para NomePlano
        public string? Descricao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public StatusPlano StatusPlano { get; set; } // Mapeado do enum StatusPlano
        // Se a entidade tivesse uma DataCriacao, você adicionaria aqui.
        // public DateTime DataCriacao { get; set; }
    }
}