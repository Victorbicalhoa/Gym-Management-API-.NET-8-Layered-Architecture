using Xunit;
using Moq;
using AutoMapper;
using CentroTreinamento.Application.Services;
using CentroTreinamento.Application.DTOs.PlanoDeTreino;
using CentroTreinamento.Application.Interfaces.Services;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums;
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Application.Mappers; // Importante para o perfil do AutoMapper
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentroTreinamento.Tests.Unit.Application.Services
{
    public class PlanoDeTreinoAppServiceTests
    {
        private readonly Mock<IRepository<PlanoDeTreino>> _mockPlanoDeTreinoRepository;
        private readonly Mock<IRepository<Aluno>> _mockAlunoRepository;
        private readonly Mock<IRepository<Instrutor>> _mockInstrutorRepository;
        private readonly IMapper _mapper;
        private readonly IPlanoDeTreinoAppService _planoDeTreinoAppService;

        public PlanoDeTreinoAppServiceTests()
        {
            _mockPlanoDeTreinoRepository = new Mock<IRepository<PlanoDeTreino>>();
            _mockAlunoRepository = new Mock<IRepository<Aluno>>();
            _mockInstrutorRepository = new Mock<IRepository<Instrutor>>();

            // Configuração robusta do AutoMapper para testes
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                // Garante que o perfil seja adicionado corretamente
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _planoDeTreinoAppService = new PlanoDeTreinoAppService(
                _mockPlanoDeTreinoRepository.Object,
                _mockAlunoRepository.Object,
                _mockInstrutorRepository.Object,
                _mapper
            );
        }

        // --- Testes para CriarPlanoDeTreinoAsync ---
        [Fact]
        public async Task CriarPlanoDeTreinoAsync_DeveCriarPlano_QuandoDadosValidos()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var inputModel = new PlanoDeTreinoInputModel
            {
                AlunoId = alunoId,
                InstrutorId = instrutorId,
                NomePlano = "Plano Teste 1",
                Descricao = "Descrição do plano de teste",
                DataInicio = DateTime.Today,
                DataFim = DateTime.Today.AddMonths(3),
                StatusPlano = StatusPlano.Ativo
            };

            // Mocks de Aluno e Instrutor usando seus CONSTRUTORES REAIS
            var alunoMock = new Aluno(
                alunoId,
                "Aluno Teste",
                "senhaHashAluno123", // SenhaHash
                StatusAluno.Ativo,
                "12345678900", // CPF
                DateTime.Now.AddYears(-25), // DataNascimento (idade > 0)
                "11987654321", // Telefone
                UserRole.Aluno // Role
            );

            var instrutorMock = new Instrutor(
                instrutorId,
                "Instrutor Teste",
                "98765432100", // CPF (pode ser null, mas fornecemos aqui)
                "senhaHashInstrutor123", // SenhaHash
                StatusInstrutor.Ativo,
                "12345-SP", // CREF
                UserRole.Instrutor // Role
            );

            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock);
            _mockPlanoDeTreinoRepository.Setup(r => r.AddAsync(It.IsAny<PlanoDeTreino>())).Returns(Task.CompletedTask);
            _mockPlanoDeTreinoRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1); // Simula 1 alteração salva

            // Act
            var result = await _planoDeTreinoAppService.CriarPlanoDeTreinoAsync(inputModel);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id); // Verifica se um ID foi gerado
            Assert.Equal(inputModel.AlunoId, result.AlunoId);
            Assert.Equal(inputModel.InstrutorId, result.InstrutorId);
            Assert.Equal(inputModel.NomePlano, result.NomePlano);
            Assert.Equal(inputModel.Descricao, result.Descricao);
            Assert.Equal(inputModel.DataInicio, result.DataInicio);
            Assert.Equal(inputModel.DataFim, result.DataFim);
            Assert.Equal(inputModel.StatusPlano, result.StatusPlano);
            Assert.Equal(alunoMock.Nome, result.NomeAluno); // Nomes preenchidos pelo AppService
            Assert.Equal(instrutorMock.Nome, result.NomeInstrutor);

            // Verifica se os métodos do repositório foram chamados
            _mockPlanoDeTreinoRepository.Verify(r => r.AddAsync(It.IsAny<PlanoDeTreino>()), Times.Once);
            _mockPlanoDeTreinoRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CriarPlanoDeTreinoAsync_DeveLancarExcecao_QuandoAlunoNaoEncontrado()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var inputModel = new PlanoDeTreinoInputModel
            {
                AlunoId = alunoId,
                InstrutorId = instrutorId,
                NomePlano = "Plano Invalido",
                DataInicio = DateTime.Today,
                DataFim = DateTime.Today.AddMonths(1),
                StatusPlano = StatusPlano.Ativo
            };

            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync((Aluno?)null); // Simula aluno não encontrado

            // Mock de instrutor válido para que a exceção seja do aluno
            var instrutorMock = new Instrutor(instrutorId, "Instrutor Valido", null, "hash", StatusInstrutor.Ativo, "CREF", UserRole.Instrutor);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _planoDeTreinoAppService.CriarPlanoDeTreinoAsync(inputModel));

            Assert.Contains($"Aluno com ID {inputModel.AlunoId} não encontrado.", exception.Message);
            _mockPlanoDeTreinoRepository.Verify(r => r.AddAsync(It.IsAny<PlanoDeTreino>()), Times.Never); // Verifica que AddAsync não foi chamado
            _mockPlanoDeTreinoRepository.Verify(r => r.SaveChangesAsync(), Times.Never); // Verifica que SaveChangesAsync não foi chamado
        }

        [Fact]
        public async Task CriarPlanoDeTreinoAsync_DeveLancarExcecao_QuandoInstrutorNaoEncontrado()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var inputModel = new PlanoDeTreinoInputModel
            {
                AlunoId = alunoId,
                InstrutorId = instrutorId,
                NomePlano = "Plano Invalido",
                DataInicio = DateTime.Today,
                DataFim = DateTime.Today.AddMonths(1),
                StatusPlano = StatusPlano.Ativo
            };

            // Mock de aluno válido para que a exceção seja do instrutor
            var alunoMock = new Aluno(alunoId, "Aluno Valido", "hash", StatusAluno.Ativo, "123", DateTime.Now.AddYears(-20), "123", UserRole.Aluno);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);

            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync((Instrutor?)null); // Simula instrutor não encontrado

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _planoDeTreinoAppService.CriarPlanoDeTreinoAsync(inputModel));

            Assert.Contains($"Instrutor com ID {inputModel.InstrutorId} não encontrado.", exception.Message);
            _mockPlanoDeTreinoRepository.Verify(r => r.AddAsync(It.IsAny<PlanoDeTreino>()), Times.Never);
            _mockPlanoDeTreinoRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        // --- Testes para AtualizarPlanoDeTreinoAsync ---
        [Fact]
        public async Task AtualizarPlanoDeTreinoAsync_DeveAtualizarPlano_QuandoDadosValidos()
        {
            // Arrange
            var planoId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var existingPlano = new PlanoDeTreino(planoId, alunoId, instrutorId, "Plano Antigo", "Desc Antiga", DateTime.Today.AddDays(-30), DateTime.Today.AddDays(60), StatusPlano.Ativo);

            var updateModel = new PlanoDeTreinoUpdateModel
            {
                AlunoId = alunoId, // Mantendo o mesmo aluno
                InstrutorId = instrutorId, // Mantendo o mesmo instrutor
                NomePlano = "Plano Atualizado",
                Descricao = "Nova Descrição",
                DataInicio = existingPlano.DataInicio, // DataInicio não é atualizada pela entidade
                DataFim = DateTime.Today.AddMonths(6),
                StatusPlano = StatusPlano.Pausado
            };

            // Mocks para resolver os IDs de aluno e instrutor para preencher o ViewModel
            var alunoMock = new Aluno(alunoId, "Aluno Existente", "hash", StatusAluno.Ativo, "123", DateTime.Now.AddYears(-20), "123", UserRole.Aluno);
            var instrutorMock = new Instrutor(instrutorId, "Instrutor Existente", null, "hash", StatusInstrutor.Ativo, "CREF", UserRole.Instrutor);

            _mockPlanoDeTreinoRepository.Setup(r => r.GetByIdAsync(planoId)).ReturnsAsync(existingPlano);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock);
            _mockPlanoDeTreinoRepository.Setup(r => r.Update(It.IsAny<PlanoDeTreino>())).Verifiable(); // Apenas verifica se Update é chamado
            _mockPlanoDeTreinoRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _planoDeTreinoAppService.AtualizarPlanoDeTreinoAsync(planoId, updateModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(planoId, result.Id);
            Assert.Equal(updateModel.NomePlano, result.NomePlano);
            Assert.Equal(updateModel.Descricao, result.Descricao);
            Assert.Equal(updateModel.DataFim, result.DataFim);
            Assert.Equal(updateModel.StatusPlano, result.StatusPlano);
            Assert.Equal(alunoMock.Nome, result.NomeAluno);
            Assert.Equal(instrutorMock.Nome, result.NomeInstrutor);

            // Verifica que o método Update da entidade foi chamado com as propriedades corretas
            _mockPlanoDeTreinoRepository.Verify(r => r.Update(
                It.Is<PlanoDeTreino>(p =>
                    p.Id == planoId &&
                    p.NomePlano == updateModel.NomePlano &&
                    p.Descricao == updateModel.Descricao &&
                    p.DataFim == updateModel.DataFim &&
                    p.Status == updateModel.StatusPlano
                )
            ), Times.Once);
            _mockPlanoDeTreinoRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AtualizarPlanoDeTreinoAsync_DeveLancarExcecao_QuandoPlanoNaoEncontrado()
        {
            // Arrange
            var planoId = Guid.NewGuid();
            var updateModel = new PlanoDeTreinoUpdateModel
            {
                AlunoId = Guid.NewGuid(),
                InstrutorId = Guid.NewGuid(),
                NomePlano = "Teste",
                DataInicio = DateTime.Today,
                DataFim = DateTime.Today.AddDays(1),
                StatusPlano = StatusPlano.Ativo
            };

            _mockPlanoDeTreinoRepository.Setup(r => r.GetByIdAsync(planoId)).ReturnsAsync((PlanoDeTreino?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _planoDeTreinoAppService.AtualizarPlanoDeTreinoAsync(planoId, updateModel));

            Assert.Contains($"Plano de Treino com ID {planoId} não encontrado.", exception.Message);
            _mockPlanoDeTreinoRepository.Verify(r => r.Update(It.IsAny<PlanoDeTreino>()), Times.Never);
            _mockPlanoDeTreinoRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AtualizarPlanoDeTreinoAsync_DeveLancarExcecao_QuandoNovoAlunoNaoEncontrado()
        {
            // Arrange
            var planoId = Guid.NewGuid();
            var alunoIdExistente = Guid.NewGuid();
            var instrutorIdExistente = Guid.NewGuid();
            var newAlunoId = Guid.NewGuid(); // ID de aluno que não existe

            var existingPlano = new PlanoDeTreino(planoId, alunoIdExistente, instrutorIdExistente, "Plano Antigo", "Desc Antiga", DateTime.Today, DateTime.Today.AddMonths(1), StatusPlano.Ativo);

            var updateModel = new PlanoDeTreinoUpdateModel
            {
                AlunoId = newAlunoId, // Mudando o AlunoId para um que não existe
                InstrutorId = instrutorIdExistente,
                NomePlano = "Teste",
                DataInicio = DateTime.Today,
                DataFim = DateTime.Today.AddDays(1),
                StatusPlano = StatusPlano.Ativo
            };

            // Mocks
            _mockPlanoDeTreinoRepository.Setup(r => r.GetByIdAsync(planoId)).ReturnsAsync(existingPlano);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(newAlunoId)).ReturnsAsync((Aluno?)null); // Simula aluno não encontrado

            // Instrutor mockado para que a exceção seja do aluno
            var instrutorMock = new Instrutor(instrutorIdExistente, "Instrutor Mock", null, "hash", StatusInstrutor.Ativo, "CREF", UserRole.Instrutor);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorIdExistente)).ReturnsAsync(instrutorMock);


            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _planoDeTreinoAppService.AtualizarPlanoDeTreinoAsync(planoId, updateModel));

            Assert.Contains($"Novo Aluno com ID {newAlunoId} não encontrado.", exception.Message);
            _mockPlanoDeTreinoRepository.Verify(r => r.Update(It.IsAny<PlanoDeTreino>()), Times.Never);
            _mockPlanoDeTreinoRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AtualizarPlanoDeTreinoAsync_DeveLancarExcecao_QuandoNovoInstrutorNaoEncontrado()
        {
            // Arrange
            var planoId = Guid.NewGuid();
            var alunoIdExistente = Guid.NewGuid();
            var instrutorIdExistente = Guid.NewGuid();
            var newInstrutorId = Guid.NewGuid(); // ID de instrutor que não existe

            var existingPlano = new PlanoDeTreino(planoId, alunoIdExistente, instrutorIdExistente, "Plano Antigo", "Desc Antiga", DateTime.Today, DateTime.Today.AddMonths(1), StatusPlano.Ativo);

            var updateModel = new PlanoDeTreinoUpdateModel
            {
                AlunoId = alunoIdExistente,
                InstrutorId = newInstrutorId, // Mudando o InstrutorId para um que não existe
                NomePlano = "Teste",
                DataInicio = DateTime.Today,
                DataFim = DateTime.Today.AddDays(1),
                StatusPlano = StatusPlano.Ativo
            };

            // Mocks
            _mockPlanoDeTreinoRepository.Setup(r => r.GetByIdAsync(planoId)).ReturnsAsync(existingPlano);

            // Aluno mockado para que a exceção seja do instrutor
            var alunoMock = new Aluno(alunoIdExistente, "Aluno Mock", "hash", StatusAluno.Ativo, "123", DateTime.Now.AddYears(-20), "123", UserRole.Aluno);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoIdExistente)).ReturnsAsync(alunoMock);

            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(newInstrutorId)).ReturnsAsync((Instrutor?)null); // Simula instrutor não encontrado


            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _planoDeTreinoAppService.AtualizarPlanoDeTreinoAsync(planoId, updateModel));

            Assert.Contains($"Novo Instrutor com ID {newInstrutorId} não encontrado.", exception.Message);
            _mockPlanoDeTreinoRepository.Verify(r => r.Update(It.IsAny<PlanoDeTreino>()), Times.Never);
            _mockPlanoDeTreinoRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }


        // --- Testes para GetPlanoDeTreinoByIdAsync ---
        [Fact]
        public async Task GetPlanoDeTreinoByIdAsync_DeveRetornarPlano_QuandoEncontrado()
        {
            // Arrange
            var planoId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var planoMock = new PlanoDeTreino(planoId, alunoId, instrutorId, "Plano Get", "Desc Get", DateTime.Today, DateTime.Today.AddMonths(1), StatusPlano.Ativo);

            var alunoMock = new Aluno(alunoId, "Aluno Encontrado", "hash", StatusAluno.Ativo, "123", DateTime.Now.AddYears(-20), "123", UserRole.Aluno);
            var instrutorMock = new Instrutor(instrutorId, "Instrutor Encontrado", null, "hash", StatusInstrutor.Ativo, "CREF", UserRole.Instrutor);

            _mockPlanoDeTreinoRepository.Setup(r => r.GetByIdAsync(planoId)).ReturnsAsync(planoMock);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock);

            // Act
            var result = await _planoDeTreinoAppService.GetPlanoDeTreinoByIdAsync(planoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(planoId, result.Id);
            Assert.Equal(planoMock.NomePlano, result.NomePlano);
            Assert.Equal(alunoMock.Nome, result.NomeAluno);
            Assert.Equal(instrutorMock.Nome, result.NomeInstrutor);
        }

        [Fact]
        public async Task GetPlanoDeTreinoByIdAsync_DeveRetornarNull_QuandoNaoEncontrado()
        {
            // Arrange
            var planoId = Guid.NewGuid();
            _mockPlanoDeTreinoRepository.Setup(r => r.GetByIdAsync(planoId)).ReturnsAsync((PlanoDeTreino?)null);

            // Act
            var result = await _planoDeTreinoAppService.GetPlanoDeTreinoByIdAsync(planoId);

            // Assert
            Assert.Null(result);
            _mockAlunoRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never); // Não deveria tentar buscar aluno/instrutor se o plano não existe
            _mockInstrutorRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        // --- Testes para GetPlanosDeTreinoByAlunoIdAsync ---
        [Fact]
        public async Task GetPlanosDeTreinoByAlunoIdAsync_DeveRetornarListaDePlanos_QuandoEncontrado()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var instrutorId1 = Guid.NewGuid();
            var instrutorId2 = Guid.NewGuid();

            var planosMock = new List<PlanoDeTreino>
            {
                new PlanoDeTreino(Guid.NewGuid(), alunoId, instrutorId1, "Plano A", "Desc A", DateTime.Today.AddDays(-10), DateTime.Today.AddDays(20), StatusPlano.Ativo),
                new PlanoDeTreino(Guid.NewGuid(), alunoId, instrutorId2, "Plano B", "Desc B", DateTime.Today.AddDays(-5), DateTime.Today.AddDays(10), StatusPlano.Pausado)
            };

            var alunoMock = new Aluno(alunoId, "Aluno Lista", "hash", StatusAluno.Ativo, "123", DateTime.Now.AddYears(-20), "123", UserRole.Aluno);
            var instrutorMock1 = new Instrutor(instrutorId1, "Instrutor 1", null, "hash", StatusInstrutor.Ativo, "CREF1", UserRole.Instrutor);
            var instrutorMock2 = new Instrutor(instrutorId2, "Instrutor 2", null, "hash", StatusInstrutor.Ativo, "CREF2", UserRole.Instrutor);

            _mockPlanoDeTreinoRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PlanoDeTreino, bool>>>()))
                                         .ReturnsAsync(planosMock);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId1)).ReturnsAsync(instrutorMock1);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId2)).ReturnsAsync(instrutorMock2);


            // Act
            var result = await _planoDeTreinoAppService.GetPlanosDeTreinoByAlunoIdAsync(alunoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.NomePlano == "Plano A" && p.NomeAluno == "Aluno Lista" && p.NomeInstrutor == "Instrutor 1");
            Assert.Contains(result, p => p.NomePlano == "Plano B" && p.NomeAluno == "Aluno Lista" && p.NomeInstrutor == "Instrutor 2");

            _mockAlunoRepository.Verify(r => r.GetByIdAsync(alunoId), Times.Once); // Aluno é buscado apenas uma vez
            _mockInstrutorRepository.Verify(r => r.GetByIdAsync(instrutorId1), Times.Once);
            _mockInstrutorRepository.Verify(r => r.GetByIdAsync(instrutorId2), Times.Once);
        }

        [Fact]
        public async Task GetPlanosDeTreinoByAlunoIdAsync_DeveRetornarListaVazia_QuandoNenhumPlanoEncontrado()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            _mockPlanoDeTreinoRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PlanoDeTreino, bool>>>()))
                                         .ReturnsAsync(new List<PlanoDeTreino>());

            var alunoMock = new Aluno(alunoId, "Aluno Vazio", "hash", StatusAluno.Ativo, "123", DateTime.Now.AddYears(-20), "123", UserRole.Aluno);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);

            // Act
            var result = await _planoDeTreinoAppService.GetPlanosDeTreinoByAlunoIdAsync(alunoId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockAlunoRepository.Verify(r => r.GetByIdAsync(alunoId), Times.Once);
            _mockInstrutorRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never); // Não deve buscar instrutor se não há planos
        }

        // --- Testes para GetPlanoDeTreinoAtivoByAlunoIdAsync ---
        [Fact]
        public async Task GetPlanoDeTreinoAtivoByAlunoIdAsync_DeveRetornarPlanoAtivo_QuandoEncontrado()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var planoAtivoMock = new PlanoDeTreino(Guid.NewGuid(), alunoId, instrutorId, "Plano Ativo", "Desc Ativo", DateTime.Today.AddDays(-10), DateTime.Today.AddDays(30), StatusPlano.Ativo);

            _mockPlanoDeTreinoRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PlanoDeTreino, bool>>>()))
                                         .ReturnsAsync(new List<PlanoDeTreino> { planoAtivoMock });

            var alunoMock = new Aluno(alunoId, "Aluno Ativo", "hash", StatusAluno.Ativo, "123", DateTime.Now.AddYears(-20), "123", UserRole.Aluno);
            var instrutorMock = new Instrutor(instrutorId, "Instrutor Ativo", null, "hash", StatusInstrutor.Ativo, "CREF", UserRole.Instrutor);

            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock);

            // Act
            var result = await _planoDeTreinoAppService.GetPlanoDeTreinoAtivoByAlunoIdAsync(alunoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(planoAtivoMock.NomePlano, result.NomePlano);
            Assert.Equal(StatusPlano.Ativo, result.StatusPlano);
            Assert.Equal(planoAtivoMock.DataFim, result.DataFim);
            Assert.Equal("Aluno Ativo", result.NomeAluno);
            Assert.Equal("Instrutor Ativo", result.NomeInstrutor);
        }

        [Fact]
        public async Task GetPlanoDeTreinoAtivoByAlunoIdAsync_DeveRetornarNull_QuandoNenhumPlanoAtivoEncontrado()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            _mockPlanoDeTreinoRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PlanoDeTreino, bool>>>()))
                                         .ReturnsAsync(new List<PlanoDeTreino>()); // Lista vazia ou sem planos ativos

            var alunoMock = new Aluno(alunoId, "Aluno Sem Ativo", "hash", StatusAluno.Ativo, "123", DateTime.Now.AddYears(-20), "123", UserRole.Aluno);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);

            // Act
            var result = await _planoDeTreinoAppService.GetPlanoDeTreinoAtivoByAlunoIdAsync(alunoId);

            // Assert
            Assert.Null(result);
            _mockInstrutorRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never); // Não deve buscar instrutor se não há plano ativo
        }
    }
}