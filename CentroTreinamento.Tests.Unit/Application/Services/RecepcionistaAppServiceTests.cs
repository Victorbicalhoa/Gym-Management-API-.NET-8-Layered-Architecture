// CentroTreinamento.Tests.Unit/Application/Services/RecepcionistaAppServiceTests.cs
using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentroTreinamento.Application.DTOs.Recepcionista;
using CentroTreinamento.Application.Interfaces; // Para IPasswordHasher
using CentroTreinamento.Application.Services;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums;
using CentroTreinamento.Domain.Repositories; // Para IRecepcionistaRepository

namespace CentroTreinamento.Tests.Unit.Application.Services
{
    public class RecepcionistaAppServiceTests
    {
        private readonly Mock<IRecepcionistaRepository> _mockRecepcionistaRepository;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly RecepcionistaAppService _sut; // System Under Test

        public RecepcionistaAppServiceTests()
        {
            _mockRecepcionistaRepository = new Mock<IRecepcionistaRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _sut = new RecepcionistaAppService(_mockRecepcionistaRepository.Object, _mockPasswordHasher.Object);
        }

        // --- Testes para CreateRecepcionistaAsync ---

        [Fact]
        public async Task CreateRecepcionistaAsync_ShouldCreateRecepcionista_WhenCpfIsUnique()
        {
            // Arrange
            var inputModel = new RecepcionistaInputModel
            {
                Nome = "Nova Recepcionista",
                Cpf = "123.456.789-00",
                Senha = "senhaSegura123"
            };
            var cleanedCpf = "12345678900";
            var hashedPassword = "hashed_senha_segura_123";

            _mockRecepcionistaRepository.Setup(r => r.GetByCpfAsync(cleanedCpf)).ReturnsAsync((Recepcionista?)null);
            _mockPasswordHasher.Setup(p => p.HashPassword(inputModel.Senha)).Returns(hashedPassword);
            _mockRecepcionistaRepository.Setup(r => r.AddAsync(It.IsAny<Recepcionista>())).Returns(Task.CompletedTask);
            _mockRecepcionistaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _sut.CreateRecepcionistaAsync(inputModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(inputModel.Nome, result.Nome);
            Assert.Equal(cleanedCpf, result.Cpf);
            Assert.Equal(StatusRecepcionista.Ativo, result.Status);
            Assert.Equal(UserRole.Recepcionista, result.Role);

            _mockRecepcionistaRepository.Verify(r => r.AddAsync(It.IsAny<Recepcionista>()), Times.Once);
            _mockRecepcionistaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateRecepcionistaAsync_ShouldThrowApplicationException_WhenCpfAlreadyExists()
        {
            // Arrange
            var inputModel = new RecepcionistaInputModel
            {
                Nome = "Recepcionista Duplicada",
                Cpf = "111.222.333-44",
                Senha = "senha"
            };
            var cleanedCpf = "11122233344";
            var existingRecepcionista = new Recepcionista(Guid.NewGuid(), "Existente", cleanedCpf, "hash");

            _mockRecepcionistaRepository.Setup(r => r.GetByCpfAsync(cleanedCpf)).ReturnsAsync(existingRecepcionista);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _sut.CreateRecepcionistaAsync(inputModel));
            Assert.Equal("Já existe um recepcionista cadastrado com este CPF.", exception.Message);

            _mockRecepcionistaRepository.Verify(r => r.AddAsync(It.IsAny<Recepcionista>()), Times.Never);
            _mockRecepcionistaRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        // --- Testes para GetRecepcionistaByIdAsync ---

        [Fact]
        public async Task GetRecepcionistaByIdAsync_ShouldReturnRecepcionista_WhenFound()
        {
            // Arrange
            var recepcionistaId = Guid.NewGuid();
            var recepcionista = new Recepcionista(recepcionistaId, "Nome Recepcionista", "12345678900", "hash");
            _mockRecepcionistaRepository.Setup(r => r.GetByIdAsync(recepcionistaId)).ReturnsAsync(recepcionista);

            // Act
            var result = await _sut.GetRecepcionistaByIdAsync(recepcionistaId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(recepcionistaId, result.Id);
            Assert.Equal(recepcionista.Nome, result.Nome);
        }

        [Fact]
        public async Task GetRecepcionistaByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var recepcionistaId = Guid.NewGuid();
            _mockRecepcionistaRepository.Setup(r => r.GetByIdAsync(recepcionistaId)).ReturnsAsync((Recepcionista?)null);

            // Act
            var result = await _sut.GetRecepcionistaByIdAsync(recepcionistaId);

            // Assert
            Assert.Null(result);
        }

        // --- Testes para GetAllRecepcionistasAsync ---

        [Fact]
        public async Task GetAllRecepcionistasAsync_ShouldReturnAllRecepcionistas()
        {
            // Arrange
            var recepcionistas = new List<Recepcionista>
            {
                new Recepcionista(Guid.NewGuid(), "Rec 1", "111", "h1"),
                new Recepcionista(Guid.NewGuid(), "Rec 2", "222", "h2")
            };
            _mockRecepcionistaRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(recepcionistas);

            // Act
            var result = await _sut.GetAllRecepcionistasAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.Nome == "Rec 1");
            Assert.Contains(result, r => r.Nome == "Rec 2");
        }

        [Fact]
        public async Task GetAllRecepcionistasAsync_ShouldReturnEmptyList_WhenNoRecepcionistasExist()
        {
            // Arrange
            _mockRecepcionistaRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Recepcionista>());

            // Act
            var result = await _sut.GetAllRecepcionistasAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // --- Testes para UpdateRecepcionistaAsync ---

        [Fact]
        public async Task UpdateRecepcionistaAsync_ShouldUpdateRecepcionista_WhenIdIsValidAndCpfIsUnique()
        {
            // Arrange
            var recepcionistaId = Guid.NewGuid();
            var existingRecepcionista = new Recepcionista(recepcionistaId, "Nome Original", "11122233344", "hashOriginal");
            var updateModel = new RecepcionistaInputModel
            {
                Nome = "Nome Atualizado",
                Cpf = "99988877766",
                Senha = "novaSenha"
            };
            var cleanedCpf = "99988877766";
            var newHashedPassword = "new_hashed_password";

            _mockRecepcionistaRepository.Setup(r => r.GetByIdAsync(recepcionistaId)).ReturnsAsync(existingRecepcionista);
            _mockRecepcionistaRepository.Setup(r => r.GetByCpfAsync(cleanedCpf)).ReturnsAsync((Recepcionista?)null); // CPF novo e único
            _mockPasswordHasher.Setup(p => p.HashPassword(updateModel.Senha)).Returns(newHashedPassword);
            _mockRecepcionistaRepository.Setup(r => r.Update(It.IsAny<Recepcionista>()));
            _mockRecepcionistaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _sut.UpdateRecepcionistaAsync(recepcionistaId, updateModel);

            // Assert
            Assert.Equal(updateModel.Nome, existingRecepcionista.Nome);
            Assert.Equal(cleanedCpf, existingRecepcionista.Cpf);
            Assert.Equal(newHashedPassword, existingRecepcionista.SenhaHash);

            _mockRecepcionistaRepository.Verify(r => r.Update(existingRecepcionista), Times.Once);
            _mockRecepcionistaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateRecepcionistaAsync_ShouldUpdateRecepcionista_WhenSenhaIsEmpty_AndKeepOriginalHash()
        {
            // Arrange
            var recepcionistaId = Guid.NewGuid();
            var originalSenhaHash = "hashOriginal";
            var existingRecepcionista = new Recepcionista(recepcionistaId, "Nome Original", "11122233344", originalSenhaHash);
            var updateModel = new RecepcionistaInputModel
            {
                Nome = "Nome Atualizado",
                Cpf = "11122233344", // Keep CPF same
                Senha = "" // Empty password, should not change hash
            };
            var cleanedCpf = "11122233344";

            _mockRecepcionistaRepository.Setup(r => r.GetByIdAsync(recepcionistaId)).ReturnsAsync(existingRecepcionista);
            _mockRecepcionistaRepository.Setup(r => r.GetByCpfAsync(cleanedCpf)).ReturnsAsync(existingRecepcionista); // Mocks that it's the same recep
            _mockPasswordHasher.Setup(p => p.HashPassword(updateModel.Senha)).Returns(""); // Mock hasher to return empty for empty input
            _mockRecepcionistaRepository.Setup(r => r.Update(It.IsAny<Recepcionista>()));
            _mockRecepcionistaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _sut.UpdateRecepcionistaAsync(recepcionistaId, updateModel);

            // Assert
            Assert.Equal(updateModel.Nome, existingRecepcionista.Nome);
            Assert.Equal(cleanedCpf, existingRecepcionista.Cpf);
            Assert.Equal(originalSenhaHash, existingRecepcionista.SenhaHash); // SenhaHash should be original

            _mockRecepcionistaRepository.Verify(r => r.Update(existingRecepcionista), Times.Once);
            _mockRecepcionistaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async Task UpdateRecepcionistaAsync_ShouldThrowApplicationException_WhenRecepcionistaNotFound()
        {
            // Arrange
            var recepcionistaId = Guid.NewGuid();
            var updateModel = new RecepcionistaInputModel
            {
                Nome = "Nome",
                Cpf = "123",
                Senha = "senha"
            };
            _mockRecepcionistaRepository.Setup(r => r.GetByIdAsync(recepcionistaId)).ReturnsAsync((Recepcionista?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _sut.UpdateRecepcionistaAsync(recepcionistaId, updateModel));
            Assert.Equal("Recepcionista não encontrado.", exception.Message);

            _mockRecepcionistaRepository.Verify(r => r.Update(It.IsAny<Recepcionista>()), Times.Never);
            _mockRecepcionistaRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateRecepcionistaAsync_ShouldThrowApplicationException_WhenCpfAlreadyExistsForAnotherRecepcionista()
        {
            // Arrange
            var recepcionistaId = Guid.NewGuid();
            var existingRecepcionista = new Recepcionista(recepcionistaId, "Nome Original", "11122233344", "hashOriginal");
            var anotherRecepcionistaWithSameCpf = new Recepcionista(Guid.NewGuid(), "Outra Recepcionista", "99988877766", "hashOutro");
            var updateModel = new RecepcionistaInputModel
            {
                Nome = "Nome Atualizado",
                Cpf = "999.888.777-66", // CPF que já existe para outro
                Senha = "novaSenha"
            };
            var cleanedCpf = "99988877766";

            _mockRecepcionistaRepository.Setup(r => r.GetByIdAsync(recepcionistaId)).ReturnsAsync(existingRecepcionista);
            _mockRecepcionistaRepository.Setup(r => r.GetByCpfAsync(cleanedCpf)).ReturnsAsync(anotherRecepcionistaWithSameCpf); // Mock CPF already exists

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _sut.UpdateRecepcionistaAsync(recepcionistaId, updateModel));
            Assert.Equal("Já existe outro recepcionista com este CPF.", exception.Message);

            _mockRecepcionistaRepository.Verify(r => r.Update(It.IsAny<Recepcionista>()), Times.Never);
            _mockRecepcionistaRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        // --- Testes para UpdateRecepcionistaStatusAsync ---

        [Fact]
        public async Task UpdateRecepcionistaStatusAsync_ShouldUpdateStatus()
        {
            // Arrange
            var recepcionistaId = Guid.NewGuid();
            var existingRecepcionista = new Recepcionista(recepcionistaId, "Nome Original", "11122233344", "hashOriginal");
            var newStatus = StatusRecepcionista.Ferias;

            _mockRecepcionistaRepository.Setup(r => r.GetByIdAsync(recepcionistaId)).ReturnsAsync(existingRecepcionista);
            _mockRecepcionistaRepository.Setup(r => r.Update(It.IsAny<Recepcionista>()));
            _mockRecepcionistaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _sut.UpdateRecepcionistaStatusAsync(recepcionistaId, newStatus);

            // Assert
            Assert.Equal(newStatus, existingRecepcionista.Status);
            _mockRecepcionistaRepository.Verify(r => r.Update(existingRecepcionista), Times.Once);
            _mockRecepcionistaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateRecepcionistaStatusAsync_ShouldThrowApplicationException_WhenRecepcionistaNotFound()
        {
            // Arrange
            var recepcionistaId = Guid.NewGuid();
            var newStatus = StatusRecepcionista.Inativo;

            _mockRecepcionistaRepository.Setup(r => r.GetByIdAsync(recepcionistaId)).ReturnsAsync((Recepcionista?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _sut.UpdateRecepcionistaStatusAsync(recepcionistaId, newStatus));
            Assert.Equal("Recepcionista não encontrado.", exception.Message);

            _mockRecepcionistaRepository.Verify(r => r.Update(It.IsAny<Recepcionista>()), Times.Never);
            _mockRecepcionistaRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        // --- Testes para DeleteRecepcionistaAsync ---

        [Fact]
        public async Task DeleteRecepcionistaAsync_ShouldDeleteRecepcionista()
        {
            // Arrange
            var recepcionistaId = Guid.NewGuid();
            var existingRecepcionista = new Recepcionista(recepcionistaId, "Nome Original", "11122233344", "hashOriginal");

            _mockRecepcionistaRepository.Setup(r => r.GetByIdAsync(recepcionistaId)).ReturnsAsync(existingRecepcionista);
            _mockRecepcionistaRepository.Setup(r => r.Delete(It.IsAny<Recepcionista>()));
            _mockRecepcionistaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _sut.DeleteRecepcionistaAsync(recepcionistaId);

            // Assert
            _mockRecepcionistaRepository.Verify(r => r.Delete(existingRecepcionista), Times.Once);
            _mockRecepcionistaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteRecepcionistaAsync_ShouldThrowApplicationException_WhenRecepcionistaNotFound()
        {
            // Arrange
            var recepcionistaId = Guid.NewGuid();
            _mockRecepcionistaRepository.Setup(r => r.GetByIdAsync(recepcionistaId)).ReturnsAsync((Recepcionista?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _sut.DeleteRecepcionistaAsync(recepcionistaId));
            Assert.Equal("Recepcionista não encontrado.", exception.Message);

            _mockRecepcionistaRepository.Verify(r => r.Delete(It.IsAny<Recepcionista>()), Times.Never);
            _mockRecepcionistaRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        // --- Testes para GetRecepcionistaByCpfAsync ---

        [Fact]
        public async Task GetRecepcionistaByCpfAsync_ShouldReturnRecepcionista_WhenFound()
        {
            // Arrange
            var cpf = "111.222.333-44";
            var cleanedCpf = "11122233344";
            var recepcionista = new Recepcionista(Guid.NewGuid(), "Nome Cpf", cleanedCpf, "hash");
            _mockRecepcionistaRepository.Setup(r => r.GetByCpfAsync(cleanedCpf)).ReturnsAsync(recepcionista);

            // Act
            var result = await _sut.GetRecepcionistaByCpfAsync(cpf);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(recepcionista.Nome, result.Nome);
            Assert.Equal(cleanedCpf, result.Cpf);
        }

        [Fact]
        public async Task GetRecepcionistaByCpfAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var cpf = "000.000.000-00";
            var cleanedCpf = "00000000000";
            _mockRecepcionistaRepository.Setup(r => r.GetByCpfAsync(cleanedCpf)).ReturnsAsync((Recepcionista?)null);

            // Act
            var result = await _sut.GetRecepcionistaByCpfAsync(cpf);

            // Assert
            Assert.Null(result);
        }
    }
}