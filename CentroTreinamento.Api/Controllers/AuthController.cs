// CentroTreinamento.Api/Controllers/AuthController.cs
using CentroTreinamento.Application.DTOs;
using CentroTreinamento.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // Necessário para acessar as configurações do JWT
using Microsoft.IdentityModel.Tokens;     // Necessário para SymmetricSecurityKey
using System.IdentityModel.Tokens.Jwt;    // Necessário para JwtSecurityTokenHandler
using System.Security.Claims;             // Necessário para Claims
using System.Text;                        // Necessário para Encoding
using System;
using System.Threading.Tasks;

namespace CentroTreinamento.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Rota base: /api/auth
    public class AuthController : ControllerBase
    {
        private readonly IAuthAppService _authAppService;
        private readonly IConfiguration _configuration; // Para ler as configurações do JWT

        public AuthController(IAuthAppService authAppService, IConfiguration configuration)
        {
            _authAppService = authAppService;
            _configuration = configuration;
        }

        /// <summary>
        /// Realiza o login de um aluno e retorna um token JWT.
        /// </summary>
        /// <param name="loginModel">O modelo de login contendo CPF e Senha.</param>
        /// <returns>Um AuthResponseViewModel com o token JWT ou Unauthorized.</returns>
        [HttpPost("login/aluno")] // Endpoint: /api/auth/login/aluno
        public async Task<ActionResult<AuthResponseViewModel>> LoginAluno([FromBody] LoginInputModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna 400 Bad Request se a validação falhar
            }

            var aluno = await _authAppService.LoginAlunoAsync(loginModel);

            if (aluno == null)
            {
                return Unauthorized(new { message = "CPF ou senha inválidos." }); // Retorna 401 Unauthorized
            }

            // Gerar o token JWT agora na camada da API
            var token = GenerateJwtToken(aluno.Id.ToString(), aluno.Cpf!, aluno.Nome!, aluno.Role.ToString());

            // Retorna o ViewModel com o token
            return Ok(new AuthResponseViewModel
            {
                AccessToken = token,
                ExpiresIn = (int)TimeSpan.FromMinutes(double.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!)).TotalSeconds,
                Cpf = aluno.Cpf!,
                Nome = aluno.Nome!,
                Role = aluno.Role.ToString()
            });
        }

        /// <summary>
        /// Método privado para gerar o token JWT.
        /// </summary>
        private string GenerateJwtToken(string userId, string userCpf, string userName, string userRole)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!);
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expirationInMinutes = double.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!);

            var claims = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.SerialNumber, userCpf), // Usando SerialNumber para CPF ou um custom claim
                new Claim(ClaimTypes.Role, userRole)
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}