using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CentroTreinamento.Application.DTOs.PlanoDeTreino;
using CentroTreinamento.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization; // Para usar atributos de autorização
using System.Linq; // Para o .Any() no GetPlanosDeTreinoByAlunoId (opcional)
using System.Security.Claims; // Para acessar claims do usuário autenticado

namespace CentroTreinamento.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Rota base: /api/PlanosDeTreino
    public class PlanosDeTreinoController : ControllerBase
    {
        private readonly IPlanoDeTreinoAppService _planoDeTreinoAppService;

        public PlanosDeTreinoController(IPlanoDeTreinoAppService planoDeTreinoAppService)
        {
            _planoDeTreinoAppService = planoDeTreinoAppService;
        }

        /// <summary>
        /// Obtém um plano de treino pelo seu ID.
        /// </summary>
        /// <param name="id">ID do plano de treino.</param>
        /// <returns>O plano de treino correspondente.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador, Instrutor, Aluno")] // Quem pode ver detalhes do plano de treino
        [ProducesResponseType(typeof(PlanoDeTreinoViewModel), 200)]
        [ProducesResponseType(typeof(string), 404)] // Not Found
        [ProducesResponseType(typeof(string), 401)] // Unauthorized
        [ProducesResponseType(typeof(string), 403)] // Forbidden
        [ProducesResponseType(typeof(string), 500)] // Internal Server Error
        public async Task<IActionResult> GetPlanoDeTreinoById(Guid id)
        {
            try
            {
                var planoDeTreino = await _planoDeTreinoAppService.GetPlanoDeTreinoByIdAsync(id);
                if (planoDeTreino == null)
                {
                    return NotFound($"Plano de Treino com ID {id} não encontrado.");
                }

                // Lógica de autorização por recurso:
                // Se o usuário logado é um aluno, ele só pode ver os próprios planos.
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (User.IsInRole("Aluno") && (userIdClaim == null || Guid.Parse(userIdClaim.Value) != planoDeTreino.AlunoId))
                {
                    return Forbid("Um aluno pode consultar apenas seus próprios planos de treino.");
                }

                return Ok(planoDeTreino);
            }
            catch (Exception ex)
            {
                // TODO: Implementar logging para erros internos
                return StatusCode(500, "Erro interno ao obter plano de treino: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtém todos os planos de treino associados a um aluno.
        /// </summary>
        /// <param name="alunoId">ID do aluno.</param>
        /// <returns>Lista de planos de treino do aluno.</returns>
        [HttpGet("aluno/{alunoId}")]
        [Authorize(Roles = "Administrador, Instrutor, Aluno")] // Permite que o próprio aluno veja seus planos
        [ProducesResponseType(typeof(IEnumerable<PlanoDeTreinoViewModel>), 200)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetPlanosDeTreinoByAlunoId(Guid alunoId)
        {
            // Lógica de autorização por recurso:
            // Se o usuário logado é um aluno, ele só pode ver os próprios planos.
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Aluno") && (userIdClaim == null || Guid.Parse(userIdClaim.Value) != alunoId))
            {
                return Forbid("Um aluno pode consultar apenas seus próprios planos de treino.");
            }

            try
            {
                var planos = await _planoDeTreinoAppService.GetPlanosDeTreinoByAlunoIdAsync(alunoId);
                // Retorna 200 OK com uma lista vazia se não houver planos para o aluno.
                return Ok(planos);
            }
            catch (ArgumentException ex)
            {
                // Por exemplo, se o AlunoAppService/Repository for injetado aqui e retornar que o aluno não existe
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // TODO: Implementar logging
                return StatusCode(500, "Erro interno ao obter planos de treino do aluno: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtém o plano de treino ativo para um aluno específico.
        /// </summary>
        /// <param name="alunoId">ID do aluno.</param>
        /// <returns>O plano de treino ativo do aluno, se existir.</returns>
        [HttpGet("aluno/{alunoId}/ativo")]
        [Authorize(Roles = "Administrador, Instrutor, Aluno")]
        [ProducesResponseType(typeof(PlanoDeTreinoViewModel), 200)]
        [ProducesResponseType(typeof(string), 404)] // Not Found
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetPlanoDeTreinoAtivoByAlunoId(Guid alunoId)
        {
            // Lógica de autorização por recurso:
            // Se o usuário logado é um aluno, ele só pode ver os próprios planos.
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Aluno") && (userIdClaim == null || Guid.Parse(userIdClaim.Value) != alunoId))
            {
                return Forbid("Um aluno pode consultar apenas seus próprios planos de treino ativos.");
            }

            try
            {
                var planoAtivo = await _planoDeTreinoAppService.GetPlanoDeTreinoAtivoByAlunoIdAsync(alunoId);
                if (planoAtivo == null)
                {
                    return NotFound($"Nenhum plano de treino ativo encontrado para o aluno com ID {alunoId}.");
                }
                return Ok(planoAtivo);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // TODO: Implementar logging
                return StatusCode(500, "Erro interno ao obter plano de treino ativo: " + ex.Message);
            }
        }

        // Os métodos POST e PUT para Criar/Atualizar Planos de Treino
        // serão implementados no InstrutorController.
    }
}