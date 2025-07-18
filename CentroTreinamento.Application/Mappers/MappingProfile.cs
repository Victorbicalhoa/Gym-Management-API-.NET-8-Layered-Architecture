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
            // --- Mapeamentos para Agendamento ---
            CreateMap<AgendamentoInputModel, Agendamento>()
                .ForCtorParam("id", opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForCtorParam("alunoId", opt => opt.MapFrom(src => src.AlunoId))
                .ForCtorParam("instrutorId", opt => opt.MapFrom(src => src.InstrutorId))
                .ForCtorParam("dataHoraInicio", opt => opt.MapFrom(src => src.DataHoraInicio))
                .ForCtorParam("dataHoraFim", opt => opt.MapFrom(src => src.DataHoraInicio.AddHours(1)))
                .ForCtorParam("status", opt => opt.MapFrom(src => StatusAgendamento.Pendente))
                .ForCtorParam("descricao", opt => opt.MapFrom(src => src.Descricao));

            CreateMap<AgendamentoUpdateModel, Agendamento>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AlunoId, opt => opt.Ignore())
                .ForMember(dest => dest.InstrutorId, opt => opt.Ignore())
                .ForMember(dest => dest.DataHoraInicio, opt => opt.MapFrom(src => src.DataHoraInicio))
                .ForMember(dest => dest.DataHoraFim, opt => opt.MapFrom(src => src.DataHoraFim))
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<Agendamento, AgendamentoViewModel>()
                .ForMember(dest => dest.NomeAluno, opt => opt.Ignore())
                .ForMember(dest => dest.NomeInstrutor, opt => opt.Ignore());

            // --- Mapeamentos para Pagamento ---
            CreateMap<PagamentoInputModel, Pagamento>()
                .ForCtorParam("id", opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForCtorParam("alunoId", opt => opt.MapFrom(src => src.AlunoId))
                .ForCtorParam("valor", opt => opt.MapFrom(src => src.Valor))
                .ForCtorParam("dataPagamento", opt => opt.MapFrom(src => src.DataPagamento))
                .ForCtorParam("metodoPagamento", opt => opt.MapFrom(src => src.MetodoPagamento))
                .ForCtorParam("observacoes", opt => opt.MapFrom(src => src.Observacoes))
                .ForCtorParam("statusPagamento", opt => opt.MapFrom(src => src.StatusPagamento));

            CreateMap<Pagamento, PagamentoViewModel>()
                .ForMember(dest => dest.NomeAluno, opt => opt.Ignore());

            // --- Mapeamentos para PlanoDeTreino ---
            CreateMap<PlanoDeTreinoInputModel, PlanoDeTreino>()
                .ForCtorParam("id", opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForCtorParam("alunoId", opt => opt.MapFrom(src => src.AlunoId))
                .ForCtorParam("instrutorId", opt => opt.MapFrom(src => src.InstrutorId))
                .ForCtorParam("nomePlano", opt => opt.MapFrom(src => src.NomePlano))
                .ForCtorParam("descricao", opt => opt.MapFrom(src => src.Descricao))
                .ForCtorParam("dataInicio", opt => opt.MapFrom(src => src.DataInicio))
                .ForCtorParam("dataFim", opt => opt.MapFrom(src => src.DataFim))
                .ForCtorParam("status", opt => opt.MapFrom(src => src.StatusPlano));

            CreateMap<PlanoDeTreinoUpdateModel, PlanoDeTreino>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AlunoId, opt => opt.Ignore())
                .ForMember(dest => dest.InstrutorId, opt => opt.Ignore())
                .ForMember(dest => dest.NomePlano, opt => opt.MapFrom(src => src.NomePlano))
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
                .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => src.DataInicio))
                .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.StatusPlano));

            CreateMap<PlanoDeTreino, PlanoDeTreinoViewModel>()
                .ForMember(dest => dest.StatusPlano, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.NomeAluno, opt => opt.Ignore())
                .ForMember(dest => dest.NomeInstrutor, opt => opt.Ignore());

            // --- Mapeamentos para Administrador (Usando ConstructUsing e ForMember para ignorar) ---
            CreateMap<AdministradorInputModel, Administrador>()
                .ConstructUsing(src => new Administrador(
                    Guid.NewGuid(),
                    src.Nome!,
                    // Passamos um valor para o construtor, para que ele seja instanciado com sucesso.
                    // O valor real do SenhaHash virá do AppService.
                    "placeholderHashForValidation",
                    StatusAdministrador.Ativo, // Valor padrão
                    UserRole.Administrador,    // Valor padrão
                    src.Cpf
                ))
                // IGNORAR PROPRIEDADES QUE JÁ SÃO SETADAS PELO CONSTRUTOR OU NÃO VÊM DO INPUTMODEL
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SenhaHash, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<Administrador, AdministradorViewModel>();

            // --- Mapeamentos para Aluno (Usando ConstructUsing e ForMember para ignorar) ---
            CreateMap<AlunoInputModel, Aluno>()
                .ConstructUsing(src => new Aluno(
                    Guid.NewGuid(),
                    src.Nome!,
                    "placeholderHashForValidation",
                    StatusAluno.Pendente, // Valor padrão
                    src.Cpf!,
                    src.DataNascimento,
                    src.Telefone!,
                    UserRole.Aluno // Valor padrão
                ))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SenhaHash, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<Aluno, AlunoViewModel>();

            // --- Mapeamentos para Instrutor (Usando ConstructUsing e ForMember para ignorar) ---
            CreateMap<InstrutorInputModel, Instrutor>()
                .ConstructUsing(src => new Instrutor(
                    Guid.NewGuid(),
                    src.Nome,
                    src.Cpf,
                    "placeholderHashForValidation",
                    src.Status ?? StatusInstrutor.Ativo, // Status do DTO ou padrão
                    src.Cref,
                    UserRole.Instrutor // Valor padrão
                ))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SenhaHash, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<Instrutor, InstrutorViewModel>();

            // --- Mapeamentos para Recepcionista (Usando ConstructUsing e ForMember para ignorar) ---
            CreateMap<RecepcionistaInputModel, Recepcionista>()
                .ConstructUsing(src => new Recepcionista(
                    Guid.NewGuid(),
                    src.Nome,
                    src.Cpf,
                    "placeholderHashForValidation"
                ))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SenhaHash, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // Ignorar Status da entidade
                .ForMember(dest => dest.Role, opt => opt.Ignore());   // Ignorar Role da entidade

            CreateMap<Recepcionista, RecepcionistaViewModel>();
        }
    }
}