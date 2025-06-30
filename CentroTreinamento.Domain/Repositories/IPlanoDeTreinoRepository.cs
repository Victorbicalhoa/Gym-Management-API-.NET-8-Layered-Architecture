using CentroTreinamento.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroTreinamento.Domain.Repositories
{
    public interface IPlanoDeTreinoRepository : IRepository<PlanoDeTreino>
    {
        // Exemplo de métodos específicos para PlanoDeTreino
        Task<IEnumerable<PlanoDeTreino>> GetPlanosDeTreinoPorAlunoAsync(Guid alunoId);
        Task<IEnumerable<PlanoDeTreino>> GetPlanosDeTreinoAtivosAsync();
        // Se houver nome/tipo de plano único
        // Task<PlanoDeTreino?> GetByNomeAsync(string nome);
    }
}