// CentroTreinamento.Domain\Entities\Instrutor.cs
using System;
using CentroTreinamento.Domain.Enums; // Importa o namespace do enum

namespace CentroTreinamento.Domain.Entities
{
    public class Instrutor // Considere herdar de uma BaseEntity com Guid Id
    {
        // Atributos (Propriedades com get privados para controle)
        public Guid Id { get; private set; } // Identificador único do instrutor, alterado para Guid
        public string? Nome { get; private set; } // Nome completo do instrutor
        public string? SenhaHash { get; private set; } // Hash da senha para segurança
        public StatusInstrutor Status { get; private set; } // <--- AGORA DO TIPO ENUM!
        public string? Cref { get; private set; } // Número de registro profissional (CREF)

        // Construtor vazio para o ORM (Entity Framework Core)
        // É essencial que ele seja público ou 'protected' se a classe for abstrata.
        public Instrutor() { }

        // Construtor completo com validações
        public Instrutor(Guid id, string nome, string senhaHash, StatusInstrutor status, string cref)
        {
            // Validações no construtor para garantir que a entidade seja criada em um estado válido.
            if (id == Guid.Empty)
            {
                throw new ArgumentException("ID do instrutor não pode ser vazio.", nameof(id));
            }
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("Nome do instrutor não pode ser vazio.", nameof(nome));
            }
            if (string.IsNullOrWhiteSpace(senhaHash))
            {
                throw new ArgumentException("Senha hash do instrutor não pode ser vazia.", nameof(senhaHash));
            }
            // A validação do enum é implícita pelo tipo.
            // Poderíamos adicionar uma validação se certos status fossem inválidos na criação.
            if (string.IsNullOrWhiteSpace(cref))
            {
                throw new ArgumentException("CREF do instrutor não pode ser vazio.", nameof(cref));
            }

            Id = id;
            Nome = nome;
            SenhaHash = senhaHash;
            Status = status;
            Cref = cref;
        }

        // --- Métodos Internos da Entidade (Comportamento de domínio) ---

        /// <summary>
        /// Atualiza o status do instrutor.
        /// Valida se o novo status é válido.
        /// </summary>
        /// <param name="novoStatus">O novo status a ser definido.</param>
        /// <exception cref="ArgumentException">Lançada se o novo status for inválido (por exemplo, status inválido no enum, embora o tipo já ajude).</exception>
        public void AtualizarStatus(StatusInstrutor novoStatus) // <--- Aceita o tipo enum
        {
            // Validações adicionais podem ser feitas aqui, se, por exemplo,
            // um instrutor 'Inativo' não puder ir diretamente para 'Ativo' sem um processo.
            // Por enquanto, apenas atualizamos diretamente.
            this.Status = novoStatus;
            // Lógica adicional pode ser adicionada aqui, como registrar um evento de domínio (Domain Event).
        }

        /// <summary>
        /// Atualiza o nome do instrutor.
        /// </summary>
        /// <param name="novoNome">O novo nome a ser definido.</param>
        public void AtualizarNome(string novoNome)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
            {
                throw new ArgumentException("Nome não pode ser vazio.", nameof(novoNome));
            }
            Nome = novoNome;
        }

        // Exemplo de como um método para definir senha hash (mas a lógica de hashing estaria em outro lugar)
        // O setter de SenhaHash é privado, então precisamos de um método para alterá-lo.
        // Este método seria chamado por um serviço de domínio/aplicação após o hash da senha.
        public void SetSenhaHash(string novaSenhaHash)
        {
            if (string.IsNullOrWhiteSpace(novaSenhaHash))
            {
                throw new ArgumentException("Nova senha hash não pode ser vazia.", nameof(novaSenhaHash));
            }
            this.SenhaHash = novaSenhaHash;
        }

        /// <summary>
        /// Retorna uma representação em string do objeto Instrutor.
        /// </summary>
        /// <returns>Uma string que representa o instrutor.</returns>
        public override string ToString()
        {
            return $"Instrutor{{ Id={Id}, Nome='{Nome}', Status='{Status}', Cref='{Cref}' }}";
        }

        // Os "Serviços" (criação/edição de planos, gestão de agenda, etc.)
        // NÃO são métodos desta classe de entidade. Eles pertencem a classes de SERVIÇO
        // que interagem COM objetos Instrutor (e outras entidades/repositórios) para realizar
        // operações de negócio mais complexas.
    }
}