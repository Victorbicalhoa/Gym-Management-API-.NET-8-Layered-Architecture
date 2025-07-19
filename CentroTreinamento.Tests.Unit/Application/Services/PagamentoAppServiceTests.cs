using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using CentroTreinamento.Application.DTOs.Pagamento;
using CentroTreinamento.Application.Services;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Repositories;
using AutoMapper;
using CentroTreinamento.Domain.Enums;
using System.Collections.Generic;
using System.Linq; // Para usar .Count() ou .Any() em IEnumerables

namespace CentroTreinamento.Tests.Unit.Application.Services
{
    public class PagamentoAppServiceTests
    {
        private readonly PagamentoAppService _pagamentoAppService;
        private readonly Mock<IRepository<Pagamento>> _mockPagamentoRepository;
        private readonly Mock<IRepository<Aluno>> _mockAlunoRepository;
        private readonly Mock<IMapper> _mockMapper;

        public PagamentoAppServiceTests()
        {
            _mockPagamentoRepository = new Mock<IRepository<Pagamento>>();
            _mockAlunoRepository = new Mock<IRepository<Aluno>>();
            _mockMapper = new Mock<IMapper>();

            _pagamentoAppService = new PagamentoAppService(
                _mockPagamentoRepository.Object,
                _mockAlunoRepository.Object,
                _mockMapper.Object
            );
        }

        // --- Testes para RegistrarPagamentoAsync ---
        [Fact]
        public async Task RegistrarPagamentoAsync_ShouldReturnViewModel_WhenValidData()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var valor = 150.0m;
            var dataPagamento = DateTime.UtcNow;
            // CORREÇÃO: Usar o enum MetodoPagamento
            var metodoPagamento = MetodoPagamento.CartaoCredito;
            var observacoes = "Mensalidade de exemplo";

            var inputModel = new PagamentoInputModel
            {
                AlunoId = alunoId,
                Valor = valor,
                DataPagamento = dataPagamento,
                MetodoPagamento = metodoPagamento,
                Observacoes = observacoes
            };

            var alunoMock = new Aluno(
                id: alunoId,
                nome: "Aluno Teste",
                senhaHash: "senhaHash123",
                status: StatusAluno.Ativo,
                cpf: "12345678900",
                dataNascimento: DateTime.Now.AddYears(-20),
                telefone: "12345678",
                role: UserRole.Aluno
            );

            // CORREÇÃO: Usar o enum MetodoPagamento no construtor da entidade
            var pagamentoEntityMock = new Pagamento(Guid.NewGuid(), alunoId, valor, dataPagamento, metodoPagamento, observacoes, StatusPagamento.Pago);

            var pagamentoViewModelMock = new PagamentoViewModel
            {
                Id = pagamentoEntityMock.Id,
                AlunoId = alunoId,
                NomeAluno = alunoMock.Nome,
                Valor = valor,
                DataPagamento = dataPagamento,
                // CORREÇÃO: Usar o enum MetodoPagamento
                MetodoPagamento = metodoPagamento,
                Observacoes = observacoes,
                StatusPagamento = StatusPagamento.Pago
            };

            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockPagamentoRepository.Setup(r => r.AddAsync(It.IsAny<Pagamento>())).Returns(Task.CompletedTask);
            _mockPagamentoRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            _mockMapper.Setup(m => m.Map<Pagamento>(inputModel)).Returns(pagamentoEntityMock);
            _mockMapper.Setup(m => m.Map<PagamentoViewModel>(pagamentoEntityMock)).Returns(pagamentoViewModelMock);

            // Act
            var result = await _pagamentoAppService.RegistrarPagamentoAsync(inputModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(inputModel.AlunoId, result.AlunoId);
            Assert.Equal(inputModel.Valor, result.Valor);
            Assert.Equal(inputModel.DataPagamento, result.DataPagamento);
            Assert.Equal(inputModel.MetodoPagamento, result.MetodoPagamento);
            Assert.Equal(inputModel.Observacoes, result.Observacoes);
            Assert.Equal(StatusPagamento.Pago, result.StatusPagamento);
            Assert.Equal(alunoMock.Nome, result.NomeAluno);

            _mockAlunoRepository.Verify(r => r.GetByIdAsync(alunoId), Times.Once);
            _mockPagamentoRepository.Verify(r => r.AddAsync(It.IsAny<Pagamento>()), Times.Once);
            _mockPagamentoRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<Pagamento>(inputModel), Times.Once);
            _mockMapper.Verify(m => m.Map<PagamentoViewModel>(pagamentoEntityMock), Times.Once);
        }

        [Fact]
        public async Task RegistrarPagamentoAsync_ShouldThrowArgumentException_WhenAlunoNotFound()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var inputModel = new PagamentoInputModel
            {
                AlunoId = alunoId,
                Valor = 100.0m,
                DataPagamento = DateTime.UtcNow,
                // CORREÇÃO: Usar o enum MetodoPagamento
                MetodoPagamento = MetodoPagamento.Dinheiro,
                Observacoes = "Teste"
            };

            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync((Aluno?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _pagamentoAppService.RegistrarPagamentoAsync(inputModel));

            Assert.Contains($"Aluno com ID {alunoId} não encontrado.", exception.Message);

            _mockPagamentoRepository.Verify(r => r.AddAsync(It.IsAny<Pagamento>()), Times.Never);
            _mockPagamentoRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
            _mockMapper.Verify(m => m.Map<Pagamento>(It.IsAny<PagamentoInputModel>()), Times.Never);
            _mockMapper.Verify(m => m.Map<PagamentoViewModel>(It.IsAny<Pagamento>()), Times.Never);
        }

        // --- Testes para GetPagamentoByIdAsync ---
        [Fact]
        public async Task GetPagamentoByIdAsync_ShouldReturnViewModel_WhenPagamentoExists()
        {
            // Arrange
            var pagamentoId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();
            var valor = 100m;
            var dataPagamento = DateTime.UtcNow;
            // CORREÇÃO: Usar o enum MetodoPagamento
            var metodoPagamento = MetodoPagamento.Pix;
            var observacoes = "Observações";
            var statusPagamento = StatusPagamento.Pago;

            // CORREÇÃO: Usar o enum MetodoPagamento no construtor da entidade
            var pagamentoEntityMock = new Pagamento(pagamentoId, alunoId, valor, dataPagamento, metodoPagamento, observacoes, statusPagamento);
            var alunoMock = new Aluno(
                id: alunoId,
                nome: "Aluno XYZ",
                senhaHash: "senhaHash456",
                status: StatusAluno.Ativo,
                cpf: "00000000000",
                dataNascimento: DateTime.Now.AddYears(-25),
                telefone: "123",
                role: UserRole.Aluno
            );

            var pagamentoViewModelMock = new PagamentoViewModel
            {
                Id = pagamentoId,
                AlunoId = alunoId,
                NomeAluno = alunoMock.Nome,
                Valor = valor,
                DataPagamento = dataPagamento,
                // CORREÇÃO: Usar o enum MetodoPagamento
                MetodoPagamento = metodoPagamento,
                Observacoes = observacoes,
                StatusPagamento = statusPagamento
            };

            _mockPagamentoRepository.Setup(r => r.GetByIdAsync(pagamentoId)).ReturnsAsync(pagamentoEntityMock);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);
            _mockMapper.Setup(m => m.Map<PagamentoViewModel>(pagamentoEntityMock)).Returns(pagamentoViewModelMock);

            // Act
            var result = await _pagamentoAppService.GetPagamentoByIdAsync(pagamentoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pagamentoId, result.Id);
            Assert.Equal(alunoMock.Nome, result.NomeAluno);
            Assert.Equal(valor, result.Valor);
            Assert.Equal(metodoPagamento, result.MetodoPagamento);
            Assert.Equal(statusPagamento, result.StatusPagamento);

            _mockPagamentoRepository.Verify(r => r.GetByIdAsync(pagamentoId), Times.Once);
            _mockAlunoRepository.Verify(r => r.GetByIdAsync(alunoId), Times.Once);
            _mockMapper.Verify(m => m.Map<PagamentoViewModel>(pagamentoEntityMock), Times.Once);
        }

        [Fact]
        public async Task GetPagamentoByIdAsync_ShouldReturnNull_WhenPagamentoDoesNotExist()
        {
            // Arrange
            var pagamentoId = Guid.NewGuid();
            _mockPagamentoRepository.Setup(r => r.GetByIdAsync(pagamentoId)).ReturnsAsync((Pagamento?)null);

            // Act
            var result = await _pagamentoAppService.GetPagamentoByIdAsync(pagamentoId);

            // Assert
            Assert.Null(result);
            _mockPagamentoRepository.Verify(r => r.GetByIdAsync(pagamentoId), Times.Once);
            _mockAlunoRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _mockMapper.Verify(m => m.Map<PagamentoViewModel>(It.IsAny<Pagamento>()), Times.Never);
        }

        // --- Testes para GetPagamentosByAlunoIdAsync ---
        [Fact]
        public async Task GetPagamentosByAlunoIdAsync_ShouldReturnViewModels_WhenPagamentosExist()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var alunoMock = new Aluno(
                id: alunoId,
                nome: "Aluno de Teste",
                senhaHash: "senhaHash789",
                status: StatusAluno.Ativo,
                cpf: "00000000000",
                dataNascimento: DateTime.Now.AddYears(-20),
                telefone: "Rua",
                role: UserRole.Aluno
            );

            var pagamentosEntityList = new List<Pagamento>
            {
                // CORREÇÃO: Usar o enum MetodoPagamento nos construtores
                new Pagamento(Guid.NewGuid(), alunoId, 100m, DateTime.UtcNow.AddDays(-10), MetodoPagamento.Dinheiro, "Mensalidade Jan", StatusPagamento.Pago),
                new Pagamento(Guid.NewGuid(), alunoId, 50m, DateTime.UtcNow.AddDays(-5), MetodoPagamento.Pix, "Multa Atraso", StatusPagamento.Pendente)
            };

            var pagamentosViewModelList = new List<PagamentoViewModel>
            {
                // CORREÇÃO: Usar o enum MetodoPagamento
                new PagamentoViewModel { Id = pagamentosEntityList[0].Id, AlunoId = alunoId, NomeAluno = alunoMock.Nome, Valor = 100m, DataPagamento = pagamentosEntityList[0].DataPagamento, MetodoPagamento = MetodoPagamento.Dinheiro, Observacoes = "Mensalidade Jan", StatusPagamento = StatusPagamento.Pago },
                new PagamentoViewModel { Id = pagamentosEntityList[1].Id, AlunoId = alunoId, NomeAluno = alunoMock.Nome, Valor = 50m, DataPagamento = pagamentosEntityList[1].DataPagamento, MetodoPagamento = MetodoPagamento.Pix, Observacoes = "Multa Atraso", StatusPagamento = StatusPagamento.Pendente }
            };

            _mockPagamentoRepository.Setup(r => r.FindAsync(p => p.AlunoId == alunoId)).ReturnsAsync(pagamentosEntityList);
            _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock);

            // Mockar o mapeamento para cada entidade na lista
            _mockMapper.Setup(m => m.Map<PagamentoViewModel>(pagamentosEntityList[0])).Returns(pagamentosViewModelList[0]);
            _mockMapper.Setup(m => m.Map<PagamentoViewModel>(pagamentosEntityList[1])).Returns(pagamentosViewModelList[1]);


            // Act
            var result = await _pagamentoAppService.GetPagamentosByAlunoIdAsync(alunoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal(alunoId, p.AlunoId));
            Assert.All(result, p => Assert.Equal(alunoMock.Nome, p.NomeAluno));

            _mockPagamentoRepository.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Pagamento, bool>>>()), Times.Once);
            // ALUNO É BUSCADO UMA VEZ PARA CADA PAGAMENTO NO LOOP. Se houver 2 pagamentos, 2x.
            _mockAlunoRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Exactly(pagamentosEntityList.Count));
            _mockMapper.Verify(m => m.Map<PagamentoViewModel>(It.IsAny<Pagamento>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetPagamentosByAlunoIdAsync_ShouldReturnEmptyList_WhenNoPagamentosExist()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var alunoMock = new Aluno(
                id: alunoId,
                nome: "Aluno Sem Pagamento",
                senhaHash: "hashNada",
                status: StatusAluno.Ativo,
                cpf: "00000000000",
                dataNascimento: DateTime.Now.AddYears(-20),
                telefone: "Rua Y",
                role: UserRole.Aluno
            );

            // Configura para retornar uma lista vazia de pagamentos
            _mockPagamentoRepository.Setup(r => r.FindAsync(p => p.AlunoId == alunoId)).ReturnsAsync(new List<Pagamento>());

            // O ALUNO PODE EXISTIR OU NÃO, mas o AppService NÃO TENTA BUSCÁ-LO SE NÃO HÁ PAGAMENTOS.
            // Não há necessidade de configurar o mock de aluno se ele nunca será chamado neste cenário.
            // _mockAlunoRepository.Setup(r => r.GetByIdAsync(alunoId)).ReturnsAsync(alunoMock); // Podemos remover este setup para ser mais preciso

            // Act
            var result = await _pagamentoAppService.GetPagamentosByAlunoIdAsync(alunoId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockPagamentoRepository.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Pagamento, bool>>>()), Times.Once);

            // >>>>>> CORREÇÃO AQUI <<<<<<
            // Como a lista de pagamentos é vazia, o loop não é executado, e GetByIdAsync para Aluno nunca é chamado.
            _mockAlunoRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            // >>>>>> FIM DA CORREÇÃO <<<<<<

            _mockMapper.Verify(m => m.Map<PagamentoViewModel>(It.IsAny<Pagamento>()), Times.Never);
        }
    }
}