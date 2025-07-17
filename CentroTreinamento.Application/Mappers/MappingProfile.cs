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
using System;

namespace CentroTreinamento.Application.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeamentos para Agendamento
            CreateMap<AgendamentoInputModel, Agendamento>()
                .ForCtorParam("id", opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForCtorParam("dataHoraFim", opt => opt.MapFrom(src => src.DataHoraInicio.AddHours(1))) // Exemplo: Duração de 1 hora
                .ForCtorParam("status", opt => opt.MapFrom(src => StatusAgendamento.Pendente)); // Status inicial padrão

            CreateMap<AgendamentoUpdateModel, Agendamento>()
                .ForMember(dest => dest.DataHoraInicio, opt => opt.MapFrom(src => src.DataHoraInicio))
                .ForMember(dest => dest.DataHoraFim, opt => opt.MapFrom(src => src.DataHoraFim))
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)) // Mapear status do DTO
                .ForMember(dest => dest.AlunoId, opt => opt.Ignore()) // AppService deve validar/obter, não mapear do update model
                .ForMember(dest => dest.InstrutorId, opt => opt.Ignore()) // AppService deve validar/obter, não mapear do update model
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // O ID do update model não sobrescreve o ID da entidade

            CreateMap<Agendamento, AgendamentoViewModel>()
                .ForMember(dest => dest.NomeAluno, opt => opt.Ignore()) // Ignorar por enquanto, AppService preenche
                .ForMember(dest => dest.NomeInstrutor, opt => opt.Ignore()); // Ignorar por enquanto, AppService preenche
                                                                             // Status já é do tipo enum no ViewModel, então não precisa de .ToString() aqui.

            // Mapeamentos para Pagamento
            CreateMap<PagamentoInputModel, Pagamento>()
                .ForCtorParam("id", opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForCtorParam("statusPagamento", opt => opt.MapFrom(src => src.StatusPagamento)); // Mapear status do DTO

            CreateMap<Pagamento, PagamentoViewModel>()
                .ForMember(dest => dest.NomeAluno, opt => opt.Ignore()); // Ignorar por enquanto, AppService preenche
                                                                         // MetodoPagamento e Observacoes agora são propriedades na entidade Pagamento
                                                                         // e no PagamentoViewModel, então não precisamos ignorá-las no mapeamento,
                                                                         // a menos que você queira que o AppService as preencha de forma diferente.
                                                                         // Como elas estão presentes na entidade e no ViewModel, o AutoMapper as mapeará por padrão.
                                                                         // StatusPagamento já é do tipo enum no ViewModel, então não precisa de .ToString() aqui.

            // Mapeamentos para PlanoDeTreino
            CreateMap<PlanoDeTreinoInputModel, PlanoDeTreino>()
                .ForCtorParam("id", opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForCtorParam("alunoId", opt => opt.MapFrom(src => src.AlunoId))
                .ForCtorParam("instrutorId", opt => opt.MapFrom(src => src.InstrutorId))
                .ForCtorParam("nomePlano", opt => opt.MapFrom(src => src.NomePlano))
                .ForCtorParam("descricao", opt => opt.MapFrom(src => src.Descricao))
                .ForCtorParam("dataInicio", opt => opt.MapFrom(src => src.DataInicio))
                .ForCtorParam("dataFim", opt => opt.MapFrom(src => src.DataFim))
                .ForCtorParam("status", opt => opt.MapFrom(src => src.StatusPlano)); // Mapear status do DTO

            CreateMap<PlanoDeTreinoUpdateModel, PlanoDeTreino>()
                .ForMember(dest => dest.NomePlano, opt => opt.MapFrom(src => src.NomePlano))
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
                .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => src.DataInicio))
                .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.StatusPlano)) // Mapear status do DTO
                .ForMember(dest => dest.AlunoId, opt => opt.Ignore()) // Ignorar, AppService já lida com isso ou não deve ser atualizado.
                .ForMember(dest => dest.InstrutorId, opt => opt.Ignore()) // Ignorar, AppService já lida com isso.
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // O ID do update model não sobrescreve o ID da entidade

            CreateMap<PlanoDeTreino, PlanoDeTreinoViewModel>()
                .ForMember(dest => dest.NomeAluno, opt => opt.Ignore()) // Ignorar por enquanto, AppService preenche
                .ForMember(dest => dest.NomeInstrutor, opt => opt.Ignore()); // Ignorar por enquanto, AppService preenche
                                                                             // StatusPlano já é do tipo enum no ViewModel, então não precisa de .ToString() aqui.


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