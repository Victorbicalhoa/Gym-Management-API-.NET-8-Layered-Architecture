// CentroTreinamento.Api/Controllers/AuthController.cs
using CentroTreinamento.Application.DTOs.Auth;
using CentroTreinamento.Application.Interfaces.Services;
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
    [Route("api/[controller]")] // Isso significa que a rota base é "api/Auth"
    public class AuthController : ControllerBase
    {
        private readonly IAuthAppService _authAppService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthAppService authAppService, IConfiguration configuration)
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

        // NOVO ENDPOINT PARA LOGIN DE INSTRUTOR
        [HttpPost("login/instrutor")] // A rota completa será "api/Auth/login/instrutor"
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseViewModel>> LoginInstrutor([FromBody] LoginInputModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Chama o método no seu AuthAppService para autenticar o instrutor
            // Você precisará ter um método como LoginInstrutorAsync no seu IAuthAppService e AuthAppService.cs
            var instrutor = await _authAppService.LoginInstrutorAsync(loginModel);

            if (instrutor == null)
            {
                return Unauthorized(new { message = "CPF ou senha inválidos." });
            }

            // Gera o token JWT para o instrutor
            var tokenString = GenerateJwtToken(instrutor.Id.ToString(), instrutor.Cpf!, instrutor.Nome!, instrutor.Role.ToString());

            return Ok(new AuthResponseViewModel
            {
                AccessToken = tokenString,
                TokenType = "Bearer",
                ExpiresIn = (int)TimeSpan.FromMinutes(double.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!)).TotalSeconds,
                Cpf = instrutor.Cpf!,
                Nome = instrutor.Nome!,
                Role = instrutor.Role.ToString()
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