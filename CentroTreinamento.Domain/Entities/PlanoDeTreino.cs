using System;
using CentroTreinamento.Domain.Enums;
using System.Collections.Generic;

namespace CentroTreinamento.Domain.Entities
{
    public class PlanoDeTreino
    {
        public Guid Id { get; private set; }
        public Guid AlunoId { get; private set; }
        public Guid InstrutorId { get; private set; } // <<<< AQUI! Adicionada a propriedade
        public string? NomePlano { get; private set; }
        public string? Descricao { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }
        public StatusPlano Status { get; private set; }

        public PlanoDeTreino() { }

        // CONSTRUTOR ATUALIZADO PARA INCLUIR instrutorId (8 ARGUMENTOS AGORA)
        public PlanoDeTreino(Guid id, Guid alunoId, Guid instrutorId, string nomePlano, string descricao, DateTime dataInicio, DateTime dataFim, StatusPlano status)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("ID do plano de treino não pode ser vazio.", nameof(id));
            }
            if (alunoId == Guid.Empty)
            {
                throw new ArgumentException("ID do aluno associado ao plano não pode ser vazio.", nameof(alunoId));
            }
            if (instrutorId == Guid.Empty) // <<<< NOVA VALIDAÇÃO
            {
                throw new ArgumentException("ID do instrutor associado ao plano não pode ser vazio.", nameof(instrutorId));
            }
            if (string.IsNullOrWhiteSpace(nomePlano))
            {
                throw new ArgumentException("Nome do plano não pode ser vazio.", nameof(nomePlano));
            }
            if (dataInicio == default(DateTime))
            {
                throw new ArgumentException("Data de início do plano não pode ser vazia.", nameof(dataInicio));
            }
            if (dataFim == default(DateTime) || dataFim < dataInicio)
            {
                throw new ArgumentException("Data de fim do plano é inválida ou anterior à data de início.", nameof(dataFim));
            }

            Id = id;
            AlunoId = alunoId;
            InstrutorId = instrutorId; // <<<< ATRIBUÍDO AQUI
            NomePlano = nomePlano;
            Descricao = descricao;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Status = status;
        }

        // Métodos de domínio (comportamentos)
        public void AtualizarStatus(StatusPlano novoStatus)
        {
            this.Status = novoStatus;
        }

        public void AtualizarPeriodo(DateTime novaDataFim)
        {
            if (novaDataFim < DataInicio)
            {
                throw new ArgumentException("Nova data de fim não pode ser anterior à data de início.", nameof(novaDataFim));
            }
            DataFim = novaDataFim;
        }

        public void AtualizarNomeEDescricao(string novoNome, string novaDescricao)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
            {
                throw new ArgumentException("Nome do plano não pode ser vazio.", nameof(novoNome));
            }
            NomePlano = novoNome;
            Descricao = novaDescricao;
        }

        public override string ToString()
        {
            return $"PlanoDeTreino{{ Id={Id}, AlunoId={AlunoId}, InstrutorId={InstrutorId}, Nome='{NomePlano}', Status='{Status}', Inicio={DataInicio.ToShortDateString()}, Fim={DataFim.ToShortDateString()} }}";
        }
    }
}