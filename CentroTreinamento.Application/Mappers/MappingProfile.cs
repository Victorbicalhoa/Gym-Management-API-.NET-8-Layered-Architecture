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

namespace CentroTreinamento.Application.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeamentos para Agendamento
            CreateMap<AgendamentoInputModel, Agendamento>()
                .ForCtorParam("id", opt => opt.MapFrom(src => Guid.NewGuid())) // Gera um novo GUID para o construtor
                .ForCtorParam("dataHoraFim", opt => opt.MapFrom(src => src.DataHoraInicio.AddHours(1))) // Exemplo: Duração de 1 hora
                .ForCtorParam("status", opt => opt.MapFrom(src => StatusAgendamento.Pendente)); // Status inicial padrão

            CreateMap<AgendamentoUpdateModel, Agendamento>()
                 // Não mapeia o ID aqui, pois o objeto existente será atualizado
                 // Se o construtor da entidade Agendamento fosse usado para atualização, precisaríamos de mais lógica
                 // Assumindo que a atualização ocorrerá via setters ou métodos específicos na entidade.
                 // A melhor prática seria ter um método na entidade para atualizar as propriedades.
                 // Ex: .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                 ;

            CreateMap<Agendamento, AgendamentoViewModel>()
                .ForMember(dest => dest.NomeAluno, opt => opt.Ignore()) // Ignorar por enquanto, pois a entidade não tem Aluno.Nome direto
                .ForMember(dest => dest.NomeInstrutor, opt => opt.Ignore()) // Ignorar por enquanto
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())); // Mapear Enum para string

            // Mapeamentos para Pagamento
            CreateMap<PagamentoInputModel, Pagamento>()
                .ForCtorParam("id", opt => opt.MapFrom(src => Guid.NewGuid())); // Gera um novo GUID para o construtor

            CreateMap<Pagamento, PagamentoViewModel>()
                .ForMember(dest => dest.NomeAluno, opt => opt.Ignore()) // Ignorar por enquanto
                .ForMember(dest => dest.MetodoPagamento, opt => opt.Ignore()) // Supondo que MetodoPagamento não está na entidade ainda
                .ForMember(dest => dest.Observacoes, opt => opt.Ignore()) // Supondo que Observacoes não está na entidade ainda
                .ForMember(dest => dest.StatusPagamento, opt => opt.MapFrom(src => src.StatusPagamento.ToString())); // Mapear Enum para string

            // Mapeamentos para PlanoDeTreino
            CreateMap<PlanoDeTreinoInputModel, PlanoDeTreino>()
                .ForCtorParam("id", opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForCtorParam("alunoId", opt => opt.MapFrom(src => src.AlunoId)) // Já deveria ter mapeado
                .ForCtorParam("instrutorId", opt => opt.MapFrom(src => src.InstrutorId)) // <<<<< CERTIFIQUE-SE QUE ESTÁ AQUI
                .ForCtorParam("nomePlano", opt => opt.MapFrom(src => src.NomePlano))
                .ForCtorParam("descricao", opt => opt.MapFrom(src => src.Descricao))
                .ForCtorParam("dataInicio", opt => opt.MapFrom(src => src.DataInicio))
                .ForCtorParam("dataFim", opt => opt.MapFrom(src => src.DataFim))
                .ForCtorParam("status", opt => opt.MapFrom(src => src.Status));

            CreateMap<PlanoDeTreino, PlanoDeTreinoViewModel>()
                .ForMember(dest => dest.NomeAluno, opt => opt.Ignore()) // Ignorar por enquanto
                .ForMember(dest => dest.NomeInstrutor, opt => opt.Ignore()) // Ignorar por enquanto
                .ForMember(dest => dest.NomePlano, opt => opt.MapFrom(src => src.NomePlano)) // Mapeia para NomePlano da entidade
                .ForMember(dest => dest.StatusPlano, opt => opt.MapFrom(src => src.Status.ToString())); // Mapear Enum para string

            // Mapeamentos para Administrador
            CreateMap<AdministradorInputModel, Administrador>();
            CreateMap<Administrador, AdministradorViewModel>();

            // Mapeamentos para Aluno
            CreateMap<AlunoInputModel, Aluno>();
            CreateMap<Aluno, AlunoViewModel>();

            // Mapeamentos para Instrutor
            CreateMap<InstrutorInputModel, Instrutor>();
            CreateMap<Instrutor, InstrutorViewModel>();

            // Mapeamentos para Recepcionista
            CreateMap<RecepcionistaInputModel, Recepcionista>();
            CreateMap<Recepcionista, RecepcionistaViewModel>();
        }
    }
}