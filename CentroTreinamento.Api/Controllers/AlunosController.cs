using CentroTreinamento.Application.DTOs.Aluno;
using CentroTreinamento.Application.DTOs.Agendamento; 
using CentroTreinamento.Application.Interfaces.Services;
using CentroTreinamento.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims; //Para acessar claims do usuário autenticado

namespace CentroTreinamento.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlunosController : ControllerBase
    {
        private readonly IAlunoAppService _alunoAppService;
        private readonly IAgendamentoAppService _agendamentoAppService; // <<<<< NOVO: Injetar IAgendamentoAppService

        public AlunosController(IAlunoAppService alunoAppService,
                                IAgendamentoAppService agendamentoAppService) // <<<<< NOVO: Adicionar ao construtor
        {
            _alunoAppService = alunoAppService;
            _agendamentoAppService = agendamentoAppService; // <<<<< NOVO: Atribuir
        }

        // --- Existing Aluno Endpoints (mantidos como estão) ---

        /// <summary>
        /// Obtém todos os alunos cadastrados.
        /// </summary>
        /// <returns>Uma lista de AlunoViewModel.</returns>
        [HttpGet]
        [Authorize(Roles = "Administrador, Recepcionista, Instrutor")] // Definindo quem pode listar todos os alunos
        public async Task<ActionResult<IEnumerable<AlunoViewModel>>> Get()
        {
            var alunos = await _alunoAppService.GetAllAlunosAsync();
            return Ok(alunos);
        }

        /// <summary>
        /// Obtém um aluno específico pelo seu ID.
        /// </summary>
        /// <param name="id">O GUID do aluno.</param>
        /// <returns>O AlunoViewModel correspondente ou NotFound se não existir.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador, Recepcionista, Instrutor, Aluno")] // Aluno pode ver seu próprio perfil
        public async Task<ActionResult<AlunoViewModel>> Get(Guid id)
        {
            // Lógica de autorização por recurso: Aluno só pode ver seu próprio perfil
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Aluno") && (userIdClaim == null || Guid.Parse(userIdClaim.Value) != id))
            {
                return Forbid("Um aluno pode consultar apenas seus próprios dados.");
            }

            var aluno = await _alunoAppService.GetAlunoByIdAsync(id);
            if (aluno == null)
            {
                return NotFound();
            }
            return Ok(aluno);
        }

        /// <summary>
        /// Cria um novo aluno.
        /// </summary>
        /// <param name="alunoInput">Dados do novo aluno.</param>
        /// <returns>O AlunoViewModel do aluno criado e a URL para acessá-lo.</returns>
        [HttpPost]
        [Authorize(Roles = "Administrador, Recepcionista")] // Apenas administradores e recepcionistas criam alunos
        public async Task<ActionResult<AlunoViewModel>> Post([FromBody] AlunoInputModel alunoInput)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var novoAluno = await _alunoAppService.CreateAlunoAsync(alunoInput);
                return CreatedAtAction(nameof(Get), new { id = novoAluno.Id }, novoAluno);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza um aluno existente.
        /// </summary>
        /// <param name="id">O GUID do aluno a ser atualizado.</param>
        /// <param name="alunoInput">Novos dados do aluno.</param>
        /// <returns>NoContent se a atualização for bem-sucedida, NotFound se o aluno não existir.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador, Recepcionista, Aluno")] // Aluno pode atualizar a si mesmo
        public async Task<ActionResult<AlunoViewModel>> Put(Guid id, [FromBody] AlunoInputModel inputModel)
        {
            // Lógica de autorização por recurso: Aluno só pode atualizar seu próprio perfil
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Aluno") && (userIdClaim == null || Guid.Parse(userIdClaim.Value) != id))
            {
                return Forbid("Um aluno pode atualizar apenas seus próprios dados.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedAluno = await _alunoAppService.UpdateAlunoAsync(id, inputModel);
                if (updatedAluno == null)
                {
                    return NotFound($"Aluno com ID {id} não encontrado.");
                }
                return Ok(updatedAluno);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza o status de um aluno (Ativo, Inativo, Suspenso, etc.).
        /// </summary>
        /// <param name="id">O GUID do aluno.</param>
        /// <param name="novoStatus">O novo status a ser atribuído.</param>
        /// <returns>NoContent se a atualização for bem-sucedida, NotFound se o aluno não existir, ou BadRequest se o status for inválido.</returns>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Administrador, Recepcionista")] // Apenas administradores e recepcionistas mudam status
        public async Task<ActionResult> UpdateStatus(Guid id, [FromQuery] StatusAluno novoStatus)
        {
            if (!Enum.IsDefined(typeof(StatusAluno), novoStatus))
            {
                return BadRequest("Status inválido.");
            }

            try
            {
                var updated = await _alunoAppService.UpdateAlunoStatusAsync(id, novoStatus);
                if (!updated)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Exclui um aluno.
        /// </summary>
        /// <param name="id">O GUID do aluno a ser excluído.</param>
        /// <returns>NoContent se a exclusão for bem-sucedida, NotFound se o aluno não existir.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")] // Apenas administradores podem excluir alunos
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _alunoAppService.DeleteAlunoAsync(id);
                if (!deleted)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // --- NEW Agendamento Endpoint for Aluno (RF3.1) ---

        /// <summary>
        /// Agendar um treino para o aluno logado.
        /// RF3.1: O Aluno agenda um treino.
        /// </summary>
        /// <param name="alunoId">ID do aluno que está agendando.</param>
        /// <param name="agendamentoInput">Dados do agendamento (data/hora, instrutorId, etc.).</param>
        /// <returns>O agendamento criado.</returns>
        [HttpPost("{alunoId}/agendamentos")] // Rota aninhada para agendamentos de um aluno específico
        [Authorize(Roles = "Aluno")] // Apenas o próprio aluno pode agendar um treino para si
        [ProducesResponseType(typeof(AgendamentoViewModel), 201)]
        [ProducesResponseType(typeof(string), 400)] // Bad Request (validação, aluno inadimplente, conflito de horário)
        [ProducesResponseType(typeof(string), 401)] // Unauthorized
        [ProducesResponseType(typeof(string), 403)] // Forbidden (se aluno tentar agendar para outro)
        [ProducesResponseType(typeof(string), 404)] // Not Found (instrutor não encontrado)
        [ProducesResponseType(typeof(string), 409)] // Conflict (horário já ocupado)
        [ProducesResponseType(typeof(string), 500)] // Internal Server Error
        public async Task<IActionResult> AgendarTreino(Guid alunoId, [FromBody] AgendamentoInputModel agendamentoInput)
        {
            // Validação de autorização: Um aluno só pode agendar para si mesmo.
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || Guid.Parse(userIdClaim.Value) != alunoId)
            {
                return Forbid("Um aluno pode agendar treinos apenas para si mesmo.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Sobrescrever o AlunoId no inputModel com o ID da rota para garantir que o agendamento
                // seja para o aluno correto e evitar que o cliente tente agendar para outro.
                agendamentoInput.AlunoId = alunoId;

                var novoAgendamento = await _agendamentoAppService.CriarAgendamentoAsync(agendamentoInput);

                // Retorna 201 Created com a localização do novo recurso
                return CreatedAtAction(
                    "GetAgendamentoById", // Nome do método no AgendamentosController para obter por ID
                    "Agendamentos",       // Nome do controlador sem "Controller"
                    new { id = novoAgendamento.Id },
                    novoAgendamento);
            }
            catch (ArgumentException ex) // Erros de validação (ex: instrutor não encontrado, datas inválidas)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) // Regras de negócio (ex: aluno inadimplente, horário ocupado)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                // TODO: Implementar logging
                return StatusCode(500, $"Erro interno ao agendar treino: {ex.Message}");
            }
        }
    }
}