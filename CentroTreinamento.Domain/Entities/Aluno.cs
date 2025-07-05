// CentroTreinamento.Domain/Entities/Aluno.cs
using System;
using CentroTreinamento.Domain.Enums;

namespace CentroTreinamento.Domain.Entities
{
    public class Aluno
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string SenhaHash { get; private set; }
        public StatusAluno Status { get; private set; }
        public string Cpf { get; private set; }
        public DateTime DataNascimento { get; private set; }
        public string Telefone { get; private set; }
        public UserRole Role { get; private set; }

        protected Aluno()
        {
            Nome = string.Empty;
            SenhaHash = string.Empty;
            Cpf = string.Empty;
            Telefone = string.Empty;
        }

        public Aluno(Guid id, string nome, string senhaHash, StatusAluno status, string cpf, DateTime dataNascimento, string telefone, UserRole role)
        {
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

            Id = id;
            Nome = nome;
            SenhaHash = senhaHash;
            Status = status;
            Cpf = cpf;
            DataNascimento = dataNascimento;
            Telefone = telefone;
            Role = role;
        }

        public void AtualizarStatus(StatusAluno novoStatus)
        {
            Status = novoStatus;
        }

        // NOVO MÉTODO para atualizar informações - CORRIGIDO AQUI
        public void AtualizarDados(string novoNome, string novoCpf, string? novaSenhaHash = null, DateTime? novaDataNascimento = null, string? novoTelefone = null)
        {
            // Validações (ou utilize um Value Object/Guards para isso)
            if (string.IsNullOrWhiteSpace(novoNome)) throw new ArgumentException("Nome não pode ser vazio.", nameof(novoNome));
            if (string.IsNullOrWhiteSpace(novoCpf)) throw new ArgumentException("CPF não pode ser vazio.", nameof(novoCpf));
            // Adicione mais validações aqui para data de nascimento e telefone se necessário
            // Por exemplo, if (novaDataNascimento.HasValue && novaDataNascimento.Value.Date > DateTime.Now.Date) throw...

            Nome = novoNome;
            Cpf = novoCpf;

            // Correção para não atualizar SenhaHash se for null ou vazio/whitespace
            if (!string.IsNullOrWhiteSpace(novaSenhaHash))
            {
                SenhaHash = novaSenhaHash;
            }

            // Correção para não atualizar DataNascimento se for null ou default (se for um comportamento desejado)
            if (novaDataNascimento.HasValue && novaDataNascimento.Value != default(DateTime))
            {
                DataNascimento = novaDataNascimento.Value;
            }

            // Correção para não atualizar Telefone se for null ou vazio/whitespace
            if (!string.IsNullOrWhiteSpace(novoTelefone))
            {
                Telefone = novoTelefone;
            }
        }

        public override string ToString()
        {
            return $"Aluno{{ Id={Id}, Nome='{Nome}', Cpf='{Cpf}', Status='{Status}' }}";
        }
    }
}