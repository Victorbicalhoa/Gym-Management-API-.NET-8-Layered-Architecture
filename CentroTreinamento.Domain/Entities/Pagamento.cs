using System;
using CentroTreinamento.Domain.Enums; // Adicione esta linha para usar o enum StatusPagamento

namespace CentroTreinamento.Domain.Entities
{
    public class Pagamento // Considere herdar de uma BaseEntity com Guid Id
    {
        // Propriedades com get privados para controle
        public Guid Id { get; private set; } // <--- ALTERADO PARA GUID
        public Guid AlunoId { get; private set; } // <--- Adicionada a propriedade AlunoId (chave estrangeira)
        public decimal Valor { get; private set; }
        public DateTime DataPagamento { get; private set; }
        public StatusPagamento StatusPagamento { get; private set; } // <--- AGORA DO TIPO ENUM!
        // Adicione outras propriedades relevantes para Pagamento, se houver (ex: Descricao, MetodoPagamento, etc.)

        // Construtor vazio para o ORM (Entity Framework Core)
        public Pagamento() { }

        // Construtor completo com validações (ajuste conforme suas necessidades)
        public Pagamento(Guid id, Guid alunoId, decimal valor, DateTime dataPagamento, StatusPagamento statusPagamento)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("ID do pagamento não pode ser vazio.", nameof(id));
            }
            if (alunoId == Guid.Empty)
            {
                throw new ArgumentException("ID do aluno não pode ser vazio para o pagamento.", nameof(alunoId));
            }
            if (valor <= 0)
            {
                throw new ArgumentException("O valor do pagamento deve ser positivo.", nameof(valor));
            }
            if (dataPagamento == default(DateTime))
            {
                throw new ArgumentException("Data do pagamento não pode ser vazia.", nameof(dataPagamento));
            }
            // A validação do enum é implícita pelo tipo.

            Id = id;
            AlunoId = alunoId;
            Valor = valor;
            DataPagamento = dataPagamento;
            StatusPagamento = statusPagamento;
        }

        // Métodos de domínio (comportamentos)
        public void MarcarComoPago()
        {
            if (StatusPagamento == StatusPagamento.Pendente || StatusPagamento == StatusPagamento.Atrasado)
            {
                StatusPagamento = StatusPagamento.Pago;
                // Lógica adicional, como registrar data de pagamento efetivo, enviar notificação, etc.
            }
            // Considere lançar uma exceção ou retornar um erro se o status já for 'Pago' ou 'Cancelado'
        }

        public void MarcarComoCancelado()
        {
            if (StatusPagamento != StatusPagamento.Pago) // Só pode cancelar se não estiver pago
            {
                StatusPagamento = StatusPagamento.Cancelado;
                // Lógica adicional para cancelamento
            }
        }
        // Você pode adicionar outros métodos como 'RegistrarAtraso', etc.

        public override string ToString()
        {
            return $"Pagamento{{ Id={Id}, AlunoId={AlunoId}, Valor={Valor}, DataPagamento={DataPagamento}, Status={StatusPagamento} }}";
        }
    }
}