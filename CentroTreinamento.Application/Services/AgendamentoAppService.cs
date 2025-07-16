using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CentroTreinamento.Application.DTOs.Agendamento;
using CentroTreinamento.Application.Interfaces.Services;
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Enums; // Para StatusAgendamento, StatusPagamento
using CentroTreinamento.Domain.Repositories; // Namespace do seu IRepository

namespace CentroTreinamento.Application.Services
{
    public class AgendamentoAppService : IAgendamentoAppService
    {
        private readonly IRepository<Agendamento> _agendamentoRepository;
        private readonly IRepository<Aluno> _alunoRepository;
        private readonly IRepository<Instrutor> _instrutorRepository;
        private readonly IRepository<Pagamento> _pagamentoRepository; // ADICIONADO PARA VERIFICAÇÃO DE INADIMPLÊNCIA
        private readonly IMapper _mapper;

        public AgendamentoAppService(
            IRepository<Agendamento> agendamentoRepository,
            IRepository<Aluno> alunoRepository,
            IRepository<Instrutor> instrutorRepository,
            IRepository<Pagamento> pagamentoRepository, // INJETAR AQUI
            IMapper mapper)
        {
            _agendamentoRepository = agendamentoRepository;
            _alunoRepository = alunoRepository;
            _instrutorRepository = instrutorRepository;
            _pagamentoRepository = pagamentoRepository; // ATRIBUIR AQUI
            _mapper = mapper;
        }

        public async Task<AgendamentoViewModel> CriarAgendamentoAsync(AgendamentoInputModel inputModel)
        {
            var aluno = await _alunoRepository.GetByIdAsync(inputModel.AlunoId);
            if (aluno == null)
            {
                throw new ArgumentException($"Aluno com ID {inputModel.AlunoId} não encontrado.", nameof(inputModel.AlunoId));
            }

            // RF4.3: Verificar inadimplência do Aluno com base nos pagamentos
            var pagamentosDoAluno = await _pagamentoRepository.FindAsync(p => p.AlunoId == inputModel.AlunoId);

            var estaInadimplente = pagamentosDoAluno.Any(p => p.StatusPagamento == StatusPagamento.Atrasado);

            if (estaInadimplente)
            {
                throw new InvalidOperationException($"Aluno {aluno.Nome} está inadimplente (possui pagamentos em atraso) e não pode agendar treinos.");
            }

            // Validar existência do Instrutor
            var instrutor = await _instrutorRepository.GetByIdAsync(inputModel.InstrutorId);
            if (instrutor == null)
            {
                throw new ArgumentException($"Instrutor com ID {inputModel.InstrutorId} não encontrado.", nameof(inputModel.InstrutorId));
            }

            // RF3.2: Verificar disponibilidade do Instrutor e do Aluno
            var dataHoraFim = inputModel.DataHoraInicio.AddHours(1); // Exemplo: agendamentos de 1 hora

            // Verificar conflitos para o Aluno
            var conflitosAluno = await _agendamentoRepository.FindAsync(a =>
                a.AlunoId == inputModel.AlunoId &&
                a.Status != StatusAgendamento.Cancelado &&
                a.Status != StatusAgendamento.Recusado &&
                (
                    (inputModel.DataHoraInicio < a.DataHoraFim && dataHoraFim > a.DataHoraInicio)
                ));
            if (conflitosAluno.Any())
            {
                throw new InvalidOperationException($"Aluno {aluno.Nome} já possui agendamento conflitante no período solicitado.");
            }

            // Verificar conflitos para o Instrutor
            var conflitosInstrutor = await _agendamentoRepository.FindAsync(a =>
                a.InstrutorId == inputModel.InstrutorId &&
                a.Status != StatusAgendamento.Cancelado &&
                a.Status != StatusAgendamento.Recusado &&
                (
                    (inputModel.DataHoraInicio < a.DataHoraFim && dataHoraFim > a.DataHoraInicio)
                ));
            if (conflitosInstrutor.Any())
            {
                throw new InvalidOperationException($"Instrutor {instrutor.Nome} não está disponível no período solicitado.");
            }

            // Mapear DTO para Entidade
            var agendamento = new Agendamento(
                Guid.NewGuid(),
                inputModel.AlunoId,
                inputModel.InstrutorId,
                inputModel.DataHoraInicio,
                dataHoraFim, // DataHoraFim calculada
                StatusAgendamento.Pendente, // Status inicial padrão para novos agendamentos
                inputModel.Descricao!
            );

            // Adicionar o agendamento
            await _agendamentoRepository.AddAsync(agendamento);
            await _agendamentoRepository.SaveChangesAsync();

            // Mapear Entidade para ViewModel para retorno
            var viewModel = _mapper.Map<AgendamentoViewModel>(agendamento);

            // Preencher nomes no ViewModel
            viewModel.NomeAluno = aluno.Nome;
            viewModel.NomeInstrutor = instrutor.Nome;

            return viewModel;
        }

        public async Task<AgendamentoViewModel> AtualizarAgendamentoAsync(Guid id, AgendamentoUpdateModel updateModel)
        {
            var agendamentoExistente = await _agendamentoRepository.GetByIdAsync(id);
            if (agendamentoExistente == null)
            {
                throw new ArgumentException($"Agendamento com ID {id} não encontrado.");
            }

            // 1. Validar existência de Aluno e Instrutor, se os IDs mudaram (improvável para update de agendamento)
            var aluno = await _alunoRepository.GetByIdAsync(updateModel.AlunoId);
            if (aluno == null) throw new ArgumentException($"Aluno com ID {updateModel.AlunoId} não encontrado.");

            var instrutor = await _instrutorRepository.GetByIdAsync(updateModel.InstrutorId);
            if (instrutor == null) throw new ArgumentException($"Instrutor com ID {updateModel.InstrutorId} não encontrado.");

            // 2. Verificar disponibilidade APENAS SE HOUVER ALTERAÇÃO DE HORÁRIO OU INSTRUTOR/ALUNO
            if (agendamentoExistente.DataHoraInicio != updateModel.DataHoraInicio ||
                agendamentoExistente.DataHoraFim != updateModel.DataHoraFim ||
                agendamentoExistente.AlunoId != updateModel.AlunoId ||
                agendamentoExistente.InstrutorId != updateModel.InstrutorId)
            {
                // Verificar conflitos, excluindo o próprio agendamento que está sendo atualizado
                var conflitosAluno = await _agendamentoRepository.FindAsync(a =>
                    a.Id != id && // Excluir o próprio agendamento
                    a.AlunoId == updateModel.AlunoId &&
                    a.Status != StatusAgendamento.Cancelado &&
                    a.Status != StatusAgendamento.Recusado &&
                    ((updateModel.DataHoraInicio < a.DataHoraFim && updateModel.DataHoraFim > a.DataHoraInicio))
                );
                if (conflitosAluno.Any())
                {
                    throw new InvalidOperationException($"Aluno {aluno.Nome} já possui agendamento conflitante no período solicitado para atualização.");
                }

                var conflitosInstrutor = await _agendamentoRepository.FindAsync(a =>
                    a.Id != id && // Excluir o próprio agendamento
                    a.InstrutorId == updateModel.InstrutorId &&
                    a.Status != StatusAgendamento.Cancelado &&
                    a.Status != StatusAgendamento.Recusado &&
                    ((updateModel.DataHoraInicio < a.DataHoraFim && updateModel.DataHoraFim > a.DataHoraInicio))
                );
                if (conflitosInstrutor.Any())
                {
                    throw new InvalidOperationException($"Instrutor {instrutor.Nome} não está disponível no período solicitado para atualização.");
                }
            }

            // 3. Atualizar a entidade existente usando os métodos de domínio
            agendamentoExistente.Remarcar(updateModel.DataHoraInicio, updateModel.DataHoraFim);
            agendamentoExistente.AtualizarStatus(updateModel.Status);
            // Se você quiser atualizar a descrição, e tiver um método para isso na entidade Agendamento:
            // agendamentoExistente.AtualizarDescricao(updateModel.Descricao);

            // 4. Salvar as alterações
            _agendamentoRepository.Update(agendamentoExistente);
            await _agendamentoRepository.SaveChangesAsync();

            // 5. Mapear para ViewModel
            var viewModel = _mapper.Map<AgendamentoViewModel>(agendamentoExistente);

            // Preencher nomes
            viewModel.NomeAluno = aluno.Nome;
            viewModel.NomeInstrutor = instrutor.Nome;

            return viewModel;
        }

        public async Task<AgendamentoViewModel?> GetAgendamentoByIdAsync(Guid id)
        {
            var agendamento = await _agendamentoRepository.GetByIdAsync(id);
            if (agendamento == null)
            {
                return null;
            }

            var viewModel = _mapper.Map<AgendamentoViewModel>(agendamento);

            // Preencher nomes
            viewModel.NomeAluno = (await _alunoRepository.GetByIdAsync(agendamento.AlunoId))?.Nome;
            viewModel.NomeInstrutor = (await _instrutorRepository.GetByIdAsync(agendamento.InstrutorId))?.Nome;

            return viewModel;
        }

        public async Task<IEnumerable<AgendamentoViewModel>> GetAgendamentosByAlunoIdAsync(Guid alunoId)
        {
            var agendamentos = await _agendamentoRepository.FindAsync(a => a.AlunoId == alunoId);
            var viewModels = new List<AgendamentoViewModel>();

            var aluno = await _alunoRepository.GetByIdAsync(alunoId);
            string alunoNome = aluno?.Nome ?? "N/A";

            foreach (var agendamento in agendamentos)
            {
                var viewModel = _mapper.Map<AgendamentoViewModel>(agendamento);
                viewModel.NomeAluno = alunoNome;
                viewModel.NomeInstrutor = (await _instrutorRepository.GetByIdAsync(agendamento.InstrutorId))?.Nome;
                viewModels.Add(viewModel);
            }
            return viewModels;
        }

        public async Task<IEnumerable<AgendamentoViewModel>> GetAgendamentosByInstrutorIdAsync(Guid instrutorId)
        {
            var agendamentos = await _agendamentoRepository.FindAsync(a => a.InstrutorId == instrutorId);
            var viewModels = new List<AgendamentoViewModel>();

            var instrutor = await _instrutorRepository.GetByIdAsync(instrutorId);
            string instrutorNome = instrutor?.Nome ?? "N/A";

            foreach (var agendamento in agendamentos)
            {
                var viewModel = _mapper.Map<AgendamentoViewModel>(agendamento);
                viewModel.NomeInstrutor = instrutorNome;
                viewModel.NomeAluno = (await _alunoRepository.GetByIdAsync(agendamento.AlunoId))?.Nome;
                viewModels.Add(viewModel);
            }
            return viewModels;
        }

        public async Task<IEnumerable<AgendamentoViewModel>> GetAllAgendamentosAsync()
        {
            var agendamentos = await _agendamentoRepository.GetAllAsync();
            var viewModels = new List<AgendamentoViewModel>();

            foreach (var agendamento in agendamentos)
            {
                var viewModel = _mapper.Map<AgendamentoViewModel>(agendamento);
                viewModel.NomeAluno = (await _alunoRepository.GetByIdAsync(agendamento.AlunoId))?.Nome;
                viewModel.NomeInstrutor = (await _instrutorRepository.GetByIdAsync(agendamento.InstrutorId))?.Nome;
                viewModels.Add(viewModel);
            }
            return viewModels;
        }

        public async Task<AgendamentoViewModel> AprovarAgendamentoAsync(Guid agendamentoId)
        {
            var agendamento = await _agendamentoRepository.GetByIdAsync(agendamentoId);
            if (agendamento == null)
            {
                throw new ArgumentException($"Agendamento com ID {agendamentoId} não encontrado.");
            }
            if (agendamento.Status != StatusAgendamento.Pendente)
            {
                throw new InvalidOperationException($"Agendamento ID {agendamentoId} não pode ser aprovado, pois seu status é '{agendamento.Status}'. Apenas agendamentos 'Pendente' podem ser aprovados.");
            }

            agendamento.AtualizarStatus(StatusAgendamento.Aprovado);
            _agendamentoRepository.Update(agendamento);
            await _agendamentoRepository.SaveChangesAsync();

            var viewModel = _mapper.Map<AgendamentoViewModel>(agendamento);
            viewModel.NomeAluno = (await _alunoRepository.GetByIdAsync(agendamento.AlunoId))?.Nome;
            viewModel.NomeInstrutor = (await _instrutorRepository.GetByIdAsync(agendamento.InstrutorId))?.Nome;
            return viewModel;
        }

        public async Task<AgendamentoViewModel> RecusarAgendamentoAsync(Guid agendamentoId, string motivo)
        {
            var agendamento = await _agendamentoRepository.GetByIdAsync(agendamentoId);
            if (agendamento == null)
            {
                throw new ArgumentException($"Agendamento com ID {agendamentoId} não encontrado.");
            }
            if (agendamento.Status != StatusAgendamento.Pendente)
            {
                throw new InvalidOperationException($"Agendamento ID {agendamentoId} não pode ser recusado, pois seu status é '{agendamento.Status}'. Apenas agendamentos 'Pendente' podem ser recusados.");
            }

            agendamento.AtualizarStatus(StatusAgendamento.Recusado);
            // Se a entidade Agendamento tivesse uma propriedade para 'motivoRecusa', você a preencheria aqui.
            // agendamento.DefinirMotivoRecusa(motivo);
            _agendamentoRepository.Update(agendamento);
            await _agendamentoRepository.SaveChangesAsync();

            var viewModel = _mapper.Map<AgendamentoViewModel>(agendamento);
            viewModel.NomeAluno = (await _alunoRepository.GetByIdAsync(agendamento.AlunoId))?.Nome;
            viewModel.NomeInstrutor = (await _instrutorRepository.GetByIdAsync(agendamento.InstrutorId))?.Nome;
            return viewModel;
        }

        public async Task<AgendamentoViewModel> CancelarAgendamentoAsync(Guid agendamentoId, string motivo)
        {
            var agendamento = await _agendamentoRepository.GetByIdAsync(agendamentoId);
            if (agendamento == null)
            {
                throw new ArgumentException($"Agendamento com ID {agendamentoId} não encontrado.");
            }
            if (agendamento.Status == StatusAgendamento.Cancelado || agendamento.Status == StatusAgendamento.Concluido)
            {
                throw new InvalidOperationException($"Agendamento ID {agendamentoId} não pode ser cancelado, pois seu status já é '{agendamento.Status}'.");
            }

            agendamento.AtualizarStatus(StatusAgendamento.Cancelado);
            // Se a entidade Agendamento tivesse uma propriedade para 'motivoCancelamento', você a preencheria aqui.
            // agendamento.DefinirMotivoCancelamento(motivo);
            _agendamentoRepository.Update(agendamento);
            await _agendamentoRepository.SaveChangesAsync();

            var viewModel = _mapper.Map<AgendamentoViewModel>(agendamento);
            viewModel.NomeAluno = (await _alunoRepository.GetByIdAsync(agendamento.AlunoId))?.Nome;
            viewModel.NomeInstrutor = (await _instrutorRepository.GetByIdAsync(agendamento.InstrutorId))?.Nome;
            return viewModel;
        }
    }
}