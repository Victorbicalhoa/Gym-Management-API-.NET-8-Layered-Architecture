/*
using Xunit;
using AutoMapper;
using CentroTreinamento.Application.Mappers;
using CentroTreinamento.Application.DTOs.Agendamento;
using CentroTreinamento.Application.DTOs.Pagamento;
using CentroTreinamento.Application.DTOs.PlanoDeTreino;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums;
using System;

namespace CentroTreinamento.Tests.Unit.Application.Mappers
{
    public class MappingProfileTests
    {
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            // CORREÇÃO FINAL: Inicialização do MapperConfiguration para máxima compatibilidade
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile()); // ✅ use "new MappingProfile()" e não "<MappingProfile>"
            });

            config.AssertConfigurationIsValid(); // Opcional, mas útil para debug

            _mapper = config.CreateMapper();
        }

        // ... o restante dos seus testes (não precisa mudar nada neles) ...

        // --- Testes para Agendamento Mappings ---
        [Fact]
        public void AgendamentoInputModel_To_Agendamento_ShouldMapCorrectly()
        {
            // Arrange
            var input = new AgendamentoInputModel
            {
                AlunoId = Guid.NewGuid(),
                InstrutorId = Guid.NewGuid(),
                DataHoraInicio = DateTime.UtcNow.AddDays(1).Date.AddHours(10),
                Descricao = "Agendamento de teste"
            };

            // Act
            var agendamento = _mapper.Map<Agendamento>(input);

            // Assert
            Assert.NotEqual(Guid.Empty, agendamento.Id);
            Assert.Equal(input.AlunoId, agendamento.AlunoId);
            Assert.Equal(input.InstrutorId, agendamento.InstrutorId);
            Assert.Equal(input.DataHoraInicio, agendamento.DataHoraInicio);
            Assert.Equal(input.Descricao, agendamento.Descricao);
            Assert.Equal(StatusAgendamento.Pendente, agendamento.Status);
            Assert.Equal(input.DataHoraInicio.AddHours(1), agendamento.DataHoraFim);
        }

        [Fact]
        public void AgendamentoUpdateModel_To_Agendamento_ShouldMapCorrectly()
        {
            // Arrange
            var existingAgendamento = new Agendamento(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                DateTime.UtcNow.AddDays(-5).Date.AddHours(9), DateTime.UtcNow.AddDays(-5).Date.AddHours(10),
                StatusAgendamento.Aprovado, "Descricao Antiga");

            var update = new AgendamentoUpdateModel
            {
                AlunoId = existingAgendamento.AlunoId,
                InstrutorId = existingAgendamento.InstrutorId,
                DataHoraInicio = DateTime.UtcNow.AddDays(2).Date.AddHours(9),
                DataHoraFim = DateTime.UtcNow.AddDays(2).Date.AddHours(11),
                Descricao = "Nova descricao do agendamento",
                Status = StatusAgendamento.Cancelado
            };

            // Act
            _mapper.Map(update, existingAgendamento);

            // Assert
            Assert.Equal(update.DataHoraInicio, existingAgendamento.DataHoraInicio);
            Assert.Equal(update.DataHoraFim, existingAgendamento.DataHoraFim);
            Assert.Equal(update.Descricao, existingAgendamento.Descricao);
            Assert.Equal(update.Status, existingAgendamento.Status);

            Assert.NotEqual(Guid.Empty, existingAgendamento.AlunoId);
            Assert.NotEqual(Guid.Empty, existingAgendamento.InstrutorId);
        }

        [Fact]
        public void Agendamento_To_AgendamentoViewModel_ShouldMapCorrectly()
        {
            // Arrange
            var agendamento = new Agendamento(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                DateTime.UtcNow.Date.AddHours(10), DateTime.UtcNow.Date.AddHours(11),
                StatusAgendamento.Concluido, "Descricao Teste");

            // Act
            var viewModel = _mapper.Map<AgendamentoViewModel>(agendamento);

            // Assert
            Assert.Equal(agendamento.Id, viewModel.Id);
            Assert.Equal(agendamento.AlunoId, viewModel.AlunoId);
            Assert.Equal(agendamento.InstrutorId, viewModel.InstrutorId);
            Assert.Equal(agendamento.DataHoraInicio, viewModel.DataHoraInicio);
            Assert.Equal(agendamento.DataHoraFim, viewModel.DataHoraFim);
            Assert.Equal(agendamento.Descricao, viewModel.Descricao);
            Assert.Equal(agendamento.Status, viewModel.Status);
            Assert.Null(viewModel.NomeAluno);
            Assert.Null(viewModel.NomeInstrutor);
        }


        // --- Testes para Pagamento Mappings ---
        [Fact]
        public void PagamentoInputModel_To_Pagamento_ShouldMapCorrectly()
        {
            // Arrange
            var input = new PagamentoInputModel
            {
                AlunoId = Guid.NewGuid(),
                Valor = 200.50m,
                DataPagamento = DateTime.UtcNow.Date,
                MetodoPagamento = "Boleto",
                Observacoes = "Pagamento da mensalidade",
                StatusPagamento = StatusPagamento.Pendente
            };

            // Act
            var pagamento = _mapper.Map<Pagamento>(input);

            // Assert
            Assert.NotEqual(Guid.Empty, pagamento.Id);
            Assert.Equal(input.AlunoId, pagamento.AlunoId);
            Assert.Equal(input.Valor, pagamento.Valor);
            Assert.Equal(input.DataPagamento, pagamento.DataPagamento);
            Assert.Equal(input.MetodoPagamento, pagamento.MetodoPagamento);
            Assert.Equal(input.Observacoes, pagamento.Observacoes);
            Assert.Equal(input.StatusPagamento, pagamento.StatusPagamento);
        }

        [Fact]
        public void Pagamento_To_PagamentoViewModel_ShouldMapCorrectly()
        {
            // Arrange
            var pagamento = new Pagamento(
                Guid.NewGuid(), Guid.NewGuid(), 150.00m, DateTime.UtcNow, "Cartao", "Alguma obs", StatusPagamento.Pago);

            // Act
            var viewModel = _mapper.Map<PagamentoViewModel>(pagamento);

            // Assert
            Assert.Equal(pagamento.Id, viewModel.Id);
            Assert.Equal(pagamento.AlunoId, viewModel.AlunoId);
            Assert.Equal(pagamento.Valor, viewModel.Valor);
            Assert.Equal(pagamento.DataPagamento, viewModel.DataPagamento);
            Assert.Equal(pagamento.MetodoPagamento, viewModel.MetodoPagamento);
            Assert.Equal(pagamento.Observacoes, viewModel.Observacoes);
            Assert.Equal(pagamento.StatusPagamento, viewModel.StatusPagamento);
            Assert.Null(viewModel.NomeAluno);
        }

        // --- Testes para PlanoDeTreino Mappings ---
        [Fact]
        public void PlanoDeTreinoInputModel_To_PlanoDeTreino_ShouldMapCorrectly()
        {
            // Arrange
            var input = new PlanoDeTreinoInputModel
            {
                AlunoId = Guid.NewGuid(),
                InstrutorId = Guid.NewGuid(),
                NomePlano = "Plano Inicial",
                Descricao = "Descrição do plano inicial",
                DataInicio = DateTime.UtcNow.Date,
                DataFim = DateTime.UtcNow.Date.AddMonths(1),
                StatusPlano = StatusPlano.Ativo
            };

            // Act
            var planoDeTreino = _mapper.Map<PlanoDeTreino>(input);

            // Assert
            Assert.NotEqual(Guid.Empty, planoDeTreino.Id);
            Assert.Equal(input.AlunoId, planoDeTreino.AlunoId);
            Assert.Equal(input.InstrutorId, planoDeTreino.InstrutorId);
            Assert.Equal(input.NomePlano, planoDeTreino.NomePlano);
            Assert.Equal(input.Descricao, planoDeTreino.Descricao);
            Assert.Equal(input.DataInicio, planoDeTreino.DataInicio);
            Assert.Equal(input.DataFim, planoDeTreino.DataFim);
            Assert.Equal(input.StatusPlano, planoDeTreino.Status);
        }

        [Fact]
        public void PlanoDeTreinoUpdateModel_To_PlanoDeTreino_ShouldMapCorrectly()
        {
            // Arrange
            var existingPlano = new PlanoDeTreino(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                "Plano Antigo", "Descrição Antiga",
                DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow,
                StatusPlano.Inativo);

            var update = new PlanoDeTreinoUpdateModel
            {
                AlunoId = existingPlano.AlunoId,
                InstrutorId = existingPlano.InstrutorId,
                NomePlano = "Plano Atualizado",
                Descricao = "Nova descrição",
                DataInicio = DateTime.UtcNow.Date.AddDays(5),
                DataFim = DateTime.UtcNow.Date.AddMonths(2),
                StatusPlano = StatusPlano.Cancelado
            };

            // Act
            _mapper.Map(update, existingPlano);

            // Assert
            Assert.Equal(update.NomePlano, existingPlano.NomePlano);
            Assert.Equal(update.Descricao, existingPlano.Descricao);
            Assert.Equal(update.DataInicio, existingPlano.DataInicio);
            Assert.Equal(update.DataFim, existingPlano.DataFim);
            Assert.Equal(update.StatusPlano, existingPlano.Status);

            Assert.Equal(existingPlano.AlunoId, existingPlano.AlunoId);
            Assert.Equal(existingPlano.InstrutorId, existingPlano.InstrutorId);
        }

        [Fact]
        public void PlanoDeTreino_To_PlanoDeTreinoViewModel_ShouldMapCorrectly()
        {
            // Arrange
            var plano = new PlanoDeTreino(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                "Plano Premium", "Detalhes Premium",
                DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddMonths(1),
                StatusPlano.Ativo);

            // Act
            var viewModel = _mapper.Map<PlanoDeTreinoViewModel>(plano);

            // Assert
            Assert.Equal(plano.Id, viewModel.Id);
            Assert.Equal(plano.AlunoId, viewModel.AlunoId);
            Assert.Equal(plano.InstrutorId, viewModel.InstrutorId);
            Assert.Equal(plano.NomePlano, viewModel.NomePlano);
            Assert.Equal(plano.Descricao, viewModel.Descricao);
            Assert.Equal(plano.DataInicio, viewModel.DataInicio);
            Assert.Equal(plano.DataFim, viewModel.DataFim);
            Assert.Equal(plano.Status, viewModel.StatusPlano);
            Assert.Null(viewModel.NomeAluno);
            Assert.Null(viewModel.NomeInstrutor);
        }
    }
}

*/