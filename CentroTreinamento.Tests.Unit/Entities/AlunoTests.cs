// CentroTreinamento.Tests.Unit/Entities/AlunoTests.cs
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums; // Certifique-se de que StatusAluno e UserRole estão aqui
using Xunit;
using System;

namespace CentroTreinamento.Tests.Unit.Domain.Entities
{
    public class AlunoTests
    {
        // Teste para o construtor principal: verifica se as propriedades são inicializadas corretamente
        [Fact]
        public void AlunoConstructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "João Silva";
            var senhaHash = "hashedPassword123";
            var status = StatusAluno.Ativo; // Usando o enum correto
            var cpf = "123.456.789-00";
            var dataNascimento = new DateTime(1990, 1, 15);
            var telefone = "99999-8888";
            var role = UserRole.Aluno; // Usando o enum correto

            // Act
            var aluno = new Aluno(id, nome, senhaHash, status, cpf, dataNascimento, telefone, role);

            // Assert
            Assert.Equal(id, aluno.Id);
            Assert.Equal(nome, aluno.Nome);
            Assert.Equal(senhaHash, aluno.SenhaHash);
            Assert.Equal(status, aluno.Status);
            Assert.Equal(cpf, aluno.Cpf);
            Assert.Equal(dataNascimento, aluno.DataNascimento);
            Assert.Equal(telefone, aluno.Telefone);
            Assert.Equal(role, aluno.Role);
        }

        // Testes para o método AtualizarStatus: verifica se o status do aluno é alterado corretamente
        [Theory]
        [InlineData(StatusAluno.Ativo)]
        [InlineData(StatusAluno.Inativo)]
        [InlineData(StatusAluno.Pendente)] // Se você tiver StatusAluno.Pendente, inclua-o também aqui
        public void AtualizarStatus_ShouldChangeAlunoStatus(StatusAluno novoStatus)
        {
            // Arrange
            var id = Guid.NewGuid();
            // Crie um aluno com um status inicial diferente do 'novoStatus' para um teste mais robusto
            var aluno = new Aluno(id, "Maria Teste", "senhaHashInicial", StatusAluno.Ativo, "111.222.333-44", new DateTime(1995, 5, 10), "98765-4321", UserRole.Aluno);

            // Act
            aluno.AtualizarStatus(novoStatus);

            // Assert
            Assert.Equal(novoStatus, aluno.Status);
        }

        // Testes para validações do construtor: cenários de erro esperados
        [Fact]
        public void AlunoConstructor_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Arrange
            var id = Guid.Empty; // ID vazio, causando a exceção
            var nome = "Nome Valido";
            var senhaHash = "senhaHashValida";
            var status = StatusAluno.Ativo;
            var cpf = "123.456.789-00";
            var dataNascimento = new DateTime(1990, 1, 1);
            var telefone = "99999-8888";
            var role = UserRole.Aluno;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Aluno(id, nome, senhaHash, status, cpf, dataNascimento, telefone, role));
            Assert.Contains("ID do aluno não pode ser vazio.", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AlunoConstructor_ShouldThrowArgumentException_WhenNomeIsNullOrWhiteSpace(string? invalidNome)
        {
            // Arrange
            var id = Guid.NewGuid();
            var senhaHash = "senhaHashValida";
            var status = StatusAluno.Ativo;
            var cpf = "123.456.789-00";
            var dataNascimento = new DateTime(1990, 1, 1);
            var telefone = "99999-8888";
            var role = UserRole.Aluno;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Aluno(id, invalidNome!, senhaHash, status, cpf, dataNascimento, telefone, role));
            Assert.Contains("Nome do aluno não pode ser vazio.", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AlunoConstructor_ShouldThrowArgumentException_WhenSenhaHashIsNullOrWhiteSpace(string? invalidSenhaHash)
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Nome Valido";
            var status = StatusAluno.Ativo;
            var cpf = "123.456.789-00";
            var dataNascimento = new DateTime(1990, 1, 1);
            var telefone = "99999-8888";
            var role = UserRole.Aluno;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Aluno(id, nome, invalidSenhaHash!, status, cpf, dataNascimento, telefone, role));
            Assert.Contains("Senha hash do aluno não pode ser vazia.", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AlunoConstructor_ShouldThrowArgumentException_WhenCpfIsNullOrWhiteSpace(string? invalidCpf)
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Nome Valido";
            var senhaHash = "senhaHashValida";
            var status = StatusAluno.Ativo;
            var dataNascimento = new DateTime(1990, 1, 1);
            var telefone = "99999-8888";
            var role = UserRole.Aluno;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Aluno(id, nome, senhaHash, status, invalidCpf!, dataNascimento, telefone, role));
            Assert.Contains("CPF do aluno não pode ser vazio.", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AlunoConstructor_ShouldThrowArgumentException_WhenTelefoneIsNullOrWhiteSpace(string? invalidTelefone)
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Nome Valido";
            var senhaHash = "senhaHashValida";
            var status = StatusAluno.Ativo;
            var cpf = "123.456.789-00";
            var dataNascimento = new DateTime(1990, 1, 1);
            var role = UserRole.Aluno;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Aluno(id, nome, senhaHash, status, cpf, dataNascimento, invalidTelefone!, role));
            Assert.Contains("Telefone do aluno não pode ser vazio.", ex.Message);
        }

        [Fact]
        public void AlunoConstructor_ShouldThrowArgumentException_WhenDataNascimentoIsDefaultOrInFuture()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Nome Valido";
            var senhaHash = "senhaHashValida";
            var status = StatusAluno.Ativo;
            var cpf = "123.456.789-00";
            var telefone = "99999-8888";
            var role = UserRole.Aluno;

            // Teste com DateTime.MinValue (default)
            var exDefault = Assert.Throws<ArgumentException>(() => new Aluno(id, nome, senhaHash, status, cpf, default(DateTime), telefone, role));
            Assert.Contains("Data de nascimento inválida.", exDefault.Message);

            // Teste com data no futuro
            var dataNascimentoFuturo = DateTime.Now.AddDays(1);
            var exFuture = Assert.Throws<ArgumentException>(() => new Aluno(id, nome, senhaHash, status, cpf, dataNascimentoFuturo, telefone, role));
            Assert.Contains("Data de nascimento inválida. O aluno não pode ter nascido no futuro.", exFuture.Message);
        }

        [Theory]
        [InlineData((StatusAluno)999)] // Valor de enum inválido
        public void AlunoConstructor_ShouldThrowArgumentException_WhenStatusIsInvalid(StatusAluno invalidStatus)
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Nome Valido";
            var senhaHash = "senhaHashValida";
            var cpf = "123.456.789-00";
            var dataNascimento = new DateTime(1990, 1, 1);
            var telefone = "99999-8888";
            var role = UserRole.Aluno;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Aluno(id, nome, senhaHash, invalidStatus, cpf, dataNascimento, telefone, role));
            Assert.Contains("Status inválido.", ex.Message);
        }

        [Theory]
        [InlineData((UserRole)999)] // Valor de enum inválido
        public void AlunoConstructor_ShouldThrowArgumentException_WhenRoleIsInvalid(UserRole invalidRole)
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Nome Valido";
            var senhaHash = "senhaHashValida";
            var status = StatusAluno.Ativo;
            var cpf = "123.456.789-00";
            var dataNascimento = new DateTime(1990, 1, 1);
            var telefone = "99999-8888";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Aluno(id, nome, senhaHash, status, cpf, dataNascimento, telefone, invalidRole));
            Assert.Contains("Role inválida.", ex.Message);
        }

        // Testes para o método AtualizarDados: verifica se as propriedades são atualizadas corretamente
        [Fact]
        public void AtualizarDados_ShouldUpdateAllProvidedPropertiesCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var aluno = new Aluno(id, "Antigo Nome", "antigaHash", StatusAluno.Ativo, "999.888.777-66", new DateTime(1980, 1, 1), "11111-2222", UserRole.Aluno);

            var novoNome = "Novo Nome Completo";
            var novoCpf = "777.666.555-44";
            var novaSenhaHash = "novaHashGerada";
            var novaDataNascimento = new DateTime(1985, 2, 2);
            var novoTelefone = "33333-4444";

            // Act
            aluno.AtualizarDados(novoNome, novoCpf, novaSenhaHash, novaDataNascimento, novoTelefone);

            // Assert
            Assert.Equal(novoNome, aluno.Nome);
            Assert.Equal(novoCpf, aluno.Cpf);
            Assert.Equal(novaSenhaHash, aluno.SenhaHash);
            Assert.Equal(novaDataNascimento, aluno.DataNascimento);
            Assert.Equal(novoTelefone, aluno.Telefone);
            // Garante que Status e Role não foram alterados por este método
            Assert.Equal(StatusAluno.Ativo, aluno.Status);
            Assert.Equal(UserRole.Aluno, aluno.Role);
        }

        // Testes para o método AtualizarDados: verifica que campos opcionais não mudam se passados como null ou vazio
        [Fact]
        public void AtualizarDados_ShouldNotChangeOptionalFields_WhenNullOrEmptyProvided()
        {
            // Arrange
            var id = Guid.NewGuid();
            var originalNome = "Nome Original";
            var originalCpf = "123.456.789-00";
            var originalSenhaHash = "hashOriginal";
            var originalDataNascimento = new DateTime(1990, 1, 1);
            var originalTelefone = "55555-4444";
            var aluno = new Aluno(id, originalNome, originalSenhaHash, StatusAluno.Ativo, originalCpf, originalDataNascimento, originalTelefone, UserRole.Aluno);

            var novoNome = "Nome Atualizado";
            var novoCpf = "000.111.222-33";
            // Passando null para novaSenhaHash, novaDataNascimento e novoTelefone
            string? novaSenhaHash = null;
            DateTime? novaDataNascimento = null;
            string? novoTelefone = null;

            // Act
            aluno.AtualizarDados(novoNome, novoCpf, novaSenhaHash, novaDataNascimento, novoTelefone);

            // Assert
            Assert.Equal(novoNome, aluno.Nome); // Nome e CPF devem ter mudado
            Assert.Equal(novoCpf, aluno.Cpf);
            Assert.Equal(originalSenhaHash, aluno.SenhaHash); // Senha não deve ter mudado
            Assert.Equal(originalDataNascimento, aluno.DataNascimento); // Data de nascimento não deve ter mudado
            Assert.Equal(originalTelefone, aluno.Telefone); // Telefone não deve ter mudado

            // Teste com strings vazias para SenhaHash e Telefone
            aluno = new Aluno(id, originalNome, originalSenhaHash, StatusAluno.Ativo, originalCpf, originalDataNascimento, originalTelefone, UserRole.Aluno); // Reset para o original
            novaSenhaHash = "";
            novoTelefone = "";
            aluno.AtualizarDados(novoNome, novoCpf, novaSenhaHash, novaDataNascimento, novoTelefone);

            Assert.Equal(originalSenhaHash, aluno.SenhaHash); // Senha não deve ter mudado
            Assert.Equal(originalTelefone, aluno.Telefone); // Telefone não deve ter mudado
        }

        // Teste para o método ToString()
        [Fact]
        public void ToString_ShouldReturnFormattedStringWithRelevantInfo()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Aluno Teste ToString";
            var cpf = "987.654.321-00";
            var status = StatusAluno.Pendente;
            var aluno = new Aluno(id, nome, "hash", status, cpf, new DateTime(2000, 1, 1), "tel", UserRole.Aluno);

            // Act
            var result = aluno.ToString();

            // Assert
            Assert.Contains($"Id={id}", result);
            Assert.Contains($"Nome='{nome}'", result);
            Assert.Contains($"Cpf='{cpf}'", result);
            Assert.Contains($"Status='{status}'", result);
        }
    }
}