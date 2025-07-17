using System;
using System.ComponentModel.DataAnnotations;
using CentroTreinamento.Domain.Enums;

namespace CentroTreinamento.Application.DTOs.PlanoDeTreino
{
    public class PlanoDeTreinoUpdateModel
    {
        // Os IDs são necessários para o AppService saber a quem o plano pertence.
        // Podem ser Hidden ou ReadOnly na UI se não forem editáveis.
        [Required(ErrorMessage = "O ID do aluno é obrigatório.")]
        public Guid AlunoId { get; set; }

        [Required(ErrorMessage = "O ID do instrutor é obrigatório.")]
        public Guid InstrutorId { get; set; }

        [Required(ErrorMessage = "O nome do plano é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do plano não pode exceder 100 caracteres.")]
        public string NomePlano { get; set; } = string.Empty; // Inicializa para evitar null

        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A data de início é obrigatória.")]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "A data de fim é obrigatória.")]
        public DateTime DataFim { get; set; }

        [Required(ErrorMessage = "O status do plano é obrigatório.")]
        [EnumDataType(typeof(StatusPlano), ErrorMessage = "Status do plano inválido.")]
        public StatusPlano StatusPlano { get; set; }
    }
}