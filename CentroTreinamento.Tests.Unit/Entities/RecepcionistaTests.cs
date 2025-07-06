// CentroTreinamento.Tests.Unit/Domain/Entities/RecepcionistaTests.cs
using Xunit;
using System;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums;

namespace CentroTreinamento.Tests.Unit.Domain.Entities
{
    public class RecepcionistaTests
    {
        private readonly Guid _validId = Guid.NewGuid();
        private readonly string _validNome = "Recepcionista Teste";
        private readonly string _validCpf = "11122233344";
        private readonly string _validSenhaHash = "hashedSenha123";
        private readonly StatusRecepcionista _validStatus = StatusRecepcionista.Ativo;
        private readonly UserRole _validRole = UserRole.Recepcionista;

        // --- Testes do Construtor ---

        [Fact]
        public void Recepcionista_Constructor_ShouldCreateInstanceWithValidParameters()
        {
            // Act
            var recepcionista = new Recepcionista(_validId, _validNome, _validCpf, _validSenhaHash);

            // Assert
            Assert.Equal(_validId, recepcionista.Id);
            Assert.Equal(_validNome, recepcionista.Nome);
            Assert.Equal(_validCpf, recepcionista.Cpf);
            Assert.Equal(_validSenhaHash, recepcionista.SenhaHash);
            Assert.Equal(_validStatus, recepcionista.Status); // Default status
            Assert.Equal(_validRole, recepcionista.Role); // Forced role
        }

        [Fact]
        public void Recepcionista_Constructor_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Recepcionista(invalidId, _validNome, _validCpf, _validSenhaHash));
            Assert.Contains("ID da recepcionista não pode ser vazio.", exception.Message);
            Assert.Equal("id", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Recepcionista_Constructor_ShouldThrowArgumentException_WhenNomeIsInvalid(string? invalidNome)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Recepcionista(_validId, invalidNome!, _validCpf, _validSenhaHash));
            Assert.Contains("Nome da recepcionista não pode ser vazio.", exception.Message);
            Assert.Equal("nome", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Recepcionista_Constructor_ShouldThrowArgumentException_WhenCpfIsInvalid(string? invalidCpf)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Recepcionista(_validId, _validNome, invalidCpf!, _validSenhaHash));
            Assert.Contains("CPF da recepcionista não pode ser vazio.", exception.Message);
            Assert.Equal("cpf", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Recepcionista_Constructor_ShouldThrowArgumentException_WhenSenhaHashIsInvalid(string? invalidSenhaHash)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Recepcionista(_validId, _validNome, _validCpf, invalidSenhaHash!));
            Assert.Contains("Senha hash da recepcionista não pode ser vazia.", exception.Message);
            Assert.Equal("senhaHash", exception.ParamName);
        }

        // --- Testes de Atualização de Status ---

        [Fact]
        public void Recepcionista_AtualizarStatus_ShouldUpdateStatus()
        {
            // Arrange
            var recepcionista = new Recepcionista(_validId, _validNome, _validCpf, _validSenhaHash);
            var newStatus = StatusRecepcionista.Inativo;

            // Act
            recepcionista.AtualizarStatus(newStatus);

            // Assert
            Assert.Equal(newStatus, recepcionista.Status);
        }

        // --- Testes de Atualização de Dados ---

        [Fact]
        public void Recepcionista_AtualizarDados_ShouldUpdateAllProvidedValidFields()
        {
            // Arrange
            var recepcionista = new Recepcionista(_validId, _validNome, _validCpf, _validSenhaHash);
            var novoNome = "Novo Nome da Rec";
            var novoCpf = "99988877766";
            var novaSenhaHash = "novaSenhaHash456";

            // Act
            recepcionista.AtualizarDados(novoNome, novoCpf, novaSenhaHash);

            // Assert
            Assert.Equal(novoNome, recepcionista.Nome);
            Assert.Equal(novoCpf, recepcionista.Cpf);
            Assert.Equal(novaSenhaHash, recepcionista.SenhaHash);
        }

        [Fact]
        public void Recepcionista_AtualizarDados_ShouldNotUpdateNome_WhenNullOrWhitespaceProvided()
        {
            // Arrange
            var recepcionista = new Recepcionista(_validId, _validNome, _validCpf, _validSenhaHash);
            var originalNome = recepcionista.Nome;

            // Act
            recepcionista.AtualizarDados(null, null, null); // Testando null
            recepcionista.AtualizarDados("", null, null);   // Testando empty
            recepcionista.AtualizarDados("   ", null, null); // Testando whitespace

            // Assert
            Assert.Equal(originalNome, recepcionista.Nome);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Recepcionista_AtualizarDados_ShouldThrowArgumentException_WhenCpfIsProvidedAsEmptyOrWhitespace(string invalidCpf)
        {
            // Arrange
            var recepcionista = new Recepcionista(_validId, _validNome, _validCpf, _validSenhaHash);
            var originalCpf = recepcionista.Cpf;

            // Act & Assert
            // Deve lançar exceção se tentar setar CPF para vazio/whitespace
            var exception = Assert.Throws<ArgumentException>(() => recepcionista.AtualizarDados(null, invalidCpf, null));
            Assert.Contains("CPF não pode ser vazio se fornecido.", exception.Message);
            Assert.Equal("novoCpf", exception.ParamName);
            Assert.Equal(originalCpf, recepcionista.Cpf); // Verifica que o CPF não foi alterado
        }

        [Fact]
        public void Recepcionista_AtualizarDados_ShouldNotUpdateCpf_WhenNullProvided()
        {
            // Arrange
            var recepcionista = new Recepcionista(_validId, _validNome, _validCpf, _validSenhaHash);
            var originalCpf = recepcionista.Cpf;

            // Act
            recepcionista.AtualizarDados(null, null, null); // Passando null para CPF

            // Assert
            Assert.Equal(originalCpf, recepcionista.Cpf); // CPF deve permanecer o mesmo
        }


        [Fact]
        public void Recepcionista_AtualizarDados_ShouldNotUpdateSenhaHash_WhenNullOrWhitespaceProvided()
        {
            // Arrange
            var recepcionista = new Recepcionista(_validId, _validNome, _validCpf, _validSenhaHash);
            var originalSenhaHash = recepcionista.SenhaHash;

            // Act
            recepcionista.AtualizarDados(null, null, null); // Testando null
            recepcionista.AtualizarDados(null, null, "");   // Testando empty
            recepcionista.AtualizarDados(null, null, "   "); // Testando whitespace

            // Assert
            Assert.Equal(originalSenhaHash, recepcionista.SenhaHash);
        }

        // --- Teste do ToString ---

        [Fact]
        public void Recepcionista_ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var recepcionista = new Recepcionista(_validId, _validNome, _validCpf, _validSenhaHash);

            // Act
            var result = recepcionista.ToString();

            // Assert
            Assert.Contains(_validId.ToString(), result);
            Assert.Contains(_validNome, result);
            Assert.Contains(_validCpf, result);
            Assert.Contains(StatusRecepcionista.Ativo.ToString(), result);
            Assert.Contains("Recepcionista{", result);
            Assert.Contains("}", result);
        }
    }
}