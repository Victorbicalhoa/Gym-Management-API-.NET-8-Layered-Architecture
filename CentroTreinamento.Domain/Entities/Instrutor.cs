using System;
using CentroTreinamento.Domain.Enums; 

namespace CentroTreinamento.Domain.Entities
{
    public class Instrutor
    {
        // Propriedades padrão para atores
        public Guid Id { get; private set; } 
        public string? Nome { get; private set; }
        public string? Cpf { get; private set; }
        public string? SenhaHash { get; private set; }
        public StatusInstrutor Status { get; private set; }
        public string? Cref { get; private set; } // CREF é uma particularidade do Instrutor
        public UserRole Role { get; private set; }

        // Construtor vazio para o ORM (Entity Framework Core)
        public Instrutor() { }

        // Construtor completo com validações
        public Instrutor(Guid id, string nome, string? cpf, string senhaHash, StatusInstrutor status, string cref, UserRole role)
        {
            // Validação do Id aqui, já que não vem da BaseEntity
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
            if (string.IsNullOrWhiteSpace(cref))
            {
                throw new ArgumentException("CREF do instrutor não pode ser vazio.", nameof(cref));
            }
            if (cpf != null && string.IsNullOrWhiteSpace(cpf))
            {
                throw new ArgumentException("CPF do instrutor não pode ser vazio se não nulo.", nameof(cpf));
            }
            if (!Enum.IsDefined(typeof(StatusInstrutor), status))
            {
                throw new ArgumentException("Status inválido.", nameof(status));
            }
            // Força a Role para ser Instrutor na criação da entidade Instrutor
            if (role != UserRole.Instrutor)
            {
                throw new ArgumentException("Role inválida para instrutor. Deve ser 'Instrutor'.", nameof(role));
            }

            Id = id;
            Nome = nome;
            Cpf = cpf;
            SenhaHash = senhaHash;
            Status = status;
            Cref = cref;
            Role = role;
        }

        // --- Métodos Internos da Entidade (Comportamento de domínio) ---

        /// <summary>
        /// Atualiza o status do instrutor.
        /// </summary>
        /// <param name="novoStatus">O novo status a ser definido.</param>
        public void AtualizarStatus(StatusInstrutor novoStatus)
        {
            // Poderiam haver validações de transição de status aqui.
            this.Status = novoStatus;
        }

        /// <summary>
        /// Atualiza dados gerais do instrutor.
        /// </summary>
        /// <param name="novoNome">Novo nome do instrutor.</param>
        /// <param name="novoCpf">Novo CPF do instrutor (opcional).</param>
        /// <param name="novaSenhaHash">Novo hash da senha (opcional, apenas se a senha for alterada).</param>
        /// <param name="novoCref">Novo CREF do instrutor.</param>
        public void AtualizarDados(string novoNome, string? novoCpf, string? novaSenhaHash, string novoCref)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
            {
                throw new ArgumentException("Nome não pode ser vazio.", nameof(novoNome));
            }
            if (string.IsNullOrWhiteSpace(novoCref))
            {
                throw new ArgumentException("CREF não pode ser vazio.", nameof(novoCref));
            }

            Nome = novoNome;
            Cref = novoCref;

            if (novoCpf != null)
            {
                if (string.IsNullOrWhiteSpace(novoCpf))
                {
                    throw new ArgumentException("CPF não pode ser vazio se fornecido.", nameof(novoCpf));
                }
                Cpf = novoCpf;
            }

            if (!string.IsNullOrEmpty(novaSenhaHash))
            {
                SenhaHash = novaSenhaHash;
            }
            // Não atualize Status ou Role aqui, pois são atualizados por métodos específicos.
        }

        /// <summary>
        /// Retorna uma representação em string do objeto Instrutor.
        /// </summary>
        /// <returns>Uma string que representa o instrutor.</returns>
        public override string ToString()
        {
            return $"Instrutor{{ Id={Id}, Nome='{Nome}', Status='{Status}', Cref='{Cref}' }}";
        }
    }
}