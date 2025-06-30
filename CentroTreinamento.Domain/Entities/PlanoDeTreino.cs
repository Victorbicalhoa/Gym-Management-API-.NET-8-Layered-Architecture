// CentroTreinamento.Domain\Entities\PlanoDeTreino.cs
using System;
using CentroTreinamento.Domain.Enums; // Adicione esta linha para usar o enum StatusPlano
using System.Collections.Generic; // Se você tiver uma coleção de exercícios ou treinos dentro do plano

namespace CentroTreinamento.Domain.Entities
{
    public class PlanoDeTreino // Considere herdar de uma BaseEntity com Guid Id
    {
        // Propriedades principais
        public Guid Id { get; private set; } // <--- ALTERADO PARA GUID
        public Guid AlunoId { get; private set; } // <--- ID do aluno associado ao plano (chave estrangeira)
        public string? NomePlano { get; private set; } // Ex: "Plano Iniciante", "Hipertrofia Avancada"
        public string? Descricao { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }
        public StatusPlano Status { get; private set; } // <--- AGORA DO TIPO ENUM!

        // Se o plano tiver uma coleção de treinos ou exercícios (opcional, dependendo do seu modelo)
        // public ICollection<ExercicioPlano> Exercicios { get; private set; }

        // Construtor vazio para o ORM (Entity Framework Core)
        public PlanoDeTreino() { }

        // Construtor completo com validações (ajuste conforme suas necessidades)
        public PlanoDeTreino(Guid id, Guid alunoId, string nomePlano, string descricao, DateTime dataInicio, DateTime dataFim, StatusPlano status)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("ID do plano de treino não pode ser vazio.", nameof(id));
            }
            if (alunoId == Guid.Empty)
            {
                throw new ArgumentException("ID do aluno associado ao plano não pode ser vazio.", nameof(alunoId));
            }
            if (string.IsNullOrWhiteSpace(nomePlano))
            {
                throw new ArgumentException("Nome do plano não pode ser vazio.", nameof(nomePlano));
            }
            if (dataInicio == default(DateTime))
            {
                throw new ArgumentException("Data de início do plano não pode ser vazia.", nameof(dataInicio));
            }
            if (dataFim == default(DateTime) || dataFim < dataInicio)
            {
                throw new ArgumentException("Data de fim do plano é inválida ou anterior à data de início.", nameof(dataFim));
            }
            // A validação do enum é implícita pelo tipo.

            Id = id;
            AlunoId = alunoId;
            NomePlano = nomePlano;
            Descricao = descricao;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Status = status;

            // Inicialize coleções se existirem
            // Exercicios = new List<ExercicioPlano>();
        }

        // Métodos de domínio (comportamentos)
        public void AtualizarStatus(StatusPlano novoStatus)
        {
            this.Status = novoStatus;
        }

        public void AtualizarPeriodo(DateTime novaDataFim)
        {
            if (novaDataFim < DataInicio)
            {
                throw new ArgumentException("Nova data de fim não pode ser anterior à data de início.", nameof(novaDataFim));
            }
            DataFim = novaDataFim;
        }

        public void AtualizarNomeEDescricao(string novoNome, string novaDescricao)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
            {
                throw new ArgumentException("Nome do plano não pode ser vazio.", nameof(novoNome));
            }
            NomePlano = novoNome;
            Descricao = novaDescricao;
        }

        public override string ToString()
        {
            return $"PlanoDeTreino{{ Id={Id}, AlunoId={AlunoId}, Nome='{NomePlano}', Status='{Status}', Inicio={DataInicio.ToShortDateString()}, Fim={DataFim.ToShortDateString()} }}";
        }
    }
}