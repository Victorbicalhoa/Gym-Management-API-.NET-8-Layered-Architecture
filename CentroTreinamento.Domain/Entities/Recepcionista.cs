// CentroTreinamento.Domain/Entities/Recepcionista.cs
using CentroTreinamento.Domain.Enums;
using System;

namespace CentroTreinamento.Domain.Entities
{
    public class Recepcionista // SEM HERANÇA DE BaseEntity
    {
        // Propriedades padrão para atores, incluindo Id explicitamente
        public Guid Id { get; private set; } // Id deve ser declarado aqui
        public string Nome { get; private set; }
        public string Cpf { get; private set; }
        public string SenhaHash { get; private set; }
        public StatusRecepcionista Status { get; private set; }
        public UserRole Role { get; private set; }

        // Construtor vazio para o ORM (Entity Framework Core)
        public Recepcionista() { }

        // Construtor completo com validações
        public Recepcionista(Guid id, string nome, string cpf, string senhaHash)
        {
            // Validação do Id
            if (id == Guid.Empty)
            {
                throw new ArgumentException("ID da recepcionista não pode ser vazio.", nameof(id));
            }
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("Nome da recepcionista não pode ser vazio.", nameof(nome));
            }
            if (string.IsNullOrWhiteSpace(cpf))
            {
                throw new ArgumentException("CPF da recepcionista não pode ser vazio.", nameof(cpf));
            }
            if (string.IsNullOrWhiteSpace(senhaHash))
            {
                throw new ArgumentException("Senha hash da recepcionista não pode ser vazia.", nameof(senhaHash));
            }

            Id = id;
            Nome = nome;
            Cpf = cpf;
            SenhaHash = senhaHash;
            Status = StatusRecepcionista.Ativo; // Status inicial
            Role = UserRole.Recepcionista; // Força a Role
        }

        // --- Métodos Internos da Entidade (Comportamento de domínio) ---

        /// <summary>
        /// Atualiza o status da recepcionista.
        /// </summary>
        /// <param name="novoStatus">O novo status a ser definido.</param>
        public void AtualizarStatus(StatusRecepcionista novoStatus)
        {
            this.Status = novoStatus;
        }

        /// <summary>
        /// Atualiza dados gerais da recepcionista.
        /// </summary>
        /// <param name="novoNome">Novo nome da recepcionista.</param>
        /// <param name="novoCpf">Novo CPF da recepcionista (opcional).</param>
        /// <param name="novaSenhaHash">Novo hash da senha (opcional, apenas se a senha for alterada).</param>
        public void AtualizarDados(string novoNome, string novoCpf, string? novaSenhaHash)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
            {
                throw new ArgumentException("Nome não pode ser vazio.", nameof(novoNome));
            }
            if (string.IsNullOrWhiteSpace(novoCpf))
            {
                throw new ArgumentException("CPF não pode ser vazio.", nameof(novoCpf));
            }

            Nome = novoNome;
            Cpf = novoCpf;

            if (!string.IsNullOrEmpty(novaSenhaHash))
            {
                SenhaHash = novaSenhaHash;
            }
        }

        /// <summary>
        /// Retorna uma representação em string do objeto Recepcionista.
        /// </summary>
        /// <returns>Uma string que representa a recepcionista.</returns>
        public override string ToString()
        {
            return $"Recepcionista{{ Id={Id}, Nome='{Nome}', Cpf='{Cpf}', Status='{Status}' }}";
        }
    }
}