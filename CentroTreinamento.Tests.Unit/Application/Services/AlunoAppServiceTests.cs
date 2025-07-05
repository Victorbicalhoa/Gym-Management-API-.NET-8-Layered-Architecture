// CentroTreinamento.Tests.Unit/Application/Services/AlunoAppServiceTests.cs
using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentroTreinamento.Application.DTOs.Aluno;
using CentroTreinamento.Application.Services;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums;
using CentroTreinamento.Domain.Repositories; // Para IAlunoRepository
using CentroTreinamento.Application.Interfaces; // Para IPasswordHasher

namespace CentroTreinamento.Tests.Unit.Application.Services
{
    public class AlunoAppServiceTests
    {
        private readonly Mock<IAlunoRepository> _mockAlunoRepository;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly AlunoAppService _alunoAppService;

        public AlunoAppServiceTests()
        {
            _mockAlunoRepository = new Mock<IAlunoRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _alunoAppService = new AlunoAppService(_mockAlunoRepository.Object, _mockPasswordHasher.Object);
        }

        [Fact]
        public async Task GetAllAlunosAsync_ShouldReturnAllAlunos()
        {
            // Arrange
            var alunos = new List<Aluno>
            {
                new Aluno(Guid.NewGuid(), "Aluno 1", "hash1", StatusAluno.Ativo, "11122233344", new DateTime(2000, 1, 1), "11987654321", UserRole.Aluno),
                new Aluno(Guid.NewGuid(), "Aluno 2", "hash2", StatusAluno.Inativo, "22233344455", new DateTime(1999, 2, 2), "22987654321", UserRole.Aluno)
            };
            _mockAlunoRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(alunos);

            // Act
            var result = await _alunoAppService.GetAllAlunosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockAlunoRepository.Verify(repo => repo.GetAllAsync(), Times.Once); // Verifica se o método do repositório foi chamado
        }

        [Fact]
        public async Task GetAlunoByIdAsync_ShouldReturnAluno_WhenAlunoExists()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var aluno = new Aluno(alunoId, "Aluno Teste", "hash", StatusAluno.Ativo, "12345678900", new DateTime(1990, 1, 1), "1111122222", UserRole.Aluno);
            _mockAlunoRepository.Setup(repo => repo.GetByIdAsync(alunoId)).ReturnsAsync(aluno);

            // Act
            var result = await _alunoAppService.GetAlunoByIdAsync(alunoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(alunoId, result.Id);
            Assert.Equal(aluno.Nome, result.Nome);
            _mockAlunoRepository.Verify(repo => repo.GetByIdAsync(alunoId), Times.Once);
        }

        [Fact]
        public async Task GetAlunoByIdAsync_ShouldReturnNull_WhenAlunoDoesNotExist()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            _mockAlunoRepository.Setup(repo => repo.GetByIdAsync(alunoId)).ReturnsAsync((Aluno)null!); // Retorna null para simular não encontrado

            // Act
            var result = await _alunoAppService.GetAlunoByIdAsync(alunoId);

            // Assert
            Assert.Null(result);
            _mockAlunoRepository.Verify(repo => repo.GetByIdAsync(alunoId), Times.Once);
        }

        [Fact]
        public async Task CreateAlunoAsync_ShouldCreateNewAlunoAndReturnViewModel()
        {
            // Arrange
            var inputModel = new AlunoInputModel
            {
                Nome = "Novo Aluno",
                Cpf = "999.888.777-66",
                Senha = "senhaTeste123",
                DataNascimento = new DateTime(1998, 7, 20),
                Telefone = "33999990000"
            };
            var hashedPassword = "hashedPassword";

            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(inputModel.Senha!)).Returns(hashedPassword);
            _mockAlunoRepository.Setup(repo => repo.AddAsync(It.IsAny<Aluno>())).Returns(Task.CompletedTask);
            _mockAlunoRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _alunoAppService.CreateAlunoAsync(inputModel);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(inputModel.Nome, result.Nome);
            Assert.Equal(inputModel.Cpf!.Replace(".", "").Replace("-", ""), result.Cpf); // Verifica o CPF normalizado
            Assert.Equal(StatusAluno.Ativo, result.Status); // Status padrão deve ser Ativo
            Assert.Equal(UserRole.Aluno, result.Role); // Role padrão deve ser Aluno
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(inputModel.Senha!), Times.Once);
            _mockAlunoRepository.Verify(repo => repo.AddAsync(It.Is<Aluno>(a =>
                a.Nome == inputModel.Nome &&
                a.SenhaHash == hashedPassword &&
                a.Cpf == inputModel.Cpf!.Replace(".", "").Replace("-", "") // Verifica o CPF normalizado passado para a entidade
            )), Times.Once);
            _mockAlunoRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAlunoAsync_ShouldUpdateExistingAlunoAndReturnViewModel_WhenAlunoExists()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var existingAluno = new Aluno(alunoId, "Nome Antigo", "oldHash", StatusAluno.Ativo, "12345678900", new DateTime(1990, 1, 1), "1111122222", UserRole.Aluno);
            var updatedInput = new AlunoInputModel
            {
                Nome = "Nome Novo",
                Cpf = "000.999.888-77",
                Senha = "novaSenha123",
                DataNascimento = new DateTime(1992, 3, 3),
                Telefone = "3333344444"
            };
            var newHashedPassword = "newHashedPassword";

            _mockAlunoRepository.Setup(repo => repo.GetByIdAsync(alunoId)).ReturnsAsync(existingAluno);
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(updatedInput.Senha!)).Returns(newHashedPassword);
            _mockAlunoRepository.Setup(repo => repo.Update(It.IsAny<Aluno>()));
            _mockAlunoRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _alunoAppService.UpdateAlunoAsync(alunoId, updatedInput);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(alunoId, result.Id);
            Assert.Equal(updatedInput.Nome, result.Nome);
            Assert.Equal(updatedInput.Cpf!.Replace(".", "").Replace("-", ""), result.Cpf);
            Assert.Equal(updatedInput.DataNascimento, result.DataNascimento);
            Assert.Equal(updatedInput.Telefone, result.Telefone);

            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(updatedInput.Senha!), Times.Once);
            _mockAlunoRepository.Verify(repo => repo.Update(It.Is<Aluno>(a =>
                a.Id == alunoId &&
                a.Nome == updatedInput.Nome &&
                a.SenhaHash == newHashedPassword && // Verifica se a senha foi atualizada
                a.Cpf == updatedInput.Cpf!.Replace(".", "").Replace("-", "")
            )), Times.Once);
            _mockAlunoRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAlunoAsync_ShouldNotUpdatePassword_WhenSenhaIsNullInInput()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var originalPasswordHash = "originalHash";
            var existingAluno = new Aluno(alunoId, "Nome Antigo", originalPasswordHash, StatusAluno.Ativo, "12345678900", new DateTime(1990, 1, 1), "1111122222", UserRole.Aluno);
            var updatedInput = new AlunoInputModel
            {
                Nome = "Nome Novo",
                Cpf = "000.999.888-77",
                Senha = null, // Senha é null
                DataNascimento = new DateTime(1992, 3, 3),
                Telefone = "3333344444"
            };

            _mockAlunoRepository.Setup(repo => repo.GetByIdAsync(alunoId)).ReturnsAsync(existingAluno);
            _mockAlunoRepository.Setup(repo => repo.Update(It.IsAny<Aluno>()));
            _mockAlunoRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _alunoAppService.UpdateAlunoAsync(alunoId, updatedInput);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(originalPasswordHash, existingAluno.SenhaHash); // Senha deve permanecer a original
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<string>()), Times.Never); // HashPassword não deve ser chamado
            _mockAlunoRepository.Verify(repo => repo.Update(It.IsAny<Aluno>()), Times.Once);
            _mockAlunoRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAlunoAsync_ShouldReturnNull_WhenAlunoDoesNotExist()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var inputModel = new AlunoInputModel { Nome = "Nome", Cpf = "123", Senha = "123", DataNascimento = DateTime.Now, Telefone = "123" };
            _mockAlunoRepository.Setup(repo => repo.GetByIdAsync(alunoId)).ReturnsAsync((Aluno)null!);

            // Act
            var result = await _alunoAppService.UpdateAlunoAsync(alunoId, inputModel);

            // Assert
            Assert.Null(result);
            _mockAlunoRepository.Verify(repo => repo.GetByIdAsync(alunoId), Times.Once);
            _mockAlunoRepository.Verify(repo => repo.Update(It.IsAny<Aluno>()), Times.Never); // Update não deve ser chamado
            _mockAlunoRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never); // SaveChanges não deve ser chamado
        }

        [Fact]
        public async Task UpdateAlunoStatusAsync_ShouldUpdateStatusAndReturnTrue_WhenAlunoExists()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var existingAluno = new Aluno(alunoId, "Aluno Status", "hash", StatusAluno.Ativo, "12345678900", new DateTime(1990, 1, 1), "1111122222", UserRole.Aluno);
            var novoStatus = StatusAluno.Inativo;

            _mockAlunoRepository.Setup(repo => repo.GetByIdAsync(alunoId)).ReturnsAsync(existingAluno);
            _mockAlunoRepository.Setup(repo => repo.Update(It.IsAny<Aluno>()));
            _mockAlunoRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _alunoAppService.UpdateAlunoStatusAsync(alunoId, novoStatus);

            // Assert
            Assert.True(result);
            Assert.Equal(novoStatus, existingAluno.Status); // Verifica se o status da entidade foi atualizado
            _mockAlunoRepository.Verify(repo => repo.GetByIdAsync(alunoId), Times.Once);
            _mockAlunoRepository.Verify(repo => repo.Update(It.Is<Aluno>(a => a.Id == alunoId && a.Status == novoStatus)), Times.Once);
            _mockAlunoRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAlunoStatusAsync_ShouldReturnFalse_WhenAlunoDoesNotExist()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            _mockAlunoRepository.Setup(repo => repo.GetByIdAsync(alunoId)).ReturnsAsync((Aluno)null!);

            // Act
            var result = await _alunoAppService.UpdateAlunoStatusAsync(alunoId, StatusAluno.Inativo);

            // Assert
            Assert.False(result);
            _mockAlunoRepository.Verify(repo => repo.GetByIdAsync(alunoId), Times.Once);
            _mockAlunoRepository.Verify(repo => repo.Update(It.IsAny<Aluno>()), Times.Never);
            _mockAlunoRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteAlunoAsync_ShouldReturnTrue_WhenAlunoExists()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var alunoToDelete = new Aluno(alunoId, "Aluno Delete", "hash", StatusAluno.Ativo, "12345678900", new DateTime(1990, 1, 1), "1111122222", UserRole.Aluno);

            _mockAlunoRepository.Setup(repo => repo.GetByIdAsync(alunoId)).ReturnsAsync(alunoToDelete);
            _mockAlunoRepository.Setup(repo => repo.Delete(alunoToDelete));
            _mockAlunoRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _alunoAppService.DeleteAlunoAsync(alunoId);

            // Assert
            Assert.True(result);
            _mockAlunoRepository.Verify(repo => repo.GetByIdAsync(alunoId), Times.Once);
            _mockAlunoRepository.Verify(repo => repo.Delete(alunoToDelete), Times.Once);
            _mockAlunoRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAlunoAsync_ShouldReturnFalse_WhenAlunoDoesNotExist()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            _mockAlunoRepository.Setup(repo => repo.GetByIdAsync(alunoId)).ReturnsAsync((Aluno)null!);

            // Act
            var result = await _alunoAppService.DeleteAlunoAsync(alunoId);

            // Assert
            Assert.False(result);
            _mockAlunoRepository.Verify(repo => repo.GetByIdAsync(alunoId), Times.Once);
            _mockAlunoRepository.Verify(repo => repo.Delete(It.IsAny<Aluno>()), Times.Never);
            _mockAlunoRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task LoginAlunoAsync_ShouldReturnViewModel_WhenCredentialsAreValid()
        {
            // Arrange
            var cpf = "123.456.789-00";
            var senha = "senhaCorreta";
            var cpfNormalizado = "12345678900";
            var hashedPassword = "hashedPassword";
            var aluno = new Aluno(Guid.NewGuid(), "Aluno Login", hashedPassword, StatusAluno.Ativo, cpfNormalizado, new DateTime(1990, 1, 1), "1111122222", UserRole.Aluno);

            // Configura o mock para FindAsync retornar uma lista contendo o aluno
            _mockAlunoRepository.Setup(repo => repo.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Aluno, bool>>>()))
                                .ReturnsAsync((System.Linq.Expressions.Expression<Func<Aluno, bool>> predicate) =>
                                {
                                    // Simula a filtragem pelo predicado
                                    var compiledPredicate = predicate.Compile();
                                    return new List<Aluno> { aluno }.Where(compiledPredicate).ToList();
                                });

            _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword(senha, hashedPassword)).Returns(true);

            // Act
            var result = await _alunoAppService.LoginAlunoAsync(cpf, senha);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(aluno.Id, result.Id);
            Assert.Equal(aluno.Nome, result.Nome);
            Assert.Equal(aluno.Cpf, result.Cpf);
            _mockAlunoRepository.Verify(repo => repo.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Aluno, bool>>>()), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyPassword(senha, hashedPassword), Times.Once);
        }

        [Fact]
        public async Task LoginAlunoAsync_ShouldReturnNull_WhenAlunoNotFound()
        {
            // Arrange
            var cpf = "123.456.789-00";
            var senha = "senhaCorreta";

            _mockAlunoRepository.Setup(repo => repo.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Aluno, bool>>>()))
                                .ReturnsAsync(new List<Aluno>()); // Retorna lista vazia, aluno não encontrado

            // Act
            var result = await _alunoAppService.LoginAlunoAsync(cpf, senha);

            // Assert
            Assert.Null(result);
            _mockAlunoRepository.Verify(repo => repo.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Aluno, bool>>>()), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never); // VerifyPassword não deve ser chamado
        }

        [Fact]
        public async Task LoginAlunoAsync_ShouldReturnNull_WhenPasswordIsInvalid()
        {
            // Arrange
            var cpf = "123.456.789-00";
            var senhaIncorreta = "senhaIncorreta";
            var cpfNormalizado = "12345678900";
            var hashedPassword = "hashedPassword";
            var aluno = new Aluno(Guid.NewGuid(), "Aluno Login", hashedPassword, StatusAluno.Ativo, cpfNormalizado, new DateTime(1990, 1, 1), "1111122222", UserRole.Aluno);

            _mockAlunoRepository.Setup(repo => repo.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Aluno, bool>>>()))
                                .ReturnsAsync(new List<Aluno> { aluno }); // Aluno encontrado

            _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword(senhaIncorreta, hashedPassword)).Returns(false); // Senha inválida

            // Act
            var result = await _alunoAppService.LoginAlunoAsync(cpf, senhaIncorreta);

            // Assert
            Assert.Null(result);
            _mockAlunoRepository.Verify(repo => repo.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Aluno, bool>>>()), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyPassword(senhaIncorreta, hashedPassword), Times.Once);
        }
    }
}