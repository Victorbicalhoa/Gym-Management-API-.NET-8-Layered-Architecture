// CentroTreinamento.Tests.Unit/Application/Services/AdministradorAppServiceTests.cs
using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentroTreinamento.Application.DTOs.Administrador;
using CentroTreinamento.Application.Services;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums;
using CentroTreinamento.Domain.Repositories; // Para IAdministradorRepository
using CentroTreinamento.Application.Interfaces; // Para IPasswordHasher

namespace CentroTreinamento.Tests.Unit.Application.Services
{
    public class AdministradorAppServiceTests
    {
        private readonly Mock<IAdministradorRepository> _mockAdministradorRepository;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly AdministradorAppService _administradorAppService;

        public AdministradorAppServiceTests()
        {
            _mockAdministradorRepository = new Mock<IAdministradorRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _administradorAppService = new AdministradorAppService(_mockAdministradorRepository.Object, _mockPasswordHasher.Object);
        }

        [Fact]
        public async Task GetAllAdministradoresAsync_ShouldReturnAllAdministradores()
        {
            // Arrange
            var administradores = new List<Administrador>
            {
                new Administrador(Guid.NewGuid(), "Admin 1", "hash1", StatusAdministrador.Ativo, UserRole.Administrador, "11122233344"),
                new Administrador(Guid.NewGuid(), "Admin 2", "hash2", StatusAdministrador.Inativo, UserRole.Administrador, "22233344455")
            };
            _mockAdministradorRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(administradores);

            // Act
            var result = await _administradorAppService.GetAllAdministradoresAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockAdministradorRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAdministradorByIdAsync_ShouldReturnAdministrador_WhenAdministradorExists()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var administrador = new Administrador(adminId, "Admin Teste", "hash", StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");
            _mockAdministradorRepository.Setup(repo => repo.GetByIdAsync(adminId)).ReturnsAsync(administrador);

            // Act
            var result = await _administradorAppService.GetAdministradorByIdAsync(adminId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(adminId, result.Id);
            Assert.Equal(administrador.Nome, result.Nome);
            _mockAdministradorRepository.Verify(repo => repo.GetByIdAsync(adminId), Times.Once);
        }

        [Fact]
        public async Task GetAdministradorByIdAsync_ShouldReturnNull_WhenAdministradorDoesNotExist()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            _mockAdministradorRepository.Setup(repo => repo.GetByIdAsync(adminId)).ReturnsAsync((Administrador)null!);

            // Act
            var result = await _administradorAppService.GetAdministradorByIdAsync(adminId);

            // Assert
            Assert.Null(result);
            _mockAdministradorRepository.Verify(repo => repo.GetByIdAsync(adminId), Times.Once);
        }

        [Fact]
        public async Task CreateAdministradorAsync_ShouldCreateNewAdministradorAndReturnViewModel()
        {
            // Arrange
            var inputModel = new AdministradorInputModel
            {
                Nome = "Novo Admin",
                Cpf = "999.888.777-66",
                Senha = "senhaAdmin123"
            };
            var hashedPassword = "hashedPasswordAdmin";

            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(inputModel.Senha!)).Returns(hashedPassword);
            _mockAdministradorRepository.Setup(repo => repo.AddAsync(It.IsAny<Administrador>())).Returns(Task.CompletedTask);
            _mockAdministradorRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _administradorAppService.CreateAdministradorAsync(inputModel);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(inputModel.Nome, result.Nome);
            Assert.Equal(inputModel.Cpf!.Replace(".", "").Replace("-", ""), result.Cpf); // Verifica o CPF normalizado
            Assert.Equal(StatusAdministrador.Ativo, result.Status); // Status padrão deve ser Ativo
            Assert.Equal(UserRole.Administrador, result.Role); // Role padrão deve ser Administrador
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(inputModel.Senha!), Times.Once);
            _mockAdministradorRepository.Verify(repo => repo.AddAsync(It.Is<Administrador>(a =>
                a.Nome == inputModel.Nome &&
                a.SenhaHash == hashedPassword &&
                a.Cpf == inputModel.Cpf!.Replace(".", "").Replace("-", "")
            )), Times.Once);
            _mockAdministradorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAdministradorAsync_ShouldUpdateExistingAdministradorAndReturnTrue_WhenAdministradorExists()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var existingAdministrador = new Administrador(adminId, "Nome Antigo", "oldHash", StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");
            var updatedInput = new AdministradorInputModel
            {
                Nome = "Nome Novo",
                Cpf = "000.999.888-77",
                Senha = "novaSenhaAdmin123"
            };
            var newHashedPassword = "newHashedPasswordAdmin";

            _mockAdministradorRepository.Setup(repo => repo.GetByIdAsync(adminId)).ReturnsAsync(existingAdministrador);
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(updatedInput.Senha!)).Returns(newHashedPassword);
            _mockAdministradorRepository.Setup(repo => repo.Update(It.IsAny<Administrador>()));
            _mockAdministradorRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _administradorAppService.UpdateAdministradorAsync(adminId, updatedInput);

            // Assert
            Assert.True(result);
            Assert.Equal(updatedInput.Nome, existingAdministrador.Nome); // Verifica se a entidade mockada foi atualizada
            Assert.Equal(updatedInput.Cpf!.Replace(".", "").Replace("-", ""), existingAdministrador.Cpf);
            Assert.Equal(newHashedPassword, existingAdministrador.SenhaHash);

            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(updatedInput.Senha!), Times.Once);
            _mockAdministradorRepository.Verify(repo => repo.Update(It.Is<Administrador>(a =>
                a.Id == adminId &&
                a.Nome == updatedInput.Nome &&
                a.SenhaHash == newHashedPassword &&
                a.Cpf == updatedInput.Cpf!.Replace(".", "").Replace("-", "")
            )), Times.Once);
            _mockAdministradorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAdministradorAsync_ShouldNotUpdatePassword_WhenSenhaIsNullInInput()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var originalPasswordHash = "originalHashAdmin";
            var existingAdministrador = new Administrador(adminId, "Nome Antigo", originalPasswordHash, StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");
            var updatedInput = new AdministradorInputModel
            {
                Nome = "Nome Novo",
                Cpf = "000.999.888-77",
                Senha = null // Senha é null
            };

            _mockAdministradorRepository.Setup(repo => repo.GetByIdAsync(adminId)).ReturnsAsync(existingAdministrador);
            _mockAdministradorRepository.Setup(repo => repo.Update(It.IsAny<Administrador>()));
            _mockAdministradorRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _administradorAppService.UpdateAdministradorAsync(adminId, updatedInput);

            // Assert
            Assert.True(result);
            Assert.Equal(originalPasswordHash, existingAdministrador.SenhaHash); // Senha deve permanecer a original
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<string>()), Times.Never); // HashPassword não deve ser chamado
            _mockAdministradorRepository.Verify(repo => repo.Update(It.IsAny<Administrador>()), Times.Once);
            _mockAdministradorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAdministradorAsync_ShouldReturnFalse_WhenAdministradorDoesNotExist()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var inputModel = new AdministradorInputModel { Nome = "Nome", Cpf = "123", Senha = "123" };
            _mockAdministradorRepository.Setup(repo => repo.GetByIdAsync(adminId)).ReturnsAsync((Administrador)null!);

            // Act
            var result = await _administradorAppService.UpdateAdministradorAsync(adminId, inputModel);

            // Assert
            Assert.False(result);
            _mockAdministradorRepository.Verify(repo => repo.GetByIdAsync(adminId), Times.Once);
            _mockAdministradorRepository.Verify(repo => repo.Update(It.IsAny<Administrador>()), Times.Never);
            _mockAdministradorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAdministradorStatusAsync_ShouldUpdateStatusAndReturnTrue_WhenAdministradorExists()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var existingAdministrador = new Administrador(adminId, "Admin Status", "hash", StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");
            var novoStatus = StatusAdministrador.Inativo;

            _mockAdministradorRepository.Setup(repo => repo.GetByIdAsync(adminId)).ReturnsAsync(existingAdministrador);
            _mockAdministradorRepository.Setup(repo => repo.Update(It.IsAny<Administrador>()));
            _mockAdministradorRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _administradorAppService.UpdateAdministradorStatusAsync(adminId, novoStatus);

            // Assert
            Assert.True(result);
            Assert.Equal(novoStatus, existingAdministrador.Status); // Verifica se o status da entidade foi atualizado
            _mockAdministradorRepository.Verify(repo => repo.GetByIdAsync(adminId), Times.Once);
            _mockAdministradorRepository.Verify(repo => repo.Update(It.Is<Administrador>(a => a.Id == adminId && a.Status == novoStatus)), Times.Once);
            _mockAdministradorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAdministradorStatusAsync_ShouldReturnFalse_WhenAdministradorDoesNotExist()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            _mockAdministradorRepository.Setup(repo => repo.GetByIdAsync(adminId)).ReturnsAsync((Administrador)null!);

            // Act
            var result = await _administradorAppService.UpdateAdministradorStatusAsync(adminId, StatusAdministrador.Inativo);

            // Assert
            Assert.False(result);
            _mockAdministradorRepository.Verify(repo => repo.GetByIdAsync(adminId), Times.Once);
            _mockAdministradorRepository.Verify(repo => repo.Update(It.IsAny<Administrador>()), Times.Never);
            _mockAdministradorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteAdministradorAsync_ShouldReturnTrue_WhenAdministradorExists()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var adminToDelete = new Administrador(adminId, "Admin Delete", "hash", StatusAdministrador.Ativo, UserRole.Administrador, "12345678900");

            _mockAdministradorRepository.Setup(repo => repo.GetByIdAsync(adminId)).ReturnsAsync(adminToDelete);
            _mockAdministradorRepository.Setup(repo => repo.Delete(adminToDelete));
            _mockAdministradorRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _administradorAppService.DeleteAdministradorAsync(adminId);

            // Assert
            Assert.True(result);
            _mockAdministradorRepository.Verify(repo => repo.GetByIdAsync(adminId), Times.Once);
            _mockAdministradorRepository.Verify(repo => repo.Delete(adminToDelete), Times.Once);
            _mockAdministradorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAdministradorAsync_ShouldReturnFalse_WhenAdministradorDoesNotExist()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            _mockAdministradorRepository.Setup(repo => repo.GetByIdAsync(adminId)).ReturnsAsync((Administrador)null!);

            // Act
            var result = await _administradorAppService.DeleteAdministradorAsync(adminId);

            // Assert
            Assert.False(result);
            _mockAdministradorRepository.Verify(repo => repo.GetByIdAsync(adminId), Times.Once);
            _mockAdministradorRepository.Verify(repo => repo.Delete(It.IsAny<Administrador>()), Times.Never);
            _mockAdministradorRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task LoginAdministradorAsync_ShouldReturnViewModel_WhenCredentialsAreValid()
        {
            // Arrange
            var cpf = "123.456.789-00";
            var senha = "senhaCorreta";
            var cpfNormalizado = "12345678900";
            var hashedPassword = "hashedPasswordAdmin";
            var administrador = new Administrador(Guid.NewGuid(), "Admin Login", hashedPassword, StatusAdministrador.Ativo, UserRole.Administrador, cpfNormalizado);

            _mockAdministradorRepository.Setup(repo => repo.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Administrador, bool>>>()))
                                .ReturnsAsync((System.Linq.Expressions.Expression<Func<Administrador, bool>> predicate) =>
                                {
                                    var compiledPredicate = predicate.Compile();
                                    return new List<Administrador> { administrador }.Where(compiledPredicate).ToList();
                                });

            _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword(senha, hashedPassword)).Returns(true);

            // Act
            var result = await _administradorAppService.LoginAdministradorAsync(cpf, senha);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(administrador.Id, result.Id);
            Assert.Equal(administrador.Nome, result.Nome);
            Assert.Equal(administrador.Cpf, result.Cpf);
            _mockAdministradorRepository.Verify(repo => repo.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Administrador, bool>>>()), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyPassword(senha, hashedPassword), Times.Once);
        }

        [Fact]
        public async Task LoginAdministradorAsync_ShouldReturnNull_WhenAdministradorNotFound()
        {
            // Arrange
            var cpf = "123.456.789-00";
            var senha = "senhaCorreta";

            _mockAdministradorRepository.Setup(repo => repo.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Administrador, bool>>>()))
                                .ReturnsAsync(new List<Administrador>()); // Retorna lista vazia

            // Act
            var result = await _administradorAppService.LoginAdministradorAsync(cpf, senha);

            // Assert
            Assert.Null(result);
            _mockAdministradorRepository.Verify(repo => repo.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Administrador, bool>>>()), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginAdministradorAsync_ShouldReturnNull_WhenPasswordIsInvalid()
        {
            // Arrange
            var cpf = "123.456.789-00";
            var senhaIncorreta = "senhaIncorreta";
            var cpfNormalizado = "12345678900";
            var hashedPassword = "hashedPasswordAdmin";
            var administrador = new Administrador(Guid.NewGuid(), "Admin Login", hashedPassword, StatusAdministrador.Ativo, UserRole.Administrador, cpfNormalizado);

            _mockAdministradorRepository.Setup(repo => repo.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Administrador, bool>>>()))
                                .ReturnsAsync(new List<Administrador> { administrador });

            _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword(senhaIncorreta, hashedPassword)).Returns(false);

            // Act
            var result = await _administradorAppService.LoginAdministradorAsync(cpf, senhaIncorreta);

            // Assert
            Assert.Null(result);
            _mockAdministradorRepository.Verify(repo => repo.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Administrador, bool>>>()), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyPassword(senhaIncorreta, hashedPassword), Times.Once);
        }
    }
}