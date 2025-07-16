using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CentroTreinamento.Application.DTOs.PlanoDeTreino;
using CentroTreinamento.Application.Interfaces.Services;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums; // Para StatusPlano
using CentroTreinamento.Domain.Repositories; // Namespace do seu IRepository

namespace CentroTreinamento.Application.Services
{
    public class PlanoDeTreinoAppService : IPlanoDeTreinoAppService
    {
        private readonly IRepository<PlanoDeTreino> _planoDeTreinoRepository;
        private readonly IRepository<Aluno> _alunoRepository; // Para obter nome do aluno
        private readonly IRepository<Instrutor> _instrutorRepository; // Para obter nome do instrutor
        private readonly IMapper _mapper;

        public PlanoDeTreinoAppService(
            IRepository<PlanoDeTreino> planoDeTreinoRepository,
            IRepository<Aluno> alunoRepository,
            IRepository<Instrutor> instrutorRepository,
            IMapper mapper)
        {
            _planoDeTreinoRepository = planoDeTreinoRepository;
            _alunoRepository = alunoRepository;
            _instrutorRepository = instrutorRepository;
            _mapper = mapper;
        }

        public async Task<PlanoDeTreinoViewModel> CriarPlanoDeTreinoAsync(PlanoDeTreinoInputModel inputModel)
        {
            // 1. Validar existência de Aluno e Instrutor
            var aluno = await _alunoRepository.GetByIdAsync(inputModel.AlunoId);
            if (aluno == null)
            {
                throw new ArgumentException($"Aluno com ID {inputModel.AlunoId} não encontrado.", nameof(inputModel.AlunoId));
            }

            var instrutor = await _instrutorRepository.GetByIdAsync(inputModel.InstrutorId);
            if (instrutor == null)
            {
                throw new ArgumentException($"Instrutor com ID {inputModel.InstrutorId} não encontrado.", nameof(inputModel.InstrutorId));
            }

            // 2. Mapear DTO para Entidade
            // O mapeamento no MappingProfile já cuida da geração do ID e mapeamento de Nome para NomePlano
            var planoDeTreino = _mapper.Map<PlanoDeTreino>(inputModel);

            // 3. Adicionar o plano de treino
            await _planoDeTreinoRepository.AddAsync(planoDeTreino);
            await _planoDeTreinoRepository.SaveChangesAsync();

            // 4. Mapear Entidade para ViewModel para retorno
            var viewModel = _mapper.Map<PlanoDeTreinoViewModel>(planoDeTreino);

            // 5. Preencher nomes (se não tiver propriedades de navegação no domínio)
            viewModel.NomeAluno = aluno.Nome;
            viewModel.NomeInstrutor = instrutor.Nome;

            return viewModel;
        }

        public async Task<PlanoDeTreinoViewModel> AtualizarPlanoDeTreinoAsync(Guid planoId, PlanoDeTreinoInputModel updateModel)
        {
            var planoExistente = await _planoDeTreinoRepository.GetByIdAsync(planoId);
            if (planoExistente == null)
            {
                throw new ArgumentException($"Plano de Treino com ID {planoId} não encontrado.");
            }

            // 1. Validar existência de Aluno e Instrutor (se os IDs mudarem na atualização)
            // Normalmente, não se muda o aluno/instrutor de um plano existente, mas se for permitido:
            if (planoExistente.AlunoId != updateModel.AlunoId)
            {
                var aluno = await _alunoRepository.GetByIdAsync(updateModel.AlunoId);
                if (aluno == null) throw new ArgumentException($"Novo Aluno com ID {updateModel.AlunoId} não encontrado.");
            }
            if (planoExistente.InstrutorId != updateModel.InstrutorId)
            {
                var instrutor = await _instrutorRepository.GetByIdAsync(updateModel.InstrutorId);
                if (instrutor == null) throw new ArgumentException($"Novo Instrutor com ID {updateModel.InstrutorId} não encontrado.");
            }

            // 2. Atualizar as propriedades da entidade existente
            // A sua entidade tem setters privados e métodos de atualização.
            // Precisamos chamar esses métodos para garantir a integridade do domínio.
            // O AutoMapper pode ser usado para mapear propriedades simples, mas para métodos:
            planoExistente.AtualizarNomeEDescricao(updateModel.NomePlano!, updateModel.Descricao!);
            planoExistente.AtualizarPeriodo(updateModel.DataFim); // Supondo que você queira atualizar apenas a data de fim

            // Se você quiser atualizar o status, sua entidade tem um método para isso:
            planoExistente.AtualizarStatus(updateModel.Status);

            // Se você não tiver métodos para cada atualização, pode ser assim (mas é menos DDD):
            // _mapper.Map(updateModel, planoExistente); // Mapeia as propriedades do DTO para a entidade existente

            // 3. Salvar as alterações
            _planoDeTreinoRepository.Update(planoExistente);
            await _planoDeTreinoRepository.SaveChangesAsync();

            // 4. Mapear para ViewModel
            var viewModel = _mapper.Map<PlanoDeTreinoViewModel>(planoExistente);

            // Preencher nomes
            viewModel.NomeAluno = (await _alunoRepository.GetByIdAsync(planoExistente.AlunoId))?.Nome;
            viewModel.NomeInstrutor = (await _instrutorRepository.GetByIdAsync(planoExistente.InstrutorId))?.Nome;

            return viewModel;
        }

        public async Task<PlanoDeTreinoViewModel?> GetPlanoDeTreinoByIdAsync(Guid id)
        {
            var plano = await _planoDeTreinoRepository.GetByIdAsync(id);
            if (plano == null)
            {
                return null;
            }

            var viewModel = _mapper.Map<PlanoDeTreinoViewModel>(plano);

            // Preencher nomes
            viewModel.NomeAluno = (await _alunoRepository.GetByIdAsync(plano.AlunoId))?.Nome;
            viewModel.NomeInstrutor = (await _instrutorRepository.GetByIdAsync(plano.InstrutorId))?.Nome;

            return viewModel;
        }

        public async Task<IEnumerable<PlanoDeTreinoViewModel>> GetPlanosDeTreinoByAlunoIdAsync(Guid alunoId)
        {
            var planos = await _planoDeTreinoRepository.FindAsync(p => p.AlunoId == alunoId);
            var viewModels = new List<PlanoDeTreinoViewModel>();

            var aluno = await _alunoRepository.GetByIdAsync(alunoId); // Busca o aluno uma vez
            string alunoNome = aluno?.Nome ?? "N/A"; // Obtém o nome do aluno

            foreach (var plano in planos)
            {
                var viewModel = _mapper.Map<PlanoDeTreinoViewModel>(plano);
                viewModel.NomeAluno = alunoNome; // Atribui o nome do aluno
                viewModel.NomeInstrutor = (await _instrutorRepository.GetByIdAsync(plano.InstrutorId))?.Nome; // Busca o nome do instrutor para cada plano
                viewModels.Add(viewModel);
            }
            return viewModels;
        }

        public async Task<PlanoDeTreinoViewModel?> GetPlanoDeTreinoAtivoByAlunoIdAsync(Guid alunoId)
        {
            // Busca planos para o aluno que estejam com status "Ativo" (ou o que você definir como ativo)
            // E cuja data de fim seja no futuro ou nula (se você permitir planos sem data de fim)
            var planoAtivo = (await _planoDeTreinoRepository.FindAsync(p =>
                p.AlunoId == alunoId &&
                p.Status == StatusPlano.Ativo && // Assumindo que você tem um enum para StatusPlano
                p.DataFim >= DateTime.Today // Verifica se o plano ainda não expirou
                )).FirstOrDefault();

            if (planoAtivo == null)
            {
                return null;
            }

            var viewModel = _mapper.Map<PlanoDeTreinoViewModel>(planoAtivo);

            // Preencher nomes
            viewModel.NomeAluno = (await _alunoRepository.GetByIdAsync(planoAtivo.AlunoId))?.Nome;
            viewModel.NomeInstrutor = (await _instrutorRepository.GetByIdAsync(planoAtivo.InstrutorId))?.Nome;

            return viewModel;
        }
    }
}