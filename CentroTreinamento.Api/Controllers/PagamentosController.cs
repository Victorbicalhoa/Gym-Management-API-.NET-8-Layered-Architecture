using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentroTreinamento.Application.DTOs.Pagamento;
using CentroTreinamento.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization; // Para usar atributos de autorização

namespace CentroTreinamento.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Rota base: /api/Pagamentos
    public class PagamentosController : ControllerBase
    {
        private readonly IPagamentoAppService _pagamentoAppService;

        public PagamentosController(IPagamentoAppService pagamentoAppService)
        {
            _pagamentoAppService = pagamentoAppService;
        }

        /// <summary>
        /// Registra um novo pagamento.
        /// RF4.1: Administrador ou Recepcionista registra pagamentos.
        /// </summary>
        /// <param name="inputModel">Dados do pagamento a ser registrado.</param>
        /// <returns>O pagamento registrado.</returns>
        [HttpPost]
        [Authorize(Roles = "Administrador, Recepcionista")] // Somente Administrador e Recepcionista podem registrar
        [ProducesResponseType(typeof(PagamentoViewModel), 201)]
        [ProducesResponseType(typeof(string), 400)] // Bad Request (validação, aluno não encontrado)
        [ProducesResponseType(typeof(string), 401)] // Unauthorized (token inválido/ausente)
        [ProducesResponseType(typeof(string), 403)] // Forbidden (usuário não tem a role)
        [ProducesResponseType(typeof(string), 409)] // Conflict (regras de negócio)
        [ProducesResponseType(typeof(string), 500)] // Internal Server Error
        public async Task<IActionResult> RegistrarPagamento([FromBody] PagamentoInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var pagamento = await _pagamentoAppService.RegistrarPagamentoAsync(inputModel);
                return CreatedAtAction(nameof(GetPagamentoById), new { id = pagamento.Id }, pagamento);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Erros de validação de IDs (aluno não encontrado)
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // Erros de regra de negócio
            }
            catch (Exception ex)
            {
                // TODO: Implementar logging para erros internos
                return StatusCode(500, "Erro interno ao registrar pagamento: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtém um pagamento pelo seu ID.
        /// </summary>
        /// <param name="id">ID do pagamento.</param>
        /// <returns>O pagamento correspondente.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador, Recepcionista, Aluno")] // Todos podem ver um pagamento específico
        [ProducesResponseType(typeof(PagamentoViewModel), 200)]
        [ProducesResponseType(typeof(string), 404)] // Not Found
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        public async Task<IActionResult> GetPagamentoById(Guid id)
        {
            var pagamento = await _pagamentoAppService.GetPagamentoByIdAsync(id);
            if (pagamento == null)
            {
                return NotFound($"Pagamento com ID {id} não encontrado.");
            }
            // Opcional: Para o aluno, verificar se o pagamento pertence a ele para 403 Forbidden.
            // Para isso, você precisaria do ID do usuário autenticado (User.FindFirst(ClaimTypes.NameIdentifier).Value).
            // E adicionar lógica aqui para comparar com pagamento.AlunoId.

            return Ok(pagamento);
        }

        /// <summary>
        /// Obtém o histórico de pagamentos de um aluno.
        /// RF4.5: Permite consultar o histórico de pagamentos de um aluno.
        /// </summary>
        /// <param name="alunoId">ID do aluno.</param>
        /// <returns>Lista de pagamentos do aluno.</returns>
        [HttpGet("aluno/{alunoId}")]
        [Authorize(Roles = "Administrador, Recepcionista, Aluno")] // Todos podem ver, mas Aluno deve ver apenas os seus
        [ProducesResponseType(typeof(IEnumerable<PagamentoViewModel>), 200)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetPagamentosByAlunoId(Guid alunoId)
        {
            // Implementação de autorização por recurso (para Alunos)
            // Se o usuário logado é um aluno, ele só pode ver os próprios pagamentos.
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (User.IsInRole("Aluno") && (userIdClaim == null || Guid.Parse(userIdClaim.Value) != alunoId))
            {
                return Forbid("Um aluno pode consultar apenas seus próprios pagamentos.");
            }
            // Administradores e Recepcionistas podem consultar de qualquer aluno.

            try
            {
                var pagamentos = await _pagamentoAppService.GetPagamentosByAlunoIdAsync(alunoId);
                // Retorna 200 OK com uma lista vazia se não houver pagamentos.
                return Ok(pagamentos);
            }
            catch (ArgumentException ex)
            {
                // Por exemplo, se o AlunoAppService/Repository for injetado aqui e retornar que o aluno não existe
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno ao obter pagamentos do aluno: " + ex.Message);
            }
        }
    }
}