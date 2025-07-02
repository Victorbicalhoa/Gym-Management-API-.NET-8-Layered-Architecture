// CentroTreinamento.Application/Services/AuthAppService.cs
using CentroTreinamento.Application.DTOs;
using CentroTreinamento.Application.Interfaces;
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Domain.Entities; // Para acessar Aluno e Administrador
using System;
using System.Linq;
using System.Threading.Tasks;
//using CentroTreinamento.Application.PasswordHasher; // Para IPasswordHasher

namespace CentroTreinamento.Application.Services
{
    public class AuthAppService : IAuthAppService
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly IAdministradorRepository _administradorRepository; // NOVO: Para buscar administradores
        private readonly IPasswordHasher _passwordHasher;

        public AuthAppService(IAlunoRepository alunoRepository,
                              IAdministradorRepository administradorRepository, // Injetar
                              IPasswordHasher passwordHasher)
        {
            _alunoRepository = alunoRepository;
            _administradorRepository = administradorRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<AlunoViewModel?> LoginAlunoAsync(LoginInputModel loginModel)
        {
            var cpfLoginLimpo = loginModel.Cpf!.Replace(".", "").Replace("-", "");

            var aluno = (await _alunoRepository.FindAsync(a => a.Cpf == cpfLoginLimpo))
                                 .FirstOrDefault();

            if (aluno == null)
            {
                return null;
            }

            if (!_passwordHasher.VerifyPassword(loginModel.Senha!, aluno.SenhaHash!))
            {
                return null;
            }

            return new AlunoViewModel
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Cpf = aluno.Cpf,
                // Status = aluno.Status, // Verifique se Aluno tem um Status, se não, remova
                Role = aluno.Role
            };
        }

        // NOVO: Método de login para Administrador
        public async Task<AdministradorViewModel?> LoginAdministradorAsync(LoginInputModel loginModel)
        {
            var cpfLoginLimpo = loginModel.Cpf!.Replace(".", "").Replace("-", "");

            var administrador = (await _administradorRepository.FindAsync(a => a.Cpf == cpfLoginLimpo))
                                     .FirstOrDefault();

            if (administrador == null)
            {
                return null; // Administrador não encontrado
            }

            if (!_passwordHasher.VerifyPassword(loginModel.Senha!, administrador.SenhaHash!))
            {
                return null; // Senha incorreta
            }

            // Mapeia a entidade Administrador para AdministradorViewModel
            return new AdministradorViewModel
            {
                Id = administrador.Id,
                Nome = administrador.Nome,
                Cpf = administrador.Cpf,
                Status = administrador.Status,
                Role = administrador.Role
            };
        }
    }
}