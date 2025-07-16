using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CentroTreinamento.Application.DTOs.PlanoDeTreino;

namespace CentroTreinamento.Application.Interfaces.Services
{
    public interface IPlanoDeTreinoAppService
    {
        Task<PlanoDeTreinoViewModel> CriarPlanoDeTreinoAsync(PlanoDeTreinoInputModel inputModel);
        Task<PlanoDeTreinoViewModel> AtualizarPlanoDeTreinoAsync(Guid planoId, PlanoDeTreinoInputModel updateModel);
        Task<PlanoDeTreinoViewModel?> GetPlanoDeTreinoByIdAsync(Guid id);
        Task<IEnumerable<PlanoDeTreinoViewModel>> GetPlanosDeTreinoByAlunoIdAsync(Guid alunoId);
        Task<PlanoDeTreinoViewModel?> GetPlanoDeTreinoAtivoByAlunoIdAsync(Guid alunoId);
        // Você pode adicionar um método para listar todos os planos, ou filtrar por instrutor etc.
    }
}