using AutoMapper;
using CentroTreinamento.Application.DTOs.Agendamento;
using CentroTreinamento.Application.Mappers;
using CentroTreinamento.Application.Services;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums;
using CentroTreinamento.Domain.Repositories;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CentroTreinamento.Tests.Unit.Application.Services
{
    public class AgendamentoAppServiceTests
    {
        private readonly Mock<IRepository<Agendamento>> _mockAgendamentoRepository;
        private readonly Mock<IRepository<Aluno>> _mockAlunoRepository;
        private readonly Mock<IRepository<Instrutor>> _mockInstrutorRepository;
        private readonly Mock<IRepository<Pagamento>> _mockPagamentoRepository;
        private readonly IMapper _mapper;
        private readonly AgendamentoAppService _agendamentoAppService;

        public AgendamentoAppServiceTests()
        {
            _mockAgendamentoRepository = new Mock<IRepository<Agendamento>>();
            _mockAlunoRepository = new Mock<IRepository<Aluno>>();
            _mockInstrutorRepository = new Mock<IRepository<Instrutor>>();
            _mockPagamentoRepository = new Mock<IRepository<Pagamento>>();

            // Configuração do AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _agendamentoAppService = new AgendamentoAppService(
                _mockAgendamentoRepository.Object,
                _mockAlunoRepository.Object,
                _mockInstrutorRepository.Object,
                _mockPagamentoRepository.Object,
                _mapper
            );
        }

        // --- Testes para CriarAgendamentoAsync ---

        [Fact]
        public async Task CriarAgendamentoAsync_DeveRetornarAgendamentoViewModel_QuandoDadosValidos()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var dataHoraInicio = DateTime.Now.AddDays(1).Date.AddHours(10);
            var descricao = "Treino de musculação";

            var inputModel = new AgendamentoInputModel
            {
                AlunoId = alunoId,
                InstrutorId = instrutorId,
                DataHoraInicio = dataHoraInicio,
                Descricao = descricao
            };

            var alunoMock = new Aluno(alunoId, "Aluno Teste", "hash", StatusAluno.Ativo, "12345678900", new DateTime(2000, 1, 1), "999999999", UserRole.Aluno);
            var instrutorMock = new Instrutor(instrutorId, "Instrutor Teste", "09876543210", "hash", StatusInstrutor.Ativo, "CREF123", UserRole.Instrutor);

            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock);

            // Mock para pagamentos (nenhum atrasado)
            _mockPagamentoRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Pagamento, bool>>>()))
                                    .ReturnsAsync(new List<Pagamento>
                                    {
                                        new Pagamento(Guid.NewGuid(), alunoId, 100, DateTime.Now, MetodoPagamento.CartaoCredito, "Pagamento em dia", StatusPagamento.Pago)
                                    });

            // Mock para verificar conflitos (nenhum conflito existente)
            // Agendamentos simulados para a expressão de conflito devem ser válidos
            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Agendamento, bool>>>()))
                                      .ReturnsAsync(new List<Agendamento>());
            _mockAgendamentoRepository.Setup(r => r.AddAsync(It.IsAny<Agendamento>())).Returns(Task.CompletedTask);
            _mockAgendamentoRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _agendamentoAppService.CriarAgendamentoAsync(inputModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(alunoId, result.AlunoId);
            Assert.Equal(instrutorId, result.InstrutorId);
            Assert.Equal(dataHoraInicio, result.DataHoraInicio);
            Assert.Equal(dataHoraInicio.AddHours(1), result.DataHoraFim); // Verifica o cálculo da DataHoraFim
            Assert.Equal(StatusAgendamento.Pendente, result.Status); // Verifica o status inicial
            Assert.Equal(descricao, result.Descricao);
            Assert.Equal(alunoMock.Nome, result.NomeAluno);
            Assert.Equal(instrutorMock.Nome, result.NomeInstrutor);

            _mockAgendamentoRepository.Verify(r => r.AddAsync(It.IsAny<Agendamento>()), Times.Once);
            _mockAgendamentoRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CriarAgendamentoAsync_DeveLancarExcecao_QuandoAlunoNaoEncontrado()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var inputModel = new AgendamentoInputModel
            {
                AlunoId = alunoId,
                InstrutorId = instrutorId,
                DataHoraInicio = DateTime.Now.AddDays(1).Date.AddHours(10),
                Descricao = "Treino"
            };

            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync((Aluno?)null);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(new Instrutor(instrutorId, "Instrutor Teste", "09876543210", "hash", StatusInstrutor.Ativo, "CREF123", UserRole.Instrutor));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _agendamentoAppService.CriarAgendamentoAsync(inputModel));
            Assert.Contains($"Aluno com ID {alunoId} não encontrado.", exception.Message);

            _mockAgendamentoRepository.Verify(r => r.AddAsync(It.IsAny<Agendamento>()), Times.Never);
            _mockAgendamentoRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CriarAgendamentoAsync_DeveLancarExcecao_QuandoInstrutorNaoEncontrado()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var inputModel = new AgendamentoInputModel
            {
                AlunoId = alunoId,
                InstrutorId = instrutorId,
                DataHoraInicio = DateTime.Now.AddDays(1).Date.AddHours(10),
                Descricao = "Treino"
            };

            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(new Aluno(alunoId, "Aluno Teste", "hash", StatusAluno.Ativo, "12345678900", new DateTime(2000, 1, 1), "999999999", UserRole.Aluno));
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync((Instrutor?)null);

            // Mock para pagamentos (nenhum atrasado)
            _mockPagamentoRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Pagamento, bool>>>()))
                                    .ReturnsAsync(new List<Pagamento>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _agendamentoAppService.CriarAgendamentoAsync(inputModel));
            Assert.Contains($"Instrutor com ID {instrutorId} não encontrado.", exception.Message);

            _mockAgendamentoRepository.Verify(r => r.AddAsync(It.IsAny<Agendamento>()), Times.Never);
            _mockAgendamentoRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CriarAgendamentoAsync_DeveLancarExcecao_QuandoAlunoInadimplente()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var dataHoraInicio = DateTime.Now.AddDays(1).Date.AddHours(10);
            var descricao = "Treino de musculação";

            var inputModel = new AgendamentoInputModel
            {
                AlunoId = alunoId,
                InstrutorId = instrutorId,
                DataHoraInicio = dataHoraInicio,
                Descricao = descricao
            };

            var alunoMock = new Aluno(alunoId, "Aluno Inadimplente", "hash", StatusAluno.Ativo, "12345678900", new DateTime(2000, 1, 1), "999999999", UserRole.Aluno);
            var instrutorMock = new Instrutor(instrutorId, "Instrutor Teste", "09876543210", "hash", StatusInstrutor.Ativo, "CREF123", UserRole.Instrutor);

            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock);

            // Mock para pagamentos (UM ATRASADO)
            _mockPagamentoRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Pagamento, bool>>>()))
                                    .ReturnsAsync(new List<Pagamento>
                                    {
                                        new Pagamento(Guid.NewGuid(), alunoId, 50, DateTime.Now.AddDays(-30), MetodoPagamento.Boleto, "Mensalidade", StatusPagamento.Atrasado)
                                    });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _agendamentoAppService.CriarAgendamentoAsync(inputModel));
            Assert.Contains($"Aluno {alunoMock.Nome} está inadimplente (possui pagamentos em atraso) e não pode agendar treinos.", exception.Message);

            _mockAgendamentoRepository.Verify(r => r.AddAsync(It.IsAny<Agendamento>()), Times.Never);
            _mockAgendamentoRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CriarAgendamentoAsync_DeveLancarExcecao_QuandoAlunoJaPossuiAgendamentoConflitante()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var dataHoraInicio = DateTime.Now.AddDays(1).Date.AddHours(10);
            var descricao = "Treino de musculação";

            var inputModel = new AgendamentoInputModel
            {
                AlunoId = alunoId,
                InstrutorId = instrutorId,
                DataHoraInicio = dataHoraInicio,
                Descricao = descricao
            };

            var alunoMock = new Aluno(alunoId, "Aluno Teste", "hash", StatusAluno.Ativo, "12345678900", new DateTime(2000, 1, 1), "999999999", UserRole.Aluno);
            var instrutorMock = new Instrutor(instrutorId, "Instrutor Teste", "09876543210", "hash", StatusInstrutor.Ativo, "CREF123", UserRole.Instrutor);

            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock);

            _mockPagamentoRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Pagamento, bool>>>()))
                                    .ReturnsAsync(new List<Pagamento>()); // Nenhum pagamento atrasado

            // Conflito existente para o Aluno
            // CRIANDO COM DATAS VÁLIDAS AQUI
            var agendamentoConflitanteAluno = new Agendamento(
                Guid.NewGuid(), alunoId, Guid.NewGuid(),
                dataHoraInicio.AddMinutes(30), dataHoraInicio.AddMinutes(90), StatusAgendamento.Aprovado, "Conflito"
            );

            // Mocks específicos para as chamadas de FindAsync
            // Simula o agendamento conflitante para o aluno
            // ATENÇÃO: A criação de Agendamento dentro do It.Is<Expression<Func<Agendamento, bool>>>
            // não deve ser a fonte dos dados mockados, apenas para fins de avaliação da expressão,
            // mas o que é retornado por ReturnsAsync DEVE ser válido.
            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.Is<Expression<Func<Agendamento, bool>>>(
                expr => expr.Compile()(new Agendamento(Guid.NewGuid(), alunoId, Guid.NewGuid(), dataHoraInicio, dataHoraInicio.AddHours(1), StatusAgendamento.Pendente, "")) // Apenas para avaliar a expressão
            ))).ReturnsAsync(new List<Agendamento> { agendamentoConflitanteAluno });

            // Simula NENHUM conflito para o instrutor
            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.Is<Expression<Func<Agendamento, bool>>>(
                expr => expr.Compile()(new Agendamento(Guid.NewGuid(), Guid.NewGuid(), instrutorId, dataHoraInicio, dataHoraInicio.AddHours(1), StatusAgendamento.Pendente, "")) // Apenas para avaliar a expressão
            ))).ReturnsAsync(new List<Agendamento>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _agendamentoAppService.CriarAgendamentoAsync(inputModel));
            Assert.Contains($"Aluno {alunoMock.Nome} já possui agendamento conflitante no período solicitado.", exception.Message);

            _mockAgendamentoRepository.Verify(r => r.AddAsync(It.IsAny<Agendamento>()), Times.Never);
            _mockAgendamentoRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CriarAgendamentoAsync_DeveLancarExcecao_QuandoInstrutorJaPossuiAgendamentoConflitante()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var dataHoraInicio = DateTime.Now.AddDays(1).Date.AddHours(10);
            var descricao = "Treino de musculação";

            var inputModel = new AgendamentoInputModel
            {
                AlunoId = alunoId,
                InstrutorId = instrutorId,
                DataHoraInicio = dataHoraInicio,
                Descricao = descricao
            };

            var alunoMock = new Aluno(alunoId, "Aluno Teste", "hash", StatusAluno.Ativo, "12345678900", new DateTime(2000, 1, 1), "999999999", UserRole.Aluno);
            var instrutorMock = new Instrutor(instrutorId, "Instrutor Teste", "09876543210", "hash", StatusInstrutor.Ativo, "CREF123", UserRole.Instrutor);

            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock);

            _mockPagamentoRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Pagamento, bool>>>()))
                                    .ReturnsAsync(new List<Pagamento>()); // Nenhum pagamento atrasado

            // Conflito existente para o Instrutor
            // CRIANDO COM DATAS VÁLIDAS AQUI
            var agendamentoConflitanteInstrutor = new Agendamento(
                Guid.NewGuid(), Guid.NewGuid(), instrutorId,
                dataHoraInicio.AddMinutes(30), dataHoraInicio.AddMinutes(90), StatusAgendamento.Aprovado, "Conflito"
            );

            // Mocks específicos para as chamadas de FindAsync
            // Simula NENHUM conflito para o aluno
            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.Is<Expression<Func<Agendamento, bool>>>(
                expr => expr.Compile()(new Agendamento(Guid.NewGuid(), alunoId, Guid.NewGuid(), dataHoraInicio, dataHoraInicio.AddHours(1), StatusAgendamento.Pendente, "")) // Apenas para avaliar a expressão
            ))).ReturnsAsync(new List<Agendamento>());

            // Simula o agendamento conflitante para o instrutor
            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.Is<Expression<Func<Agendamento, bool>>>(
                expr => expr.Compile()(new Agendamento(Guid.NewGuid(), Guid.NewGuid(), instrutorId, dataHoraInicio, dataHoraInicio.AddHours(1), StatusAgendamento.Pendente, "")) // Apenas para avaliar a expressão
            ))).ReturnsAsync(new List<Agendamento> { agendamentoConflitanteInstrutor });


            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _agendamentoAppService.CriarAgendamentoAsync(inputModel));
            Assert.Contains($"Instrutor {instrutorMock.Nome} não está disponível no período solicitado.", exception.Message);

            _mockAgendamentoRepository.Verify(r => r.AddAsync(It.IsAny<Agendamento>()), Times.Never);
            _mockAgendamentoRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        // --- Testes para AtualizarAgendamentoAsync ---

        [Fact]
        public async Task AtualizarAgendamentoAsync_DeveRetornarAgendamentoViewModel_QuandoDadosValidos()
        {
            // Arrange
            var agendamentoId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var dataHoraInicioAntiga = DateTime.Now.AddDays(5).Date.AddHours(9);
            var dataHoraFimAntiga = dataHoraInicioAntiga.AddHours(1);
            var novaDataHoraInicio = DateTime.Now.AddDays(6).Date.AddHours(11);
            var novaDataHoraFim = novaDataHoraInicio.AddHours(1);
            var novaDescricao = "Treino de cardio atualizado";
            var novoStatus = StatusAgendamento.Aprovado;

            var agendamentoExistente = new Agendamento(
                agendamentoId, alunoId, instrutorId, dataHoraInicioAntiga, dataHoraFimAntiga, StatusAgendamento.Pendente, "Descricao antiga"
            );

            var updateModel = new AgendamentoUpdateModel
            {
                AlunoId = alunoId,
                InstrutorId = instrutorId,
                DataHoraInicio = novaDataHoraInicio,
                DataHoraFim = novaDataHoraFim,
                Descricao = novaDescricao,
                Status = novoStatus
            };

            var alunoMock = new Aluno(alunoId, "Aluno Teste", "hash", StatusAluno.Ativo, "12345678900", new DateTime(2000, 1, 1), "999999999", UserRole.Aluno);
            var instrutorMock = new Instrutor(instrutorId, "Instrutor Teste", "09876543210", "hash", StatusInstrutor.Ativo, "CREF123", UserRole.Instrutor);

            _mockAgendamentoRepository.Setup(r => r.GetByIdAsync(agendamentoId)).ReturnsAsync(agendamentoExistente);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock);

            // Mock para não haver conflitos (excluindo o próprio agendamento)
            // As instâncias de Agendamento passadas para o Compile() precisam ser válidas
            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.Is<Expression<Func<Agendamento, bool>>>(expr =>
                (expr.Compile()(new Agendamento(Guid.NewGuid(), alunoId, Guid.NewGuid(), novaDataHoraInicio, novaDataHoraFim, StatusAgendamento.Pendente, "")) &&
                 !expr.Compile()(agendamentoExistente))
            ))).ReturnsAsync(new List<Agendamento>());

            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.Is<Expression<Func<Agendamento, bool>>>(expr =>
                (expr.Compile()(new Agendamento(Guid.NewGuid(), Guid.NewGuid(), instrutorId, novaDataHoraInicio, novaDataHoraFim, StatusAgendamento.Pendente, "")) &&
                 !expr.Compile()(agendamentoExistente))
            ))).ReturnsAsync(new List<Agendamento>());

            _mockAgendamentoRepository.Setup(r => r.Update(It.IsAny<Agendamento>()));
            _mockAgendamentoRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _agendamentoAppService.AtualizarAgendamentoAsync(agendamentoId, updateModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(agendamentoId, result.Id);
            Assert.Equal(novaDataHoraInicio, result.DataHoraInicio);
            Assert.Equal(novaDataHoraFim, result.DataHoraFim);
            Assert.Equal(novoStatus, result.Status); // Verifica a atualização do status
            Assert.Equal(alunoMock.Nome, result.NomeAluno);
            Assert.Equal(instrutorMock.Nome, result.NomeInstrutor);

            _mockAgendamentoRepository.Verify(r => r.Update(It.Is<Agendamento>(a =>
                a.Id == agendamentoId &&
                a.DataHoraInicio == novaDataHoraInicio &&
                a.DataHoraFim == novaDataHoraFim &&
                a.Status == novoStatus
            )), Times.Once);
            _mockAgendamentoRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AtualizarAgendamentoAsync_DeveLancarExcecao_QuandoAgendamentoNaoEncontrado()
        {
            // Arrange
            var agendamentoId = Guid.NewGuid();
            var updateModel = new AgendamentoUpdateModel
            {
                AlunoId = Guid.NewGuid(),
                InstrutorId = Guid.NewGuid(),
                DataHoraInicio = DateTime.Now.AddDays(1).Date.AddHours(10),
                DataHoraFim = DateTime.Now.AddDays(1).Date.AddHours(11),
                Status = StatusAgendamento.Aprovado
            };

            _mockAgendamentoRepository.Setup(r => r.GetByIdAsync(agendamentoId)).ReturnsAsync((Agendamento?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _agendamentoAppService.AtualizarAgendamentoAsync(agendamentoId, updateModel));
            Assert.Contains($"Agendamento com ID {agendamentoId} não encontrado.", exception.Message);

            _mockAgendamentoRepository.Verify(r => r.Update(It.IsAny<Agendamento>()), Times.Never);
            _mockAgendamentoRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AtualizarAgendamentoAsync_DeveLancarExcecao_QuandoNovoHorarioConflitaComOutroAgendamentoDoAluno()
        {
            // Arrange
            var agendamentoId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var dataHoraInicioAntiga = DateTime.Now.AddDays(5).Date.AddHours(9);
            var dataHoraFimAntiga = dataHoraInicioAntiga.AddHours(1);
            var novaDataHoraInicio = DateTime.Now.AddDays(7).Date.AddHours(10); // Horário que vai conflitar
            var novaDataHoraFim = novaDataHoraInicio.AddHours(1);
            var novoStatus = StatusAgendamento.Remarcado;

            var agendamentoExistente = new Agendamento(
                agendamentoId, alunoId, instrutorId, dataHoraInicioAntiga, dataHoraFimAntiga, StatusAgendamento.Aprovado, "Original"
            );

            // CRIANDO COM DATAS VÁLIDAS AQUI
            var agendamentoConflitanteAluno = new Agendamento(
                Guid.NewGuid(), alunoId, Guid.NewGuid(), // Outro instrutor
                novaDataHoraInicio.AddMinutes(15), novaDataHoraInicio.AddMinutes(75), StatusAgendamento.Aprovado, "Conflito aluno"
            );

            var updateModel = new AgendamentoUpdateModel
            {
                AlunoId = alunoId,
                InstrutorId = instrutorId,
                DataHoraInicio = novaDataHoraInicio,
                DataHoraFim = novaDataHoraFim,
                Descricao = "Atualizado",
                Status = novoStatus
            };

            var alunoMock = new Aluno(alunoId, "Aluno Conflito", "hash", StatusAluno.Ativo, "123", new DateTime(2000, 1, 1), "999", UserRole.Aluno);
            var instrutorMock = new Instrutor(instrutorId, "Instrutor Teste", "09876543210", "hash", StatusInstrutor.Ativo, "CREF123", UserRole.Instrutor);

            _mockAgendamentoRepository.Setup(r => r.GetByIdAsync(agendamentoId)).ReturnsAsync(agendamentoExistente);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock);

            // Mock de conflito para o aluno (excluindo o agendamento sendo atualizado)
            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.Is<Expression<Func<Agendamento, bool>>>(expr =>
                // Simula a expressão que o serviço usa para buscar conflitos de aluno, excluindo o agendamento atual
                // A instância para o Compile() deve ser válida
                expr.Compile()(new Agendamento(Guid.NewGuid(), alunoId, Guid.NewGuid(), novaDataHoraInicio, novaDataHoraFim, StatusAgendamento.Pendente, ""))
                && !expr.Compile()(agendamentoExistente)
            ))).ReturnsAsync(new List<Agendamento> { agendamentoConflitanteAluno });

            // Mock de não conflito para o instrutor
            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.Is<Expression<Func<Agendamento, bool>>>(expr =>
                // Simula a expressão que o serviço usa para buscar conflitos de instrutor, excluindo o agendamento atual
                // A instância para o Compile() deve ser válida
                expr.Compile()(new Agendamento(Guid.NewGuid(), Guid.NewGuid(), instrutorId, novaDataHoraInicio, novaDataHoraFim, StatusAgendamento.Pendente, ""))
                && !expr.Compile()(agendamentoExistente)
            ))).ReturnsAsync(new List<Agendamento>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _agendamentoAppService.AtualizarAgendamentoAsync(agendamentoId, updateModel));
            Assert.Contains($"Aluno {alunoMock.Nome} já possui agendamento conflitante no período solicitado para atualização.", exception.Message);

            _mockAgendamentoRepository.Verify(r => r.Update(It.IsAny<Agendamento>()), Times.Never);
            _mockAgendamentoRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        // Teste para conflito de instrutor
        [Fact]
        public async Task AtualizarAgendamentoAsync_DeveLancarExcecao_QuandoNovoHorarioConflitaComOutroAgendamentoDoInstrutor()
        {
            // Arrange
            var agendamentoId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            var dataHoraInicioAntiga = DateTime.Now.AddDays(5).Date.AddHours(9);
            var dataHoraFimAntiga = dataHoraInicioAntiga.AddHours(1);
            var novaDataHoraInicio = DateTime.Now.AddDays(7).Date.AddHours(10); // Horário que vai conflitar
            var novaDataHoraFim = novaDataHoraInicio.AddHours(1);
            var novoStatus = StatusAgendamento.Remarcado;

            var agendamentoExistente = new Agendamento(
                agendamentoId, alunoId, instrutorId, dataHoraInicioAntiga, dataHoraFimAntiga, StatusAgendamento.Aprovado, "Original"
            );

            // CRIANDO COM DATAS VÁLIDAS AQUI
            var agendamentoConflitanteInstrutor = new Agendamento(
                Guid.NewGuid(), Guid.NewGuid(), instrutorId, // Outro aluno
                novaDataHoraInicio.AddMinutes(15), novaDataHoraInicio.AddMinutes(75), StatusAgendamento.Aprovado, "Conflito instrutor"
            );

            var updateModel = new AgendamentoUpdateModel
            {
                AlunoId = alunoId,
                InstrutorId = instrutorId,
                DataHoraInicio = novaDataHoraInicio,
                DataHoraFim = novaDataHoraFim,
                Descricao = "Atualizado",
                Status = novoStatus
            };

            var alunoMock = new Aluno(alunoId, "Aluno Teste", "hash", StatusAluno.Ativo, "123", new DateTime(2000, 1, 1), "999", UserRole.Aluno);
            var instrutorMock = new Instrutor(instrutorId, "Instrutor Conflito", "09876543210", "hash", StatusInstrutor.Ativo, "CREF123", UserRole.Instrutor);

            _mockAgendamentoRepository.Setup(r => r.GetByIdAsync(agendamentoId)).ReturnsAsync(agendamentoExistente);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock);

            // Mock de não conflito para o aluno
            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.Is<Expression<Func<Agendamento, bool>>>(expr =>
                // Simula a expressão que o serviço usa para buscar conflitos de aluno, excluindo o agendamento atual
                // A instância para o Compile() deve ser válida
                expr.Compile()(new Agendamento(Guid.NewGuid(), alunoId, Guid.NewGuid(), novaDataHoraInicio, novaDataHoraFim, StatusAgendamento.Pendente, ""))
                && !expr.Compile()(agendamentoExistente)
            ))).ReturnsAsync(new List<Agendamento>());

            // Mock de conflito para o instrutor (excluindo o agendamento sendo atualizado)
            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.Is<Expression<Func<Agendamento, bool>>>(expr =>
                // Simula a expressão que o serviço usa para buscar conflitos de instrutor, excluindo o agendamento atual
                // A instância para o Compile() deve ser válida
                expr.Compile()(new Agendamento(Guid.NewGuid(), Guid.NewGuid(), instrutorId, novaDataHoraInicio, novaDataHoraFim, StatusAgendamento.Pendente, ""))
                && !expr.Compile()(agendamentoExistente)
            ))).ReturnsAsync(new List<Agendamento> { agendamentoConflitanteInstrutor });


            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _agendamentoAppService.AtualizarAgendamentoAsync(agendamentoId, updateModel));
            Assert.Contains($"Instrutor {instrutorMock.Nome} não está disponível no período solicitado para atualização.", exception.Message);

            _mockAgendamentoRepository.Verify(r => r.Update(It.IsAny<Agendamento>()), Times.Never);
            _mockAgendamentoRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        // --- Testes para GetAgendamentoByIdAsync ---

        [Fact]
        public async Task GetAgendamentoByIdAsync_DeveRetornarAgendamentoViewModel_QuandoAgendamentoExiste()
        {
            // Arrange
            var agendamentoId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();
            // Crie a instância de Agendamento com datas e horas VÁLIDAS
            var agendamentoMock = new Agendamento(agendamentoId, alunoId, instrutorId, DateTime.Now, DateTime.Now.AddHours(1), StatusAgendamento.Aprovado, "Teste");
            var alunoMock = new Aluno(alunoId, "Aluno Get", "hash", StatusAluno.Ativo, "123", new DateTime(2000, 1, 1), "999", UserRole.Aluno);
            var instrutorMock = new Instrutor(instrutorId, "Instrutor Get", "098", "hash", StatusInstrutor.Ativo, "CREF1", UserRole.Instrutor);

            _mockAgendamentoRepository.Setup(r => r.GetByIdAsync(agendamentoId)).ReturnsAsync(agendamentoMock);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock);

            // Act
            var result = await _agendamentoAppService.GetAgendamentoByIdAsync(agendamentoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(agendamentoId, result.Id);
            Assert.Equal(alunoMock.Nome, result.NomeAluno);
            Assert.Equal(instrutorMock.Nome, result.NomeInstrutor);
        }

        [Fact]
        public async Task GetAgendamentoByIdAsync_DeveRetornarNull_QuandoAgendamentoNaoExiste()
        {
            // Arrange
            var agendamentoId = Guid.NewGuid();
            _mockAgendamentoRepository.Setup(r => r.GetByIdAsync(agendamentoId)).ReturnsAsync((Agendamento?)null);

            // Act
            var result = await _agendamentoAppService.GetAgendamentoByIdAsync(agendamentoId);

            // Assert
            Assert.Null(result);
        }

        // --- Novos Testes para GetAgendamentosByInstrutorIdAsync ---
       
        [Fact]
        public async Task GetAgendamentosByInstrutorIdAsync_DeveRetornarListaDeAgendamentos_QuandoInstrutorTemAgendamentos()
        {
            // Arrange
            var instrutorId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();

            // Crie Agendamentos VÁLIDOS para serem retornados pelo mock
            var agendamento1 = new Agendamento(
                Guid.NewGuid(), alunoId, instrutorId,
                DateTime.Now.AddDays(1).Date.AddHours(9), // Data válida
                DateTime.Now.AddDays(1).Date.AddHours(10), // Data válida
                StatusAgendamento.Aprovado, "Aula de C#"
            );
            var agendamento2 = new Agendamento(
                Guid.NewGuid(), alunoId, instrutorId,
                DateTime.Now.AddDays(2).Date.AddHours(14), // Data válida
                DateTime.Now.AddDays(2).Date.AddHours(15), // Data válida
                StatusAgendamento.Pendente, "Sessão de mentoria"
            );
            var agendamentosMock = new List<Agendamento> { agendamento1, agendamento2 };

            var alunoMock = new Aluno(alunoId, "Aluno XYZ", "hash", StatusAluno.Ativo, "12345678900", new DateTime(2000, 1, 1), "999999999", UserRole.Aluno);
            var instrutorMock = new Instrutor(instrutorId, "Instrutor ABC", "09876543210", "hash", StatusInstrutor.Ativo, "CREF123", UserRole.Instrutor);

            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Agendamento, bool>>>()))
                                      .ReturnsAsync(agendamentosMock);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock); // Pode ser necessário se o service busca o instrutor para popular o nome no VM

            // Act
            var result = await _agendamentoAppService.GetAgendamentosByInstrutorIdAsync(instrutorId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.True(result.All(a => a.InstrutorId == instrutorId));
            Assert.True(result.All(a => a.NomeAluno == alunoMock.Nome)); // Verifica se o nome do aluno foi preenchido
            Assert.True(result.All(a => a.NomeInstrutor == instrutorMock.Nome)); // Verifica se o nome do instrutor foi preenchido
        }

        [Fact]
        public async Task GetAgendamentosByInstrutorIdAsync_DeveRetornarListaVazia_QuandoInstrutorNaoTemAgendamentos()
        {
            // Arrange
            var instrutorId = Guid.NewGuid();

            // Configurar o mock para retornar uma lista vazia.
            // Neste caso, nenhuma instância de Agendamento será criada, então o construtor não é invocado.
            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Agendamento, bool>>>()))
                                      .ReturnsAsync(new List<Agendamento>());

            // Act
            var result = await _agendamentoAppService.GetAgendamentosByInstrutorIdAsync(instrutorId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // --- Novos Testes para GetAgendamentosByAlunoIdAsync ---
        
        [Fact]
        public async Task GetAgendamentosByAlunoIdAsync_DeveRetornarListaDeAgendamentos_QuandoAlunoTemAgendamentos()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var instrutorId = Guid.NewGuid();

            // Crie Agendamentos VÁLIDOS para serem retornados pelo mock
            var agendamento1 = new Agendamento(
                Guid.NewGuid(), alunoId, instrutorId,
                DateTime.Now.AddDays(3).Date.AddHours(10), // Data válida
                DateTime.Now.AddDays(3).Date.AddHours(11), // Data válida
                StatusAgendamento.Aprovado, "Treino Funcional"
            );
            var agendamento2 = new Agendamento(
                Guid.NewGuid(), alunoId, instrutorId,
                DateTime.Now.AddDays(4).Date.AddHours(16), // Data válida
                DateTime.Now.AddDays(4).Date.AddHours(17), // Data válida
                StatusAgendamento.Cancelado, "Avaliação física"
            );
            var agendamentosMock = new List<Agendamento> { agendamento1, agendamento2 };

            var alunoMock = new Aluno(alunoId, "Aluno Get", "hash", StatusAluno.Ativo, "123", new DateTime(2000, 1, 1), "999", UserRole.Aluno);
            var instrutorMock = new Instrutor(instrutorId, "Instrutor Get", "098", "hash", StatusInstrutor.Ativo, "CREF1", UserRole.Instrutor);

            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Agendamento, bool>>>()))
                                      .ReturnsAsync(agendamentosMock);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock); // Pode ser necessário se o service busca o aluno para popular o nome no VM
            _mockInstrutorRepository.Setup(r => r.GetByIdAsync(instrutorId)).ReturnsAsync(instrutorMock);

            // Act
            var result = await _agendamentoAppService.GetAgendamentosByAlunoIdAsync(alunoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.True(result.All(a => a.AlunoId == alunoId));
            Assert.True(result.All(a => a.NomeAluno == alunoMock.Nome));
            Assert.True(result.All(a => a.NomeInstrutor == instrutorMock.Nome));
        }

        [Fact]
        public async Task GetAgendamentosByAlunoIdAsync_DeveRetornarListaVazia_QuandoAlunoNaoTemAgendamentos()
        {
            // Arrange
            var alunoId = Guid.NewGuid();

            // Configurar o mock para retornar uma lista vazia.
            // Nenhuma instância de Agendamento é criada, então o construtor não é invocado.
            _mockAgendamentoRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Agendamento, bool>>>()))
                                      .ReturnsAsync(new List<Agendamento>());

            // Act
            var result = await _agendamentoAppService.GetAgendamentosByAlunoIdAsync(alunoId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // --- Fim dos Testes de AgendamentoAppServiceTests ---
    }
}