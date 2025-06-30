// CentroTreinamento.Domain\Entities\Aluno.cs
using System;
using CentroTreinamento.Domain.Enums; // Adicione esta linha para usar o enum StatusAluno

namespace CentroTreinamento.Domain.Entities
{
    public class Aluno // Considere herdar de uma BaseEntity com Guid Id
    {
        // Propriedades padrão para atores (Id, Nome, SenhaHash, Status)
        public Guid Id { get; private set; } // <--- ALTERADO PARA GUID
        public string? Nome { get; private set; }
        public string? SenhaHash { get; private set; }
        public StatusAluno Status { get; private set; } // <--- AGORA DO TIPO ENUM!

        // Outras propriedades específicas do Aluno
        public string? Cpf { get; private set; } // CPF do aluno
        public DateTime DataNascimento { get; private set; }
        public string? Telefone { get; private set; }
        // Adicione outras propriedades relevantes para Aluno, se houver (ex: DataMatricula, PlanoAtualId, etc.)

        // Construtor vazio para o ORM (Entity Framework Core)
        public Aluno() { }

        // Construtor completo com validações (ajuste conforme suas necessidades)
        public Aluno(Guid id, string nome, string senhaHash, StatusAluno status, string cpf, DateTime dataNascimento, string telefone)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("ID do aluno não pode ser vazio.", nameof(id));
            }
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("Nome do aluno não pode ser vazio.", nameof(nome));
            }
            if (string.IsNullOrWhiteSpace(senhaHash))
            {
                throw new ArgumentException("Senha hash do aluno não pode ser vazia.", nameof(senhaHash));
            }
            if (string.IsNullOrWhiteSpace(cpf))
            {
                throw new ArgumentException("CPF do aluno não pode ser vazio.", nameof(cpf));
            }
            if (dataNascimento == default(DateTime) || dataNascimento > DateTime.Now)
            {
                throw new ArgumentException("Data de nascimento inválida.", nameof(dataNascimento));
            }
            if (string.IsNullOrWhiteSpace(telefone))
            {
                throw new ArgumentException("Telefone do aluno não pode ser vazio.", nameof(telefone));
            }

            Id = id;
            Nome = nome;
            SenhaHash = senhaHash;
            Status = status;
            Cpf = cpf;
            DataNascimento = dataNascimento;
            Telefone = telefone;
        }

        // Métodos de domínio (comportamentos)

        /// <summary>
        /// Atualiza o status do aluno.
        /// </summary>
        /// <param name="novoStatus">O novo status a ser definido.</param>
        public void AtualizarStatus(StatusAluno novoStatus)
        {
            this.Status = novoStatus;
        }

        /// <summary>
        /// Atualiza o nome do aluno.
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

        /// <summary>
        /// Define a nova senha hash do aluno.
        /// </summary>
        /// <param name="novaSenhaHash">A nova senha hash.</param>
        public void SetSenhaHash(string novaSenhaHash)
        {
            if (string.IsNullOrWhiteSpace(novaSenhaHash))
            {
                throw new ArgumentException("Nova senha hash não pode ser vazia.", nameof(novaSenhaHash));
            }
            this.SenhaHash = novaSenhaHash;
        }

        public override string ToString()
        {
            return $"Aluno{{ Id={Id}, Nome='{Nome}', Cpf='{Cpf}', Status='{Status}' }}";
        }
    }
}