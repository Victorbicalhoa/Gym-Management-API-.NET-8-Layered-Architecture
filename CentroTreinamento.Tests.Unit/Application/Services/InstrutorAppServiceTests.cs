// CentroTreinamento.Tests.Unit/Application/Services/InstrutorAppServiceTests.cs
using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentroTreinamento.Application.DTOs.Instrutor;
using CentroTreinamento.Application.Services;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums;
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Application.Interfaces; // Para IPasswordHasher

namespace CentroTreinamento.Tests.Unit.Application.Services
{
    public class InstrutorAppServiceTests
    {
        private readonly Mock<IInstrutorRepository> _mockInstrutorRepository;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly InstrutorAppService _instrutorAppService;

        public InstrutorAppServiceTests()
        {
            _mockInstrutorRepository = new Mock<IInstrutorRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _instrutorAppService = new InstrutorAppService(_mockInstrutorRepository.Object, _mockPasswordHasher.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnInstrutorViewModel_WhenInstrutorExists()
        {
            // Arrange
            var instrutorId = Guid.NewGuid();
            var instrutor = new Instrutor(instrutorId, "Instrutor Teste", "12345678900", "hash", StatusInstrutor.Ativo, "CREF123", UserRole.Instrutor);
            _mockInstrutorRepository.Setup(repo => repo.GetByIdAsync(instrutorId)).ReturnsAsync(instrutor);

            // Act
            var result = await _instrutorAppService.GetByIdAsync(instrutorId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(instrutorId, result.Id);
            Assert.Equal(instrutor.Nome, result.Nome);
            _mockInstrutorRepository.Verify(repo => repo.GetByIdAsync(instrutorId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowApplicationException_WhenInstrutorDoesNotExist()
        {
            // Arrange
            var instrutorId = Guid.NewGuid();
            _mockInstrutorRepository.Setup(repo => repo.GetByIdAsync(instrutorId)).ReturnsAsync((Instrutor)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _instrutorAppService.GetByIdAsync(instrutorId));
            Assert.Contains($"Instrutor com ID {instrutorId} não encontrado.", exception.Message);
            _mockInstrutorRepository.Verify(repo => repo.GetByIdAsync(instrutorId), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllInstrutores()
        {
            // Arrange
            var instrutores = new List<Instrutor>
            {
                new Instrutor(Guid.NewGuid(), "Instrutor 1", "11122233344", "hash1", StatusInstrutor.Ativo, "CREF1", UserRole.Instrutor),
                new Instrutor(Guid.NewGuid(), "Instrutor 2", "22233344455", "hash2", StatusInstrutor.Inativo, "CREF2", UserRole.Instrutor)
            };
            _mockInstrutorRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(instrutores);

            // Act
            var result = await _instrutorAppService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockInstrutorRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateInstrutorAsync_ShouldCreateNewInstrutorAndReturnViewModel()
        {
            // Arrange
            var inputModel = new InstrutorInputModel
            {
                Nome = "Novo Instrutor",
                Cpf = "99988877766",
                Senha = "senhaInstrutor123",
                Cref = "CREF999",
                Status = StatusInstrutor.Ativo
            };
            var hashedPassword = "hashedPasswordInstrutor";

            _mockInstrutorRepository.Setup(repo => repo.GetByCpfAsync(inputModel.Cpf!)).ReturnsAsync((Instrutor)null!);
            _mockInstrutorRepository.Setup(repo => repo.GetByCrefAsync(inputModel.Cref!)).ReturnsAsync((Instrutor)null!);
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(inputModel.Senha!)).Returns(hashedPassword);
            _mockInstrutorRepository.Setup(repo => repo.AddAsync(It.IsAny<Instrutor>())).Returns(Task.CompletedTask);
            _mockInstrutorRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _instrutorAppService.CreateInstrutorAsync(inputModel);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(inputModel.Nome, result.Nome);
            Assert.Equal(inputModel.Cpf, result.Cpf);
            Assert.Equal(inputModel.Cref, result.Cref);
            Assert.Equal(StatusInstrutor.Ativo, result.Status);
            Assert.Equal(UserRole.Instrutor, result.Role);

            _mockInstrutorRepository.Verify(repo => repo.GetByCpfAsync(inputModel.Cpf!), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.GetByCrefAsync(inputModel.Cref!), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(inputModel.Senha!), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.AddAsync(It.Is<Instrutor>(i =>
                i.Nome == inputModel.Nome &&
                i.Cpf == inputModel.Cpf &&
                i.SenhaHash == hashedPassword &&
                i.Cref == inputModel.Cref
            )), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateInstrutorAsync_ShouldThrowArgumentException_WhenPasswordIsMissing()
        {
            // Arrange
            var inputModel = new InstrutorInputModel
            {
                Nome = "Novo Instrutor",
                Cpf = "99988877766",
                Senha = null, // Senha faltando
                Cref = "CREF999"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _instrutorAppService.CreateInstrutorAsync(inputModel));
            Assert.Contains("A senha é obrigatória para a criação de um instrutor.", exception.Message);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<string>()), Times.Never);
            _mockInstrutorRepository.Verify(repo => repo.AddAsync(It.IsAny<Instrutor>()), Times.Never);
            _mockInstrutorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateInstrutorAsync_ShouldThrowArgumentException_WhenCpfAlreadyExists()
        {
            // Arrange
            var inputModel = new InstrutorInputModel
            {
                Nome = "Novo Instrutor",
                Cpf = "99988877766",
                Senha = "senhaInstrutor123",
                Cref = "CREF999"
            };
            var existingInstrutor = new Instrutor(Guid.NewGuid(), "Existing", "99988877766", "hash", StatusInstrutor.Ativo, "CREF000", UserRole.Instrutor);

            _mockInstrutorRepository.Setup(repo => repo.GetByCpfAsync(inputModel.Cpf!)).ReturnsAsync(existingInstrutor);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _instrutorAppService.CreateInstrutorAsync(inputModel));
            Assert.Contains($"Já existe um instrutor com o CPF '{inputModel.Cpf}'.", exception.Message);
            _mockInstrutorRepository.Verify(repo => repo.GetByCpfAsync(inputModel.Cpf!), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.GetByCrefAsync(It.IsAny<string>()), Times.Never); // Não deve chamar CREF
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<string>()), Times.Never);
            _mockInstrutorRepository.Verify(repo => repo.AddAsync(It.IsAny<Instrutor>()), Times.Never);
        }

        [Fact]
        public async Task CreateInstrutorAsync_ShouldThrowArgumentException_WhenCrefAlreadyExists()
        {
            // Arrange
            var inputModel = new InstrutorInputModel
            {
                Nome = "Novo Instrutor",
                Cpf = "99988877766",
                Senha = "senhaInstrutor123",
                Cref = "CREF999"
            };
            var existingInstrutor = new Instrutor(Guid.NewGuid(), "Existing", "00011122233", "hash", StatusInstrutor.Ativo, "CREF999", UserRole.Instrutor);

            _mockInstrutorRepository.Setup(repo => repo.GetByCpfAsync(inputModel.Cpf!)).ReturnsAsync((Instrutor)null!); // CPF é único
            _mockInstrutorRepository.Setup(repo => repo.GetByCrefAsync(inputModel.Cref!)).ReturnsAsync(existingInstrutor);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _instrutorAppService.CreateInstrutorAsync(inputModel));
            Assert.Contains($"Já existe um instrutor com o CREF '{inputModel.Cref}'.", exception.Message);
            _mockInstrutorRepository.Verify(repo => repo.GetByCpfAsync(inputModel.Cpf!), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.GetByCrefAsync(inputModel.Cref!), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<string>()), Times.Never);
            _mockInstrutorRepository.Verify(repo => repo.AddAsync(It.IsAny<Instrutor>()), Times.Never);
        }

        [Fact]
        public async Task UpdateInstrutorAsync_ShouldUpdateExistingInstrutor()
        {
            // Arrange
            var instrutorId = Guid.NewGuid();
            var existingInstrutor = new Instrutor(instrutorId, "Old Name", "11122233344", "oldHash", StatusInstrutor.Ativo, "OLDCREF", UserRole.Instrutor);
            var updatedInput = new InstrutorInputModel
            {
                Nome = "New Name",
                Cpf = "99988877766",
                Senha = "newPassword",
                Cref = "NEWCREF",
                Status = StatusInstrutor.Inativo
            };
            var newHashedPassword = "newHashedPassword";

            _mockInstrutorRepository.Setup(repo => repo.GetByIdAsync(instrutorId)).ReturnsAsync(existingInstrutor);
            _mockInstrutorRepository.Setup(repo => repo.GetByCpfAsync(updatedInput.Cpf!)).ReturnsAsync((Instrutor)null!); // CPF não existe para outro
            _mockInstrutorRepository.Setup(repo => repo.GetByCrefAsync(updatedInput.Cref!)).ReturnsAsync((Instrutor)null!); // CREF não existe para outro
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(updatedInput.Senha!)).Returns(newHashedPassword);
            _mockInstrutorRepository.Setup(repo => repo.Update(It.IsAny<Instrutor>()));
            _mockInstrutorRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _instrutorAppService.UpdateInstrutorAsync(instrutorId, updatedInput);

            // Assert
            Assert.Equal(updatedInput.Nome, existingInstrutor.Nome);
            Assert.Equal(updatedInput.Cpf, existingInstrutor.Cpf);
            Assert.Equal(newHashedPassword, existingInstrutor.SenhaHash);
            Assert.Equal(updatedInput.Cref, existingInstrutor.Cref);
            Assert.Equal(updatedInput.Status, existingInstrutor.Status);

            _mockInstrutorRepository.Verify(repo => repo.GetByIdAsync(instrutorId), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.GetByCpfAsync(updatedInput.Cpf!), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.GetByCrefAsync(updatedInput.Cref!), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(updatedInput.Senha!), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.Update(existingInstrutor), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateInstrutorAsync_ShouldNotUpdatePassword_WhenSenhaIsNull()
        {
            // Arrange
            var instrutorId = Guid.NewGuid();
            var originalHash = "originalHash";
            var existingInstrutor = new Instrutor(instrutorId, "Old Name", "11122233344", originalHash, StatusInstrutor.Ativo, "OLDCREF", UserRole.Instrutor);
            var updatedInput = new InstrutorInputModel
            {
                Nome = "New Name",
                Cpf = "99988877766",
                Senha = null, // Senha é nula
                Cref = "NEWCREF",
                Status = StatusInstrutor.Inativo
            };

            _mockInstrutorRepository.Setup(repo => repo.GetByIdAsync(instrutorId)).ReturnsAsync(existingInstrutor);
            _mockInstrutorRepository.Setup(repo => repo.GetByCpfAsync(updatedInput.Cpf!)).ReturnsAsync((Instrutor)null!);
            _mockInstrutorRepository.Setup(repo => repo.GetByCrefAsync(updatedInput.Cref!)).ReturnsAsync((Instrutor)null!);
            _mockInstrutorRepository.Setup(repo => repo.Update(It.IsAny<Instrutor>()));
            _mockInstrutorRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _instrutorAppService.UpdateInstrutorAsync(instrutorId, updatedInput);

            // Assert
            Assert.Equal(originalHash, existingInstrutor.SenhaHash); // SenhaHash deve permanecer inalterada
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<string>()), Times.Never); // HashPassword não deve ser chamado
            _mockInstrutorRepository.Verify(repo => repo.Update(existingInstrutor), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateInstrutorAsync_ShouldUpdateOnlyNameAndCref_WhenCpfAndSenhaAreNull()
        {
            // Arrange
            var instrutorId = Guid.NewGuid();
            var originalCpf = "11122233344";
            var originalHash = "oldHash";
            var existingInstrutor = new Instrutor(instrutorId, "Old Name", originalCpf, originalHash, StatusInstrutor.Ativo, "OLDCREF", UserRole.Instrutor);
            var updatedInput = new InstrutorInputModel
            {
                Nome = "New Name",
                Cpf = null, // CPF nulo
                Senha = null, // Senha nula
                Cref = "NEWCREF",
                Status = StatusInstrutor.Inativo
            };

            _mockInstrutorRepository.Setup(repo => repo.GetByIdAsync(instrutorId)).ReturnsAsync(existingInstrutor);
            _mockInstrutorRepository.Setup(repo => repo.GetByCrefAsync(updatedInput.Cref!)).ReturnsAsync((Instrutor)null!);
            _mockInstrutorRepository.Setup(repo => repo.Update(It.IsAny<Instrutor>()));
            _mockInstrutorRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _instrutorAppService.UpdateInstrutorAsync(instrutorId, updatedInput);

            // Assert
            Assert.Equal(updatedInput.Nome, existingInstrutor.Nome);
            Assert.Equal(originalCpf, existingInstrutor.Cpf); // CPF não deve mudar
            Assert.Equal(originalHash, existingInstrutor.SenhaHash); // Senha não deve mudar
            Assert.Equal(updatedInput.Cref, existingInstrutor.Cref);
            Assert.Equal(updatedInput.Status, existingInstrutor.Status);

            _mockInstrutorRepository.Verify(repo => repo.GetByIdAsync(instrutorId), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.GetByCpfAsync(It.IsAny<string>()), Times.Never); // Não verifica CPF se é nulo
            _mockInstrutorRepository.Verify(repo => repo.GetByCrefAsync(updatedInput.Cref!), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<string>()), Times.Never);
            _mockInstrutorRepository.Verify(repo => repo.Update(existingInstrutor), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateInstrutorAsync_ShouldThrowApplicationException_WhenInstrutorDoesNotExist()
        {
            // Arrange
            var instrutorId = Guid.NewGuid();
            var inputModel = new InstrutorInputModel { Nome = "Nome", Cpf = "123", Senha = "123", Cref = "Cref" };
            _mockInstrutorRepository.Setup(repo => repo.GetByIdAsync(instrutorId)).ReturnsAsync((Instrutor)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _instrutorAppService.UpdateInstrutorAsync(instrutorId, inputModel));
            Assert.Contains($"Instrutor com ID {instrutorId} não encontrado para atualização.", exception.Message);
            _mockInstrutorRepository.Verify(repo => repo.GetByIdAsync(instrutorId), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.Update(It.IsAny<Instrutor>()), Times.Never);
            _mockInstrutorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateInstrutorAsync_ShouldThrowArgumentException_WhenUpdatedCpfAlreadyExistsForAnotherInstrutor()
        {
            // Arrange
            var instrutorId = Guid.NewGuid();
            var existingInstrutor = new Instrutor(instrutorId, "Original Name", "11122233344", "hash", StatusInstrutor.Ativo, "ORIGINALCREF", UserRole.Instrutor);
            var inputModel = new InstrutorInputModel
            {
                Nome = "Updated Name",
                Cpf = "99988877766", // Este CPF já existe para outro instrutor
                Senha = "newPassword",
                Cref = "UPDATEDCREF"
            };
            var anotherInstrutor = new Instrutor(Guid.NewGuid(), "Another Instrutor", "99988877766", "anotherHash", StatusInstrutor.Ativo, "ANOTHERCREF", UserRole.Instrutor);

            _mockInstrutorRepository.Setup(repo => repo.GetByIdAsync(instrutorId)).ReturnsAsync(existingInstrutor);
            _mockInstrutorRepository.Setup(repo => repo.GetByCpfAsync(inputModel.Cpf!)).ReturnsAsync(anotherInstrutor); // CPF já existe

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _instrutorAppService.UpdateInstrutorAsync(instrutorId, inputModel));
            Assert.Contains($"Já existe outro instrutor com o CPF '{inputModel.Cpf}'.", exception.Message);
            _mockInstrutorRepository.Verify(repo => repo.GetByIdAsync(instrutorId), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.GetByCpfAsync(inputModel.Cpf!), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.Update(It.IsAny<Instrutor>()), Times.Never);
            _mockInstrutorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateInstrutorAsync_ShouldThrowArgumentException_WhenUpdatedCrefAlreadyExistsForAnotherInstrutor()
        {
            // Arrange
            var instrutorId = Guid.NewGuid();
            var existingInstrutor = new Instrutor(instrutorId, "Original Name", "11122233344", "hash", StatusInstrutor.Ativo, "ORIGINALCREF", UserRole.Instrutor);
            var inputModel = new InstrutorInputModel
            {
                Nome = "Updated Name",
                Cpf = "00011122233",
                Senha = "newPassword",
                Cref = "DUPLICATECREF" // Este CREF já existe para outro instrutor
            };
            var anotherInstrutor = new Instrutor(Guid.NewGuid(), "Another Instrutor", "99988877766", "anotherHash", StatusInstrutor.Ativo, "DUPLICATECREF", UserRole.Instrutor);

            _mockInstrutorRepository.Setup(repo => repo.GetByIdAsync(instrutorId)).ReturnsAsync(existingInstrutor);
            _mockInstrutorRepository.Setup(repo => repo.GetByCpfAsync(inputModel.Cpf!)).ReturnsAsync((Instrutor)null!); // CPF é único
            _mockInstrutorRepository.Setup(repo => repo.GetByCrefAsync(inputModel.Cref!)).ReturnsAsync(anotherInstrutor); // CREF já existe

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _instrutorAppService.UpdateInstrutorAsync(instrutorId, inputModel));
            Assert.Contains($"Já existe outro instrutor com o CREF '{inputModel.Cref}'.", exception.Message);
            _mockInstrutorRepository.Verify(repo => repo.GetByIdAsync(instrutorId), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.GetByCpfAsync(inputModel.Cpf!), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.GetByCrefAsync(inputModel.Cref!), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.Update(It.IsAny<Instrutor>()), Times.Never);
            _mockInstrutorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteInstrutorAsync_ShouldDeleteInstrutor_WhenInstrutorExists()
        {
            // Arrange
            var instrutorId = Guid.NewGuid();
            var instrutorToDelete = new Instrutor(instrutorId, "Delete Me", "123", "hash", StatusInstrutor.Ativo, "CREFDELETE", UserRole.Instrutor);
            _mockInstrutorRepository.Setup(repo => repo.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorToDelete);
            _mockInstrutorRepository.Setup(repo => repo.Delete(instrutorToDelete));
            _mockInstrutorRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _instrutorAppService.DeleteInstrutorAsync(instrutorId);

            // Assert
            _mockInstrutorRepository.Verify(repo => repo.GetByIdAsync(instrutorId), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.Delete(instrutorToDelete), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteInstrutorAsync_ShouldThrowApplicationException_WhenInstrutorDoesNotExist()
        {
            // Arrange
            var instrutorId = Guid.NewGuid();
            _mockInstrutorRepository.Setup(repo => repo.GetByIdAsync(instrutorId)).ReturnsAsync((Instrutor)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _instrutorAppService.DeleteInstrutorAsync(instrutorId));
            Assert.Contains($"Instrutor com ID {instrutorId} não encontrado para exclusão.", exception.Message);
            _mockInstrutorRepository.Verify(repo => repo.GetByIdAsync(instrutorId), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.Delete(It.IsAny<Instrutor>()), Times.Never);
            _mockInstrutorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateInstrutorStatusAsync_ShouldUpdateStatus_WhenInstrutorExists()
        {
            // Arrange
            var instrutorId = Guid.NewGuid();
            var instrutor = new Instrutor(instrutorId, "Status Changer", "123", "hash", StatusInstrutor.Ativo, "CREFSTAT", UserRole.Instrutor);
            var novoStatus = StatusInstrutor.Afastado;
            _mockInstrutorRepository.Setup(repo => repo.GetByIdAsync(instrutorId)).ReturnsAsync(instrutor);
            _mockInstrutorRepository.Setup(repo => repo.Update(It.IsAny<Instrutor>()));
            _mockInstrutorRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _instrutorAppService.UpdateInstrutorStatusAsync(instrutorId, novoStatus);

            // Assert
            Assert.Equal(novoStatus, instrutor.Status);
            _mockInstrutorRepository.Verify(repo => repo.GetByIdAsync(instrutorId), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.Update(instrutor), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateInstrutorStatusAsync_ShouldThrowApplicationException_WhenInstrutorDoesNotExist()
        {
            // Arrange
            var instrutorId = Guid.NewGuid();
            _mockInstrutorRepository.Setup(repo => repo.GetByIdAsync(instrutorId)).ReturnsAsync((Instrutor)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _instrutorAppService.UpdateInstrutorStatusAsync(instrutorId, StatusInstrutor.Ferias));
            Assert.Contains($"Instrutor com ID {instrutorId} não encontrado para atualização de status.", exception.Message);
            _mockInstrutorRepository.Verify(repo => repo.GetByIdAsync(instrutorId), Times.Once);
            _mockInstrutorRepository.Verify(repo => repo.Update(It.IsAny<Instrutor>()), Times.Never);
            _mockInstrutorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetByCrefAsync_ShouldReturnInstrutorViewModel_WhenInstrutorExists()
        {
            // Arrange
            var cref = "CREFTESTE";
            var instrutor = new Instrutor(Guid.NewGuid(), "Instrutor Cref", "12345678900", "hash", StatusInstrutor.Ativo, cref, UserRole.Instrutor);
            _mockInstrutorRepository.Setup(repo => repo.GetByCrefAsync(cref)).ReturnsAsync(instrutor);

            // Act
            var result = await _instrutorAppService.GetByCrefAsync(cref);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cref, result.Cref);
            _mockInstrutorRepository.Verify(repo => repo.GetByCrefAsync(cref), Times.Once);
        }

        [Fact]
        public async Task GetByCrefAsync_ShouldThrowApplicationException_WhenInstrutorDoesNotExist()
        {
            // Arrange
            var cref = "CREFNOTFOUND";
            _mockInstrutorRepository.Setup(repo => repo.GetByCrefAsync(cref)).ReturnsAsync((Instrutor)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _instrutorAppService.GetByCrefAsync(cref));
            Assert.Contains($"Instrutor com CREF '{cref}' não encontrado.", exception.Message);
            _mockInstrutorRepository.Verify(repo => repo.GetByCrefAsync(cref), Times.Once);
        }

        [Fact]
        public async Task GetByCpfAsync_ShouldReturnInstrutorViewModel_WhenInstrutorExists()
        {
            // Arrange
            var cpf = "12345678900";
            var instrutor = new Instrutor(Guid.NewGuid(), "Instrutor CPF", cpf, "hash", StatusInstrutor.Ativo, "CREFCPF", UserRole.Instrutor);
            _mockInstrutorRepository.Setup(repo => repo.GetByCpfAsync(cpf)).ReturnsAsync(instrutor);

            // Act
            var result = await _instrutorAppService.GetByCpfAsync(cpf);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cpf, result.Cpf);
            _mockInstrutorRepository.Verify(repo => repo.GetByCpfAsync(cpf), Times.Once);
        }

        [Fact]
        public async Task GetByCpfAsync_ShouldThrowApplicationException_WhenInstrutorDoesNotExist()
        {
            // Arrange
            var cpf = "99999999999";
            _mockInstrutorRepository.Setup(repo => repo.GetByCpfAsync(cpf)).ReturnsAsync((Instrutor)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _instrutorAppService.GetByCpfAsync(cpf));
            Assert.Contains($"Instrutor com CPF '{cpf}' não encontrado.", exception.Message);
            _mockInstrutorRepository.Verify(repo => repo.GetByCpfAsync(cpf), Times.Once);
        }

        [Fact]
        public async Task GetAvailableInstrutoresAsync_ShouldReturnAvailableInstrutores()
        {
            // Arrange
            var data = new DateTime(2025, 7, 5);
            var horario = new TimeSpan(10, 0, 0);
            var instrutoresDisponiveis = new List<Instrutor>
            {
                new Instrutor(Guid.NewGuid(), "Disponivel 1", "111", "h1", StatusInstrutor.Ativo, "C1", UserRole.Instrutor),
                new Instrutor(Guid.NewGuid(), "Disponivel 2", "222", "h2", StatusInstrutor.Ativo, "C2", UserRole.Instrutor)
            };

            _mockInstrutorRepository.Setup(repo => repo.GetInstrutoresDisponiveisAsync(data, horario)).ReturnsAsync(instrutoresDisponiveis);

            // Act
            var result = await _instrutorAppService.GetAvailableInstrutoresAsync(data, horario);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockInstrutorRepository.Verify(repo => repo.GetInstrutoresDisponiveisAsync(data, horario), Times.Once);
            Assert.True(result.All(i => i.Status == StatusInstrutor.Ativo)); // Confirma que apenas ativos são retornados pela query do repo
        }
    }
}