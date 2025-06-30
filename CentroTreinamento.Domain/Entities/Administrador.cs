// CentroTreinamento.Domain\Entities\Administrador.cs
using System;
using CentroTreinamento.Domain.Enums; // Adicione esta linha

namespace CentroTreinamento.Domain.Entities
{
    public class Administrador
    {
        public Guid Id { get; private set; } // <--- ALTERADO PARA GUID
        public string? Nome { get; private set; }
        public string? SenhaHash { get; private set; } // Adicionada propriedade SenhaHash
        public StatusAdministrador Status { get; private set; } // <--- AGORA DO TIPO ENUM!
        // Adicione outras propriedades específicas de Administrador, se houver.

        // Construtor vazio para o ORM
        public Administrador() { }

        // Construtor completo
        public Administrador(Guid id, string nome, string senhaHash, StatusAdministrador status)
        {
            if (id == Guid.Empty) throw new ArgumentException("ID do administrador não pode ser vazio.", nameof(id));
            if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome do administrador não pode ser vazio.", nameof(nome));
            if (string.IsNullOrWhiteSpace(senhaHash)) throw new ArgumentException("Senha hash do administrador não pode ser vazia.", nameof(senhaHash));

            Id = id;
            Nome = nome;
            SenhaHash = senhaHash;
            Status = status;
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