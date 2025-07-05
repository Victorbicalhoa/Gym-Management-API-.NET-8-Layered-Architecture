// CentroTreinamento.Tests.Unit/Domain/Entities/AdministradorTests.cs
using Xunit;
using System;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums; // Para StatusAdministrador e UserRole

namespace CentroTreinamento.Tests.Unit.Domain.Entities
{
    public class AdministradorTests
    {
        [Fact]
        public void Administrador_Constructor_ShouldCreateInstanceWithValidParameters()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Admin Teste";
            var senhaHash = "hashedPassword";
            var status = StatusAdministrador.Ativo;
            var role = UserRole.Administrador;
            var cpf = "12345678900";

            // Act
            var administrador = new Administrador(id, nome, senhaHash, status, role, cpf);

            // Assert
            Assert.Equal(id, administrador.Id);
            Assert.Equal(nome, administrador.Nome);
            Assert.Equal(senhaHash, administrador.SenhaHash);
            Assert.Equal(status, administrador.Status);
            Assert.Equal(role, administrador.Role);
            Assert.Equal(cpf, administrador.Cpf);
        }

        [Fact]
        public void Administrador_Constructor_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Arrange
            var id = Guid.Empty;
            var nome = "Admin Teste";
            var senhaHash = "hashedPassword";
            var status = StatusAdministrador.Ativo;
            var role = UserRole.Administrador;
            var cpf = "12345678900";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Administrador(id, nome, senhaHash, status, role, cpf));
            Assert.Contains("ID do administrador não pode ser vazio.", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Administrador_Constructor_ShouldThrowArgumentException_WhenNomeIsInvalid(string? invalidNome)
        {
            // Arrange
            var id = Guid.NewGuid();
            var senhaHash = "hashedPassword";
            var status = StatusAdministrador.Ativo;
            var role = UserRole.Administrador;
            var cpf = "12345678900";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Administrador(id, invalidNome!, senhaHash, status, role, cpf));
            Assert.Contains("Nome do administrador não pode ser vazio.", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Administrador_Constructor_ShouldThrowArgumentException_WhenSenhaHashIsInvalid(string? invalidSenhaHash)
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Admin Teste";
            var status = StatusAdministrador.Ativo;
            var role = UserRole.Administrador;
            var cpf = "12345678900";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Administrador(id, nome, invalidSenhaHash!, status, role, cpf));
            Assert.Contains("Senha hash do administrador não pode ser vazia.", exception.Message);
        }

        [Fact]
        public void Administrador_Constructor_ShouldThrowArgumentException_WhenStatusIsInvalid()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Admin Teste";
            var senhaHash = "hashedPassword";
            var status = (StatusAdministrador)99; // Valor inválido
            var role = UserRole.Administrador;
            var cpf = "12345678900";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Administrador(id, nome, senhaHash, status, role, cpf));
            Assert.Contains("Status inválido.", exception.Message);
        }

        [Fact]
        public void Administrador_Constructor_ShouldThrowArgumentException_WhenRoleIsInvalid()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Admin Teste";
            var senhaHash = "hashedPassword";
            var status = StatusAdministrador.Ativo;
            var role = (UserRole)99; // Valor inválido
            var cpf = "12345678900";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Administrador(id, nome, senhaHash, status, role, cpf));
            Assert.Contains("Role inválida.", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Administrador_Constructor_ShouldThrowArgumentException_WhenCpfIsInvalidAndNotNull(string invalidCpf)
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Admin Teste";
            var senhaHash = "hashedPassword";
            var status = StatusAdministrador.Ativo;
            var role = UserRole.Administrador;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Administrador(id, nome, senhaHash, status, role, invalidCpf));
            Assert.Contains("CPF do administrador não pode ser vazio.", exception.Message);
        }

        [Fact]
        public void Administrador_AtualizarStatus_ShouldUpdateStatus()
        {
            // Arrange
            var administrador = new Administrador(Guid.NewGuid(), "Admin Original", "hash", StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");
            var novoStatus = StatusAdministrador.Bloqueado;

            // Act
            administrador.AtualizarStatus(novoStatus);

            // Assert
            Assert.Equal(novoStatus, administrador.Status);
        }

        [Fact]
        public void Administrador_AtualizarDados_ShouldUpdateNomeCpfAndSenhaHash()
        {
            // Arrange
            var administrador = new Administrador(Guid.NewGuid(), "Admin Original", "hashOriginal", StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");
            var novoNome = "Novo Nome Admin";
            var novoCpf = "00099988877";
            var novaSenhaHash = "novaHash";

            // Act
            administrador.AtualizarDados(novoNome, novoCpf, novaSenhaHash);

            // Assert
            Assert.Equal(novoNome, administrador.Nome);
            Assert.Equal(novoCpf, administrador.Cpf);
            Assert.Equal(novaSenhaHash, administrador.SenhaHash);
        }

        [Fact]
        public void Administrador_AtualizarDados_ShouldUpdateNomeCpf_WhenSenhaHashIsNull()
        {
            // Arrange
            var administrador = new Administrador(Guid.NewGuid(), "Admin Original", "hashOriginal", StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");
            var novoNome = "Novo Nome Admin 2";
            var novoCpf = "11122233344";

            // Act
            administrador.AtualizarDados(novoNome, novoCpf, null);

            // Assert
            Assert.Equal(novoNome, administrador.Nome);
            Assert.Equal(novoCpf, administrador.Cpf);
            Assert.Equal("hashOriginal", administrador.SenhaHash); // Senha hash não deve mudar
        }

        [Fact]
        public void Administrador_AtualizarDados_ShouldUpdateNomeCpf_WhenSenhaHashIsEmptyString()
        {
            // Arrange
            var administrador = new Administrador(Guid.NewGuid(), "Admin Original", "hashOriginal", StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");
            var novoNome = "Novo Nome Admin 3";
            var novoCpf = "22233344455";

            // Act
            administrador.AtualizarDados(novoNome, novoCpf, "");

            // Assert
            Assert.Equal(novoNome, administrador.Nome);
            Assert.Equal(novoCpf, administrador.Cpf);
            Assert.Equal("hashOriginal", administrador.SenhaHash); // Senha hash não deve mudar
        }

        [Fact]
        public void Administrador_AtualizarDados_ShouldUpdateNomeCpf_WhenSenhaHashIsWhiteSpace()
        {
            // Arrange
            var administrador = new Administrador(Guid.NewGuid(), "Admin Original", "hashOriginal", StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");
            var novoNome = "Novo Nome Admin 4";
            var novoCpf = "33344455566";

            // Act
            administrador.AtualizarDados(novoNome, novoCpf, "   "); // Espaços em branco

            // Assert
            Assert.Equal(novoNome, administrador.Nome);
            Assert.Equal(novoCpf, administrador.Cpf);
            Assert.Equal("hashOriginal", administrador.SenhaHash); // Senha hash não deve mudar
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Administrador_AtualizarDados_ShouldThrowArgumentException_WhenNomeIsInvalid(string? invalidNome)
        {
            // Arrange
            var administrador = new Administrador(Guid.NewGuid(), "Admin Original", "hash", StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => administrador.AtualizarDados(invalidNome!, "00000000000", "novaHash"));
            Assert.Contains("Nome não pode ser vazio.", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Administrador_AtualizarDados_ShouldThrowArgumentException_WhenCpfIsInvalid(string invalidCpf)
        {
            // Arrange
            var administrador = new Administrador(Guid.NewGuid(), "Admin Original", "hash", StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => administrador.AtualizarDados("Novo Nome", invalidCpf!, "novaHash"));
            Assert.Contains("CPF não pode ser vazio ou conter apenas espaços em branco.", exception.Message);
        }

        [Fact]
        public void Administrador_SetSenhaHash_ShouldUpdateSenhaHash()
        {
            // Arrange
            var administrador = new Administrador(Guid.NewGuid(), "Admin Original", "hashOriginal", StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");
            var novaSenhaHash = "outraNovaHash";

            // Act
            administrador.SetSenhaHash(novaSenhaHash);

            // Assert
            Assert.Equal(novaSenhaHash, administrador.SenhaHash);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Administrador_SetSenhaHash_ShouldThrowArgumentException_WhenNovaSenhaHashIsInvalid(string? invalidSenhaHash)
        {
            // Arrange
            var administrador = new Administrador(Guid.NewGuid(), "Admin Original", "hashOriginal", StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => administrador.SetSenhaHash(invalidSenhaHash!));
            Assert.Contains("Nova senha hash não pode ser vazia.", exception.Message);
        }

        [Fact]
        public void Administrador_ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var id = Guid.NewGuid();
            var administrador = new Administrador(id, "Admin ToString", "hash", StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");

            // Act
            var result = administrador.ToString();

            // Assert
            Assert.Contains(id.ToString(), result);
            Assert.Contains("Admin ToString", result);
            Assert.Contains("Ativo", result);
            Assert.Contains("Administrador{", result);
            Assert.Contains("}", result);
        }
    }
}