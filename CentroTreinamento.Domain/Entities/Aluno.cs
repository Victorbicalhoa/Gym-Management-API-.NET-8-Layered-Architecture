using System; // Necessário para DateTime

namespace CentroTreinamento.Domain.Entities
{
    // Define a classe Aluno como uma entidade de domínio
    public class Aluno
    {
        // Atributos (Propriedades)
        // 'Id' é a chave primária da entidade, padrão para ORMs como Entity Framework Core.
        public int Id { get; private set; } // 'private set' garante que o Id só pode ser definido internamente, geralmente pelo ORM ou construtor.
        public string Nome { get; private set; }
        public string SenhaHash { get; private set; } // Alterado para SenhaHash para indicar que não é texto puro
        public string Status { get; private set; } // Representa o estado do aluno (Pendente, Ativo, Inadimplente, Inativo)
        public DateTime DataCadastro { get; private set; }

        // Construtor: Usado para criar uma nova instância de Aluno
        // O construtor garante que a entidade seja criada em um estado válido.
        // Parâmetros mínimos para a criação.
        public Aluno(string nome, string senhaHash)
        {
            // Validações básicas no construtor
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome do aluno não pode ser vazio.");
            if (string.IsNullOrWhiteSpace(senhaHash))
                throw new ArgumentException("A senha do aluno não pode ser vazia.");

            Nome = nome;
            SenhaHash = senhaHash;
            Status = "Pendente"; // Estado inicial padrão ao cadastrar
            DataCadastro = DateTime.UtcNow; // Usa UTC para consistência de fuso horário
        }

        // Construtor privado sem parâmetros para uso de ORMs (como Entity Framework Core)
        // ORMs precisam de um construtor sem parâmetros para instanciar objetos do banco de dados.
        private Aluno() { }

        // Métodos (Operações Internas da Entidade)
        // Estes métodos representam a lógica de negócio intrínseca ao Aluno.
        // Eles mudam o estado interno do Aluno de forma controlada.

        /// <summary>
        /// Obtém o status atual do aluno.
        /// </summary>
        /// <returns>O status do aluno (e.g., "Ativo", "Inadimplente").</returns>
        public string GetStatus()
        {
            return Status;
        }

        /// <summary>
        /// Atualiza o status do aluno para um novo estado.
        /// </summary>
        /// <param name="novoStatus">O novo status a ser aplicado (e.g., "Ativo", "Inadimplente", "Inativo").</param>
        public void AtualizarStatus(string novoStatus)
        {
            // Poderíamos adicionar validações aqui para garantir transições de estado válidas,
            // por exemplo, um aluno "Inativo" não pode ir direto para "Ativo" sem "Pendente" ou algo assim.
            // Para este exemplo, apenas atualiza.
            if (string.IsNullOrWhiteSpace(novoStatus))
                throw new ArgumentException("O novo status não pode ser vazio.");

            // Exemplo de transição de estado mais robusta (opcional, dependendo da complexidade):
            // switch (novoStatus)
            // {
            //     case "Ativo":
            //         if (this.Status == "Pendente" || this.Status == "Inadimplente" || this.Status == "Inativo")
            //             this.Status = novoStatus;
            //         else throw new InvalidOperationException("Não é possível ativar o aluno a partir do status atual.");
            //         break;
            //     case "Inadimplente":
            //         if (this.Status == "Ativo")
            //             this.Status = novoStatus;
            //         else throw new InvalidOperationException("Só é possível marcar como inadimplente a partir do status Ativo.");
            //         break;
            //     // ... e assim por diante
            //     default:
            //         throw new ArgumentException($"Status '{novoStatus}' inválido.");
            // }

            this.Status = novoStatus;
        }

        // Métodos como visualizarPlano(), agendarTreino(), consultarPagamentos(), visualizarProgresso()
        // Não são métodos da ENTIDADE Aluno propriamente dita, mas sim funcionalidades que o Aluno (ator) realiza
        // ATRAVÉS DO SISTEMA, que são orquestradas por SERVIÇOS.
        // A entidade Aluno, como um objeto de domínio, não "agenda" ou "consulta".
        // Ela é UM DADO que é usado por outros serviços para realizar essas ações.
        // Ex: Um AgendamentoService receberia o Aluno e faria o agendamento para ele.
        // Por isso, esses métodos NÃO estão incluídos aqui.
    }
}