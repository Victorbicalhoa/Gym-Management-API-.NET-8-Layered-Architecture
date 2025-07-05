// CentroTreinamento.Tests.Unit/Domain/Entities/InstrutorTests.cs
using Xunit;
using System;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums;

namespace CentroTreinamento.Tests.Unit.Domain.Entities
{
    public class InstrutorTests
    {
        [Fact]
        public void Instrutor_Constructor_ShouldCreateInstanceWithValidParameters()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Instrutor Teste";
            var cpf = "11122233344";
            var senhaHash = "hashedSenha";
            var status = StatusInstrutor.Ativo;
            var cref = "CREF123456";
            var role = UserRole.Instrutor;

            // Act
            var instrutor = new Instrutor(id, nome, cpf, senhaHash, status, cref, role);

            // Assert
            Assert.Equal(id, instrutor.Id);
            Assert.Equal(nome, instrutor.Nome);
            Assert.Equal(cpf, instrutor.Cpf);
            Assert.Equal(senhaHash, instrutor.SenhaHash);
            Assert.Equal(status, instrutor.Status);
            Assert.Equal(cref, instrutor.Cref);
            Assert.Equal(role, instrutor.Role);
        }

        [Fact]
        public void Instrutor_Constructor_ShouldCreateInstanceWithNullCpf()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Instrutor Teste Sem CPF";
            string? cpf = null;
            var senhaHash = "hashedSenha";
            var status = StatusInstrutor.Ativo;
            var cref = "CREF000000";
            var role = UserRole.Instrutor;

            // Act
            var instrutor = new Instrutor(id, nome, cpf, senhaHash, status, cref, role);

            // Assert
            Assert.Null(instrutor.Cpf);
            Assert.Equal(nome, instrutor.Nome);
        }

        [Fact]
        public void Instrutor_Constructor_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Arrange
            var id = Guid.Empty;
            var nome = "Instrutor Teste";
            var cpf = "11122233344";
            var senhaHash = "hashedSenha";
            var status = StatusInstrutor.Ativo;
            var cref = "CREF123456";
            var role = UserRole.Instrutor;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Instrutor(id, nome, cpf, senhaHash, status, cref, role));
            Assert.Contains("ID do instrutor não pode ser vazio.", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Instrutor_Constructor_ShouldThrowArgumentException_WhenNomeIsInvalid(string? invalidNome)
        {
            // Arrange
            var id = Guid.NewGuid();
            var cpf = "11122233344";
            var senhaHash = "hashedSenha";
            var status = StatusInstrutor.Ativo;
            var cref = "CREF123456";
            var role = UserRole.Instrutor;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Instrutor(id, invalidNome!, cpf, senhaHash, status, cref, role));
            Assert.Contains("Nome do instrutor não pode ser vazio.", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Instrutor_Constructor_ShouldThrowArgumentException_WhenSenhaHashIsInvalid(string? invalidSenhaHash)
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Instrutor Teste";
            var cpf = "11122233344";
            var status = StatusInstrutor.Ativo;
            var cref = "CREF123456";
            var role = UserRole.Instrutor;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Instrutor(id, nome, cpf, invalidSenhaHash!, status, cref, role));
            Assert.Contains("Senha hash do instrutor não pode ser vazia.", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Instrutor_Constructor_ShouldThrowArgumentException_WhenCrefIsInvalid(string? invalidCref)
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Instrutor Teste";
            var cpf = "11122233344";
            var senhaHash = "hashedSenha";
            var status = StatusInstrutor.Ativo;
            var role = UserRole.Instrutor;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Instrutor(id, nome, cpf, senhaHash, status, invalidCref!, role));
            Assert.Contains("CREF do instrutor não pode ser vazio.", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Instrutor_Constructor_ShouldThrowArgumentException_WhenCpfIsNotNullButInvalid(string invalidCpf)
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Instrutor Teste";
            var senhaHash = "hashedSenha";
            var status = StatusInstrutor.Ativo;
            var cref = "CREF123456";
            var role = UserRole.Instrutor;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Instrutor(id, nome, invalidCpf, senhaHash, status, cref, role));
            Assert.Contains("CPF do instrutor não pode ser vazio se não nulo.", exception.Message);
        }

        [Fact]
        public void Instrutor_Constructor_ShouldThrowArgumentException_WhenStatusIsInvalid()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Instrutor Teste";
            var cpf = "11122233344";
            var senhaHash = "hashedSenha";
            var status = (StatusInstrutor)99; // Valor inválido
            var cref = "CREF123456";
            var role = UserRole.Instrutor;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Instrutor(id, nome, cpf, senhaHash, status, cref, role));
            Assert.Contains("Status inválido.", exception.Message);
        }

        [Fact]
        public void Instrutor_Constructor_ShouldThrowArgumentException_WhenRoleIsNotInstrutor()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Instrutor Teste";
            var cpf = "11122233344";
            var senhaHash = "hashedSenha";
            var status = StatusInstrutor.Ativo;
            var cref = "CREF123456";
            var role = UserRole.Aluno; // Role inválida

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Instrutor(id, nome, cpf, senhaHash, status, cref, role));
            Assert.Contains("Role inválida para instrutor. Deve ser 'Instrutor'.", exception.Message);
        }

        [Fact]
        public void Instrutor_AtualizarStatus_ShouldUpdateStatus()
        {
            // Arrange
            var instrutor = new Instrutor(Guid.NewGuid(), "Instrutor Original", "11122233344", "hash", StatusInstrutor.Ativo, "CREF123", UserRole.Instrutor);
            var novoStatus = StatusInstrutor.Ferias;

            // Act
            instrutor.AtualizarStatus(novoStatus);

            // Assert
            Assert.Equal(novoStatus, instrutor.Status);
        }

        [Fact]
        public void Instrutor_AtualizarDados_ShouldUpdateAllProvidedFields()
        {
            // Arrange
            var instrutor = new Instrutor(Guid.NewGuid(), "Nome Antigo", "11122233344", "senhaAntiga", StatusInstrutor.Ativo, "CREF000", UserRole.Instrutor);
            var novoNome = "Nome Novo";
            var novoCpf = "99988877766";
            var novaSenhaHash = "novaSenhaHash";
            var novoCref = "CREF999";

            // Act
            instrutor.AtualizarDados(novoNome, novoCpf, novaSenhaHash, novoCref);

            // Assert
            Assert.Equal(novoNome, instrutor.Nome);
            Assert.Equal(novoCpf, instrutor.Cpf);
            Assert.Equal(novaSenhaHash, instrutor.SenhaHash);
            Assert.Equal(novoCref, instrutor.Cref);
        }

        [Fact]
        public void Instrutor_AtualizarDados_ShouldUpdateNomeAndCref_WhenCpfAndSenhaHashAreNull()
        {
            // Arrange
            var instrutor = new Instrutor(Guid.NewGuid(), "Nome Antigo", "11122233344", "senhaAntiga", StatusInstrutor.Ativo, "CREF000", UserRole.Instrutor);
            var novoNome = "Nome Novo Sem CPF/Senha";
            string? novoCpf = null; // Testando CPF nulo
            string? novaSenhaHash = null; // Testando SenhaHash nula
            var novoCref = "CREF111";

            // Act
            instrutor.AtualizarDados(novoNome, novoCpf, novaSenhaHash, novoCref);

            // Assert
            Assert.Equal(novoNome, instrutor.Nome);
            Assert.Equal("11122233344", instrutor.Cpf); // CPF original deve ser mantido
            Assert.Equal("senhaAntiga", instrutor.SenhaHash); // SenhaHash original deve ser mantida
            Assert.Equal(novoCref, instrutor.Cref);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Instrutor_AtualizarDados_ShouldKeepOriginalCpf_WhenNewCpfIsEmptyOrWhitespace(string invalidCpf)
        {
            // Arrange
            var instrutor = new Instrutor(Guid.NewGuid(), "Nome Antigo", "11122233344", "senhaAntiga", StatusInstrutor.Ativo, "CREF000", UserRole.Instrutor);
            var originalCpf = instrutor.Cpf; // Store the original CPF
            var novoNome = "Nome Atualizado";
            var novaSenhaHash = "novaSenhaHash";
            var novoCref = "CREF111";

            // Act & Assert
            // Continua a lançar uma ArgumentException para CPF vazio/whitespace, pois a entidade mantém essa validação
            var exception = Assert.Throws<ArgumentException>(() => instrutor.AtualizarDados(novoNome, invalidCpf, novaSenhaHash, novoCref));
            Assert.Contains("CPF não pode ser vazio se fornecido.", exception.Message);
            Assert.Equal(originalCpf, instrutor.Cpf); // Ensure CPF was not changed due to exception
        }

        [Fact]
        public void Instrutor_AtualizarDados_ShouldUpdateNomeCref_WhenSenhaHashIsEmptyString()
        {
            // Arrange
            var instrutor = new Instrutor(Guid.NewGuid(), "Nome Antigo", "11122233344", "senhaAntiga", StatusInstrutor.Ativo, "CREF000", UserRole.Instrutor);
            var novoNome = "Nome Atualizado";
            var novoCpf = "99988877766";
            var novaSenhaHash = ""; // Empty string for senhaHash
            var novoCref = "CREF111";

            // Act
            instrutor.AtualizarDados(novoNome, novoCpf, novaSenhaHash, novoCref);

            // Assert
            Assert.Equal(novoNome, instrutor.Nome);
            Assert.Equal(novoCpf, instrutor.Cpf);
            Assert.Equal("senhaAntiga", instrutor.SenhaHash); // SenhaHash should remain unchanged
            Assert.Equal(novoCref, instrutor.Cref);
        }

        [Fact]
        public void Instrutor_AtualizarDados_ShouldUpdateNomeCref_WhenSenhaHashIsWhiteSpace()
        {
            // Arrange
            var instrutor = new Instrutor(Guid.NewGuid(), "Nome Antigo", "11122233344", "senhaAntiga", StatusInstrutor.Ativo, "CREF000", UserRole.Instrutor);
            var novoNome = "Nome Atualizado";
            var novoCpf = "99988877766";
            var novaSenhaHash = "   "; // Whitespace for senhaHash
            var novoCref = "CREF111";

            // Act
            instrutor.AtualizarDados(novoNome, novoCpf, novaSenhaHash, novoCref);

            // Assert
            Assert.Equal(novoNome, instrutor.Nome);
            Assert.Equal(novoCpf, instrutor.Cpf);
            Assert.Equal("senhaAntiga", instrutor.SenhaHash); // SenhaHash should remain unchanged
            Assert.Equal(novoCref, instrutor.Cref);
        }

        // NOVOS TESTES PARA VALIDAR O COMPORTAMENTO DE "NÃO ATUALIZAR" NOME/CREF SE NULL/EMPTY/WHITESPACE
        [Fact]
        public void Instrutor_AtualizarDados_ShouldNotUpdateNomeOrCref_WhenProvidedAsNullOrEmpty()
        {
            // Arrange
            var instrutor = new Instrutor(Guid.NewGuid(), "Nome Original", "11122233344", "hash", StatusInstrutor.Ativo, "CREFORIGINAL", UserRole.Instrutor);
            var originalNome = instrutor.Nome;
            var originalCref = instrutor.Cref;

            // Act
            // Passando null para nome e cref, e vazio/whitespace para CPF e senha (que não devem atualizar)
            instrutor.AtualizarDados(null, null, "", null); // Removi o "   " do cref para testar null

            // Assert
            // Nome, CPF, SenhaHash e Cref devem permanecer inalterados
            Assert.Equal(originalNome, instrutor.Nome);
            Assert.Equal("11122233344", instrutor.Cpf); // CPF não deve ter mudado (passamos null)
            Assert.Equal("hash", instrutor.SenhaHash); // SenhaHash não deve ter mudado (passamos "")
            Assert.Equal(originalCref, instrutor.Cref);
        }

        [Fact]
        public void Instrutor_AtualizarDados_ShouldNotUpdateNomeOrCref_WhenProvidedAsWhitespace()
        {
            // Arrange
            var instrutor = new Instrutor(Guid.NewGuid(), "Nome Original", "11122233344", "hash", StatusInstrutor.Ativo, "CREFORIGINAL", UserRole.Instrutor);
            var originalNome = instrutor.Nome;
            var originalCref = instrutor.Cref;

            // Act
            // Passando apenas espaços em branco para nome e cref
            instrutor.AtualizarDados("   ", null, null, "   "); // Passei null para CPF e SenhaHash para focar em Nome/Cref

            // Assert
            // Nome e Cref devem permanecer inalterados
            Assert.Equal(originalNome, instrutor.Nome);
            Assert.Equal("11122233344", instrutor.Cpf); // CPF não deve ter mudado (passamos null)
            Assert.Equal("hash", instrutor.SenhaHash); // SenhaHash não deve ter mudado (passamos null)
            Assert.Equal(originalCref, instrutor.Cref);
        }

        [Fact]
        public void Instrutor_ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var id = Guid.NewGuid();
            var instrutor = new Instrutor(id, "Instrutor ToString", "12345678900", "hash", StatusInstrutor.Ativo, "CREF789", UserRole.Instrutor);

            // Act
            var result = instrutor.ToString();

            // Assert
            Assert.Contains(id.ToString(), result);
            Assert.Contains("Instrutor ToString", result);
            Assert.Contains("Ativo", result);
            Assert.Contains("CREF789", result);
            Assert.Contains("Instrutor{", result);
            Assert.Contains("}", result);
        }        
    }
}