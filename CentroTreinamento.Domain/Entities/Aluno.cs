// CentroTreinamento.Domain/Entities/Aluno.cs
using System;
using CentroTreinamento.Domain.Enums;

namespace CentroTreinamento.Domain.Entities
{
    // Recomendo fortemente criar uma BaseEntity para Id.
    // Ex: public class BaseEntity { public Guid Id { get; protected set; } }
    // E então Aluno herda: public class Aluno : BaseEntity
    public class Aluno
    {
        // Propriedades da Entidade Aluno (string NÃO anulável por padrão para CPF/Nome/Telefone que são obrigatórios)
        public Guid Id { get; private set; } // Id deve ter set para o EF Core poder popular
        public string Nome { get; private set; } // Não string? se for obrigatório
        public string SenhaHash { get; private set; } // Não string?
        public StatusAluno Status { get; private set; }
        public string Cpf { get; private set; } // Não string?
        public DateTime DataNascimento { get; private set; }
        public string Telefone { get; private set; } // Não string?
        public UserRole Role { get; private set; } // Adicionado para a Role

        // Construtor vazio (para o Entity Framework Core)
        protected Aluno()
        {
            // Inicializar strings não anuláveis para evitar avisos caso o EF Core não popule imediatamente
            Nome = string.Empty;
            SenhaHash = string.Empty;
            Cpf = string.Empty;
            Telefone = string.Empty;
        }

        // Construtor principal para criar um novo Aluno
        public Aluno(Guid id, string nome, string senhaHash, StatusAluno status, string cpf, DateTime dataNascimento, string telefone, UserRole role)
        {
            // Validações de entrada do construtor
            if (id == Guid.Empty) throw new ArgumentException("ID do aluno não pode ser vazio.", nameof(id));
            if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome do aluno não pode ser vazio.", nameof(nome));
            if (string.IsNullOrWhiteSpace(senhaHash)) throw new ArgumentException("Senha hash do aluno não pode ser vazia.", nameof(senhaHash));
            if (string.IsNullOrWhiteSpace(cpf)) throw new ArgumentException("CPF do aluno não pode ser vazio.", nameof(cpf));
            if (dataNascimento == default(DateTime) || dataNascimento.Date > DateTime.Now.Date)
            {
                throw new ArgumentException("Data de nascimento inválida. O aluno não pode ter nascido no futuro.", nameof(dataNascimento));
            }
            if (string.IsNullOrWhiteSpace(telefone)) throw new ArgumentException("Telefone do aluno não pode ser vazio.", nameof(telefone));
            if (!Enum.IsDefined(typeof(StatusAluno), status)) throw new ArgumentException("Status inválido.", nameof(status));
            if (!Enum.IsDefined(typeof(UserRole), role)) throw new ArgumentException("Role inválida.", nameof(role));

            // Atribuições das propriedades
            Id = id;
            Nome = nome;
            SenhaHash = senhaHash;
            Status = status;
            Cpf = cpf;
            DataNascimento = dataNascimento;
            Telefone = telefone;
            Role = role;
        }

        // Métodos de domínio (comportamentos) - Sem alterações aqui

        public void AtualizarStatus(StatusAluno novoStatus)
        {
            Status = novoStatus;
        }

        public void AtualizarNome(string novoNome)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
            {
                throw new ArgumentException("Nome não pode ser vazio.", nameof(novoNome));
            }
            Nome = novoNome;
        }

        public void SetSenhaHash(string novaSenhaHash)
        {
            if (string.IsNullOrWhiteSpace(novaSenhaHash))
            {
                throw new ArgumentException("Nova senha hash não pode ser vazia.", nameof(novaSenhaHash));
            }
            SenhaHash = novaSenhaHash;
        }

        public override string ToString()
        {
            return $"Aluno{{ Id={Id}, Nome='{Nome}', Cpf='{Cpf}', Status='{Status}' }}";
        }
    }
}