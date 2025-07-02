// CentroTreinamento.Api/Controllers/AdministradorController.cs
using CentroTreinamento.Application.DTOs;
using CentroTreinamento.Application.Interfaces;
using CentroTreinamento.Domain.Enums; // Para StatusAdministrador
using Microsoft.AspNetCore.Authorization; // Para o atributo [Authorize]
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroTreinamento.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Define a rota base como /api/administradores
                                // [Authorize(Roles = "Administrador")] // Opcional: Protege o controller inteiro.
                                // Se você quer que apenas admins logados criem/vejam outros admins, use isso.
                                // Se não, pode deixar livre para que o primeiro admin possa ser criado.
    public class AdministradoresController : ControllerBase
    {
        private readonly IAdministradorAppService _administradorAppService;

        public AdministradoresController(IAdministradorAppService administradorAppService)
        {
            _administradorAppService = administradorAppService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")] // Apenas administradores podem listar outros administradores
        public async Task<ActionResult<IEnumerable<AdministradorViewModel>>> GetAdministradores()
        {
            var administradores = await _administradorAppService.GetAllAdministradoresAsync();
            return Ok(administradores);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Administrador")] // Apenas administradores podem buscar por ID
        public async Task<ActionResult<AdministradorViewModel>> GetAdministradorById(Guid id)
        {
            var administrador = await _administradorAppService.GetAdministradorByIdAsync(id);
            if (administrador == null)
            {
                return NotFound("Administrador não encontrado.");
            }
            return Ok(administrador);
        }

        [HttpPost]
        // Se este for o endpoint para CRIAR o PRIMEIRO ADMINISTRADOR, ele não pode ser protegido.
        // Se for para criar *novos* administradores *após* um administrador já estar logado, então [Authorize(Roles = "Administrador")]
        // Por simplicidade para o primeiro cadastro, vamos deixá-lo público por enquanto, ou você pode ter um endpoint específico de "bootstrap"
        [AllowAnonymous] // Permite acesso sem autenticação para criar o primeiro admin
        public async Task<ActionResult<AdministradorViewModel>> CreateAdministrador([FromBody] AdministradorInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var administrador = await _administradorAppService.CreateAdministradorAsync(inputModel);
            return CreatedAtAction(nameof(GetAdministradorById), new { id = administrador.Id }, administrador);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Administrador")] // Apenas administradores podem atualizar administradores
        public async Task<ActionResult<bool>> UpdateAdministrador(Guid id, [FromBody] AdministradorInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _administradorAppService.UpdateAdministradorAsync(id, inputModel);
            if (!result)
            {
                return NotFound("Administrador não encontrado para atualização.");
            }
            return NoContent(); // Retorna 204 No Content para sucesso de atualização
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Administrador")] // Apenas administradores podem deletar administradores
        public async Task<ActionResult<bool>> DeleteAdministrador(Guid id)
        {
            var result = await _administradorAppService.DeleteAdministradorAsync(id);
            if (!result)
            {
                return NotFound("Administrador não encontrado para exclusão.");
            }
            return NoContent(); // Retorna 204 No Content para sucesso de exclusão
        }

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "Administrador")] // Apenas administradores podem atualizar status
        public async Task<ActionResult> UpdateAdministradorStatus(Guid id, [FromQuery] StatusAdministrador novoStatus)
        {
            if (!Enum.IsDefined(typeof(StatusAdministrador), novoStatus))
            {
                return BadRequest("Status inválido.");
            }

            var result = await _administradorAppService.UpdateAdministradorStatusAsync(id, novoStatus);
            if (!result)
            {
                return NotFound("Administrador não encontrado.");
            }
            return NoContent();
        }
    }
}