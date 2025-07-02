using System;
using CentroTreinamento.Domain.Enums; 
namespace CentroTreinamento.Domain.Entities
{
    public class Administrador
    {
        // Propriedades padrão para atores
        public Guid Id { get; private set; } 
        public string? Nome { get; private set; }
        public string? Cpf { get; private set; } 
        public string? SenhaHash { get; private set; } 
        public StatusAdministrador Status { get; private set; } 
        public UserRole Role { get; private set; }        

        // Construtor vazio para o ORM
        public Administrador() { }

        // Construtor completo
        public Administrador(Guid id, string nome, string senhaHash, StatusAdministrador status, UserRole role, string? cpf)
        {
            if (id == Guid.Empty) throw new ArgumentException("ID do administrador não pode ser vazio.", nameof(id));
            if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome do administrador não pode ser vazio.", nameof(nome));
            if (string.IsNullOrWhiteSpace(senhaHash)) throw new ArgumentException("Senha hash do administrador não pode ser vazia.", nameof(senhaHash));
            if (cpf != null && string.IsNullOrWhiteSpace(cpf)) throw new ArgumentException("CPF do administrador não pode ser vazio.", nameof(cpf));
            if (!Enum.IsDefined(typeof(StatusAdministrador), status)) throw new ArgumentException("Status inválido.", nameof(status));
            if (!Enum.IsDefined(typeof(UserRole), role)) throw new ArgumentException("Role inválida.", nameof(role));

            Id = id;
            Nome = nome;
            SenhaHash = senhaHash;
            Status = status;
            Role = role;
            Cpf = cpf;
        }

        // Métodos de domínio
        public void AtualizarStatus(StatusAdministrador novoStatus)
        {
            this.Status = novoStatus;
        }

        public void SetSenhaHash(string novaSenhaHash)
        {
            if (string.IsNullOrWhiteSpace(novaSenhaHash)) throw new ArgumentException("Nova senha hash não pode ser vazia.", nameof(novaSenhaHash));
            this.SenhaHash = novaSenhaHash;
        }

        public override string ToString()
        {
            return $"Administrador{{ Id={Id}, Nome='{Nome}', Status='{Status}' }}";
        }
    }
}