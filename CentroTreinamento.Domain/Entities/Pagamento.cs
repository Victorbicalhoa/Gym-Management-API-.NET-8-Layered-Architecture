using System;
using CentroTreinamento.Domain.Enums; // Adicione esta linha para usar o enum StatusPagamento

namespace CentroTreinamento.Domain.Entities
{
    public class Pagamento // Considere herdar de uma BaseEntity com Guid Id
    {
        // Propriedades com get privados para controle
        public Guid Id { get; private set; }
        public Guid AlunoId { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataPagamento { get; private set; }
        public MetodoPagamento MetodoPagamento { get; private set; } // <<<< ADICIONADO
        public string? Observacoes { get; private set; }     // <<<< ADICIONADO
        public StatusPagamento StatusPagamento { get; private set; }

        // Construtor vazio para o ORM (Entity Framework Core)
        public Pagamento() { }

        // Construtor completo com validações (ajuste conforme suas necessidades)
        // <<<< ATUALIZADO PARA INCLUIR MetodoPagamento e Observacoes
        public Pagamento(Guid id, Guid alunoId, decimal valor, DateTime dataPagamento, MetodoPagamento metodoPagamento, string? observacoes, StatusPagamento statusPagamento)
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

            Id = id;
            AlunoId = alunoId;
            Valor = valor;
            DataPagamento = dataPagamento;
            MetodoPagamento = metodoPagamento; // <<<< ATRIBUÍDO
            Observacoes = observacoes;         // <<<< ATRIBUÍDO
            StatusPagamento = statusPagamento;
        }

        // Métodos de domínio (comportamentos)
        public void MarcarComoPago()
        {
            if (StatusPagamento == StatusPagamento.Pendente || StatusPagamento == StatusPagamento.Atrasado)
            {
                StatusPagamento = StatusPagamento.Pago;
            }
            // Considere lançar uma exceção ou retornar um erro se o status já for 'Pago' ou 'Cancelado'
            // if (StatusPagamento == StatusPagamento.Pago) throw new InvalidOperationException("Pagamento já está pago.");
        }

        public void MarcarComoCancelado()
        {
            if (StatusPagamento != StatusPagamento.Pago) // Só pode cancelar se não estiver pago
            {
                StatusPagamento = StatusPagamento.Cancelado;
            }
        }

        public override string ToString()
        {
            return $"Pagamento{{ Id={Id}, AlunoId={AlunoId}, Valor={Valor}, DataPagamento={DataPagamento}, Metodo={MetodoPagamento}, Status={StatusPagamento} }}";
        }
    }
}