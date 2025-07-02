// CentroTreinamento.Api/Controllers/AuthController.cs
using CentroTreinamento.Application.DTOs;
using CentroTreinamento.Application.Interfaces;
// REMOVA: using CentroTreinamento.Application.Services; // Este using não é mais necessário aqui
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CentroTreinamento.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthAppService _authAppService; // Único AppService para login
        private readonly IConfiguration _configuration;

        public AuthController(IAuthAppService authAppService, IConfiguration configuration) // Apenas um AppService injetado
        {
            _authAppService = authAppService;
            _configuration = configuration;
        }

        [HttpPost("login/aluno")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseViewModel>> LoginAluno([FromBody] LoginInputModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Chama o método do AuthAppService que agora aceita LoginInputModel
            var aluno = await _authAppService.LoginAlunoAsync(loginModel);

            if (aluno == null)
            {
                return Unauthorized(new { message = "CPF ou senha inválidos." });
            }

            var token = GenerateJwtToken(aluno.Id.ToString(), aluno.Cpf!, aluno.Nome!, aluno.Role.ToString());

            return Ok(new AuthResponseViewModel
            {
                AccessToken = token,
                TokenType = "Bearer",
                ExpiresIn = (int)TimeSpan.FromMinutes(double.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!)).TotalSeconds,
                Cpf = aluno.Cpf!,
                Nome = aluno.Nome!,
                Role = aluno.Role.ToString()
            });
        }

        [HttpPost("login/administrador")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseViewModel>> LoginAdministrador([FromBody] LoginInputModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Chama o método do AuthAppService que agora aceita LoginInputModel
            var administrador = await _authAppService.LoginAdministradorAsync(loginModel);

            if (administrador == null)
            {
                return Unauthorized(new { message = "Credenciais inválidas." });
            }

            var tokenString = GenerateJwtToken(administrador.Id.ToString(), administrador.Cpf!, administrador.Nome!, administrador.Role.ToString());

            return Ok(new AuthResponseViewModel
            {
                AccessToken = tokenString,
                TokenType = "Bearer",
                ExpiresIn = (int)TimeSpan.FromMinutes(double.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!)).TotalSeconds,
                Cpf = administrador.Cpf!,
                Nome = administrador.Nome!,
                Role = administrador.Role.ToString()
            });
        }

        // Mantenha APENAS ESTA VERSÃO do GenerateJwtToken
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
                new Claim(ClaimTypes.SerialNumber, userCpf),
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