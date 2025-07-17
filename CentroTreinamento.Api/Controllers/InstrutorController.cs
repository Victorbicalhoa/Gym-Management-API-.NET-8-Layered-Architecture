using CentroTreinamento.Application.DTOs.Instrutor;
using CentroTreinamento.Application.DTOs.PlanoDeTreino; // <<<<< NOVO: Adicionar DTOs de PlanoDeTreino
using CentroTreinamento.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims; // <<<<< NOVO: Para acessar claims do usuário autenticado
using System.Threading.Tasks;

namespace CentroTreinamento.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Define a rota base como /api/Instrutores
    // [Authorize(Roles = "Administrador")] // Exemplo: Apenas administradores podem gerenciar instrutores
    public class InstrutoresController : ControllerBase
    {
        private readonly IInstrutorAppService _instrutorAppService;
        private readonly IPlanoDeTreinoAppService _planoDeTreinoAppService; // <<<<< NOVO: Injetar IPlanoDeTreinoAppService

        public InstrutoresController(IInstrutorAppService instrutorAppService,
                                     IPlanoDeTreinoAppService planoDeTreinoAppService) // <<<<< NOVO: Adicionar ao construtor
        {
            _instrutorAppService = instrutorAppService;
            _planoDeTreinoAppService = planoDeTreinoAppService; // <<<<< NOVO: Atribuir
        }

        // --- Existing Instrutor Endpoints (mantidos como estão) ---

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

        // --- NEW PlanoDeTreino Endpoints for Instrutor (RF2.1, RF2.2) ---

        /// <summary>
        /// Cria um novo plano de treino para um aluno específico.
        /// RF2.1: O instrutor cria planos de treino.
        /// O plano de treino será associado ao Instrutor logado e ao Aluno especificado.
        /// </summary>
        /// <param name="alunoId">ID do aluno para quem o plano de treino será criado.</param>
        /// <param name="inputModel">Dados do plano de treino a ser criado.</param>
        /// <returns>O plano de treino criado.</returns>
        [HttpPost("{instrutorId}/planosDeTreino/aluno/{alunoId}")] // Rota aninhada
        [Authorize(Roles = "Instrutor, Administrador")] // Instrutor e Administrador podem criar planos
        [ProducesResponseType(typeof(PlanoDeTreinoViewModel), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 404)] // Aluno ou Instrutor não encontrado
        [ProducesResponseType(typeof(string), 409)] // Conflito (ex: aluno já tem plano ativo conflitante)
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> CriarPlanoDeTreino(Guid instrutorId, Guid alunoId, [FromBody] PlanoDeTreinoInputModel inputModel)
        {
            // Validação de autorização: um instrutor só pode criar planos para si mesmo.
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Instrutor") && (userIdClaim == null || Guid.Parse(userIdClaim.Value) != instrutorId))
            {
                return Forbid("Um instrutor pode criar planos de treino apenas para si mesmo.");
            }
            // Administradores podem criar planos para qualquer instrutor.

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Sobrescrever os IDs no inputModel com os IDs da rota para garantir consistência e segurança
                inputModel.AlunoId = alunoId;
                inputModel.InstrutorId = instrutorId; // Garante que o instrutor logado é o instrutor do plano

                var novoPlano = await _planoDeTreinoAppService.CriarPlanoDeTreinoAsync(inputModel);
                // Retorna 201 Created com a localização do novo recurso
                return CreatedAtAction(
                    "GetPlanoDeTreinoById", // Referência ao método no PlanosDeTreinoController (se a rota for acessível)
                    "PlanosDeTreino", // Nome do controlador sem "Controller"
                    new { id = novoPlano.Id },
                    novoPlano);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Validação de IDs, datas, etc.
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // Regras de negócio (ex: aluno já tem plano ativo)
            }
            catch (ApplicationException ex) // Se o AppService lançar ApplicationException para NotFound
            {
                return NotFound(ex.Message); // Aluno ou Instrutor não encontrado no AppService
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao criar plano de treino: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza um plano de treino existente.
        /// RF2.2: O instrutor edita planos de treino.
        /// </summary>
        /// <param name="instrutorId">ID do instrutor que é dono do plano.</param>
        /// <param name="planoId">ID do plano de treino a ser atualizado.</param>
        /// <param name="updateModel">Dados atualizados do plano de treino.</param>
        /// <returns>Status 204 No Content se a atualização for bem-sucedida.</returns>
        [HttpPut("{instrutorId}/planosDeTreino/{planoId}")] // Rota aninhada
        [Authorize(Roles = "Instrutor, Administrador")] // Instrutor e Administrador podem editar planos
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 404)] // Plano de Treino não encontrado
        [ProducesResponseType(typeof(string), 409)] // Conflito (ex: datas inválidas, status)
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> AtualizarPlanoDeTreino(Guid instrutorId, Guid planoId, [FromBody] PlanoDeTreinoUpdateModel updateModel)
        {
            // Validação de autorização: um instrutor só pode atualizar planos que ele mesmo criou.
            // Aqui, precisamos de uma forma de verificar o InstrutorId do PlanoDeTreino para o usuário Instrutor.
            // O ideal seria que o `AtualizarPlanoDeTreinoAsync` no AppService já fizesse essa validação.
            // Por enquanto, vamos confiar na lógica do AppService, mas um check extra aqui pode ser bom.

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Instrutor") && (userIdClaim == null || Guid.Parse(userIdClaim.Value) != instrutorId))
            {
                return Forbid("Um instrutor pode atualizar apenas planos de treino associados a ele.");
            }
            // Administradores podem atualizar planos de qualquer instrutor.

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Sobrescrever os IDs no updateModel com os IDs da rota para garantir consistência e segurança
                updateModel.InstrutorId = instrutorId; // Garante que o instrutor logado é o instrutor do plano
                // updateModel.AlunoId já deveria vir do DTO ou ser validado internamente no AppService

                await _planoDeTreinoAppService.AtualizarPlanoDeTreinoAsync(planoId, updateModel);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ApplicationException ex) // Se o AppService lançar ApplicationException para NotFound
            {
                return NotFound(ex.Message); // Plano de Treino, Aluno ou Instrutor não encontrado
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar plano de treino: {ex.Message}");
            }
        }
    }
}