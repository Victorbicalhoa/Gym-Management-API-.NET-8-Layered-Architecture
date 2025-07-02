// CentroTreinamento.Application/Services/AuthAppService.cs
using CentroTreinamento.Application.DTOs;
using CentroTreinamento.Application.Interfaces;
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Domain.Entities; // Para acessar Aluno
using System;
using System.Linq; // Para FirstOrDefault
using System.Threading.Tasks;

namespace CentroTreinamento.Application.Services
{
    public class AuthAppService : IAuthAppService
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthAppService(IAlunoRepository alunoRepository, IPasswordHasher passwordHasher)
        {
            _alunoRepository = alunoRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Aluno?> LoginAlunoAsync(LoginInputModel loginModel) // Retorna Aluno ou null
        {
            // Normaliza o CPF fornecido no login: remove pontos e traços
            var cpfLoginLimpo = loginModel.Cpf.Replace(".", "").Replace("-", ""); // <--- ALTERAÇÃO AQUI

            // Busca o aluno pelo CPF normalizado
            var aluno = (await _alunoRepository.GetAllAsync())
                                .FirstOrDefault(a => a.Cpf == cpfLoginLimpo); // <--- Use o CPF limpo aqui

            if (aluno == null)
            {
                return null; // Aluno não encontrado
            }

            // Verifica a senha
            if (!_passwordHasher.VerifyPassword(loginModel.Senha, aluno.SenhaHash!))
            {
                return null; // Senha incorreta
            }

            // Se as credenciais estiverem corretas, retorna o objeto Aluno
            return aluno;
        }

        // Futuramente, você pode adicionar métodos RegisterAlunoAsync, LoginAdministradorAsync, etc.
        // Para o registro, o AuthAppService pode chamar o AlunoRepository para salvar o novo aluno
        // e garantir que a senha seja hashed antes de salvar.
    }
}