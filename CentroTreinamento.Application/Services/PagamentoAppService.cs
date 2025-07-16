using System;
using System.Threading.Tasks;
using AutoMapper;
using CentroTreinamento.Application.DTOs.Pagamento;
using CentroTreinamento.Application.Interfaces.Services;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Repositories; // <<<<< ATENÇÃO: Namespace do seu IRepository
using CentroTreinamento.Domain.Enums; // Para usar StatusPagamento
using System.Collections.Generic;
using System.Linq; // Para usar métodos LINQ

namespace CentroTreinamento.Application.Services
{
    public class PagamentoAppService : IPagamentoAppService
    {
        private readonly IRepository<Pagamento> _pagamentoRepository;
        private readonly IRepository<Aluno> _alunoRepository; // Para verificar o aluno e nome
        private readonly IMapper _mapper;

        public PagamentoAppService(IRepository<Pagamento> pagamentoRepository, IRepository<Aluno> alunoRepository, IMapper mapper)
        {
            _pagamentoRepository = pagamentoRepository;
            _alunoRepository = alunoRepository;
            _mapper = mapper;
        }

        public async Task<PagamentoViewModel> RegistrarPagamentoAsync(PagamentoInputModel inputModel)
        {
            // 1. Validar se o Aluno existe
            var aluno = await _alunoRepository.GetByIdAsync(inputModel.AlunoId);
            if (aluno == null)
            {
                throw new ArgumentException($"Aluno com ID {inputModel.AlunoId} não encontrado.", nameof(inputModel.AlunoId));
            }

            // 2. Mapear DTO para Entidade
            // O mapeamento no MappingProfile já está gerando o ID e definindo o StatusPagamento
            var pagamento = _mapper.Map<Pagamento>(inputModel);

            // 3. Adicionar o pagamento
            await _pagamentoRepository.AddAsync(pagamento);
            await _pagamentoRepository.SaveChangesAsync(); // <<<<<<<<<< CHAMADA AQUI!

            // 4. Opcional: Atualizar status de inadimplência do Aluno após pagamento
            // Se o pagamento for registrado e o aluno estava inadimplente, pode ser marcado como adimplente
            // (Assumindo que a entidade Aluno tem um método para isso e a lógica para determinar inadimplência)
            // if (aluno.StatusInadimplencia == StatusInadimplencia.Inadimplente && pagamento.StatusPagamento == StatusPagamento.Pago)
            // {
            //     // aluno.MarcarComoAdimplente(); // Exemplo: você precisa implementar este método na entidade Aluno
            //     // _alunoRepository.Update(aluno);
            //     // await _alunoRepository.SaveChangesAsync();
            // }

            // 5. Mapear Entidade para ViewModel para retorno
            var viewModel = _mapper.Map<PagamentoViewModel>(pagamento);

            // Preencher NomeAluno manualmente, pois não temos propriedades de navegação no domínio
            viewModel.NomeAluno = aluno.Nome; // Assumindo que a entidade Aluno tem uma propriedade Nome

            return viewModel;
        }

        public async Task<PagamentoViewModel?> GetPagamentoByIdAsync(Guid id)
        {
            var pagamento = await _pagamentoRepository.GetByIdAsync(id);
            if (pagamento == null)
            {
                return null;
            }

            var viewModel = _mapper.Map<PagamentoViewModel>(pagamento);

            // Preencher NomeAluno
            var aluno = await _alunoRepository.GetByIdAsync(viewModel.AlunoId);
            if (aluno != null)
            {
                viewModel.NomeAluno = aluno.Nome;
            }

            return viewModel;
        }

        // Implementação para RF4.5: Consultar Histórico de Pagamentos do Aluno
        public async Task<IEnumerable<PagamentoViewModel>> GetPagamentosByAlunoIdAsync(Guid alunoId)
        {
            var pagamentos = await _pagamentoRepository.FindAsync(p => p.AlunoId == alunoId);

            var viewModels = new List<PagamentoViewModel>();
            foreach (var pagamento in pagamentos)
            {
                var viewModel = _mapper.Map<PagamentoViewModel>(pagamento);
                // Preencher NomeAluno para cada pagamento
                var aluno = await _alunoRepository.GetByIdAsync(pagamento.AlunoId); // Aluno já foi buscado, mas por segurança
                if (aluno != null)
                {
                    viewModel.NomeAluno = aluno.Nome;
                }
                viewModels.Add(viewModel);
            }
            return viewModels;
        }
    }
}