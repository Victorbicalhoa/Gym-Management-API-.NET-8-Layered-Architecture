using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CentroTreinamento.Application.DTOs.Agendamento;
using CentroTreinamento.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization; // Para usar atributos de autorização
using System.Security.Claims; // Para acessar claims do usuário autenticado

namespace CentroTreinamento.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Rota base: /api/Agendamentos
    public class AgendamentosController : ControllerBase
    {
        private readonly IAgendamentoAppService _agendamentoAppService;

        public AgendamentosController(IAgendamentoAppService agendamentoAppService)
        {
            _agendamentoAppService = agendamentoAppService;
        }

        /// <summary>
        /// Obtém um agendamento pelo seu ID.
        /// </summary>
        /// <param name="id">ID do agendamento.</param>
        /// <returns>O agendamento correspondente.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador, Recepcionista, Instrutor, Aluno")]
        [ProducesResponseType(typeof(AgendamentoViewModel), 200)]
        [ProducesResponseType(typeof(string), 404)] // Not Found
        [ProducesResponseType(typeof(string), 401)] // Unauthorized
        [ProducesResponseType(typeof(string), 403)] // Forbidden
        [ProducesResponseType(typeof(string), 500)] // Internal Server Error
        public async Task<IActionResult> GetAgendamentoById(Guid id)
        {
            try
            {
                var agendamento = await _agendamentoAppService.GetAgendamentoByIdAsync(id);
                if (agendamento == null)
                {
                    return NotFound($"Agendamento com ID {id} não encontrado.");
                }

                // Lógica de autorização por recurso:
                // Aluno só pode ver seus próprios agendamentos.
                // Instrutor só pode ver agendamentos onde ele é o instrutor.
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    Guid userId = Guid.Parse(userIdClaim.Value);
                    if (User.IsInRole("Aluno") && userId != agendamento.AlunoId)
                    {
                        return Forbid("Um aluno pode consultar apenas seus próprios agendamentos.");
                    }
                    if (User.IsInRole("Instrutor") && userId != agendamento.InstrutorId)
                    {
                        return Forbid("Um instrutor pode consultar apenas agendamentos em que ele é o instrutor.");
                    }
                }

                return Ok(agendamento);
            }
            catch (Exception ex)
            {
                // TODO: Implementar logging
                return StatusCode(500, "Erro interno ao obter agendamento: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtém todos os agendamentos.
        /// </summary>
        /// <returns>Lista de todos os agendamentos.</returns>
        [HttpGet]
        [Authorize(Roles = "Administrador, Recepcionista")] // Geralmente, apenas administradores e recepcionistas veem todos
        [ProducesResponseType(typeof(IEnumerable<AgendamentoViewModel>), 200)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetAllAgendamentos()
        {
            try
            {
                var agendamentos = await _agendamentoAppService.GetAllAgendamentosAsync();
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno ao obter todos os agendamentos: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtém agendamentos de um aluno específico.
        /// </summary>
        /// <param name="alunoId">ID do aluno.</param>
        /// <returns>Lista de agendamentos do aluno.</returns>
        [HttpGet("aluno/{alunoId}")]
        [Authorize(Roles = "Administrador, Recepcionista, Instrutor, Aluno")]
        [ProducesResponseType(typeof(IEnumerable<AgendamentoViewModel>), 200)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetAgendamentosByAlunoId(Guid alunoId)
        {
            // Lógica de autorização por recurso: Aluno só pode ver seus próprios agendamentos.
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Aluno") && (userIdClaim == null || Guid.Parse(userIdClaim.Value) != alunoId))
            {
                return Forbid("Um aluno pode consultar apenas seus próprios agendamentos.");
            }
            // Instrutores/Administradores/Recepcionistas podem consultar de qualquer aluno.

            try
            {
                var agendamentos = await _agendamentoAppService.GetAgendamentosByAlunoIdAsync(alunoId);
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno ao obter agendamentos por aluno: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtém agendamentos de um instrutor específico.
        /// </summary>
        /// <param name="instrutorId">ID do instrutor.</param>
        /// <returns>Lista de agendamentos do instrutor.</returns>
        [HttpGet("instrutor/{instrutorId}")]
        [Authorize(Roles = "Administrador, Recepcionista, Instrutor")]
        [ProducesResponseType(typeof(IEnumerable<AgendamentoViewModel>), 200)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetAgendamentosByInstrutorId(Guid instrutorId)
        {
            // Lógica de autorização por recurso: Instrutor só pode ver seus próprios agendamentos.
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Instrutor") && (userIdClaim == null || Guid.Parse(userIdClaim.Value) != instrutorId))
            {
                return Forbid("Um instrutor pode consultar apenas seus próprios agendamentos.");
            }
            // Administradores/Recepcionistas podem consultar de qualquer instrutor.

            try
            {
                var agendamentos = await _agendamentoAppService.GetAgendamentosByInstrutorIdAsync(instrutorId);
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno ao obter agendamentos por instrutor: " + ex.Message);
            }
        }

        /// <summary>
        /// Aprova um agendamento.
        /// RF3.3: Instrutor ou Administrador aprova um agendamento.
        /// </summary>
        /// <param name="id">ID do agendamento a ser aprovado.</param>
        /// <returns>O agendamento aprovado.</returns>
        [HttpPatch("{id}/aprovar")] // PATCH é mais adequado para atualizar um sub-recurso/estado
        [Authorize(Roles = "Administrador, Instrutor")]
        [ProducesResponseType(typeof(AgendamentoViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)] // Bad Request (status inválido para aprovação)
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 404)] // Not Found
        [ProducesResponseType(typeof(string), 409)] // Conflict (se o status não permitir aprovação)
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> AprovarAgendamento(Guid id)
        {
            try
            {
                var agendamento = await _agendamentoAppService.GetAgendamentoByIdAsync(id);
                if (agendamento == null)
                {
                    return NotFound($"Agendamento com ID {id} não encontrado.");
                }

                // Lógica de autorização por recurso: Instrutor só pode aprovar seus próprios agendamentos.
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (User.IsInRole("Instrutor") && (userIdClaim == null || Guid.Parse(userIdClaim.Value) != agendamento.InstrutorId))
                {
                    return Forbid("Um instrutor pode aprovar apenas agendamentos em que ele é o instrutor.");
                }

                var aprovado = await _agendamentoAppService.AprovarAgendamentoAsync(id);
                return Ok(aprovado);
            }
            catch (ArgumentException ex) // Erros do AppService como 'não encontrado'
            {
                return BadRequest(ex.Message); // Ou NotFound se a exceção indicar isso.
            }
            catch (InvalidOperationException ex) // Regras de negócio (ex: status não pendente)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno ao aprovar agendamento: " + ex.Message);
            }
        }

        /// <summary>
        /// Recusa um agendamento.
        /// RF3.3: Instrutor ou Administrador recusa um agendamento.
        /// </summary>
        /// <param name="id">ID do agendamento a ser recusado.</param>
        /// <param name="motivo">Motivo da recusa.</param>
        /// <returns>O agendamento recusado.</returns>
        [HttpPatch("{id}/recusar")] // PATCH é mais adequado
        [Authorize(Roles = "Administrador, Instrutor")]
        [ProducesResponseType(typeof(AgendamentoViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 409)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> RecusarAgendamento(Guid id, [FromBody] string motivo) // Ou um DTO com { "motivo": "..." }
        {
            try
            {
                var agendamento = await _agendamentoAppService.GetAgendamentoByIdAsync(id);
                if (agendamento == null)
                {
                    return NotFound($"Agendamento com ID {id} não encontrado.");
                }

                // Lógica de autorização por recurso: Instrutor só pode recusar seus próprios agendamentos.
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (User.IsInRole("Instrutor") && (userIdClaim == null || Guid.Parse(userIdClaim.Value) != agendamento.InstrutorId))
                {
                    return Forbid("Um instrutor pode recusar apenas agendamentos em que ele é o instrutor.");
                }

                var recusado = await _agendamentoAppService.RecusarAgendamentoAsync(id, motivo);
                return Ok(recusado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno ao recusar agendamento: " + ex.Message);
            }
        }

        /// <summary>
        /// Cancela um agendamento.
        /// RF3.5: Aluno ou Administrador ou Recepcionista pode cancelar.
        /// </summary>
        /// <param name="id">ID do agendamento a ser cancelado.</param>
        /// <param name="motivo">Motivo do cancelamento.</param>
        /// <returns>O agendamento cancelado.</returns>
        [HttpPatch("{id}/cancelar")] // PATCH é mais adequado
        [Authorize(Roles = "Administrador, Recepcionista, Aluno")]
        [ProducesResponseType(typeof(AgendamentoViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 409)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> CancelarAgendamento(Guid id, [FromBody] string motivo) // Ou um DTO com { "motivo": "..." }
        {
            try
            {
                var agendamento = await _agendamentoAppService.GetAgendamentoByIdAsync(id);
                if (agendamento == null)
                {
                    return NotFound($"Agendamento com ID {id} não encontrado.");
                }

                // Lógica de autorização por recurso: Aluno só pode cancelar seus próprios agendamentos.
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (User.IsInRole("Aluno") && (userIdClaim == null || Guid.Parse(userIdClaim.Value) != agendamento.AlunoId))
                {
                    return Forbid("Um aluno pode cancelar apenas seus próprios agendamentos.");
                }
                // Administradores e Recepcionistas podem cancelar qualquer agendamento.

                var cancelado = await _agendamentoAppService.CancelarAgendamentoAsync(id, motivo);
                return Ok(cancelado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno ao cancelar agendamento: " + ex.Message);
            }
        }

        // Os métodos POST e PUT para Criar/Atualizar Agendamentos
        // serão implementados no AlunosController para criação
        // e, se necessário, um PUT geral aqui ou em outro controller.
        // Por enquanto, o foco é na criação por aluno e gerenciamento de status.
        // A atualização de um agendamento existente (remarcar, etc.) pode ser via PUT aqui.
        /// <summary>
        /// Atualiza um agendamento existente (e.g., remarcar).
        /// </summary>
        /// <param name="id">ID do agendamento a ser atualizado.</param>
        /// <param name="updateModel">Dados atualizados do agendamento.</param>
        /// <returns>O agendamento atualizado.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador, Recepcionista, Instrutor, Aluno")] // Quem pode atualizar
        [ProducesResponseType(typeof(AgendamentoViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 409)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> AtualizarAgendamento(Guid id, [FromBody] AgendamentoUpdateModel updateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var agendamentoExistente = await _agendamentoAppService.GetAgendamentoByIdAsync(id);
                if (agendamentoExistente == null)
                {
                    return NotFound($"Agendamento com ID {id} não encontrado.");
                }

                // Lógica de autorização por recurso para atualização:
                // Aluno só pode atualizar seus próprios agendamentos.
                // Instrutor só pode atualizar agendamentos em que ele é o instrutor.
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    Guid userId = Guid.Parse(userIdClaim.Value);
                    if (User.IsInRole("Aluno") && userId != agendamentoExistente.AlunoId)
                    {
                        return Forbid("Um aluno pode atualizar apenas seus próprios agendamentos.");
                    }
                    if (User.IsInRole("Instrutor") && userId != agendamentoExistente.InstrutorId)
                    {
                        return Forbid("Um instrutor pode atualizar apenas agendamentos em que ele é o instrutor.");
                    }
                    // Administrador/Recepcionista pode atualizar qualquer um
                }

                var updatedAgendamento = await _agendamentoAppService.AtualizarAgendamentoAsync(id, updateModel);
                return Ok(updatedAgendamento);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno ao atualizar agendamento: " + ex.Message);
            }
        }
    }
}