using CentroTreinamento.Application.DTOs;
using CentroTreinamento.Application.Interfaces;
using CentroTreinamento.Domain.Enums;
using Microsoft.AspNetCore.Mvc; // ESSENCIAL: Garante que o controlador funcione como uma API ASP.NET Core
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroTreinamento.Api.Controllers
{
    [ApiController] // Indica que esta classe é um controlador de API RESTful
    [Route("api/[controller]")] // Define a rota base para este controlador (ex: /api/alunos)
    public class AlunosController : ControllerBase // Controladores de API geralmente herdam de ControllerBase
    {
        private readonly IAlunoAppService _alunoAppService;

        // Construtor: O ASP.NET Core injetará automaticamente a instância de IAlunoAppService
        // configurada no Program.cs
        public AlunosController(IAlunoAppService alunoAppService)
        {
            _alunoAppService = alunoAppService;
        }

        /// <summary>
        /// Obtém todos os alunos cadastrados.
        /// </summary>
        /// <returns>Uma lista de AlunoViewModel.</returns>
        [HttpGet] // Mapeia para requisições GET em /api/alunos
        public async Task<ActionResult<IEnumerable<AlunoViewModel>>> Get()
        {
            var alunos = await _alunoAppService.GetAllAlunosAsync();
            return Ok(alunos); // Retorna 200 OK com a lista de alunos
        }

        /// <summary>
        /// Obtém um aluno específico pelo seu ID.
        /// </summary>
        /// <param name="id">O GUID do aluno.</param>
        /// <returns>O AlunoViewModel correspondente ou NotFound se não existir.</returns>
        [HttpGet("{id}")] // Mapeia para requisições GET em /api/alunos/{id}
        public async Task<ActionResult<AlunoViewModel>> Get(Guid id)
        {
            var aluno = await _alunoAppService.GetAlunoByIdAsync(id);
            if (aluno == null)
            {
                return NotFound(); // Retorna 404 Not Found se o aluno não for encontrado
            }
            return Ok(aluno); // Retorna 200 OK com o aluno encontrado
        }

        /// <summary>
        /// Cria um novo aluno.
        /// </summary>
        /// <param name="alunoInput">Dados do novo aluno.</param>
        /// <returns>O AlunoViewModel do aluno criado e a URL para acessá-lo.</returns>
        [HttpPost] // Mapeia para requisições POST em /api/alunos
        public async Task<ActionResult<AlunoViewModel>> Post([FromBody] AlunoInputModel alunoInput)
        {
            // Validação automática do modelo com base nos Data Annotations em AlunoInputModel
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna 400 Bad Request com detalhes dos erros de validação
            }

            var novoAluno = await _alunoAppService.CreateAlunoAsync(alunoInput);
            // Retorna 201 Created, com o aluno criado e o cabeçalho Location contendo a URL para o recurso
            return CreatedAtAction(nameof(Get), new { id = novoAluno.Id }, novoAluno);
        }

        /// <summary>
        /// Atualiza um aluno existente.
        /// </summary>
        /// <param name="id">O GUID do aluno a ser atualizado.</param>
        /// <param name="alunoInput">Novos dados do aluno.</param>
        /// <returns>NoContent se a atualização for bem-sucedida, NotFound se o aluno não existir.</returns>
        [HttpPut("{id}")] // Mapeia para requisições PUT em /api/alunos/{id}
        public async Task<ActionResult> Put(Guid id, [FromBody] AlunoInputModel alunoInput)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna 400 Bad Request
            }

            var updated = await _alunoAppService.UpdateAlunoAsync(id, alunoInput);
            if (!updated)
            {
                return NotFound(); // Retorna 404 Not Found se o aluno não for encontrado
            }
            return NoContent(); // Retorna 204 No Content para atualização bem-sucedida sem retorno de corpo
        }

        /// <summary>
        /// Atualiza o status de um aluno (Ativo, Inativo, Suspenso, etc.).
        /// </summary>
        /// <param name="id">O GUID do aluno.</param>
        /// <param name="novoStatus">O novo status a ser atribuído.</param>
        /// <returns>NoContent se a atualização for bem-sucedida, NotFound se o aluno não existir, ou BadRequest se o status for inválido.</returns>
        [HttpPatch("{id}/status")] // Mapeia para requisições PATCH em /api/alunos/{id}/status
        public async Task<ActionResult> UpdateStatus(Guid id, [FromQuery] StatusAluno novoStatus)
        {
            // Validação simples para o enum StatusAluno
            if (!Enum.IsDefined(typeof(StatusAluno), novoStatus))
            {
                return BadRequest("Status inválido."); // Retorna 400 Bad Request para status inválido
            }

            var updated = await _alunoAppService.UpdateAlunoStatusAsync(id, novoStatus);
            if (!updated)
            {
                return NotFound(); // Retorna 404 Not Found
            }
            return NoContent(); // Retorna 204 No Content
        }

        /// <summary>
        /// Exclui um aluno.
        /// </summary>
        /// <param name="id">O GUID do aluno a ser excluído.</param>
        /// <returns>NoContent se a exclusão for bem-sucedida, NotFound se o aluno não existir.</returns>
        [HttpDelete("{id}")] // Mapeia para requisições DELETE em /api/alunos/{id}
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _alunoAppService.DeleteAlunoAsync(id);
            if (!deleted)
            {
                return NotFound(); // Retorna 404 Not Found se o aluno não for encontrado
            }
            return NoContent(); // Retorna 204 No Content para exclusão bem-sucedida
        }
    }
}