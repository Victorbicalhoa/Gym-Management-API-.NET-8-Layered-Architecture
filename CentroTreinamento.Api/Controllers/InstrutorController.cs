// CentroTreinamento.Api/Controllers/InstrutorController.cs
using CentroTreinamento.Application.DTOs.Instrutor;
using CentroTreinamento.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization; // Para o atributo [Authorize]
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroTreinamento.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Define a rota base como /api/Instrutores
    // [Authorize(Roles = "Administrador")] // Exemplo: Apenas administradores podem gerenciar instrutores
    public class InstrutoresController : ControllerBase
    {
        private readonly IInstrutorAppService _instrutorAppService;

        public InstrutoresController(IInstrutorAppService instrutorAppService)
        {
            _instrutorAppService = instrutorAppService;
        }

        // GET api/Instrutor
        [HttpGet]
        [AllowAnonymous] // Ou [Authorize] se necessário autenticação para listar
        public async Task<ActionResult<IEnumerable<InstrutorViewModel>>> GetAllInstrutores()
        {
            var instrutores = await _instrutorAppService.GetAllAsync();
            return Ok(instrutores);
        }

        // GET api/Instrutor/{id}
        [HttpGet("{id}")]
        [AllowAnonymous] // Ou [Authorize]
        public async Task<ActionResult<InstrutorViewModel>> GetInstrutorById(Guid id)
        {
            try
            {
                var instrutor = await _instrutorAppService.GetByIdAsync(id);
                return Ok(instrutor);
            }
            catch (ApplicationException ex) // Captura a exceção de "Não Encontrado" do serviço
            {
                return NotFound(ex.Message); // Retorna 404 Not Found
            }
        }

        // GET api/Instrutor/cref/{cref}
        [HttpGet("cref/{cref}")]
        [AllowAnonymous] // Ou [Authorize]
        public async Task<ActionResult<InstrutorViewModel>> GetInstrutorByCref(string cref)
        {
            try
            {
                var instrutor = await _instrutorAppService.GetByCrefAsync(cref);
                return Ok(instrutor);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET api/Instrutor/cpf/{cpf}
        [HttpGet("cpf/{cpf}")]
        [AllowAnonymous] // Ou [Authorize]
        public async Task<ActionResult<InstrutorViewModel>> GetInstrutorByCpf(string cpf)
        {
            try
            {
                var instrutor = await _instrutorAppService.GetByCpfAsync(cpf);
                return Ok(instrutor);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST api/Instrutor
        [HttpPost]
        [Authorize(Roles = "Administrador")] // Apenas administradores podem criar novos instrutores
        public async Task<ActionResult<InstrutorViewModel>> CreateInstrutor([FromBody] InstrutorInputModel inputModel)
        {
            try
            {
                var newInstrutor = await _instrutorAppService.CreateInstrutorAsync(inputModel);
                // Retorna 201 Created com a localização do novo recurso
                return CreatedAtAction(nameof(GetInstrutorById), new { id = newInstrutor.Id }, newInstrutor);
            }
            catch (ArgumentException ex) // Captura validações de domínio ou unicidade
            {
                return BadRequest(ex.Message); // Retorna 400 Bad Request
            }
            catch (Exception ex) // Captura outras exceções inesperadas
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // PUT api/Instrutor/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")] // Apenas administradores podem atualizar instrutores
        public async Task<IActionResult> UpdateInstrutor(Guid id, [FromBody] InstrutorInputModel inputModel)
        {
            try
            {
                await _instrutorAppService.UpdateInstrutorAsync(id, inputModel);
                return NoContent(); // Retorna 204 No Content para atualização bem-sucedida
            }
            catch (ApplicationException ex) // "Não Encontrado"
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex) // Validações ou unicidade
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // PATCH api/Instrutor/{id}/status
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Administrador")] // Apenas administradores podem alterar o status
        public async Task<IActionResult> UpdateInstrutorStatus(Guid id, [FromQuery] Domain.Enums.StatusInstrutor newStatus)
        {
            try
            {
                await _instrutorAppService.UpdateInstrutorStatusAsync(id, newStatus);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // DELETE api/Instrutor/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")] // Apenas administradores podem deletar instrutores
        public async Task<IActionResult> DeleteInstrutor(Guid id)
        {
            try
            {
                await _instrutorAppService.DeleteInstrutorAsync(id);
                return NoContent(); // Retorna 204 No Content para exclusão bem-sucedida
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}