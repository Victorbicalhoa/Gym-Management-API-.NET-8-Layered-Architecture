using AutoMapper;
using CentroTreinamento.Application.DTOs.Agendamento;
using CentroTreinamento.Application.DTOs.Pagamento;
using CentroTreinamento.Application.DTOs.PlanoDeTreino;
using CentroTreinamento.Application.DTOs.Administrador;
using CentroTreinamento.Application.DTOs.Aluno;
using CentroTreinamento.Application.DTOs.Instrutor;
using CentroTreinamento.Application.DTOs.Recepcionista;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums;
using Xunit; // Ou NUnit, se preferir
using System;

namespace CentroTreinamento.Tests.Unit.Application.Mappers
{
    public class MappingProfileTests
    {
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public MappingProfileTests()
        {
            // Configura o AutoMapper para os testes
            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CentroTreinamento.Application.Mappers.MappingProfile>();
            });

            _mapper = _mapperConfiguration.CreateMapper();
        }

        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            // Este teste vai validar se a configuração do AutoMapper é válida.
            // Ele verifica se todos os mapeamentos estão configurados corretamente e
            // se não há membros não mapeados que causem erro no runtime.
            // Com as últimas correções usando ConstructUsing e ForMember().Ignore(),
            // este teste deve passar.
            _mapperConfiguration.AssertConfigurationIsValid();
        }

        // --- Testes de Mapeamento Específicos para Agendamento ---
        [Fact]
        public void AgendamentoInputModel_To_Agendamento_ShouldMapCorrectly()
        {
            var inputModel = new AgendamentoInputModel
            {
                AlunoId = Guid.NewGuid(),
                InstrutorId = Guid.NewGuid(),
                DataHoraInicio = DateTime.Now.AddDays(1),
                Descricao = "Treino de pernas com instrutor X"
            };

            var entity = _mapper.Map<Agendamento>(inputModel);

            Assert.NotNull(entity); // Adicionado para garantir que a entidade foi criada
            Assert.NotEqual(Guid.Empty, entity.Id); // ID deve ser gerado
            Assert.Equal(inputModel.AlunoId, entity.AlunoId);
            Assert.Equal(inputModel.InstrutorId, entity.InstrutorId);
            Assert.Equal(inputModel.DataHoraInicio, entity.DataHoraInicio);
            Assert.Equal(inputModel.DataHoraInicio.AddHours(1), entity.DataHoraFim); // Regra de 1 hora
            Assert.Equal(StatusAgendamento.Pendente, entity.Status); // Status inicial
            Assert.Equal(inputModel.Descricao, entity.Descricao);
        }

        [Fact]
        public void AgendamentoUpdateModel_To_Agendamento_ShouldMapCorrectly()
        {
            var entityId = Guid.NewGuid();
            var existingAlunoId = Guid.NewGuid();
            var existingInstrutorId = Guid.NewGuid();

            // Entidade existente (simula a que seria buscada do banco)
            var existingEntity = new Agendamento(
                entityId,
                existingAlunoId,
                existingInstrutorId,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                StatusAgendamento.Aprovado,
                "Descricao Antiga"
            );

            var updateModel = new AgendamentoUpdateModel
            {
                // AlunoId e InstrutorId devem ser ignorados no mapeamento de atualização
                DataHoraInicio = DateTime.Now.AddDays(2),
                DataHoraFim = DateTime.Now.AddDays(2).AddHours(2),
                Descricao = "Nova descricao de treino",
                Status = StatusAgendamento.Remarcado
            };

            // Mapeia as propriedades do updateModel para a entidade existente
            _mapper.Map(updateModel, existingEntity);

            Assert.Equal(entityId, existingEntity.Id); // ID não deve mudar
            Assert.Equal(existingAlunoId, existingEntity.AlunoId); // AlunoId não deve mudar
            Assert.Equal(existingInstrutorId, existingEntity.InstrutorId); // InstrutorId não deve mudar
            Assert.Equal(updateModel.DataHoraInicio, existingEntity.DataHoraInicio);
            Assert.Equal(updateModel.DataHoraFim, existingEntity.DataHoraFim);
            Assert.Equal(updateModel.Descricao, existingEntity.Descricao);
            Assert.Equal(updateModel.Status, existingEntity.Status);
        }

        [Fact]
        public void Agendamento_To_AgendamentoViewModel_ShouldMapCorrectly()
        {
            var entity = new Agendamento(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.Now,
                DateTime.Now.AddHours(1),
                StatusAgendamento.Concluido,
                "Treino Finalizado"
            );

            var viewModel = _mapper.Map<AgendamentoViewModel>(entity);

            Assert.NotNull(viewModel); // Adicionado
            Assert.Equal(entity.Id, viewModel.Id);
            Assert.Equal(entity.AlunoId, viewModel.AlunoId);
            Assert.Equal(entity.InstrutorId, viewModel.InstrutorId);
            Assert.Equal(entity.DataHoraInicio, viewModel.DataHoraInicio);
            Assert.Equal(entity.DataHoraFim, viewModel.DataHoraFim);
            Assert.Equal(entity.Status, viewModel.Status);
            Assert.Equal(entity.Descricao, viewModel.Descricao);
            Assert.Null(viewModel.NomeAluno); // Ignorado
            Assert.Null(viewModel.NomeInstrutor); // Ignorado
        }

        // --- Testes de Mapeamento Específicos para Pagamento ---
        [Fact]
        public void PagamentoInputModel_To_Pagamento_ShouldMapCorrectly()
        {
            var inputModel = new PagamentoInputModel
            {
                AlunoId = Guid.NewGuid(),
                Valor = 150.00m,
                DataPagamento = DateTime.Now.Date,
                // CORRIGIDO: Usar o valor enum MetodoPagamento.CartaoCredito
                MetodoPagamento = MetodoPagamento.CartaoCredito,
                Observacoes = "Mensalidade do mês corrente",
                StatusPagamento = StatusPagamento.Pago
            };

            var entity = _mapper.Map<Pagamento>(inputModel);

            Assert.NotNull(entity); // Adicionado
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(inputModel.AlunoId, entity.AlunoId);
            Assert.Equal(inputModel.Valor, entity.Valor);
            Assert.Equal(inputModel.DataPagamento, entity.DataPagamento);
            // CORRIGIDO: Comparar com o valor enum MetodoPagamento.CartaoCredito
            Assert.Equal(MetodoPagamento.CartaoCredito, entity.MetodoPagamento);
            Assert.Equal(inputModel.Observacoes, entity.Observacoes);
            Assert.Equal(inputModel.StatusPagamento, entity.StatusPagamento);
        }

        [Fact]
        public void Pagamento_To_PagamentoViewModel_ShouldMapCorrectly()
        {
            var entity = new Pagamento(
                Guid.NewGuid(),
                Guid.NewGuid(),
                200.50m,
                DateTime.Now.AddDays(-5),
                // CORRIGIDO: Usar o valor enum MetodoPagamento.Pix
                MetodoPagamento.Pix,
                "Pagamento de aula avulsa",
                StatusPagamento.Pendente
            );

            var viewModel = _mapper.Map<PagamentoViewModel>(entity);

            Assert.NotNull(viewModel); // Adicionado
            Assert.Equal(entity.Id, viewModel.Id);
            Assert.Equal(entity.AlunoId, viewModel.AlunoId);
            Assert.Equal(entity.Valor, viewModel.Valor);
            Assert.Equal(entity.DataPagamento, viewModel.DataPagamento);
            // CORRIGIDO: Comparar com o valor enum MetodoPagamento.Pix
            Assert.Equal(MetodoPagamento.Pix, viewModel.MetodoPagamento);
            Assert.Equal(entity.Observacoes, viewModel.Observacoes);
            Assert.Equal(entity.StatusPagamento, viewModel.StatusPagamento);
            Assert.Null(viewModel.NomeAluno); // Ignorado
        }

        // --- Testes de Mapeamento Específicos para PlanoDeTreino ---
        [Fact]
        public void PlanoDeTreinoInputModel_To_PlanoDeTreino_ShouldMapCorrectly()
        {
            var inputModel = new PlanoDeTreinoInputModel
            {
                AlunoId = Guid.NewGuid(),
                InstrutorId = Guid.NewGuid(),
                NomePlano = "Plano de Ganho de Massa",
                Descricao = "Foco em exercícios compostos e dieta",
                DataInicio = DateTime.Now.Date,
                DataFim = DateTime.Now.Date.AddMonths(3),
                StatusPlano = StatusPlano.Ativo
            };

            var entity = _mapper.Map<PlanoDeTreino>(inputModel);

            Assert.NotNull(entity); // Adicionado
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(inputModel.AlunoId, entity.AlunoId);
            Assert.Equal(inputModel.InstrutorId, entity.InstrutorId);
            Assert.Equal(inputModel.NomePlano, entity.NomePlano);
            Assert.Equal(inputModel.Descricao, entity.Descricao);
            Assert.Equal(inputModel.DataInicio, entity.DataInicio);
            Assert.Equal(inputModel.DataFim, entity.DataFim);
            Assert.Equal(inputModel.StatusPlano, entity.Status); // Verifica o mapeamento StatusPlano -> Status
        }

        [Fact]
        public void PlanoDeTreinoUpdateModel_To_PlanoDeTreino_ShouldMapCorrectly()
        {
            var entityId = Guid.NewGuid();
            var existingAlunoId = Guid.NewGuid();
            var existingInstrutorId = Guid.NewGuid();

            var existingEntity = new PlanoDeTreino(
                entityId,
                existingAlunoId,
                existingInstrutorId,
                "Plano Antigo",
                "Desc Antiga",
                DateTime.Now.AddMonths(-1),
                DateTime.Now.AddMonths(1),
                StatusPlano.Ativo
            );

            var updateModel = new PlanoDeTreinoUpdateModel
            {
                // AlunoId e InstrutorId devem ser ignorados
                NomePlano = "Plano Novo Revisado",
                Descricao = "Nova descrição atualizada",
                DataInicio = DateTime.Now.AddDays(5),
                DataFim = DateTime.Now.AddMonths(6),
                StatusPlano = StatusPlano.Pausado
            };

            _mapper.Map(updateModel, existingEntity);

            Assert.Equal(entityId, existingEntity.Id); // ID não deve mudar
            Assert.Equal(existingAlunoId, existingEntity.AlunoId); // AlunoId não deve mudar
            Assert.Equal(existingInstrutorId, existingEntity.InstrutorId); // InstrutorId não deve mudar
            Assert.Equal(updateModel.NomePlano, existingEntity.NomePlano);
            Assert.Equal(updateModel.Descricao, existingEntity.Descricao);
            Assert.Equal(updateModel.DataInicio, existingEntity.DataInicio);
            Assert.Equal(updateModel.DataFim, existingEntity.DataFim);
            Assert.Equal(updateModel.StatusPlano, existingEntity.Status); // Verifica o mapeamento StatusPlano -> Status
        }

        [Fact]
        public void PlanoDeTreino_To_PlanoDeTreinoViewModel_ShouldMapCorrectly()
        {
            var entity = new PlanoDeTreino(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Plano Dieta",
                "Dieta Cutting",
                DateTime.Now.AddDays(-10),
                DateTime.Now.AddDays(20),
                StatusPlano.Expirado
            );

            var viewModel = _mapper.Map<PlanoDeTreinoViewModel>(entity);

            Assert.NotNull(viewModel); // Adicionado
            Assert.Equal(entity.Id, viewModel.Id);
            Assert.Equal(entity.AlunoId, viewModel.AlunoId);
            Assert.Equal(entity.InstrutorId, viewModel.InstrutorId);
            Assert.Equal(entity.NomePlano, viewModel.NomePlano);
            Assert.Equal(entity.Descricao, viewModel.Descricao);
            Assert.Equal(entity.DataInicio, viewModel.DataInicio);
            Assert.Equal(entity.DataFim, viewModel.DataFim);
            Assert.Equal(entity.Status, viewModel.StatusPlano); // Verifica o mapeamento Status -> StatusPlano
            Assert.Null(viewModel.NomeAluno); // Ignorado
            Assert.Null(viewModel.NomeInstrutor); // Ignorado
        }

        // --- Testes de Mapeamento Específicos para Administrador ---
        [Fact]
        public void AdministradorInputModel_To_Administrador_ShouldMapCorrectly()
        {
            var inputModel = new AdministradorInputModel
            {
                Nome = "Admin Teste",
                Cpf = "12345678900",
                Senha = "senhaforte123" // Esta senha não é mapeada para SenhaHash pelo AutoMapper
            };

            var entity = _mapper.Map<Administrador>(inputModel);

            Assert.NotNull(entity); // Adicionado
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(inputModel.Nome, entity.Nome);
            Assert.Equal(inputModel.Cpf, entity.Cpf);
            // CORREÇÃO AQUI: Espera-se o placeholder, não null
            Assert.Equal("placeholderHashForValidation", entity.SenhaHash);
            Assert.Equal(StatusAdministrador.Ativo, entity.Status); // Status padrão
            Assert.Equal(UserRole.Administrador, entity.Role); // Role padrão
        }

        [Fact]
        public void Administrador_To_AdministradorViewModel_ShouldMapCorrectly()
        {
            var entity = new Administrador(
                Guid.NewGuid(),
                "Admin View",
                "hashdaSenha123",
                StatusAdministrador.Bloqueado,
                UserRole.Administrador,
                "09876543210"
            );

            var viewModel = _mapper.Map<AdministradorViewModel>(entity);

            Assert.NotNull(viewModel); // Adicionado
            Assert.Equal(entity.Id, viewModel.Id);
            Assert.Equal(entity.Nome, viewModel.Nome);
            Assert.Equal(entity.Cpf, viewModel.Cpf);
            Assert.Equal(entity.Status, viewModel.Status);
            Assert.Equal(entity.Role, viewModel.Role);
            // SenhaHash não deve existir no ViewModel, implicitamente testado pela ausência da propriedade
        }

        // --- Testes de Mapeamento Específicos para Aluno ---
        [Fact]
        public void AlunoInputModel_To_Aluno_ShouldMapCorrectly()
        {
            var inputModel = new AlunoInputModel
            {
                Nome = "Aluno Teste",
                Senha = "alunosenha",
                Cpf = "11122233344",
                DataNascimento = new DateTime(2000, 1, 1),
                Telefone = "9912345678"
            };

            var entity = _mapper.Map<Aluno>(inputModel);

            Assert.NotNull(entity); // Adicionado
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(inputModel.Nome, entity.Nome);
            // CORREÇÃO AQUI: Espera-se o placeholder, não null
            Assert.Equal("placeholderHashForValidation", entity.SenhaHash);
            Assert.Equal(inputModel.Cpf, entity.Cpf);
            Assert.Equal(inputModel.DataNascimento, entity.DataNascimento);
            Assert.Equal(inputModel.Telefone, entity.Telefone);
            Assert.Equal(StatusAluno.Pendente, entity.Status); // Status padrão
            Assert.Equal(UserRole.Aluno, entity.Role); // Role padrão
        }

        [Fact]
        public void Aluno_To_AlunoViewModel_ShouldMapCorrectly()
        {
            var entity = new Aluno(
                Guid.NewGuid(),
                "Aluno View",
                "hashaluno",
                StatusAluno.Trancado,
                "55544433322",
                new DateTime(1995, 5, 10),
                "9876543210",
                UserRole.Aluno
            );

            var viewModel = _mapper.Map<AlunoViewModel>(entity);

            Assert.NotNull(viewModel); // Adicionado
            Assert.Equal(entity.Id, viewModel.Id);
            Assert.Equal(entity.Nome, viewModel.Nome);
            Assert.Equal(entity.Cpf, viewModel.Cpf);
            Assert.Equal(entity.DataNascimento, viewModel.DataNascimento);
            Assert.Equal(entity.Telefone, viewModel.Telefone);
            Assert.Equal(entity.Status, viewModel.Status);
            Assert.Equal(entity.Role, viewModel.Role);
        }

        // --- Testes de Mapeamento Específicos para Instrutor ---
        [Fact]
        public void InstrutorInputModel_To_Instrutor_ShouldMapCorrectly()
        {
            var inputModel = new InstrutorInputModel
            {
                Nome = "Instrutor Exemplo",
                Cpf = "22233344455",
                Cref = "123456-G/SP",
                Senha = "instrutorsenha",
                Status = StatusInstrutor.Ferias
            };

            var entity = _mapper.Map<Instrutor>(inputModel);

            Assert.NotNull(entity); // Adicionado
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(inputModel.Nome, entity.Nome);
            Assert.Equal(inputModel.Cpf, entity.Cpf);
            Assert.Equal(inputModel.Cref, entity.Cref);
            // CORREÇÃO AQUI: Espera-se o placeholder, não null
            Assert.Equal("placeholderHashForValidation", entity.SenhaHash);
            Assert.Equal(inputModel.Status, entity.Status); // Status do DTO
            Assert.Equal(UserRole.Instrutor, entity.Role); // Role padrão
        }

        [Fact]
        public void Instrutor_To_InstrutorViewModel_ShouldMapCorrectly()
        {
            var entity = new Instrutor(
                Guid.NewGuid(),
                "Instrutor View",
                "99988877766",
                "hashinstrutor",
                StatusInstrutor.Afastado,
                "654321-P/RJ",
                UserRole.Instrutor
            );

            var viewModel = _mapper.Map<InstrutorViewModel>(entity);

            Assert.NotNull(viewModel); // Adicionado
            Assert.Equal(entity.Id, viewModel.Id);
            Assert.Equal(entity.Nome, viewModel.Nome);
            Assert.Equal(entity.Cpf, viewModel.Cpf);
            Assert.Equal(entity.Cref, viewModel.Cref);
            Assert.Equal(entity.Status, viewModel.Status);
            Assert.Equal(entity.Role, viewModel.Role);
        }

        // --- Testes de Mapeamento Específicos para Recepcionista ---
        [Fact]
        public void RecepcionistaInputModel_To_Recepcionista_ShouldMapCorrectly()
        {
            var inputModel = new RecepcionistaInputModel
            {
                Nome = "Recepcionista Maria",
                Cpf = "33344455566",
                Senha = "recepcionistasenha"
            };

            var entity = _mapper.Map<Recepcionista>(inputModel);

            Assert.NotNull(entity); // Adicionado
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(inputModel.Nome, entity.Nome);
            Assert.Equal(inputModel.Cpf, entity.Cpf);
            // CORREÇÃO AQUI: Espera-se o placeholder, não null
            Assert.Equal("placeholderHashForValidation", entity.SenhaHash);
            Assert.Equal(StatusRecepcionista.Ativo, entity.Status); // Status padrão
            Assert.Equal(UserRole.Recepcionista, entity.Role); // Role padrão
        }

        [Fact]
        public void Recepcionista_To_RecepcionistaViewModel_ShouldMapCorrectly()
        {
            var entity = new Recepcionista(
                Guid.NewGuid(),
                "Recepcionista Joana",
                "77766655544",
                "hashrecepcionista"
            );

            var viewModel = _mapper.Map<RecepcionistaViewModel>(entity);

            Assert.NotNull(viewModel); // Adicionado
            Assert.Equal(entity.Id, viewModel.Id);
            Assert.Equal(entity.Nome, viewModel.Nome);
            Assert.Equal(entity.Cpf, viewModel.Cpf);
            Assert.Equal(entity.Status, viewModel.Status);
            Assert.Equal(entity.Role, viewModel.Role);
        }
    }
}