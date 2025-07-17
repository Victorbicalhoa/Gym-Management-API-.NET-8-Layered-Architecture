using System;
using System.ComponentModel.DataAnnotations;
using CentroTreinamento.Domain.Enums; // Para o StatusPlano

namespace CentroTreinamento.Application.DTOs.PlanoDeTreino
{
    public class PlanoDeTreinoInputModel
    {
        [Required(ErrorMessage = "O ID do instrutor é obrigatório.")]
        public Guid InstrutorId { get; set; }

        [Required(ErrorMessage = "O ID do aluno é obrigatório.")]
        public Guid AlunoId { get; set; }

        [Required(ErrorMessage = "O nome do plano de treino é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome do plano de treino deve ter entre 3 e 100 caracteres.")]
        public string? NomePlano { get; set; } // Ajustado para NomePlano para corresponder à entidade

        [StringLength(1000, ErrorMessage = "A descrição do plano de treino não pode exceder 1000 caracteres.")]
        public string? Descricao { get; set; } // Permitir nulo

        [Required(ErrorMessage = "A data de início é obrigatória.")]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "A data de fim é obrigatória.")] // Agora é obrigatória no DTO
        public DateTime DataFim { get; set; }

        [Required(ErrorMessage = "O status inicial do plano é obrigatório.")]
        public StatusPlano StatusPlano { get; set; } // Para definir o status inicial
    }
}