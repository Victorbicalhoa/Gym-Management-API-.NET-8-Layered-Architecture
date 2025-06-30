// CentroTreinamento.Domain\Entities\Agendamento.cs
using System;
using CentroTreinamento.Domain.Enums; // Adicione esta linha para usar o enum StatusAgendamento

namespace CentroTreinamento.Domain.Entities
{
    public class Agendamento // Considere herdar de uma BaseEntity com Guid Id
    {
        // Propriedades principais
        public Guid Id { get; private set; } // <--- ALTERADO PARA GUID
        public Guid AlunoId { get; private set; } // <--- ID do aluno (chave estrangeira)
        public Guid InstrutorId { get; private set; } // <--- ID do instrutor (chave estrangeira)
        public DateTime DataHoraInicio { get; private set; }
        public DateTime DataHoraFim { get; private set; }
        public StatusAgendamento Status { get; private set; } // <--- AGORA DO TIPO ENUM!
        public string? Descricao { get; private set; } // Descrição opcional do agendamento

        // Construtor vazio para o ORM (Entity Framework Core)
        public Agendamento() { }

        // Construtor completo com validações (ajuste conforme suas necessidades)
        public Agendamento(Guid id, Guid alunoId, Guid instrutorId, DateTime dataHoraInicio, DateTime dataHoraFim, StatusAgendamento status, string descricao = "")
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("ID do agendamento não pode ser vazio.", nameof(id));
            }
            if (alunoId == Guid.Empty)
            {
                throw new ArgumentException("ID do aluno não pode ser vazio para o agendamento.", nameof(alunoId));
            }
            if (instrutorId == Guid.Empty)
            {
                throw new ArgumentException("ID do instrutor não pode ser vazio para o agendamento.", nameof(instrutorId));
            }
            if (dataHoraInicio == default(DateTime))
            {
                throw new ArgumentException("Data e hora de início do agendamento não podem ser vazias.", nameof(dataHoraInicio));
            }
            if (dataHoraFim == default(DateTime) || dataHoraFim <= dataHoraInicio)
            {
                throw new ArgumentException("Data e hora de fim do agendamento são inválidas ou anteriores/iguais à data de início.", nameof(dataHoraFim));
            }
            // A validação do enum é implícita pelo tipo.

            Id = id;
            AlunoId = alunoId;
            InstrutorId = instrutorId;
            DataHoraInicio = dataHoraInicio;
            DataHoraFim = dataHoraFim;
            Status = status;
            Descricao = descricao ?? string.Empty;
        }

        // Métodos de domínio (comportamentos)

        /// <summary>
        /// Atualiza o status do agendamento.
        /// </summary>
        /// <param name="novoStatus">O novo status a ser definido.</param>
        public void AtualizarStatus(StatusAgendamento novoStatus)
        {
            this.Status = novoStatus;
        }

        /// <summary>
        /// Remarca o agendamento para novas data e hora.
        /// </summary>
        /// <param name="novaDataHoraInicio">Nova data e hora de início.</param>
        /// <param name="novaDataHoraFim">Nova data e hora de fim.</param>
        public void Remarcar(DateTime novaDataHoraInicio, DateTime novaDataHoraFim)
        {
            if (novaDataHoraInicio == default(DateTime) || novaDataHoraFim == default(DateTime) || novaDataHoraFim <= novaDataHoraInicio)
            {
                throw new ArgumentException("Novas data e hora de agendamento são inválidas.");
            }
            this.DataHoraInicio = novaDataHoraInicio;
            this.DataHoraFim = novaDataHoraFim;
            this.Status = StatusAgendamento.Remarcado; // Define o status como remarcado
        }

        public override string ToString()
        {
            return $"Agendamento{{ Id={Id}, AlunoId={AlunoId}, InstrutorId={InstrutorId}, Inicio={DataHoraInicio}, Fim={DataHoraFim}, Status='{Status}' }}";
        }
    }
}