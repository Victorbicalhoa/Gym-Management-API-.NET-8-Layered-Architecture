// CentroTreinamento.API/Controllers/RecepcionistasController.cs
using CentroTreinamento.Application.DTOs.Recepcionista;
using CentroTreinamento.Application.Interfaces.Services;
using CentroTreinamento.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroTreinamento.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecepcionistasController : ControllerBase
    {
        private readonly IRecepcionistaAppService _recepcionistaAppService;

        public RecepcionistasController(IRecepcionistaAppService recepcionistaAppService)
        {
            _recepcionistaAppService = recepcionistaAppService;
        }

        // POST: api/recepcionistas - Criar uma nova recepcionista
        [HttpPost]
        [AllowAnonymous] // Ajuste as roles conforme sua política de segurança
        public async Task<ActionResult<RecepcionistaViewModel>> Post([FromBody] RecepcionistaInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var recepcionista = await _recepcionistaAppService.CreateRecepcionistaAsync(inputModel);
                // Retorna 201 CreatedAtAction com o novo recurso
                return CreatedAtAction(nameof(GetById), new { id = recepcionista.Id }, recepcionista);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/recepcionistas/{id} - Obter recepcionista por ID
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Administrador,Recepcionista,Instrutor,Aluno")] // Exemplo de autorização
        public async Task<ActionResult<RecepcionistaViewModel>> GetById(Guid id)
        {
            var recepcionista = await _recepcionistaAppService.GetRecepcionistaByIdAsync(id);
            if (recepcionista == null)
            {
                return NotFound();
            }
            return Ok(recepcionista);
        }

        // GET: api/recepcionistas - Obter todas as recepcionistas
        [HttpGet]
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<ActionResult<IEnumerable<RecepcionistaViewModel>>> GetAll()
        {
            var recepcionistas = await _recepcionistaAppService.GetAllRecepcionistasAsync();
            return Ok(recepcionistas);
        }

        // GET: api/recepcionistas/by-cpf/{cpf} - Obter recepcionista por CPF
        [HttpGet("by-cpf/{cpf}")]
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<ActionResult<RecepcionistaViewModel>> GetByCpf(string cpf)
        {
            var recepcionista = await _recepcionistaAppService.GetRecepcionistaByCpfAsync(cpf);
            if (recepcionista == null)
            {
                return NotFound();
            }
            return Ok(recepcionista);
        }

        // PUT: api/recepcionistas/{id} - Atualizar dados gerais da recepcionista
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Administrador,Recepcionista")]
        public async Task<IActionResult> Put(Guid id, [FromBody] RecepcionistaInputModel updateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _recepcionistaAppService.UpdateRecepcionistaAsync(id, updateModel);
                return NoContent(); // 204 No Content para atualização bem-sucedida
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno ao atualizar a recepcionista.");
            }
        }

        // PATCH: api/recepcionistas/{id}/status - Atualizar apenas o status da recepcionista
        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] StatusRecepcionista newStatus)
        {
            try
            {
                await _recepcionistaAppService.UpdateRecepcionistaStatusAsync(id, newStatus);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno ao atualizar o status da recepcionista.");
            }
        }

        // DELETE: api/recepcionistas/{id} - Deletar recepcionista
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _recepcionistaAppService.DeleteRecepcionistaAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno ao deletar a recepcionista.");
            }
        }
    }
}